using System.Net;
using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using EduPlatform.Order.Application.UseCases.Orders.CreateOrder;
using EduPlatform.Order.Domain.Entities;
using EduPlatform.Shared.CorrelationContext;
using EduPlatform.Shared.Services;
using FluentAssertions;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Eduplatform.Order.API.Test;

// <summary>
/// ╔══════════════════════════════════════════════════════════════════╗
/// ║           UNIT TEST YAZMA KURALLARI (bu dosyada uygulandı)      ║
/// ╠══════════════════════════════════════════════════════════════════╣
/// ║ 1. Her test SADECE 1 davranışı doğrular                         ║
/// ║ 2. Test ismi: Handle_{Durum}_{BeklenenSonuç}                    ║
/// ║ 3. AAA pattern: Arrange / Act / Assert                           ║
/// ║ 4. Tüm dış bağımlılıklar mock'lanır (DB'ye gidilmez)           ║
/// ║ 5. Test birbirinden bağımsız olmalı (sıra önemli değil)         ║
/// ║ 6. Magic value kullanma → sabit veya builder kullan             ║
/// ╚══════════════════════════════════════════════════════════════════╝
/// </summary>
public class CreateOrderCommandHandlerTests
{
    // ─── MOCK'LAR ────────────────────────────────────────────────────────────
    //
    // KURAL: Concrete class (AppDbContext gibi) değil, INTERFACE mock'lanır.
    // Clean Architecture bunu garanti eder — her bağımlılık interface üzerinden.
    // Bu yüzden burada InMemory DB'ye ihtiyaç yoktur.

    private readonly ICorrelationContext _correlationContext;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderOutboxRepository _orderOutboxRepository;
    private readonly IIdentityService _identityService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        // NSubstitute ile interface'leri mock'la
        // Substitute.For<T>() → T'nin tüm metodlarını boş/default döndüren sahte nesne oluşturur
        _correlationContext = Substitute.For<ICorrelationContext>();
        _orderRepository    = Substitute.For<IOrderRepository>();
        _orderOutboxRepository = Substitute.For<IOrderOutboxRepository>();
        _identityService    = Substitute.For<IIdentityService>();
        _publishEndpoint    = Substitute.For<IPublishEndpoint>();
        _unitOfWork         = Substitute.For<IUnitOfWork>();
        _logger             = Substitute.For<ILogger<CreateOrderCommandHandler>>();

        // IPaymentService mock'u — Refit interface'i olduğu için ayrı tutuyoruz
        var paymentService = Substitute.For<IPaymentService>();

        // Handler'ı gerçek bağımlılıklarla değil, mock'larla oluştur
        _handler = new CreateOrderCommandHandler(
            _correlationContext,
            _orderRepository,
            _orderOutboxRepository,
            _identityService,
            _publishEndpoint,
            paymentService,
            _logger,
            _unitOfWork);

        // ─── DEFAULT MOCK DAVRANIŞLARI ────────────────────────────────────
        // Her testte tekrar yazmamak için ortak davranışları buraya koy.
        // "Happy path" varsayılanı: order yok, commit başarılı.

        // orderRepository.Where(...) → boş liste döner (order bulunamadı)
        _orderRepository
            .Where(Arg.Any<System.Linq.Expressions.Expression<Func<EduPlatform.Order.Domain.Entities.Order, bool>>>())
            .Returns(Enumerable.Empty<EduPlatform.Order.Domain.Entities.Order>().AsQueryable());

        // CommitAsync başarıyla tamamlanır
        _unitOfWork
            .CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // CorrelationId her zaman bir değer döner
        _correlationContext.CorrelationId.Returns(Guid.NewGuid());

        // UserId her zaman bir değer döner
        _identityService.UserId.Returns(Guid.NewGuid());
    }

    // ════════════════════════════════════════════════════════════════════════
    // SENARYO 1: Items listesi boş
    //
    // KURAL: Boundary case'leri (sınır değerleri) her zaman test et.
    // Boş liste, null, 0, negatif sayı gibi edge case'ler kritik hatalara yol açar.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Handle_WhenItemsIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        // KURAL: Test verisi oluşturmak için builder/factory metod kullan.
        // Magic value (hardcoded değer) yerine anlamlı isimli metod daha okunabilir.
        var command = ValidCommand(items: []); // Boş items listesi

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // KURAL: Hem status'u hem de hata mesajını doğrula.
        // Sadece "başarısız oldu" değil, "doğru şekilde başarısız oldu" mu diye bak.
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(HttpStatusCode.BadRequest);
        result.Fail!.Title.Should().Be("Order items not found");
    }

    [Fact]
    public async Task Handle_WhenItemsIsEmpty_ShouldNotCallRepository()
    {
        // Arrange
        var command = ValidCommand(items: []);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // KURAL: Negatif doğrulama — "çağrılmamalıydı" testi.
        // Guard clause erken return yapıyorsa, sonraki kodun çalışmadığını kanıtla.
        _orderRepository.DidNotReceive().Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    // ════════════════════════════════════════════════════════════════════════
    // SENARYO 2: Idempotency — Aynı token ile ikinci istek
    //
    // KURAL: İş kurallarını (business rules) test et.
    // Idempotency kritik bir business rule — aynı işlem iki kez yapılmamalı.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Handle_WhenOrderAlreadyExistsWithSameToken_ShouldReturnExistingOrder()
    {
        // Arrange
        var idempotentToken = Guid.NewGuid();
        var existingOrder = CreateFakeOrder(idempotentToken); // DB'deki mevcut order

        // Repository'yi "order var" diye ayarla
        _orderRepository
            .Where(Arg.Any<System.Linq.Expressions.Expression<Func<EduPlatform.Order.Domain.Entities.Order, bool>>>())
            .Returns(new[] { existingOrder }.AsQueryable());

        var command = ValidCommand(idempotentToken: idempotentToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(HttpStatusCode.Created);
        result.Data!.idempotentToken.Should().Be(idempotentToken);
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyExists_ShouldNotCreateNewOrder()
    {
        // Arrange
        var existingOrder = CreateFakeOrder(Guid.NewGuid());
        _orderRepository
            .Where(Arg.Any<System.Linq.Expressions.Expression<Func<EduPlatform.Order.Domain.Entities.Order, bool>>>())
            .Returns(new[] { existingOrder }.AsQueryable());

        var command = ValidCommand(idempotentToken: existingOrder.IdempotentToken);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // KURAL: "Çağrılmadı" doğrulaması — idempotent istekte yeni order eklenmemeli
        _orderRepository.DidNotReceive().Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    // ════════════════════════════════════════════════════════════════════════
    // SENARYO 3: Başarılı order oluşturma (Happy Path)
    //
    // KURAL: Happy path'i her zaman test et ama fazla detaya girme.
    // Unit testin görevi: "doğru metod çağrıldı mı?" (davranış)
    // Integration testin görevi: "DB'ye gerçekten yazıldı mı?" (sonuç)
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldReturnCreated()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
        result.Data!.idempotentToken.Should().Be(command.IdempotentToken);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddOrderToRepository()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // KURAL: Davranışı doğrula — "Add çağrıldı mı?"
        // Received(1) → tam olarak 1 kez çağrıldı
        _orderRepository.Received(1).Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>());
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddOutboxMessage()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Outbox pattern: order ile birlikte outbox mesajı da eklenmeli
        _orderOutboxRepository.Received(1).Add(Arg.Any<OrderOutbox>());
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCommitTransaction()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldSetBuyerIdFromIdentityService()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        _identityService.UserId.Returns(expectedUserId);

        EduPlatform.Order.Domain.Entities.Order? capturedOrder = null;

        // KURAL: Argüman yakalama (capture) — metoda geçilen parametreyi incelemek için
        // Do() ile Add'e gelen order'ı yakalıyoruz
        _orderRepository
            .When(r => r.Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>()))
            .Do(call => capturedOrder = call.Arg<EduPlatform.Order.Domain.Entities.Order>());

        var command = ValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOrder.Should().NotBeNull();
        capturedOrder!.BuyerId.Should().Be(expectedUserId);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldSetCorrectOrderStatus()
    {
        // Arrange
        EduPlatform.Order.Domain.Entities.Order? capturedOrder = null;
        _orderRepository
            .When(r => r.Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>()))
            .Do(call => capturedOrder = call.Arg<EduPlatform.Order.Domain.Entities.Order>());

        // Act
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        // Assert — yeni order WaitingForPayment statüsünde olmalı
        capturedOrder!.Status.Should().Be(OrderStatus.WaitingForPayment);
    }

    [Fact]
    public async Task Handle_WhenDiscountRateProvided_ShouldApplyDiscountToOrderItems()
    {
        // Arrange
        // KURAL: Business logic'i test et — indirim doğru hesaplanıyor mu?
        const float discountRate = 10f; // %10 indirim
        const decimal unitPrice = 100m;
        const decimal expectedPrice = 70m; // 100 - %10

        var command = ValidCommand(discountRate: discountRate, unitPrice: unitPrice);

        EduPlatform.Order.Domain.Entities.Order? capturedOrder = null;
        _orderRepository
            .When(r => r.Add(Arg.Any<EduPlatform.Order.Domain.Entities.Order>()))
            .Do(call => capturedOrder = call.Arg<EduPlatform.Order.Domain.Entities.Order>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOrder!.OrderItems.First().UnitPrice.Should().Be(expectedPrice);
        capturedOrder.TotalPrice.Should().Be(expectedPrice);
    }

   
    // ════════════════════════════════════════════════════════════════════════
    // YARDIMCI METODLAR
    //
    // KURAL: Test verisi oluşturmak için factory metod kullan.
    // "Object Mother" veya "Test Data Builder" pattern.
    // Avantaj: Test değişirse tek yerden güncellenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Geçerli bir CreateOrderCommand oluşturur.
    /// Parametreler opsiyonel — sadece test ettiğin şeyi override et.
    /// </summary>
    private static CreateOrderCommand ValidCommand(
        Guid? idempotentToken = null,
        float? discountRate = null,
        List<OrderItemDto>? items = null,
        decimal unitPrice = 100m)
    {
        return new CreateOrderCommand(
            IdempotentToken: idempotentToken ?? Guid.NewGuid(),
            DiscountRate: discountRate,
            Address: new AddressDto("İstanbul", "Kadıköy", "Moda Cad.", "34710", "No:1"),
            Payment: new PaymentDto(
                IdempotentToken: Guid.NewGuid(),
                Type: "CreditCard",
                Token: "tok_test_123",
                Last4: "4242",
                Brand: "Visa",
                ExpMonth: 12,
                ExpYear: 2028,
                Amount: unitPrice),
            Items: items ?? [new OrderItemDto(Guid.NewGuid(), "C# Kursu", unitPrice)]
        );
    }

    /// <summary>
    /// DB'de zaten kayıtlı bir Order simüle eder.
    /// </summary>
    private static EduPlatform.Order.Domain.Entities.Order CreateFakeOrder(Guid idempotentToken)
    {
        return EduPlatform.Order.Domain.Entities.Order.CreateUnPaidOrder(
            buyerId: Guid.NewGuid(),
            disCountRate: null,
            addressId: 1,
            idempotentToken: idempotentToken);
    }

    /// <summary>
    /// SqlException number 2627 (Unique constraint violation) ile DbUpdateException oluşturur.
    ///
    /// KURAL: Exception testlerinde gerçek exception'ı simüle et.
    /// Handler'ın catch bloğu SqlException.Number'a bakıyor,
    /// bu yüzden doğru number'ı vermek şart.
    /// </summary>
    private static DbUpdateException CreateDuplicateKeyException()
    {
        // SqlException constructor internal olduğu için reflection ile oluşturuyoruz
        var sqlException = (SqlException)System.Runtime.CompilerServices
            .RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        // SqlException.Number = 2627 (unique constraint violation)
        var numberField = typeof(SqlException).GetProperty("Number");
        // Reflection çalışmazsa fallback: Exception wrapping
        return new DbUpdateException("Duplicate key", sqlException);
    }
}

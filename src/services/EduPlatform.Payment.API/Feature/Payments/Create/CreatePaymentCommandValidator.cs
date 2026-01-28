#region

using EduPlatform.Payment.API.Feature.Payments.Create;
using FluentValidation;

#endregion

namespace EduPlatform.Payment.Api.Feature.Payments.Create;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderCode).NotEmpty().WithMessage("Order code is required.");
        RuleFor(x => x.Type).NotEmpty().WithMessage("Card Type is required.");
        RuleFor(x => x.Brand).NotEmpty().WithMessage("Brand is required.");
        RuleFor(x => x.ExpMonth).NotEmpty().WithMessage("ExpMonth is required.");
        RuleFor(x => x.ExpYear).NotEmpty().WithMessage("ExpYear is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
🎓 EduPlatform

EduPlatform, modern .NET ekosistemi kullanılarak geliştirilmiş,
mikroservis mimarisi odaklı bir eğitim platformu (Udemy benzeri) örnek projesidir.
Bu proje, bir eğitim / kurs içeriği temel alınarak başlatılmıştır.

Ancak zamanla:

🧠 Mimari kararlar yeniden değerlendirilmiş
🧩 Mikroservis yaklaşımı genişletilmiş
🔄 Sync / Async iletişim desenleri bilinçli şekilde uygulanmış
🛠 Altyapı ve entegrasyonlar gerçek hayat senaryolarına yaklaştırılmıştır

📚 Amaç, sadece eğitimi takip etmek değil;
öğrenilen kavramları gerçek bir production mindset ile yeniden kurgulamak ve derinleştirmektir.

Bu repo, özellikle:
Mikroservis mimarisi
Event-driven sistemler
Modern .NET backend pratikleri
konularında teknik gelişim amacıyla aktif olarak geliştirilmektedir.

🧱 Teknoloji Stack
🖥 Backend
🟦 .NET / ASP.NET Core
⚡ Minimal API
🧠 MediatR (CQRS yaklaşımı)
📘 OpenAPI / Swagger
🏷 API Versioning
🗄 Veri Katmanı
🍃 MongoDB – Doküman bazlı servisler
🐘 PostgreSQL – Transactional servisler
⚡ Redis – Cache & hızlı erişim
⚡ SQL Server – Keycloak DB

📨 İletişim
🐰 RabbitMQ
🚌 MassTransit – Event-driven & message-based iletişim
🌐 Sync – Refit
🌐 Gateway & Security

🔀 YARP Reverse Proxy – API Gateway
🔐 Keycloak – OAuth2 / OpenID Connect (JWT)
⚙️ DevOps & Tooling
🐳 Docker & Docker Compose
🧑‍💻 JetBrains Rider
🧪 Qodana – Statik kod analizi
🌱 .NET Aspire – Distributed application orchestration (net-aspire branch)
🌱 .NET Aspire (Distributed Application Orchestration)

Bu proje, net-aspire branch’i altında .NET Aspire ile orkestre edilmiştir.
.NET Aspire, mikroservis tabanlı uygulamaların:
local development
servis bağımlılıkları
environment configuration
observability entegrasyonları
gibi ihtiyaçlarını tek bir distributed application modeli altında toplamayı hedefler.

EduPlatform’ta Aspire şu amaçlarla kullanılmıştır:
🔗 Servisler arası bağımlılıkların merkezi tanımı
🧩 API, Worker, Gateway ve altyapı servislerinin tek noktadan ayağa kaldırılması
🧪 Local development ortamında:
Daha hızlı bootstrap
Daha az manuel Docker Compose konfigürasyonu
📊 Observability altyapısına (logs, metrics, traces) hazırlıklı mimari
ℹ️ Aspire, bu projede Docker / Kubernetes yerine geçmek için değil;
local geliştirme ve sistemin bütününü görselleştirmek amacıyla tercih edilmiştir.

Production ortamında container orchestration (Docker / Kubernetes) yaklaşımı geçerliliğini korur.
🧩 Projenin Genel Çerçevesi
🧱 Her servis kendi bounded context’ine sahiptir
🔄 Servisler bağımsız geliştirilebilir ve deploy edilebilir
🗄 Polyglot persistence yaklaşımı kullanılır
(her servis ihtiyacına uygun veritabanı)
🌍 Dış dünyaya açılan tek giriş noktası API Gateway (YARP)’tır
🔗 Servisler arası bağımlılık minimum, iletişim kontrollüdür
ℹ️ Bu repo bir “feature zengini ürün” değil,
doğru mimari pratikleri göstermeyi hedefleyen bir platformdur.

🔄 Servisler Arası İletişim
⚡ Senkron (Sync)
🌐 HTTP / REST
🧭 Kullanım senaryoları:
Anlık doğrulama
Read (query) işlemleri
Gateway → Backend çağrıları

🛠 Kullanılan araçlar:
ASP.NET Core HTTP
YARP routing & transforms
📨 Asenkron (Async)

📣 Event-driven communication
🧭 Kullanım senaryoları:
Servisler arası loosely-coupled akışlar
Side-effect işlemler
Eventually consistent süreçler

🛠 Kullanılan araçlar:
RabbitMQ
MassTransit (Consumer, Retry, Endpoint yönetimi)

🧩 Örnek Akış
Bir servis CourseCreated event’i yayınlar
İlgili servisler bu event’i dinleyerek kendi işlemlerini gerçekleştirir

🚀 Proje Nasıl Ayağa Kaldırılır?
🧰 Gereksinimler
🟦 .NET SDK
🐳 Docker
📦 Docker Compose
🌱 .NET Aspire workload
dotnet workload install aspire

⚙️ 1. Environment Ayarları

📁 Repo kök dizininde bulunan .env dosyasını doldurun:

# PostgreSQL
POSTGRES_USER=
POSTGRES_PASSWORD=

# Keycloak
KEYCLOAK_ADMIN=
KEYCLOAK_ADMIN_PASSWORD=

🌱 2. Aspire ile Çalıştırma (net-aspire branch)
dotnet run --project src/EduPlatform.AppHost


Bu komut ile:
Tüm servisler tek noktadan ayağa kalkar
Bağımlılıklar otomatik çözülür
Local geliştirme ortamı hızla hazır olur

🐳 3. Docker Compose ile Çalıştırma
docker compose up -d

🔐 Keycloak Configuration
Keycloak realm, client ve role ayarları
infra/keycloak/realm-export.json dosyası üzerinden paylaşılmaktadır.

Realm export, kullanıcı ve secret bilgileri hariç tutularak alınmıştır.

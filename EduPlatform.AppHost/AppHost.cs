var builder = DistributedApplication.CreateBuilder(args);


#region RabbitMQ

var rabbitMqUserName = builder.AddParameter("RABBITMQ-USERNAME");
var rabbitMqPassword = builder.AddParameter("RABBITMQ-PASSWORD");


var rabbitMq = builder.AddRabbitMQ("rabbitMQ", rabbitMqUserName, rabbitMqPassword, 5672).WithManagementPlugin(15672);

#endregion

#region Keycloak

var postgresUser = builder.AddParameter("POSTGRES-USER");
var postgresPassword = builder.AddParameter("POSTGRES-PASSWORD");
var keycloakDb = "keycloak-db";

var postgresDb = builder
    .AddPostgres("postgres-db-keycloak", postgresUser, postgresPassword, 5432)
    .WithImage("postgres", "16.2")
    .WithVolume("eduplatform_postgres.db.keycloak.volume", "/var/lib/postgresql/data").AddDatabase(keycloakDb);


var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "25.0")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "password")
    .WithEnvironment("KC_DB", "postgres")
    .WithEnvironment("KC_DB_URL", "jdbc:postgresql://postgres-db-keycloak/keycloak_db")
    .WithEnvironment("KC_DB_USERNAME", postgresUser)
    .WithEnvironment("KC_DB_PASSWORD", postgresPassword)
    .WithEnvironment("KC_HOSTNAME_PORT", "8080")
    .WithEnvironment("KC_HOSTNAME_STRICT_BACKCHANNEL", "false")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithArgs("start").WaitFor(postgresDb)
    .WithHttpEndpoint(8080, 8080, "keycloak-http-endpoint");


var keycloakEndpoint = keycloak.GetEndpoint("keycloak-http-endpoint");

#endregion


#region Catalog-API

var mongoUser = builder.AddParameter("MONGO-USERNAME");
var mongoPassword = builder.AddParameter("MONGO-PASSWORD");


var mongoCatalogDb = builder.AddMongoDB("mongo-db-catalog", 27030, mongoUser, mongoPassword).WithImage("mongo:8.0-rc")
    .WithDataVolume("mongo.db.catalog.volume").AddDatabase("catalog-db");

var catalogApi = builder.AddProject<Projects.EduPlatform_Catalog_API>("eduplatform-catalog-api");


catalogApi.WithReference(mongoCatalogDb).WaitFor(mongoCatalogDb).WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint).WaitFor(keycloak);

#endregion

#region Basket-API

var redisPassword = builder.AddParameter("REDIS-PASSWORD");


var redisBasketDb = builder.AddRedis("redis-db-basket").WithImage("redis:7.0-alpine")
    .WithDataVolume("redis.db.basket.volume").WithPassword(redisPassword);

var basketApi = builder.AddProject<Projects.EduPlatform_Basket_API>("eduplatform-basket-api");

basketApi.WithReference(redisBasketDb).WithReference(rabbitMq).WaitFor(rabbitMq).WithReference(keycloakEndpoint)
    .WaitFor(keycloak);

#endregion

#region Discount-API

var mongoDiscountDb = builder.AddMongoDB("mongo-db-discount", 27034, mongoUser, mongoPassword).WithImage("mongo:8.0-rc")
    .WithDataVolume("mongo.db.discount.volume").AddDatabase("discount-db");

var discountApi = builder.AddProject<Projects.EduPlatform_Discount_API>("eduplatform-discount-api");

discountApi.WithReference(mongoDiscountDb).WaitFor(mongoDiscountDb).WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint).WaitFor(keycloak);

#endregion

#region File-API

var fileApi = builder.AddProject<Projects.EduPlatform_File_API>("eduplatform-file-api");
fileApi.WithReference(rabbitMq).WaitFor(rabbitMq).WithReference(keycloakEndpoint).WaitFor(keycloak);

#endregion

#region Payment-API

var postgrePaymentUser = builder.AddParameter("POSTGRES-PAYMENT-USER");
var postgrePaymentPassword = builder.AddParameter("POSTGRES-PAYMENT-PASSWORD");

var postgresPaymentDb = builder.AddPostgres("postgres-db-payment", postgrePaymentUser, postgrePaymentPassword, 5434)
    .WithDataVolume("postgres.db.payment.volume").AddDatabase("payment-db");

var paymentApi = builder.AddProject<Projects.EduPlatform_Payment_API>("eduplatform-payment-api");
paymentApi.WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint).WaitFor(keycloak)
    .WithReference(postgresPaymentDb).WaitFor(postgresPaymentDb);

#region Payment-Outbox-Worker-Service


var paymentOutboxWorkerService =
    builder.AddProject<Projects.EduPlatform_Payment_Outbox_Worker_Service>("eduplatform-payment-outbox-worker-service");
paymentOutboxWorkerService.WithReference(postgresPaymentDb).WaitFor(postgresPaymentDb).WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint).WaitFor(keycloak);

#endregion


#endregion

#region Order-API

var sqlserverPassword = builder.AddParameter("SQLSERVER-SA-PASSWORD");


var sqlserverOrderDb = builder.AddSqlServer("sqlserver-db-order", sqlserverPassword, 1433)
    .WithDataVolume("sqlserver.db.order.volume").AddDatabase("order-db");

var orderApi = builder.AddProject<Projects.EduPlatform_Order_API>("eduplatform-order-api");

orderApi.WithReference(sqlserverOrderDb).WaitFor(sqlserverOrderDb).WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(keycloakEndpoint).WaitFor(keycloak);
// #region Order-Outbox-Worker-Service
//
//
// var orderOutboxWorkerService =
//     builder.AddProject<Projects.EduPlatform_Order_Outbox_Worker_Service>("eduplatform-order-outbox-worker-service");
// orderOutboxWorkerService.WithReference(sqlserverOrderDb).WaitFor(sqlserverOrderDb).WithReference(rabbitMq)
//     .WaitFor(rabbitMq)
//     .WithReference(keycloakEndpoint).WaitFor(keycloak);
//
// #endregion

#endregion



#region Gateway-API

builder.AddProject<Projects.EduPlatform_Gateway>("eduplatform-gateway").WithReference(keycloakEndpoint)
    .WaitFor(keycloak);

#endregion

#region Web

var web = builder.AddProject<Projects.EduPlatform_Web>("eduplatform-web");
web.WithReference(basketApi).WithReference(catalogApi).WithReference(discountApi).WithReference(orderApi)
    .WithReference(fileApi).WithReference(paymentApi).WithReference(keycloakEndpoint).WaitFor(keycloak);

#endregion

builder.Build().Run();
using EduPlatform.Discount.API.Options;
using MongoDB.Driver;

namespace EduPlatform.Discount.API.Repositories;

public static class DatabaseExtension
{
    public static IServiceCollection AddMongoDbConfigureExt(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            var options = sp.GetRequiredService<MongoOption>();
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var options = sp.GetRequiredService<MongoOption>();

            return AppDbContext.Create(mongoClient.GetDatabase(options.DatabaseName));
        });


        return services;
    }
}
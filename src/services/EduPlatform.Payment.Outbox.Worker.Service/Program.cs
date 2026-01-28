using EduPlatform.Bus;
using EduPlatform.Payment.Outbox.Worker.Service;
using EduPlatform.Payment.Outbox.Worker.Service.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddRabbitMqBusExt(builder.Configuration);

builder.Services.AddQuartz(configurator =>
{
    JobKey jobKey = new("PaymentOutboxPublishJob");
    configurator.AddJob<PaymentOutboxPublishJob>(options => options.WithIdentity(jobKey));

    TriggerKey triggerKey = new("PaymentOutboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobKey)
        .WithIdentity(triggerKey)
        .StartAt(DateTime.UtcNow)
        .WithSimpleSchedule(builder => builder
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<PaymentOutboxReadDbContext>(options =>options.UseNpgsql(builder.Configuration.GetConnectionString("payment-db")));
var host = builder.Build();
host.Run();
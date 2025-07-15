using AutoResultAmendmentService;
using Fanex.Data.SqlServer;
using MassTransit;
using ResultAmendment;
using ResultAmendment.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddFanexSql(builder.Configuration);

services.Configure<AppSettings>(builder.Configuration);

services.InitializeServices(builder.Configuration);

builder.AddResultAmendmentModule();

services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:VirtualHost"], h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });

        cfg.ReceiveEndpoint("result-amended", e =>
        {
            e.ConfigureConsumer<ResultAmendedConsumer>(context);

            e.PrefetchCount = 1;
            e.ConcurrentMessageLimit = 1;
        });
    });

    x.AddResultAmendConsumers();
});

services.AddHostedService<AutoService>();
services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.Run();
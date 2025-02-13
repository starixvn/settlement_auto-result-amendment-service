using AutoResultAmendmentService;
using Fanex.Data.SqlServer;
using MassTransit;
using ResultAmendment;
using ResultAmendment.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.InitializeServices(builder.Configuration);

builder
    .AddResultAmendmentModule();

builder.Services.AddMassTransit(x =>
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

builder.Services.AddHostedService<AutoService>();

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});

app.Run();
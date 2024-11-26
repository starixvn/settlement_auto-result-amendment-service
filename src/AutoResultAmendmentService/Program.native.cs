using AutoResultAmendment.Models;
using AutoResultAmendmentService;
using Fanex.Data.SqlServer;
using MassTransit;
using ResultAmendment;
using ResultAmendment.Consumers;

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
        cfg.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        //cfg.ConfigureEndpoints(context);
        cfg.ReceiveEndpoint("result-amended", e =>
        {
            e.Consumer<ResultAmendedConsumer>(context);
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
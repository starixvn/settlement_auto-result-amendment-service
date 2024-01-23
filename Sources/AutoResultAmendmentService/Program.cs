using AutoResultAmendment.Models;
using AutoResultAmendmentService;
using Fanex.Data.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.InitializeServices(builder.Configuration);

builder.Services.AddHostedService<AutoService>();

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});

app.Run();
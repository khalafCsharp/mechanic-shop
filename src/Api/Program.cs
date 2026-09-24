using Api;
using Application;
using Infrastructure;
using Infrastructure.RealTime;
using MechanicShop.Infrastructure.Data;
using Scalar.AspNetCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);


builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();

    app.MapScalarApiReference();

    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();


app.MapHub<WorkOrderHub>("/hubs/workorders");

app.Run();

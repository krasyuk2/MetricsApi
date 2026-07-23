using MetricsApi.Application.Implementation;
using MetricsApi.Domain.Abstractions;
using MetricsApi.Domain.Abstractions.Applications;
using MetricsApi.Domain.Abstractions.Repositories;
using MetricsApi.Middlewares;
using MetricsApi.Repository;
using MetricsApi.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ICsvParser, CsvParser>();
builder.Services.AddSingleton<ICsvValidator, CsvValidator>();
builder.Services.AddScoped<IFileProcessingService, FileProcessingService>();
builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();
}

app.MapControllers();
app.Run();
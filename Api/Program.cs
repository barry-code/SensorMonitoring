using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SensorMonitoring.Api;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(nameof(ApiOptions).ToString()));

builder.Services.AddDbContext<SensorContext>((serviceProvider, options) =>
{
    var apiOptions = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
    options.UseSqlite(apiOptions.SensorRepositoryConnection);
});

builder.Services.AddScoped<ISensorRepository, SensorRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

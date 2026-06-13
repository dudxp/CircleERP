using Microsoft.EntityFrameworkCore;
using CircleERP.Model.Services;
using CircleERP.Model.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("A string de conexão 'CircleERPConnection' não foi encontrada ou é nula.");

builder.Services.AddControllers();
builder.Services.AddScoped<CurrencyService, CurrencyService>();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseLazyLoadingProxies()
       .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:54783")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

//StartReactApp();

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapFallbackToFile("/index.html");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();

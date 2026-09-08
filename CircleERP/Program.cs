using CircleERP.Model.Data;
using CircleERP.Model.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

const string CorsPolicy = "CircleErpClient";

var builder = WebApplication.CreateBuilder(args);

// A variavel de ambiente tem prioridade (container/deploy); em desenvolvimento use
// `dotnet user-secrets set "ConnectionStrings:CircleERP" "<string>"`.
var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("CircleERP");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "String de conexao ausente. Defina a variavel de ambiente MYSQL_CONNECTION_STRING " +
        "ou a chave de configuracao 'ConnectionStrings:CircleERP'.");
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddScoped<CurrencyService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(CorsPolicy);

app.MapControllers();
app.MapFallbackToFile("/index.html");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();

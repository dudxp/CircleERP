using CircleERP.Application;
using CircleERP.Infrastructure;
using CircleERP.Middleware;
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
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    connectionString,
    builder.Configuration["Database:ServerVersion"]);

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

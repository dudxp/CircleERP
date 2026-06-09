using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using CircleERP.Model.Services;
using CircleERP.Model.Data;

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", info: new OpenApiInfo
    {
        Title = "CircleERP API",
        Version = "v1"
    });
});

var app = builder.Build();

//StartReactApp();

app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CircleERP API v1");
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapFallbackToFile("/index.html");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();

using Microsoft.OpenApi.Models;
using CircleERP.Model.ServiceModel.Data;
using Microsoft.EntityFrameworkCore;
using CircleERP.Model.Model.Services;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<CurrencyService, CurrencyService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseLazyLoadingProxies()
       .UseMySQL(builder.Configuration.GetConnectionString("CircleERPConnection"));
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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currency API",
        Version = "v1",
        Description = "API for managing currencies",
    });
});

var app = builder.Build();

//StartReactApp();

app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapFallbackToFile("/index.html");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();


//void StartReactApp()
//{
//    // Path to your React app's folder where the package.json is located
//    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

//    // Set up the process to run npm start
//    var processStartInfo = new ProcessStartInfo
//    {
//        FileName = "npm",
//        Arguments = "start",
//        WorkingDirectory = baseDirectory, // Set the working directory to your React app's folder
//        UseShellExecute = false, // Do not use the shell to execute the command
//        CreateNoWindow = true // Don't show the command window
//    };

//    // Start the React app
//    Process.Start(processStartInfo);
//}

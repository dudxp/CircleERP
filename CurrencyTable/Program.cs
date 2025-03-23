using Microsoft.OpenApi.Models;
using CurrencyTable.ServiceModel.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currency API",
        Version = "v1",
        Description = "API for managing currencies",
    });
});

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // React app's URL
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

StartReactApp();

app.UseCors("AllowReactApp")
   .UseSwagger()
   .UseSwaggerUI()
   .UseHttpsRedirection()
   .UseRouting()
   .UseAuthorization()
   .UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();

void StartReactApp()
{
    // Path to your React app's folder where the package.json is located
    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    // Set up the process to run npm start
    var processStartInfo = new ProcessStartInfo
    {
        FileName = "npm",
        Arguments = "start",
        WorkingDirectory = baseDirectory, // Set the working directory to your React app's folder
        UseShellExecute = false, // Do not use the shell to execute the command
        CreateNoWindow = true // Don't show the command window
    };

    // Start the React app
    Process.Start(processStartInfo);
}

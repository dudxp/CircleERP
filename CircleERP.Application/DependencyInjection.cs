using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CircleERP.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra todos os handlers de comando e consulta desta assembly.
    /// Casos de uso novos passam a funcionar sem tocar na composicao.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

        return services;
    }
}

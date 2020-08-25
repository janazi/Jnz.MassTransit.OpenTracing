using Microsoft.Extensions.DependencyInjection;

namespace Jnz.MassTransit.OpenTracing
{
    public static class MassTransitOpenTracingExtensions
    {
        public static void AddMassTransitOpenTracing(this IServiceCollection services)
        {
            services.AddScoped(typeof(MassTransitOpenTracingSendFilter<>));
            services.AddScoped(typeof(MassTransitOpenTracingConsumeFilter<>));
        }
    }
}

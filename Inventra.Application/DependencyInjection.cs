using Inventra.Application.Common.Interfaces;
using Inventra.Application.Inventories.CustomIds;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<ICustomIdGenerator, InventoryCustomIdGenerator>();
            services.AddUseCases();

            return services;
        }

        private IServiceCollection AddUseCases()
        {
            var useCases = typeof(DependencyInjection).Assembly
                .GetTypes()
                .Where(x => x is { IsClass: true, IsAbstract: false })
                .Where(x => typeof(IUseCase).IsAssignableFrom(x));

            foreach (var useCase in useCases)
                services.AddScoped(useCase);

            return services;
        }
    }
}

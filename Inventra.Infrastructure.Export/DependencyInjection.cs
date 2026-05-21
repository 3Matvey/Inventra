using Inventra.Application.Inventories.Exports;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure.Export;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddExportServices()
        {
            services.AddScoped<IInventoryExportFileWriter, InventoryExportFileWriter>();

            return services;
        }
    }
}

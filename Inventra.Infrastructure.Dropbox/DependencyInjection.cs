using Inventra.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure.Dropbox;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDropboxServices(IConfiguration configuration)
        {
            services.Configure<DropboxOptions>(
                configuration.GetSection(DropboxOptions.SectionName));

            services.AddHttpClient<ISupportTicketFileStorage, DropboxSupportTicketFileStorage>();

            return services;
        }
    }
}

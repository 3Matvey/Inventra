using Inventra.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Inventra.Infrastructure.Cloudinary;

public static class DependencyInjection
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddCloudinaryServices(IConfiguration configuration)
        {
            services.Configure<CloudinaryOptions>(
                configuration.GetSection(CloudinaryOptions.SectionName));

            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

            return services;
        }
    }
}

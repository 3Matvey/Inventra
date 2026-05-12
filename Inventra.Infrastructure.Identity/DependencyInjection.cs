using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure.Identity;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentityServices(IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddIdentityDatabase(configuration);
            services.AddIdentityCore();
            services.AddExternalAuthentication(configuration);
            services.AddIdentityPorts();

            return services;
        }

        private IServiceCollection AddIdentityDatabase(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());

            return services;
        }

        private IServiceCollection AddIdentityCore()
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        private IServiceCollection AddExternalAuthentication(IConfiguration configuration)
        {
            services.ConfigureApplicationCookie()
                .AddGoogleIfConfigured(configuration)
                .AddFacebookIfConfigured(configuration);

            return services;
        }

        private IServiceCollection ConfigureApplicationCookie()
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/access-denied";
            });

            return services;
        }

        private IServiceCollection AddIdentityPorts()
        {
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IAuthenticationSession, AuthenticationSession>();
            services.AddScoped<IExternalIdentityService, ExternalIdentityService>();
            services.AddScoped<IIdentityAccountService, IdentityAccountService>();

            return services;
        }
    }

    extension(IServiceCollection services)
    {
        private IServiceCollection AddGoogleIfConfigured(IConfiguration configuration)
        {
            var section = configuration.GetSection("Authentication:Google");

            if (!section.Exists())
                return services;

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = section["ClientId"] ?? string.Empty;
                options.ClientSecret = section["ClientSecret"] ?? string.Empty;
            });

            return services;
        }

        private IServiceCollection AddFacebookIfConfigured(IConfiguration configuration)
        {
            var section = configuration.GetSection("Authentication:Facebook");

            if (!section.Exists())
                return services;

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = section["AppId"] ?? string.Empty;
                options.AppSecret = section["AppSecret"] ?? string.Empty;
            });

            return services;
        }
    }
}

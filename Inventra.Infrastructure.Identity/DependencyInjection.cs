using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
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
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

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
                .AddGitHubIfConfigured(configuration);

            return services;
        }

        private IServiceCollection ConfigureApplicationCookie()
        {
            services.AddScoped<BlockedUserCookieEvents>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/access-denied";
                options.EventsType = typeof(BlockedUserCookieEvents);
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

            if (!IsConfigured(section, "ClientId", "ClientSecret"))
                return services;

            services.AddAuthentication().AddGoogle(options =>
            {
                ConfigureOAuth(options, section);
                options.CallbackPath = "/signin-google";
            });

            return services;
        }

        private IServiceCollection AddGitHubIfConfigured(IConfiguration configuration)
        {
            var section = configuration.GetSection("Authentication:GitHub");

            if (!IsConfigured(section, "ClientId", "ClientSecret"))
                return services;

            services.AddAuthentication().AddGitHub(options =>
            {
                ConfigureOAuth(options, section);
                options.CallbackPath = "/signin-github";
                options.Scope.Add("user:email");
            });

            return services;
        }
    }

    private static void ConfigureOAuth(OAuthOptions options, IConfigurationSection section)
    {
        options.ClientId = GetRequired(section, "ClientId");
        options.ClientSecret = GetRequired(section, "ClientSecret");
        options.SignInScheme = IdentityConstants.ExternalScheme;
    }

    private static bool IsConfigured(IConfigurationSection section, params string[] keys)
    {
        var presentKeysCount = keys.Count(key => !string.IsNullOrWhiteSpace(section[key]));

        return presentKeysCount switch
        {
            0 => false,
            var count when count == keys.Length => true, // all required keys are configured.
            _ => throw CreatePartialConfigurationException(section, keys)
        };
    }

    private static string GetRequired(IConfigurationSection section, string key)
    {
        var value = section[key];

        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw CreateMissingConfigurationException(section, key);
    }

    private static InvalidOperationException CreateMissingConfigurationException(
        IConfigurationSection section,
        string key)
    {
        return new InvalidOperationException(
            $"Required configuration key '{section.Path}:{key}' is missing.");
    }

    private static InvalidOperationException CreatePartialConfigurationException(
        IConfigurationSection section,
        string[] keys)
    {
        var expectedKeys = string.Join(", ", keys.Select(key => $"{section.Path}:{key}"));

        return new InvalidOperationException(
            $"External auth provider '{section.Path}' is partially configured. " +
            $"Required keys: {expectedKeys}.");
    }
}

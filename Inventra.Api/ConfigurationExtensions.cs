namespace Inventra.Api;

internal static class ConfigurationExtensions
{
    public static ConfigurationManager AddPortableSecrets(this ConfigurationManager configuration)
    {
        if (Directory.Exists("/run/secrets"))
            configuration.AddKeyPerFile("/run/secrets", optional: true);

        if (Directory.Exists("/etc/secrets"))
            configuration.AddKeyPerFile("/etc/secrets", optional: true);

        configuration.AddEnvironmentVariables();

        return configuration;
    }
}

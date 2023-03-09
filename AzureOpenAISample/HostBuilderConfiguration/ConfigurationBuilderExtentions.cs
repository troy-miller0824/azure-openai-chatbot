using AzureOpenAISample.Implementations;
using AzureOpenAISample.Models;
using AzureOpenAISample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AzureOpenAISample.HostBuilderConfiguration;

/// <summary>
/// Extensions for the <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// A constant with the Developer environment variable name.
    /// </summary>
    public const string DevelopmentEnvironment = "Development";
    /// <summary>
    /// A constant with the Production environment variable name.
    /// </summary>
    public const string ProductionEnvironment = "Production";

    /// <summary>
    /// Configures the appsettings order depending on the current environment.
    /// </summary>
    /// <param name="builder">The instance of type <see cref="IConfigurationBuilder"/> used to build application configuration.</param>
    /// <returns>The same <see cref="IConfigurationBuilder"/> for chaining.</returns>
    public static IConfigurationBuilder ConfigureSettingDefaults(this IConfigurationBuilder builder)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ProductionEnvironment;

        builder.AddEnvironmentVariables();
        if (env.Equals(DevelopmentEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            builder.AddUserSecrets(Assembly.GetEntryAssembly(), true);
        }

        return builder;
    }

    /// <summary>
    /// Configures Greenhopper
    /// </summary>
    /// <param name="hostBuilder">An instance of a <see cref="IHostBuilder"/>.</param>
    /// <returns> The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public static IHostBuilder ConfigureApp(this IHostBuilder hostBuilder)
    {

        return hostBuilder
            .ConfigureAppConfiguration(builder =>
            {
                builder.ConfigureSettingDefaults();
            })
            .ConfigureServices(sc =>
            {
                sc.AddLogging(builder => builder.AddDebug());

                sc.AddHttpClient<IOpenAIService, OpenAIService>()
                  .ConfigureHttpClient((serviceProvider, client) =>
                  {
                      var config = serviceProvider.GetService<IConfiguration>();

                      //Set Base Address
                      var baseUri = new Uri(config.GetValue<string>(OpenAISettingNames.OpenAIBaseUrl));
                      if (baseUri == default)
                      {
                          throw new Exception($"The {OpenAISettingNames.OpenAIBaseUrl} setting is empty or not a URL!");
                      }

                      var deploymentName = config.GetValue<string>(OpenAISettingNames.OpenAIDeploymentName);
                      if (string.IsNullOrWhiteSpace(deploymentName))
                      {
                          throw new Exception($"The {OpenAISettingNames.OpenAIDeploymentName} setting is empty!");
                      }
                      var relativeUri = $"/openai/deployments/{deploymentName.Trim('/')}/";

                      client.BaseAddress = new Uri(baseUri, relativeUri);

                      //Set Default Headers
                      var key = config.GetValue<string>(OpenAISettingNames.OpenAIKey);
                      client.DefaultRequestHeaders.Add("api-key", key);
                  });

                sc.AddSingleton<IDiscussionService, DiscussionService>();
            });
    }
}
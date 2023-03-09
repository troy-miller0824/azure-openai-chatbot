using AzureOpenAISample.HostBuilderConfiguration;
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureApp()
    .Build()
    .Run();

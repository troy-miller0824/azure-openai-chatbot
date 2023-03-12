using AzureOpenAISample.Marv;
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureMarv()
    .Build()
    .Run();

using DocumentService.Configuration;
using DocumentService.Sagas;
using MassTransit;
using Microsoft.Azure.Cosmos.Table;

namespace DocumentService;

public static class MessagingConfiguration
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddSagaStateMachine<GenerateDocumentSaga, GenerateDocumentSagaState>()
                .AzureTableRepository(repoConfig => repoConfig.ConnectionFactory(configuration.CreateCloudTable));

            busConfig.UsingAzureServiceBus((context, config) =>
            {
                config.UseInMemoryOutbox();
                config.UseConsumeFilter(typeof(LoggingEnrichmentFilter<>), context);
                config.Host(configuration.GetConnectionString("ServiceBus"));
                config.ConfigureEndpoints(context);
            });
        });
    }

    private static CloudTable CreateCloudTable(this IConfiguration configuration)
    {
        var azureTableConfiguration = configuration
            .GetSection("Sagas")
            .Get<SagaAzureTableConfiguration>();

        var cloudTable = new CloudTable(
            new Uri(azureTableConfiguration.TableAddress),
            new StorageCredentials(
                azureTableConfiguration.AccountName,
                azureTableConfiguration.AccessKey));
        return cloudTable;
    }
}
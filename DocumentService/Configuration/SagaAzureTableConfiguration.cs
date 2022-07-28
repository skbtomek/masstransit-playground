namespace DocumentService.Configuration;

internal class SagaAzureTableConfiguration
{
    public string TableAddress { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
}
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MessageSender.Fixtures;

public class Configuration
{
    private static readonly Lazy<IConfiguration> ConfigurationLazy;

    static Configuration()
    {
        ConfigurationLazy = new Lazy<IConfiguration>(GetConfiguration);
    }

    public static IConfiguration ConfigurationInstance => ConfigurationLazy.Value;

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();    }
}
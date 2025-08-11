using Microsoft.Extensions.Configuration;

public class ConfigLoader
{
    public IConfigurationRoot Configuration { get; }

    public ConfigLoader()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // where to look for the file
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
    }
}

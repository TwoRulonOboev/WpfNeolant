using Microsoft.Extensions.Configuration;
using System.IO;

namespace WpfNeolant.Utils
{
    public class ConfigUtility
    {
        public static IConfigurationRoot Config {  get; private set; }

        static ConfigUtility()
        {
            Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json")
            .Build();
        }
    }
}
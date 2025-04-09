using Microsoft.Extensions.Configuration;

namespace MyRecipeBook.Infraestructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string ConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("Connection")!;
        }

        public static bool IsUnitTestEnvironment(this IConfiguration configuration)
        {
            var value = configuration.GetSection("InMemoryTest").Value;
            return bool.TryParse(value, out var result) && result;
        }
    }
}
using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAccess;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;
using MyRecipeBook.Infraestructure.Extensions;
using MyRecipeBook.Infraestructure.Security.Tokens.Access.Generator;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services);
            AddTokens(services, configuration);

            if (configuration.IsUnitTestEnvironment())
                return;

            AddDbContext_MySqlServer(services, configuration);
            AddFluentMigrator_MySql(services, configuration);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        }

        private static void AddDbContext_MySqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 41));
           
            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });
        }

        private static void AddFluentMigrator_MySql(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore()
                .ConfigureRunner(options =>
                {
                    options.AddMySql5()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(Assembly.Load("MyRecipeBook.Infraestructure")).For.All();
                });
        }

        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = uint.Parse(configuration.GetRequiredSection("Settings:Jwt:ExpirationTimeMinutes").Value!);
            var signingKey = configuration.GetRequiredSection("Settings:Jwt:SigningKey").Value;

            services.AddScoped<IAccessTokenGenerator>(option => new JwtTokenGenerator(signingKey!, expirationTimeMinutes));
        }
    }
}

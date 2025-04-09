using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Infraestructure.DataAccess;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;
using MyRecipeBook.Infraestructure.Extensions;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services);

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

        public static void AddFluentMigrator_MySql(IServiceCollection services, IConfiguration configuration)
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
    }
}

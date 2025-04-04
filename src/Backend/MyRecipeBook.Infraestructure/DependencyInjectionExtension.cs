using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Infraestructure.DataAccess;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext_MySqlServer(services, configuration);
            AddRepositories(services);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        }

        private static void AddDbContext_MySqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 41));
           
            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });
        }
    }
}

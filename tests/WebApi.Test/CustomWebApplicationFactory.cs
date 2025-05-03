using CommomTesteUtilities.Entities;
using CommomTesteUtilities.IdEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Entities.Enums;
using MyRecipeBook.Infraestructure.DataAccess;

namespace WebApi.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private MyRecipeBook.Domain.Entities.User _user = default!;
        private MyRecipeBook.Domain.Entities.Recipe _recipe = default!;
        private string _password = string.Empty;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test")
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<MyRecipeBookDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                    
                    services.AddDbContext<MyRecipeBookDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(provider);
                    });

                    var scope = services.BuildServiceProvider().CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
                    dbContext.Database.EnsureDeleted();

                    StartDatabase(dbContext);
                });
        }

        public string GetEmail() => _user.Email;
        public string GetPassword() => _password;
        public string GetName() => _user.Name;

        public Guid GetUserIdentifier() => _user.UserIdentifier;

        public string GetRecipeTitle() => _recipe.Title;

        public Difficulty GetDifficulty() => _recipe.Difficulty!.Value;
        public CookingTime GetCookingTime() => _recipe.CookingTime!.Value;

        public string GetRecipeId() => IdEncrypterBuilder.Build().Encode(_recipe.id);

        public IList<MyRecipeBook.Domain.Entities.Enums.DishType> GetDishTypes() => _recipe.DishTypes.Select(x => x.Type).ToList();

        private void StartDatabase(MyRecipeBookDbContext dbContext)
        {
            (_user, _password) = UserBuilder.Build();

            _recipe = RecipeBuilder.Build(_user);

            dbContext.Users.Add(_user);

            dbContext.Recipes.Add(_recipe);

            dbContext.SaveChanges();
        }
    }
}

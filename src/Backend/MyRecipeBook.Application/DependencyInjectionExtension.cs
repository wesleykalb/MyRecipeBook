using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.UseCases.Dashboard;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Application.UseCases.Login.External;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using MyRecipeBook.Application.UseCases.User.Delete.Request;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using Sqids;

namespace MyRecipeBook.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddAutoMapper(services);
            AddEncoder(services, configuration);
            AddUseCases(services);
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddScoped(options => new AutoMapper.MapperConfiguration(optionsMapper =>
            {
                var sqids = options.GetService<SqidsEncoder<long>>()!;
                optionsMapper.AddProfile(new AutoMapping(sqids));
            }).CreateMapper());

        }

        public static void AddEncoder(this IServiceCollection services, IConfiguration configuration)
        {
            var sqids = new SqidsEncoder<long>(new ()
            {
                MinLength = 3,
                Alphabet = configuration.GetValue<string>("Settings:IdCryptographyAlphabet")!,
            });
            services.AddSingleton(sqids);
        }

        public static void AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IDoLoginUserCase, DoLoginUserCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IRegisterRecipeUseCase, RegisterRecipeUseCase>();
            services.AddScoped<IFilterRecipeUseCase, FilterRecipeUseCase>();
            services.AddScoped<IGetRecipeByIdUseCase, GetRecipeByIdUseCase>();
            services.AddScoped<IDeleteRecipeUseCase, DeleteRecipeUseCase>();
            services.AddScoped<IUpdateRecipeUseCase, UpdateRecipeUseCase>();
            services.AddScoped<IGetDashboardUseCase, GetDashboardUseCase>();
            services.AddScoped<IGenerateRecipeUseCase, GenerateRecipeUseCase>();
            services.AddScoped<IAddUpdateImageCoverUseCase, AddUpdateImageCoverUseCase>();
            services.AddScoped<IRequestDeleteUserUseCase, RequestDeleteUserUseCase>();
            services.AddScoped<IDeleteUserAccountUseCase, DeleteUserAccountUseCase>();
            services.AddScoped<IExternalLoginUseCase, ExternalLoginUseCase>();
        }
    }
}

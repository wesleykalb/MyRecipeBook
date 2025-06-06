﻿using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAccess;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;
using MyRecipeBook.Infraestructure.Extensions;
using MyRecipeBook.Infraestructure.Security.Cryptography;
using MyRecipeBook.Infraestructure.Security.Tokens.Access.Generator;
using MyRecipeBook.Infraestructure.Security.Tokens.Access.Validator;
using Namespace.MyRecipeBook.Domain.Security.LoggedUser;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Infraestructure.Services.OpenAI;
using OpenAI;
using OpenAI.Chat;
using Microsoft.EntityFrameworkCore.Metadata;
using MyRecipeBook.Domain.ValueObjects;
using FluentMigrator.Runner.Initialization;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Infraestructure.Services.Storage;
using Azure.Storage.Blobs;
using MyRecipeBook.Domain.Services.ServiceBus;
using Azure.Messaging.ServiceBus;
using MyRecipeBook.Infraestructure.Services.ServiceBus;
using MyRecipeBook.Infraestructure.Security.Tokens.Refresh;
using MyRecipeBook.Domain.Repositories.Token;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddPasswordEncripter(services);
            AddRepositories(services);
            AddLoggedUser(services);
            AddTokens(services, configuration);

            if (configuration.IsUnitTestEnvironment())
                return;

            AddDbContext_MySqlServer(services, configuration);
            AddFluentMigrator_MySql(services, configuration);
            AddOpenAI(services, configuration);
            AddAzureStorage(services, configuration);
            AddQueue(services, configuration);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepositoy>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepositoy>();
            services.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepositoy>();
            services.AddScoped<IUserDeleteOnlyRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
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
            services.AddScoped<IAccessTokenValidator>(option => new JwtTokenValidator(signingKey!));

            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        }

        private static void AddLoggedUser(IServiceCollection services) => services.AddScoped<ILoggedUser, LoggedUser>();
    
        private static void AddPasswordEncripter(this IServiceCollection services)
        {
            services.AddScoped<IPasswordEncripter, BCryptNet>();
        }

        private static void AddOpenAI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGenerateRecipeAI, ChatGPTService>();
            var openAIKey = configuration.GetRequiredSection("Settings:OpenAI:ApiKey").Value!;

            services.AddScoped(_ => new ChatClient(MyRecipeBookRuleConstants.CHAT_MODEL, openAIKey));

        }
        private static void AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetRequiredSection("Settings:BlobStorage:Azure").Value!;

            if (!string.IsNullOrEmpty(connectionString))
                services.AddScoped<IBlobStorageService>(c => new AzureStorageService(new BlobServiceClient(connectionString)));
        }

        private static void AddQueue(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetRequiredSection("Settings:ServiceBus:DeleteUserAccount").Value!;

            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            var deleteUserQueue = new DeleteUserQueue(client.CreateSender("user"));

            var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("user", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1
            }));

            services.AddSingleton(deleteUserProcessor);

            services.AddScoped<IDeleteUserQueue>(option => deleteUserQueue);
        }
    }
}

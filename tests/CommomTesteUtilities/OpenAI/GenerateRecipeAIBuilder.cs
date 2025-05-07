using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Services.OpenAI;
using OpenAI.VectorStores;

namespace CommomTesteUtilities.OpenAI;

public class GenerateRecipeAIBuilder
{
    public static IGenerateRecipeAI Build(GenerateRecipeDto recipeDto)
    {
        var mock = new Mock<IGenerateRecipeAI>();

        mock.Setup(x => x.Generate(It.IsAny<IList<string>>())).ReturnsAsync(recipeDto);
        return mock.Object;
    }
}
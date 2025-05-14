using CommomTesteUtilities.BlobStorage;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Repositories;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.User.Delete;

public class DeleteUserAccountUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUserCase();

        Func<Task> result = async () => {await useCase.Execute(user.UserIdentifier);};

        await result.ShouldNotThrowAsync();
    }
    private static DeleteUserAccountUseCase CreateUserCase()
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userDeleteOnlyRepository = new UserDeleteOnlyRepositoryBuilder().Build();
        var blobService = new BlobStorageServiceBuilder().Build();

        return new DeleteUserAccountUseCase(
            userDeleteOnlyRepository,
            blobService,
            unitOfWork);
    }
}
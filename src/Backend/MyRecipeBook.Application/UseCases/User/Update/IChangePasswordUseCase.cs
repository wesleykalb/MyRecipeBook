using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.User.Update;
    public interface IChangePasswordUseCase
    {
        Task Execute(RequestChangePasswordJson request);
    }

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace MyRecipeBook.Communication.Requests;

public class RequestRegisterRecipeFormData : RequestRecipeJson
{
    public IFormFile? Image {get; set;} 
}
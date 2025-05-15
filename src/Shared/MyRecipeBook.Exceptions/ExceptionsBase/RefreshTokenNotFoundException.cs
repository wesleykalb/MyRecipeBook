using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class RefreshTokenNotFoundException : MyRecipeBookException
{
    public RefreshTokenNotFoundException() : base(ResourceMessagesException.EXPIRED_SESSION)
    {
    }
}
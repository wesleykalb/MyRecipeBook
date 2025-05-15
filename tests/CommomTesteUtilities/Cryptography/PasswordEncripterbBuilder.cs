using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infraestructure.Security.Cryptography;

namespace CommomTesteUtilities.Cryptography
{
    public class PasswordEncripterbBuilder
    {
        public static IPasswordEncripter Build() => new BCryptNet();
    }
}

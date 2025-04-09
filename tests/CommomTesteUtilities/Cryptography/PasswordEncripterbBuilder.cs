using MyRecipeBook.Application.Services.Cryptography;

namespace CommomTesteUtilities.Cryptography
{
    public class PasswordEncripterbBuilder
    {
        public static PasswordEncripter Build() => new PasswordEncripter("abc1234");
    }
}

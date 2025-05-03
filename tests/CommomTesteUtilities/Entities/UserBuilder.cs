using Bogus;
using CommomTesteUtilities.Cryptography;
using MyRecipeBook.Domain.Entities;

namespace CommomTesteUtilities.Entities
{
    public class UserBuilder
    {
        public static (User user, string password) Build()
        {            
            var passwordEncriter = PasswordEncripterbBuilder.Build();
            
            var password = new Faker().Internet.Password();

            var user =  new Faker<User>()
                .RuleFor(x => x.id, () => 1)
                .RuleFor(x => x.Name, f => f.Person.FullName)
                .RuleFor(x => x.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(x => x.UserIdentifier, _ => Guid.NewGuid())
                .RuleFor(x => x.Password, (f) => passwordEncriter.Encrypt(password));

            return (user, password);
        }
    }
}

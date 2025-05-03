using System.Security.Cryptography.X509Certificates;
using Bogus;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.tests.CommomTesteUtilities.Requests;

public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.Email, (f, person) => f.Internet.Email(person.Name));
    }
}
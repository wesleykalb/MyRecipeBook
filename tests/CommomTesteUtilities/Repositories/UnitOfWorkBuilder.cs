using Moq;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace CommomTesteUtilities.Repositories
{
    public class UnitOfWorkBuilder
    {
        public static IUnitOfWork Build()
        {
            var mock = new Mock<IUnitOfWork>();

            return mock.Object;
        }
    }
}

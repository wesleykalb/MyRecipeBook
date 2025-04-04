using Microsoft.EntityFrameworkCore;

namespace MyRecipeBook.Infraestructure.DataAccess.Repositories
{
    public interface IUnitOfWork
    {
        public Task Commit();
    }
}

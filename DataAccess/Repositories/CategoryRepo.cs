using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public abstract class CategoryRepoBase : RepoBase<Category> 
    {
        protected CategoryRepoBase(ETradeContext dbContext) : base(dbContext) 
        {
        }
    }

    public class CategoryRepo : CategoryRepoBase 
    {
        public CategoryRepo(ETradeContext dbContext) : base(dbContext) 
        {
        }
    }
}

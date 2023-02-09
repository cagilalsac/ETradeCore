using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
	public abstract class UserRepoBase : RepoBase<User>
	{
		protected UserRepoBase(ETradeContext dbContext) : base(dbContext)
		{
		}
	}

	public class UserRepo : UserRepoBase
	{
		public UserRepo(ETradeContext dbContext) : base(dbContext)
		{
		}
	}
}

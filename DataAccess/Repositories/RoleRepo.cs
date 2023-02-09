using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
	public abstract class RoleRepoBase : RepoBase<Role>
	{
		protected RoleRepoBase(ETradeContext dbContext) : base(dbContext)
		{
		}
	}

	public class RoleRepo : RoleRepoBase
	{
		public RoleRepo(ETradeContext dbContext) : base(dbContext)
		{
		}
	}
}

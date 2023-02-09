using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
	public abstract class CityRepoBase : RepoBase<City>
	{
		protected CityRepoBase(ETradeContext dbContext) : base(dbContext)
		{
		}
	}

	public class CityRepo : CityRepoBase
	{
		public CityRepo(ETradeContext dbContext) : base(dbContext)
		{
		}
	}
}

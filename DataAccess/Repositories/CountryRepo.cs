using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
	public abstract class CountryRepoBase : RepoBase<Country>
	{
		protected CountryRepoBase(ETradeContext dbContext) : base(dbContext)
		{
		}
	}

	public class CountryRepo : CountryRepoBase
	{
		public CountryRepo(ETradeContext dbContext) : base(dbContext)
		{
		}
	}
}

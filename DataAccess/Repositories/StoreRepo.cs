using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories
{
    public abstract class StoreRepoBase : RepoBase<Store>
    {
        protected StoreRepoBase(ETradeContext dbContext) : base(dbContext)
        {
        }

        // StoreRepoBase dolayısıyla RepoBase'deki esas sorgulama yaptığımız ve virtual tanımladığımız Query methodunu eziyoruz ki
        // mağazalar için her zaman uygulamamızda silindi (IsDeleted) sütunu 0 (false) olan silinmemiş kayıtları sorgulayabilelim
        public override IQueryable<Store> Query(params Expression<Func<Store, object>>[] entitiesToInclude)
        {
            return base.Query(entitiesToInclude).Where(q => !q.IsDeleted);
        }

        // StoreRepoBase dolayısıyla RepoBase'deki esas silme işlemi yaptığımız Delete methodunu eziyoruz ki
        // bir mağaza silindiğinde bu method üzerinden veritabanı tablosundan silinmesin, silindi (IsDeleted) sütunu 1 (true) olarak güncellensin
        public override void Delete(Store entity, bool save = true)
        {
            entity.IsDeleted = true;
            base.Update(entity, save);
        }
    }

    public class StoreRepo : StoreRepoBase
    {
        public StoreRepo(ETradeContext dbContext) : base(dbContext)
        {
        }
    }
}

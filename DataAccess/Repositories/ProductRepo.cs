using AppCore.DataAccess.EntityFramework.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public abstract class ProductRepoBase : RepoBase<Product> // ProductRepoBase Product tipi üzerinden RepoBase'den miras alan ve
                                                              // Product CRUD işlemleri için oluşturulan abstract (soyut) bir class'tır.
    {
        protected ProductRepoBase(ETradeContext dbContext) : base(dbContext) // bu abstract class'a MVC projesindeki Program.cs IoC Container'ında
                                                                             // bağımlılık yönetimi yapılacak ve buna göre dışarıdan ETradeContext
                                                                             // tipinde new'lenerek gönderilecek dbContext objesi constructor üzerinden enjekte edilerek
                                                                             // RepoBase'in parametreli constructor'ına gönderilir ki RepoBase'de kullanılabilsin.
        {
        }
    }

    public class ProductRepo : ProductRepoBase // ProductRepo ProductRepoBase'den miras alan ve MVC projesindeki Program.cs IoC Container'ında
                                               // bağımlılığı ProductRepoBase ile yönetilecek ve bu sayede ilgili servislere constructor üzerinden
                                               // new'lenerek enjekte edilerek kullanılacak concrete (somut) bir class'tır.
    {
        public ProductRepo(ETradeContext dbContext) : base(dbContext) // bu concrete class'a MVC projesindeki Program.cs IoC Container'ında
                                                                      // bağımlılık yönetimi yapılacak ve buna göre dışarıdan ETradeContext
                                                                      // tipinde new'lenerek gönderilecek dbContext objesi constructor üzerinden enjekte edilerek
                                                                      // ProductRepoBase'in parametreli constructor'ına gönderilir ki ProductRepoBase'de
                                                                      // dolayısıyla da RepoBase'de kullanılabilsin.
        {
        }
    }
}

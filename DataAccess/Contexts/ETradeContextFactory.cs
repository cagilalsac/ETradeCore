using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess.Contexts
{
    public class ETradeContextFactory : IDesignTimeDbContextFactory<ETradeContext> // ETradeContext objesini oluşturup kullanılmasını sağlayan fabrika class'ı,
                                                                                   // scaffolding işlemleri için bu class oluşturulmalıdır
    {
        public ETradeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ETradeContext>();
            optionsBuilder.UseSqlServer("server=.\\SQLEXPRESS;database=LEC_ETradeCore;user id=sa;password=sa;multipleactiveresultsets=true;trustservercertificate=true;");
            // önce veritabanımızın (development veritabanı kullanılması daha uygundur) connection string'ini içeren bir obje oluşturuyoruz

            return new ETradeContext(optionsBuilder.Options); // daha sonra yukarıda oluşturduğumuz obje üzerinden ETradeContext tipinde bir obje dönüyoruz
        }
    }
}

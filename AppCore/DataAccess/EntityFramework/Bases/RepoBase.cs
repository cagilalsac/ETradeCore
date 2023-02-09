#nullable disable

using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore.DataAccess.EntityFramework.Bases
{
    // Repository Pattern: Veritabanındaki tablolarda (entity) kolay ve merkezi olarak CRUD (create, read, update, delete)
    // işlemlerinin yapılmasını sağlayan tasarım desenidir (design pattern). Önce DbSet'ler üzerinde istenilen değişiklikler yapılır
    // daha sonra tek bir iş olarak veritabanına yapılan değişiklikler SQL sorguları çalıştırılarak yansıtılır (Unit of Work).



    //public abstract class RepoBase<TEntity> 
    // Tip olarak TEntity üzerinden herhangi bir tip kullanacak class.

    //public abstract class RepoBase<TEntity> : where TEntity : class 
    // Referans tip olarak TEntity üzerinden herhangi bir tip kullanacak class.

    //public abstract class RepoBase<TEntity> : where TEntity : class, new() 
    // new'lenebilen referans tip olarak TEntity üzerinden herhangi bir tip kullanacak class.

    //public abstract class RepoBase<TEntity> : where TEntity : RecordBase, new() 
    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden herhangi bir tip (entity ve model) kullanacak class.



    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden herhangi bir tip (entity ve model) kullanacak,
    // aynı zamanda IDisposable interface'ini implemente edecek class.
    public abstract class RepoBase<TEntity> : IDisposable where TEntity : RecordBase, new()
    {
        protected readonly DbContext _dbContext; // DbContext EntityFramework'ün CRUD işlemleri yapmamızı sağlayan temel class'ı,
                                                 // readonly olarak sadece constructor üzerinden veya bu satırda set edilebilir.
                                                 // protected erişim bildirgeci ile _dbContext'in ihtiyaç halinde sadece bu class'tan miras alan repository'lerde kullanılması sağlanır.

        protected RepoBase(DbContext dbContext) // dbContext Dependency Injection (Constructor Injection) ile RepoBase'e dışarıdan new'lenerek enjekte edilecek.
        {
            _dbContext = dbContext;
        }

        // Read işlemi: parametre olarak sorguya dahil olacak entity navigasyon özelliklerini alır (opsiyonel) ve ilgili entity için sorguyu oluşturur ancak çalıştırmaz.
        // Sorguyu çalıştırmak için ToList, SingleOrDefault, vb. methodları çağrılmalıdır.
        // virtual tanımladık ki bu class'dan miras alan class'larda ihtiyaca göre bu method ezilebilsin ve implementasyonu özelleştirilebilsin.
        // Expression, Func (delege) veya Action (delege) görülen yerde Lambda Expression (örneğin e => e.) ile delege edilen tipin (örneğin burada TEntity)
        // özellikleri üzerinden eğer Action kullanılıyorsa sonuç dönmeksizin, Func kullanılıyorsa Func içerisinde kullanılan ilk tip (örneğin burada TEntity) yanında
        // dönülecek ikinci tip (örneğin burada object veya bool) üzerinden sonuç dönülerek istenilen işlem gerçekleştirilebilir
        // (örneğin Func delegesi bool sonuç dönüyorsa e => e.Id == 7 ya da e => e.Guid == "bd7caa37-2c5f-4df1-972f-cddca4314008").
        public virtual IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] entitiesToInclude)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable(); // TEntity tipindeki DbSet'i sorgu olarak al.
            foreach (var entityToInclude in entitiesToInclude) // parametre olarak gelen entity referanslarını sorguya dahil et.
            {
                query = query.Include(entityToInclude);
            }
            return query;
        }

        // Read işlemi: yukarıdaki Query methodunun predicate ile bool sonuç dönen, bir veya isteğe göre daha fazla koşulun and ya da or ile birleştirilerek
        // sorguyu where ile filtreleyen ve dönen overload methodu.
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] entitiesToInclude)
        {
            var query = Query(entitiesToInclude); // önce yukarıdaki Query methodu üzerinden parametre olarak gönderilen entity referanslarını sorguya dahil ediyoruz.
            return query.Where(predicate); // daha sonra sorguyu where ile gelen koşul veya koşullar üzerinden filtreleyip dönüyoruz.
        }

        // Read işlemi: bu class'ta belirtilen tip dışında belirtilen başka bir entity tipi üzerinden sorgu oluşturmamızı sağlar.
        // where tip tanımlanan methodlarda da class'larda kullanıldığı şekilde kullanılabilir.
        public virtual IQueryable<TRelationalEntity> Query<TRelationalEntity>() where TRelationalEntity : class, new()
        {
            return _dbContext.Set<TRelationalEntity>().AsQueryable();
        }

        // Eğer istenirse predicate ile belirtilen koşul veya koşullara sahip herhangi bir kayıt var mı kontrolü bu method üzerinden yapılabilir.
        // Koşul veya koşullarda belki ilişkili entity referansları kullanılabilir diye sorguya bu entity'leri dahil ediyoruz. 
        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] entitiesToInclude)
        {
            return Query(entitiesToInclude).Any(predicate);
        }

        // Create işlemi: gönderilen entity'yi DbSet'e ekler ve eğer save parametresi true ise değişikliği Save methodu üzerinden veritabanına yansıtır.
        public virtual void Add(TEntity entity, bool save = true)
        {
            entity.Guid = Guid.NewGuid().ToString(); // her eklenecek kayıt için tekil bir Guid oluşturup entity'e atıyoruz ki istenirse Id yerine Guid üzerinden de işlemler yapılabilsin

            //DbContext.Set<TEntity>().Add(entity); // aşağıdaki satır ile de ekleme işlemi yapılabilir.
            _dbContext.Add(entity);

            if (save)
                Save();
        }

        // Update işlemi: gönderilen entity'yi DbSet'te günceller ve eğer save parametresi true ise değişikliği Save methodu üzerinden veritabanına yansıtır.
        public virtual void Update(TEntity entity, bool save = true)
        {
            //DbContext.Set<TEntity>().Update(entity); // aşağıdaki satır ile de güncelleme işlemi yapılabilir.
            _dbContext.Update(entity);

            if (save)
                Save();
        }

        // Delete işlemi: gönderilen entity'yi DbSet'ten çıkarır ve eğer save parametresi true ise değişikliği Save methodu üzerinden veritabanına yansıtır.
        public virtual void Delete(TEntity entity, bool save = true)
        {
            //DbContext.Set<TEntity>().Remove(entity); // aşağıdaki satır ile de silme işlemi yapılabilir.
            _dbContext.Remove(entity);

            if (save)
                Save();
        }

        // Delete işlemi: gönderilen id üzerinden TEntity'de RecordBase'den miras alınan Id özelliği ile entity'e ulaşır ve yukarıdaki Delete methoduna gönderir.
        public virtual void Delete(int id, bool save = true)
        {
            var entity = _dbContext.Set<TEntity>().Find(id); // Find methodu DbSet'ler ile primary key veya primary key'ler üzerinden kullanılabilir,
                                                            // eğer belirtilen id'ye sahip kayıt varsa kaydı obje olarak döner, yoksa null döner.
            Delete(entity, save);
        }

        // Delete işlemi: gönderilen koşul veya koşullar üzerinden entity'lere ulaşır, bir döngü üzerinden her turda veritabanına yansıtma işlemi yapmaması için
        // save parametresini false göndererek yukarıdaki entity parametreli Delete methoduna entity'yi gönderir ve DbSet'ten çıkarır, son olarak Save methodu ile
        // tüm değişiklikleri tek seferde veritabanına yansıtır.
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate, bool save = true)
        {
            var entities = _dbContext.Set<TEntity>().Where(predicate).ToList(); // ToList methodu sorgu oluşturulduktan sonra çağrılır ve geriye sorgu sonucundan
                                                                               // dönen kayıtları bir obje listesi olarak döner.
            foreach (var entity in entities) 
            {
                Delete(entity, false);
            }
            if (save)
                Save();
        }

        // Delete işlemi: TEntity tipindeki entity'nin başka entity tipindeki bir referans üzerinden ilişkilerini tutan TRelationalEntity tipi ile bir veya daha fazla koşul için
        // ilişkili kayıtlarının silinmesini sağlar, bu method çağrıldıktan sonra genelde TEntity tipindeki kayıt güncellendiği veya silindiği için
        // save parametresi default false olarak atanmıştır, class dışında generic tipler bu methodda olduğu gibi kullanılabilir.
        public virtual void Delete<TRelationalEntity>(Expression<Func<TRelationalEntity, bool>> predicate, bool save = false) where TRelationalEntity : class, new()
        {
            var relationalEntities = Query<TRelationalEntity>().Where(predicate).ToList(); // yukarıdaki ilişkili entity'ler için oluşturduğumuz Query methodundan listeyi çekiyoruz.
            _dbContext.Set<TRelationalEntity>().RemoveRange(relationalEntities); // daha sonra ilişkili entity DbSet'inden çektiğimiz listeyi siliyoruz.
            if (save)
                Save();
		}

        // DbSet'lerdeki tüm değişikliklerden sonra oluşturulacak sorguların (insert, update ve delete) tek seferde veritabanında çalıştırılması: Unit of Work
        // SaveChanges methodu ile sorgunun çalıştırılması sonucunda etkilenen kayıt sayısı dönülebilir.
        public virtual int Save()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                // eğer istenirse buraya loglama kodları yazılarak hata alındığında örneğin exc.Message üzerinden logların veritabanında, dosyada veya Windows Event Log'da
                // tutulması sağlanabilir.

                throw exc; // hatayı SaveChanges methodunu çağırdığımız methoda fırlatıyoruz.
            }
        }

        public void Dispose()
        {
            _dbContext?.Dispose(); // ?: DbContext null ise bu satırı atla, değilse Dispose et.
            GC.SuppressFinalize(this); // Garbage Collector'a işimizin bittiğini söylüyoruz ki objeyi en kısa sürede hafızadan temizlesin.
        }
    }
}
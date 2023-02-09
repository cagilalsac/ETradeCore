#nullable disable // eğer istenirse null değer atanabilen referans tiplerle ilgili gelen yeşil uyarıları dosyada devre dışı bırakmak için kullanılabilir

using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class ETradeContext : DbContext // veritabanı tablolarına DbSet'ler üzerinden ulaşarak CRUD işlemleri yapacağımız sınıf
    {
        // tüm entity'ler için DbSet özellikleri oluşturulmalı

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductStore> ProductStores { get; set; } // ürün ile mağaza arasındaki many to many ilişki tablosuna karşılık DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }



        public ETradeContext(DbContextOptions options) : base(options) // options parametresi MvcWebUI katmanındaki Program.cs IoC Container'ında AddDbContext methodu ile
                                                                       // bağımlılığı yönetilen ve appsettings.json veya appsettings.Development.json dosyalarında
                                                                       // tanımlanmış connection string'i bu class'ın constructor'ına, dolayısıyla esas veritabanı
                                                                       // işlemlerini yapacak olan DbContext class'ının constructor'ına taşır. Genelde bu kullanım tercih edilir.
        {

        }



        // eğer istenirse connection string DbContext'in OnConfiguring methodu ezilerek de tanımlanıp kullanılabilir, genelde bu kullanım tercih edilmez.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // 1. yöntem: Windows Authentication
        //    //string connectionString = "server=.\\SQLEXPRESS;database=LEC_ETradeCore;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;";

        //    // 2. yöntem: SQL Server Authentication
        //    string connectionString = "server=.\\SQLEXPRESS;database=LEC_ETradeCore;user id=sa;password=sa;multipleactiveresultsets=true;trustservercertificate=true;";

        //    optionsBuilder.UseSqlServer(connectionString);
        //}



        protected override void OnModelCreating(ModelBuilder modelBuilder) // DbContext'teki bu method ezilerek veritabanı tabloları yapısıyla ilgili değişiklikler yapılabilir,
                                                                           // bu yapısal değişiklikler istenirse entity'lerde istenirse de bu method üzerinden gerçekleştirilebilir,
                                                                           // ancak entity'ler arasındaki ilişki tipi Entity Framework Code First ile oluşturulan projelerde
                                                                           // default cascade'dir (bir kayıt silindiğinde ilişkili tablolarındaki verileri zincirleme otomatik silinir),
                                                                           // uygun olan ilişkileri no action yapmaktır ki ilişkiler bu method içerisinde değiştirilmelidir
        {
            modelBuilder.Entity<ProductStore>().HasKey(ps => new { ps.ProductId, ps.StoreId });
            // bir üst satırda methodlar kullanıldıkça farklı methodların kullanılmasını sağlayan yapıya Fluent API denir,
            // SOLID prensipleri gereği aşağıdaki konfigürasyonu başka bir class üzerinden gerçekleştirmek daha uygun olacaktır,
            // ürün ve mağaza arasındaki many to many ilişki için ps delegesi üzerinden hem ProductStore entity'sindeki ProductId'yi hem de StoreId'yi beraber primary key yaptık

            modelBuilder.Entity<Product>() // CategoryId foreign key'i Product'ta olduğu için 1 to many ilişkiyi ürün üzerinden ilişkili kayıt silme kuralını no action olarak yeniden tanımlıyoruz
                .HasOne(p => p.Category) // 1 ürünün 1 kategorisi olabilir, p: product
                .WithMany(c => c.Products) // 1 kategorinin 1'den çok ürünü olabilir, c: category
                .HasForeignKey(p => p.CategoryId) // foreign key Product'taki CategoryId'dir
                .OnDelete(DeleteBehavior.NoAction); // ilişkili ürünleri olan bir kategori silindiğinde veritabanı izin vermez

            modelBuilder.Entity<ProductStore>() // ürün mağaza many to many ilişkisi için ürün foreign key'i ProductStore'da olduğundan önce ürün ile ilişkisini oluşturuyoruz
                .HasOne(ps => ps.Product) // ps: product store
                .WithMany(p => p.ProductStores)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ProductStore>() // ürün mağaza many to many ilişkisi için mağaza foreign key'i ProductStore'da olduğundan daha sonra mağaza ile ilişkisini oluşturuyoruz
				.HasOne(ps => ps.Store)
				.WithMany(s => s.ProductStores) // s: store
				.HasForeignKey(ps => ps.StoreId)
				.OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>() // kullanıcı rol 1 to many ilişkisi için RoleId foreign key'i User'da olduğundan rol ile ilişkisini oluşturuyoruz
                .HasOne(u => u.Role) // u: user
                .WithMany(r => r.Users) // r: role
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDetail>() // kullanıcı kullanıcı detayı 1 to 1 ilişkisi için UserId foreign key'i UserDetail'da olduğundan kullanıcı ile ilişkisini oluşturuyoruz
                .HasOne(ud => ud.User) // ud: user detail
                .WithOne(u => u.UserDetail)
                .HasForeignKey<UserDetail>(ud => ud.UserId) // eğer burada olduğu gibi lambda expression üzerinden delegenin özelliklerine ulaşamazsak delegenin generic tip'ini method adından sonra belirtmeliyiz
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDetail>() // kullanıcı detayı ülke 1 to many ilişkisi için CountryId foreign key'i UserDetail'da olduğundan ülke ile ilişkisini oluşturuyoruz
				.HasOne(ud => ud.Country) 
				.WithMany(c => c.UserDetails) // c: country
				.HasForeignKey(ud => ud.CountryId) 
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<UserDetail>() // kullanıcı detayı şehir 1 to many ilişkisi için CityId foreign key'i UserDetail'da olduğundan şehir ile ilişkisini oluşturuyoruz
				.HasOne(ud => ud.City)
				.WithMany(c => c.UserDetails) // c: city
				.HasForeignKey(ud => ud.CityId)
				.OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<City>() // ülke şehir 1 to many ilişkisi için CountryId foreign key'i City'de olduğundan ülke ile ilişkisini oluşturuyoruz
                .HasOne(ci => ci.Country) // ci: city
                .WithMany(co => co.Cities) // co: country
                .HasForeignKey(ci => ci.CountryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>() // eğer istenirse en çok arama yapılan entity özelliklerine index tanımlanarak veritabanında sayfalama üzerinden index'e göre sıralanan kayıtlara daha hızlı ulaşılması sağlanabilir
                .HasIndex(p => p.Name);

            modelBuilder.Entity<UserDetail>() // eğer istenirse entity özelliği üzerinden ilgili tablosundaki verilerinin tekil olması IsUnique methodu ile sağlanabilir
                .HasIndex(ud => ud.Email)
                .IsUnique(true);
		}
    }
}

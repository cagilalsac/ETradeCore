using Business.Models;
using Business.Models.Report;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace Business.Services.Report
{
    public interface IReportService // bu interface'in IService'i implemente etmesine gerek yoktur çünkü bu servis CRUD işlemleri yapmayacak.
    {
        // ReportService class'ındaki method tanımları
        List<ReportItemModel> GetList(ReportFilterModel filter, bool useInnerJoin = false);
    }

    public class ReportService : IReportService
    {
        private readonly ProductRepoBase _repo;

        public ReportService(ProductRepoBase repo)
        {
            _repo = repo;
        }

        /* SQL sorgusu örnek:
        select p.Name [Product Name], p.Description [Product Description], FORMAT(p.UnitPrice, 'C2') [Unit Price],
        CAST(p.StockAmount as varchar) + ' units' [Stock Amount], CONVERT(varchar(10), p.ExpirationDate, 101) [Expiration Date],
        c.Name Category, c.Description [Category Description], s.Name + case when s.IsVirtual = 1 then ' (Virtual)' else ' (Real)' end as Store
        from Products p inner join Categories c
        on p.CategoryId = c.Id
        inner join ProductStores ps
        on ps.ProductId = p.Id
        inner join Stores s
        on ps.StoreId = s.Id
        --where p.UnitPrice < 3000 and c.Id = 1 and s.Id = 2 -- category id ve store id uygulama üzerinden Seed işlemi yapıldıktan sonra değişebilir
        --order by s.Name, c.Name, p.Name
        */
        public List<ReportItemModel> GetList(ReportFilterModel filter, bool useInnerJoin = false) 
        {
            #region Sorgu oluşturma
            var productQuery = _repo.Query(); // Products sorgusu
            var categoryQuery = _repo.Query<Category>(); // Categories sorgusu,
                                                         // RepoBase'deki Query methodunu Product tipi dışında başka bir tip (örneğin burada Category) tanımlayarak çağırabiliriz.
            var storeQuery = _repo.Query<Store>(); // Stores sorgusu
            var productStoreQuery = _repo.Query<ProductStore>(); // ProductStores sorgusu

            IQueryable<ReportItemModel> query;

            if (useInnerJoin) // SQL Inner Join iki tablo arasında sadece primary key ve foreign key id'leri üzerinden eşleşenleri getirir. 
            {
                query = from product in productQuery // product: Product tipindeki delege
                        join category in categoryQuery // category: Category tipindeki delege
                        on product.CategoryId equals category.Id // Category -> Id primary key'i ile Product -> CategoryId foreign key'lerini eşleştiriyoruz
                        join productStore in productStoreQuery // productStore: ProductStore tipindeki delege
                        on product.Id equals productStore.ProductId // Product -> Id primary key'i ile ProductStore -> ProductId foreign key'lerini eşleştiriyoruz
                        join store in storeQuery // store: Store tipindeki delege
                        on productStore.StoreId equals store.Id // Store -> Id primary key'i ile ProductStore -> StoreId foreign key'lerini eşleştiriyoruz

                        //where product.UnitPrice < 3000 && category.Id == 1 && store.Id == 2 // eğer istenirse sorguya where koşulu eklenebilir
                        //orderby store.Name, category.Name, product.Name // eğer istenirse sorguya order by eklenebilir

                        select new ReportItemModel() // entity delegeleri ile çektiğimiz veri üzerinden modeli oluşturuyoruz
                        {
                            CategoryDescription = category.Description,
                            CategoryName = category.Name,
                            ExpirationDate = product.ExpirationDate.HasValue ? product.ExpirationDate.Value.ToString("MM/dd/yyyy") : "",
                            ProductDescription = product.Description,
                            ProductName = product.Name,
                            StockAmount = product.StockAmount + " units",
                            StoreName = store.Name + (store.IsVirtual ? " (Virtual)" : " (Real)"),
                            UnitPrice = product.UnitPrice.ToString("C2"),

                            CategoryId = category.Id, // aşağıda CategoryId üzerinden filtreleme yapabilmek için atamalıyız,
                                                      // bunun için de ReportItemModel'a CategoryId özelliğini eklemeliyiz
                            StoreId = store.Id, // aşağıda StoreId üzerinden filtreleme yapabilmek için atamalıyız,
                                                // bunun için de ReportItemModel'a StoreId özelliğini eklemeliyiz
                            UnitPriceValue = product.UnitPrice, // aşağıda UnitPriceValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                                // bunun için de ReportItemModel'a UnitPriceValue özelliğini eklemeliyiz
                            StockAmountValue = product.StockAmount, // aşağıda StockAmountValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                                    // bunun için de ReportItemModel'a StockAmountValue özelliğini eklemeliyiz
                            ExpirationDateValue = product.ExpirationDate // aşağıda ExpirationDateValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                                         // bunun için de ReportItemModel'a ExpirationDateValue özelliğini eklemeliyiz
                        };
            }
            else // SQL Left Outer Join iki tablo arasında soldaki tablodan tüm kayıtları, sağdaki tablodan ise primary key ve foreign key id'leri
                 // üzerinden eşleşenler varsa eşleşenleri yoksa null getirir.
            {
                query = from p in productQuery // p: Product tipindeki delege
                        join c in categoryQuery // c: Category tipindeki delege
                        on p.CategoryId equals c.Id into categoryJoin // c -> Id ile p -> CategoryId'yi eşleştiriyoruz ve categoryJoin delegesine atıyoruz
                        from category in categoryJoin.DefaultIfEmpty() // categoryJoin üzerinden sorguda kullanacağımız category delegesi ile devam ediyoruz
                        join ps in productStoreQuery // ps: ProductStore tipindeki delege
                        on p.Id equals ps.ProductId into productStoreJoin // p -> Id ile ps -> ProductId eşleştiriyoruz ve productStoreJoin delegesine atıyoruz
                        from productStore in productStoreJoin.DefaultIfEmpty() // productStoreJoin üzerinden sorguda kullanacağımız productStore ile devam ediyoruz
                        join s in storeQuery // s: Store tipindeki delege
                        on productStore.StoreId equals s.Id into storeJoin // ps değil, son delegemiz productStore -> StoreId ile s -> Id eşleştiriyoruz ve storeJoin delegesine atıyoruz
                        from store in storeJoin.DefaultIfEmpty() // storeJoin üzerinden sorguda kullanacağımız store ile devam ediyoruz
                        select new ReportItemModel() // entity delegeleri ile çektiğimiz veri üzerinden modeli oluşturuyoruz
                        {
                            CategoryDescription = category.Description,
                            CategoryName = category.Name,
                            ExpirationDate = p.ExpirationDate.HasValue ? p.ExpirationDate.Value.ToString("MM/dd/yyyy") : "",
                            ProductDescription = p.Description,
                            ProductName = p.Name,
                            StockAmount = $"{p.StockAmount} units",
                            StoreName = !string.IsNullOrWhiteSpace(store.Name) ? (store.Name + (store.IsVirtual ? " (Virtual)" : " (Real)")) : "",
                            UnitPrice = p.UnitPrice.ToString("C2"),

                            CategoryId = category.Id, // aşağıda CategoryId üzerinden filtreleme yapabilmek için atamalıyız,
                                                      // bunun için de ReportItemModel'a CategoryId özelliğini eklemeliyiz
                            StoreId = store.Id, // aşağıda StoreId üzerinden filtreleme yapabilmek için atamalıyız,
                                                // bunun için de ReportItemModel'a StoreId özelliğini eklemeliyiz
                            UnitPriceValue = p.UnitPrice, // aşağıda UnitPriceValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                          // bunun için de ReportItemModel'a UnitPriceValue özelliğini eklemeliyiz
                            StockAmountValue = p.StockAmount, // aşağıda StockAmountValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                              // bunun için de ReportItemModel'a StockAmountValue özelliğini eklemeliyiz
                            ExpirationDateValue = p.ExpirationDate // aşağıda ExpirationDateValue üzerinden filtreleme yapabilmek için atamalıyız,
                                                                   // bunun için de ReportItemModel'a ExpirationDateValue özelliğini eklemeliyiz
                        };
            }
            #endregion

            #region Sıralama
            // sorgu üzerinden where, order by, vb. işlemleri sorguyu oluşturduktan sonra uygulamak daha uygundur,
            // önce mağaza adına, mağaza adı aynı olanlar için sonra kategori adına, mağaza adı ve kategori adı aynı olanlar için de
            // en son ürün adına göre artan sıralıyoruz
            query = query.OrderBy(q => q.StoreName).ThenBy(q => q.CategoryName).ThenBy(q => q.ProductName);
            #endregion

            #region Filtreleme
            if (filter is not null)
            {
                if (filter.CategoryId.HasValue) // eğer kullanıcı kategori seçtiyse
                {
                    query = query.Where(q => q.CategoryId == filter.CategoryId.Value);
                    // q yukarıda oluşturduğumuz sorguya (query) delegelik yapıyor dolayısıyla yukarıdaki sorguda
                    // Select içerisinde CategoryId ataması (mapping işlemi) yapmalıyız ki burada kullanabilelim,
                    // bunun için de öncelikle ReportItemModel'a CategoryId özelliği eklemeliyiz
                }
                if (filter.StoreIds is not null && filter.StoreIds.Count > 0) // eğer kullanıcı bir veya daha fazla mağaza seçtiyse
                {
                    query = query.Where(q => filter.StoreIds.Contains(q.StoreId ?? 0));
                    // StoreIds gibi bir kolleksiyon üzerinden filtreleme yapabilmek için Contains methodu ile q delegesinin 
                    // ilgili özelliği (StoreId) kullanılmalıdır, q delegesinin StoreId'si nullable olduğu için eğer null ise 0
                    // olsun dedik çünkü StoreIds kolleksiyonu içerisindeki elemanların tipi int,
                    // 0'ı herhangi bir mağaza id'si olamayacağı için yani mağaza id'leri 1'den başlayıp artarak devam ettiği için kullandık
                }
                if (filter.UnitPriceBegin.HasValue) // eğer kullanıcı birim fiyat başlangıç değeri girdiyse
                {
                    query = query.Where(q => q.UnitPriceValue >= filter.UnitPriceBegin.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden birim fiyatı kullanıcının girdiği başlangıç
                    // değerinden büyük veya eşit olanları filtreliyoruz
                }
                if (filter.UnitPriceEnd.HasValue) // eğer kullanıcı birim fiyat bitiş değeri girdiyse
                {
                    query = query.Where(q => q.UnitPriceValue <= filter.UnitPriceEnd.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden birim fiyatı kullanıcının girdiği bitiş
                    // değerinden küçük veya eşit olanları filtreliyoruz
                }
                if (filter.StockAmountBegin.HasValue) // eğer kullanıcı stok miktarı başlangıç değeri girdiyse
                {
                    query = query.Where(q => q.StockAmountValue >= filter.StockAmountBegin.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden stok miktarı kullanıcının girdiği başlangıç
                    // değerinden büyük veya eşit olanları filtreliyoruz
                }
                if (filter.StockAmountEnd.HasValue) // eğer kullanıcı stok miktarı bitiş değeri girdiyse
                {
                    query = query.Where(q => q.StockAmountValue <= filter.StockAmountEnd.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden stok miktarı kullanıcının girdiği bitiş
                    // değerinden küçük veya eşit olanları filtreliyoruz
                }
                if (filter.ExpirationDateBegin.HasValue) // eğer kullanıcı son kullanma tarihi başlangıç değeri girdiyse
                {
                    query = query.Where(q => q.ExpirationDateValue >= filter.ExpirationDateBegin.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden son kullanma tarihi kullanıcının girdiği başlangıç
                    // değerinden büyük veya eşit olanları filtreliyoruz
                }
                if (filter.ExpirationDateEnd.HasValue) // eğer kullanıcı son kullanma tarihi bitiş değeri girdiyse
                {
                    query = query.Where(q => q.ExpirationDateValue <= filter.ExpirationDateEnd.Value);
                    // yukarıdaki sorgu için delege olarak kullandığımız q üzerinden son kullanma tarihi kullanıcının girdiği bitiş
                    // değerinden küçük veya eşit olanları filtreliyoruz
                }
                if (!string.IsNullOrWhiteSpace(filter.ProductName)) // eğer kullanıcı ürün adı girdiyse
                {
                    query = query.Where(q => q.ProductName.ToLower().Contains(filter.ProductName.ToLower().Trim()));
                    // Contains methodu ile kullanıcının girdiği ürün adını içeren kayıtları filtreliyoruz,
                    // iki tarafta da ToLower methodu kullandık ki case insensitive (büyük küçük harf hassasiyetsiz) filtreleme yapabilelim,
                    // son olarak kullanıcının filter.ProductName üzerinden gönderdiği verinin başındaki ve sonundaki boşlukları Trim methodu
                    // ile temizledik
                }
            }
            #endregion

            return query.ToList(); // ToList methodu ile sorgumuzu çalıştırıp sonucu List<ReportModel> tipinde methoddan dönüyoruz 
        }
    }
}

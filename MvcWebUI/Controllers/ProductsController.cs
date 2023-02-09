using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebUI.Settings;

namespace MvcWebUI.Controllers
{
    [Authorize] // Authorize attribute'u controller üzerinde de yazılabilir, yazıldığında controller içerisindeki tüm action'larda geçerlidir,
                // Controller üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                // bu controller'ın tüm action'larını çağırabilir.
    public class ProductsController : Controller // controller class isimleri mutlaka Controller ile bitmelidir, tarayıcının adresi veya link üzerinden çağrılırken ise Controller yazılmaz.
    {
        private readonly IProductService _productService; // controller'da ürünle ilgili işleri gerçekleştirebilmek için servis alanı tanımlanır ve constructor üzerinden enjekte edilir.

        private readonly ICategoryService _categoryService; // controller'da kategori ile ilgili işleri gerçekleştirebilmek için servis alanı tanımlanır ve constructor üzerinden enjekte edilir.
        
        private readonly IStoreService _storeService; // controller'da mağaza ile ilgili işleri gerçekleştirebilmek için servis alanı tanımlanır ve constructor üzerinden enjekte edilir.

        public ProductsController(IProductService productService, ICategoryService categoryService, IStoreService storeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _storeService = storeService;
        }



        [AllowAnonymous] // Eğer Authorize attribute'u controller üzerinde yazıldıysa ve action çağrımı sisteme giriş yapmış veya yapmamış
                         // tüm kullanıcılar için geçerli hale getirilmek isteniyorsa AllowAnonymous attribute'u kullanılarak Authorize ezilir,
                         // dolayısıyla herkes bu action'ı çağırıp ürün listesini görebilir.
        public IActionResult Index() // tarayıcıda ~/Products/Index adresi girilerek veya herhangi bir view'da (örneğin Views -> Shared klasörü altındaki _Layout.cshtml)
                                     // bu adres için link oluşturularak çağrılabilir.
        {
            List<ProductModel> products = _productService.Query().ToList(); // ToList LINQ (Language Integrated Query) methodu sorgunun çalıştırılmasını ve sonucunun IQueryable'da kullanılan tip
                                                                            // üzerinden bir liste olarak dönmesini sağlar.

            //return View("ProductList", products); // *1
            return View(products); // *2
            // *1: Eğer istenirse dönen liste Views altındaki Products klasörüne ProductList.cshtml adlı view oluşturularak bu view'a model olarak gönderilebilir
            // *2: veya dönen liste Views altındaki Products klasöründeki Index.cshtml view'ına model olarak gönderilir.
            // View'a action'dan sadece tek bir model gönderilebilir.
        }



        // Authorize attribute'u yazılmadığı için controller'ın üzerindeki Authorize geçerli olacaktır.
        public IActionResult Details(int id) // örneğin tarayıcıda ~/Products/Details/1 adresi girilerek veya herhangi bir view'da (örneğin Views -> Products -> Index.cshtml)
                                             // bu adres için id parametresini de gönderen bir link oluşturularak çağrılabilir.
        {
            ProductModel product = _productService.Query().SingleOrDefault(p => p.Id == id); // parametre olarak bu action'a gönderilen id üzerinden kayıt sorgulanır
            /*
            SingleOrDefault LINQ methodu kullanılarak ID üzerinden tek bir kayda ulaşılabilir.
            SingleOrDefault lambda expression kullanılarak belirtilen koşul veya koşullar üzerinden tek bir kayıt döner,
            eğer sorgu sonucunda birden çok kayıt dönerse exception fırlatır, eğer belirtilen koşula sahip
            kayıt bulamazsa null referansı döner.
            Single, SingleOrDefault yerine kullanılabilir, eğer belirtilen koşula sahip birden çok kayıt bulursa
            veya kayıt bulamazsa exception fırlatır.
            */
            /*
            SingleOrDefault yerine FirstOrDefault LINQ methodu da kullanılabilir.
            FirstOrDefault lamda expression kullanılarak belirtilen koşul veya koşullar üzerinden sorgu sonucunda
            tek kayıt dönse de birden çok kayıt dönse de her zaman ilk kaydı döner,
            eğer kayıt bulunamazsa null referansı döner.
            First, FirstOrDefault yerine kullanılabilir, eğer belirtilen koşula sahip kayıt bulunamazsa
            exception fırlatır.
            LastOrDefault ve Last methodları da FirstOrDefault ve First methodlarının tersi düşünülerek
            belirtilen koşul veya koşullara göre bulunan son kayıt üzerinden işleme devam eder.
            */
            /*
            DbContext objesindeki DbSet'ler üzerinden SingleOrDefault'a alternatif olarak Find methodu kullanılabilir ve 
            parametre olarak bir veya daha fazla anahtar (primary key) kullanılarak tek bir kayda (objeye) ulaşılabilir. 
            */
            /*
            Where LINQ methodu ile kayıtlar lamda expression kullanılarak bir veya daha fazla
            koşul üzerinden filtrelenerek kolleksiyon olarak geri dönülebilir.
            Koşullarda && (and), || (or) ile ! (not) operatorleri istenirse bir arada kullanılabilir.
            Bu operatörler ile oluşturulan koşullar SingleOrDefault, Single, FirstOrDefault, First,
            LastOrDefault ve Last gibi methodlarda da kullanılabilir.
            */

            //if (product == null) // bu veya bir alt satırdaki şekilde null kontrolü yapılabilir
            if (product is null) // eğer sorgu sonucunda kayıt bulunamadıysa
            {
                //return NotFound(); // 404 HTTP durum kodu üzerinden kaynak bulunamadı HTTP response'u (yanıtı) dönülebilir
                return View("_Error", "Product not found!"); // alternatif olarak tüm projede tüm controller action'larında alınabilecek hatalar için Views -> Shared klasörü altına
                                                             // _Error.cshtml paylaşılan view'ı oluşturularak alınan hatalar yazdırılabilir
            }
            return View(product); // kayıt bulunduysa Details view'ına model olarak gönderilir
        }



        [HttpGet] // Bu aksiyonun HTTP GET yani sunucudan kaynak getirme işlemini yapacağını belirten Action Method Selector'ıdır.
                  // Yazmak zorunlu değildir çünkü yazılmazsa default HttpGet methodu action'larda kullanılır.
        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        public IActionResult Create() // tarayıcıda ~/Products/Create adresi girilerek veya herhangi bir view'da (örneğin Views -> Producs klasörü altındaki Index.cshtml)
                                      // bu adres için link oluşturularak çağrılabilir. Create view'ındaki form kullanıcıya dönülür ki kullanıcı veri girip sunucuya gönderebilsin.
                                      // Veritabanında yeni kayıt oluşturmak için kullanılır.
        {
            ViewBag.Categories = new SelectList(_categoryService.Query().ToList(), "Id", "Name");
            // Eğer view'da kullanılacak model'den farklı bir tipte veriye ihtiyaç varsa ViewBag veya ViewData üzerinden gerek action'dan view'a gerekse view'lar arası
            // model verisi dışındaki veriler taşınabilir.
            // ViewBag ile ViewData aynı yapı olarak birbirlerinin yerlerine kullanılabilir, sadece kullanımları farklıdır. Örneğin ViewData["Categories"] olarak da burada kullanabilirdik.
            // View'da kategoriler için bir Drop Down List (HTML select tag'i) kullanacağımızdan yeni bir SelectList objesi oluştururken içerisine parametre olarak sırasıyla
            // kategori listemizi, listenin tipi (CategoryModel) üzerinden arka planda tutacağımız (yani kullanıcının görmeyeceği) özellik ismini (Id) ve
            // listenin tipi (CategoryModel) üzerinden kullanıcıya göstereceğimiz özellik ismini (Name) belirtiyoruz.

            ViewBag.Stores = new MultiSelectList(_storeService.Query().ToList(), "Id", "Name");
            // Tıpkı yukarıda kategori listesini bir drop down list üzerinden kullanıcıya gösterebilmek için bir SelectList oluşturduğumuz ve ViewBag'e attığımız gibi
            // mağazaları da ilgili servisi üzerinden çekip bir MultiSelectList'e doldurup ViewBag'e atıyoruz ki view'da kullanıcı List Box (multiple attribute'lu HTML select tag'i)
            // üzerinden hiç, bir veya daha çok mağaza seçebilsin.
            // Id parametresini StoreModel'e göre List Box'un arka planda kullanacağı değer, Name parametresini de yine StoreModel'e göre kullanıcıya göstereceği veri
            // olarak MultiSelectList'e belirtiyoruz.

            #region IActionResult'ı implemente eden class'lar ve bu class tiplerini dönen methodlar
            //return new ViewResult(); // ViewResult ActionResult'tan miras aldığı için ve ActionResult da IActionResult'ı implemente ettiği için dönülebilir.
            // Ancak bu şekilde ViewResult objesini new'leyerek dönmek yerine aşağıdaki ViewResult dönen View methodu kullanılır.
            // Detaylı bilgi için aşağıdaki aynı isme sahip region'a bakılabilir.

            //return View(); // Views altındaki Products klasöründeki Create.cshtml view'ını döner.

            // Eğer istenirse view'de bazı ilk input'ların set edilmesi için yeni bir model oluşturularak view'a gönderilebilir.
            ProductModel product = new ProductModel()
            {
                StockAmount = 0, // view'da stok miktarı her zaman ilk olarak 0 gelsin
                ExpirationDate = DateTime.Now.AddMonths(6) // view'da son kullanma tarihi her zaman ilk olarak bugünün tarihinden 6 ay sonra gelsin
            };
            return View(product);
            #endregion
        }



        [HttpPost] // post methodu ile veri gönderen HTML form'unun veya isteklerin (request) verilerinin sunucu tarafından alınmasını sağlar. post işlemleri için yazmak zorunludur.
        [ValidateAntiForgeryToken] // View'da AntiforgeryToken HTML Helper'ı ile oluşturulan token'ın validasyonunu sağlayan attribute'tur. 
        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        //public IActionResult Create(string Name, string Description, double? UnitPrice, int? StockAmount, DateTime? ExpirationDate, int? CategoryId, List<int> StoreIds)
        // *1
        //public IActionResult Create(ProductModel product)
        // *1: bir yukarıdaki parametreler ProductModel'da özellik olarak bulunduğundan ProductModel tipinde model parametresi kullanıyoruz.
        public IActionResult Create(ProductModel product, IFormFile? image)
		// *2: view'dan input name'i image type'ı file HTML input'u üzerinden imajı alıyoruz,
		// image'ın IFormFile tipini kullanıcının imaj yükleme zorunluluğu olmadığı için nullable (?) tanımladık.
		// form verileri name ile belirtilen input HTML elemanları üzerinden parametre olarak alınabildiği gibi bu özellikler ProductModel'in içerisinde olduğundan
		// parametre olarak ProductModel tipinde bir product parametresi (model) de kullanılabilir. Genelde model kullanımı tercih edilir.
		{
			if (ModelState.IsValid) // eğer kullanıcıdan parametre olarak gelen product model verilerinde data annotation'lar üzerinden bir validasyon hatası yoksa
            {
                Result result; // imaj yükleme ile ürün ekleme işlemleri sonucu

                // imaj yükleme
                result = UpdateImage(product, image);

                if (result.IsSuccessful) // eğer imaj null gönderildiyse veya UpdateImage içerisindeki validasyonlardan geçip product modelimiz güncellendiyse
                {
                    result = _productService.Add(product); // product model'i eklenmesi için servisin ekleme (Add) methoduna gönderiyoruz ve Result tipindeki değişkene sonucu atıyoruz

                    if (result.IsSuccessful) // eğer sonuç başarılıysa (servisten SuccessResult tipinde obje dönülmüş demektir)
                    {
                        TempData["Message"] = result.Message; // başarılı işlem sonucunun mesajını başka bir action'a yönlendirdiğimiz için ViewBag
                                                              // veya ViewData ile taşıyamayacağımızdan TempData üzerinden taşıyoruz,
                                                              // işlem sonuç mesajları servislerde atanmalıdır, controller'da atamak pek tercih edilmez

                        //return RedirectToAction("Index"); // son olarak bu controller'ın Index action'ına yönlendiriyoruz ki veriler o action'da
                        // veritabanından tekrar çekilip Index view'ında listelenebilsin
                        return RedirectToAction(nameof(Index)); // string olarak Index yazmak yerine Index methodunun adını nameof ile alıp kullanmak hata yapmamızı ortadan kaldırır
                    }
                }

				//ViewBag.Message = result.Message; // 1. yöntem: bu satırda servisten ErrorResult objesi dönmüş demektir, dolayısıyla sonucun mesajını Create view'ına bu şekilde
				                                    // taşıyıp view'deki ViewBag.Message üzerinden gösterebiliriz
				ModelState.AddModelError("", result.Message); // 2. daha iyi yöntem: view'da validation summary kullandığımız için hata sonucunun mesajının bu şekilde validation summary'de
                                                              // gösterimini sağlayabiliriz
            }
            ViewBag.Categories = new SelectList(_categoryService.Query().ToList(), "Id", "Name", product.CategoryId);
            // bu satırda model validasyondan geçememiş demektir
            // Create view'ını tekrar döneceğimiz için view'da select HTML tag'inde (drop down list) kullandığımız kategori listesini tekrar doldurmak zorundayız,
            // new SelectList'teki son parametre kategori listesinde kullanıcının product model üzerinden seçmiş olduğu kategorinin CategoryId üzerinden seçilmesini sağlar

            ViewBag.Stores = new MultiSelectList(_storeService.Query().ToList(), "Id", "Name", product.StoreIds);
            // view'da multiple attribute'lu select HTML tag'inde (list box) kullandığımız mağaza listesini tekrar doldurmak zorundayız,
            // new MultiSelectList'teki son parametre mağaza listesinde kullanıcının product model üzerinden seçmiş olduğu mağazaların StoreIds üzerinden seçilmesini sağlar

            return View(product); // bu action'a parametre olarak gelen ve kullanıcının view üzerinden doldurduğu product modelini tekrar kullanıcıya gönderiyoruz ki
                                  // kullanıcı view'da girdiği verileri kaybetmesin ve hataları giderip tekrar işlem yapabilsin
        }



        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        public IActionResult Edit(int id) // örneğin tarayıcıda ~/Products/Edit/1 adresi girilerek veya herhangi bir view'da (örneğin Views -> Products -> Index.cshtml)
                                          // bu adres için id parametresini de gönderen bir link oluşturularak çağrılabilir.
        {
            ProductModel product = _productService.Query().SingleOrDefault(p => p.Id == id); // önce action'a gelen id parametresine göre ürün verisini çekiyoruz

            if (product is null) // eğer gelen id'ye göre ürün bulunamadıysa
            {
                return View("_Error", "Product not found!"); // ürün bulunamadı mesajını daha önce oluşturduğumuz _Error.cshtml view'ına gönderiyoruz
            }

            ViewBag.CategoryId = new SelectList(_categoryService.Query().ToList(), "Id", "Name", product.CategoryId);
            // view'da select HTML tag'inde (drop down list) kullandığımız kategori listesini SelectList objesine doldurarak ViewBag'e atıyoruz,
            // new SelectList'teki son parametre kategori listesinde kullanıcının product model üzerinden daha önce kaydetmiş olduğu kategorinin
            // CategoryId üzerinden seçilmesini sağlar

            ViewBag.StoreIds = new MultiSelectList(_storeService.Query().ToList(), "Id", "Name", product.StoreIds);
            // view'da multiple attribute'lu select HTML tag'inde (list box) kullandığımız mağaza listesini tekrar doldurmak zorundayız,
            // new MultiSelectList'teki son parametre mağaza listesinde kullanıcının product model üzerinden seçmiş olduğu mağazaların StoreIds üzerinden seçilmesini sağlar

            return View(product); // Veritabanından çektiğimiz ürünü Edit.cshtml view'ına gönderiyoruz.
                                  // Edit.cshtml view'ını scaffolding (controller ve istenirse view'larının kodlarının şablonlara göre otomatik oluşturulması)
                                  // ile bu action'ın herhangi bir yerinde fare ile sağ tıklanarak Add View -> Add Razor View ->
                                  // View name: Edit (action adı Edit olduğu için), Template: Edit (diğer aksiyonlar için List, Details, Create, Delete veya Empty kullanılabilir),
                                  // Model class: Product (mutlaka entity seçilmeli) -> Data context class: ETradeContext,
                                  // Options olarak da Create as a partial view (Edit view'ı bir partial view olmamalı) ve Reference script libraries
                                  // (seçilirse client side yani tarayıcı üzerinden Javascript ile validation aktif olur, seçilmezse server side yani
                                  // sunucudaki controller action'ları üzerinden validation aktif olur) seçmeden ve
                                  // Use a layout page'i işaretleyip boş bırakarak (Views -> _ViewStart.cshtml altında tanımlanan projenin _Layout.cshtml view'ını kullan,
                                  // istenirse sağdaki üç noktaya tıklanarak başka bir layout view da seçilebilir) oluşturuyoruz.
                                  // Burada yaptığımız gibi Entity Framework üzerinden scaffolding yapabilmek için DataAccess -> Contexts altına
                                  // projenin DbContext class'ı (ETradeContext) için bir factory (ETradeContextFactory, fabrika: örneğin scaffolding işlemi için
                                  // ETradeContext objesini oluşturup kullanılmasını sağlayacak) class'ı oluşturulmalıdır.
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        public IActionResult Edit(ProductModel product, IFormFile? image)
		// view'dan input name'i image type'ı file HTML input'u üzerinden imajı alıyoruz,
		// image'ın IFormFile tipini kullanıcının imaj yükleme zorunluluğu olmadığı için nullable (?) tanımladık.
		{
			if (ModelState.IsValid) // eğer kullanıcıdan parametre olarak gelen product model verilerinde data annotation'lar üzerinden bir validasyon hatası yoksa
			{
                Result result; // imaj yükleme ile ürün güncelleme işlemleri sonucu

				// imaj yükleme
				result = UpdateImage(product, image);

                if (result.IsSuccessful) // eğer imaj null gönderildiyse veya UpdateImage içerisindeki validasyonlardan geçip product modelimiz güncellendiyse
                {
                    result = _productService.Update(product); // product model'i güncellenmesi için servisin güncelleme (Update) methoduna gönderiyoruz ve Result tipindeki değişkene sonucu atıyoruz

                    if (result.IsSuccessful) // eğer sonuç başarılıysa (servisten SuccessResult tipinde obje dönülmüş demektir)
                    {
                        TempData["Message"] = result.Message; // başarılı işlem sonucunun mesajını başka bir action'a yönlendirdiğimiz için ViewBag
                                                              // veya ViewData ile taşıyamayacağımızdan TempData üzerinden taşıyoruz

                        //return RedirectToAction(nameof(Index)); // son olarak bu controller'ın Index action'ına yönlendiriyoruz ki veriler o action'da
                                                                  // veritabanından tekrar çekilip Index view'ında listelenebilsin
                         return RedirectToAction(nameof(Details), new { id = product.Id }); // alternatif olarak kullanıcıyı güncellenen ürün id'sini
																					        // route value olarak kullanarak Details action'ına yönlendirip
																					        // güncelleme sonucunu detay sayfasında görmesini sağlayabiliriz
                    }
                }

				ModelState.AddModelError("", result.Message); // view'da validation summary kullandığımız için hata sonucunun mesajının bu şekilde validation summary'de
															  // gösterimini sağlayabiliriz
			}
			ViewBag.CategoryId = new SelectList(_categoryService.Query().ToList(), "Id", "Name", product.CategoryId);
            // bu satırda model validasyondan geçememiş demektir
            // Edit view'ını tekrar döneceğimiz için view'da select HTML tag'inde (drop down list) kullandığımız kategori listesini tekrar doldurmak zorundayız,
            // new SelectList'teki son parametre kategori listesinde kullanıcının product model üzerinden seçmiş olduğu kategorinin CategoryId üzerinden seçilmesini sağlar

            ViewBag.StoreIds = new MultiSelectList(_storeService.Query().ToList(), "Id", "Name", product.StoreIds);
            // view'da multiple attribute'lu select HTML tag'inde (list box) kullandığımız mağaza listesini tekrar doldurmak zorundayız,
            // new MultiSelectList'teki son parametre mağaza listesinde kullanıcının product model üzerinden seçmiş olduğu mağazaların StoreIds üzerinden seçilmesini sağlar

            return View(product); // bu action'a parametre olarak gelen ve kullanıcının view üzerinden doldurduğu product modelini tekrar kullanıcıya gönderiyoruz ki
								  // kullanıcı view'da girdiği verileri kaybetmesin ve hataları giderip tekrar işlem yapabilsin
		}



        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        public IActionResult Delete(int id) // örneğin tarayıcıda ~/Products/Delete/1 adresi girilerek veya herhangi bir view'da (örneğin Views -> Products -> Index.cshtml)
                                            // bu adres için id parametresini de gönderen bir link oluşturularak çağrılabilir.
        {
            ProductModel product = _productService.Query().SingleOrDefault(p => p.Id == id); // önce action'a gelen id parametresine göre ürün verisini çekiyoruz

            if (product is null) // eğer gelen id'ye göre ürün bulunamadıysa
            {
                return View("_Error", "Product not found!"); // ürün bulunamadı mesajını daha önce oluşturduğumuz _Error.cshtml view'ına gönderiyoruz
            }

            return View(product); // Veritabanından çektiğimiz ürünü Delete.cshtml view'ına gönderiyoruz ki kullanıcı ürünü görüp silme işlemini onaylayabilsin.
                                  // Delete view'ını da scaffolding ile template'ı Delete ve model class'ı da Product entity'si seçerek oluşturduk.
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")] // Yukarıda id parametresi alan Delete methodu olduğundan aynı parametreyi alan başka bir Delete adında method oluşturamayız.
                               // Bu yüzden action'ın adını DeleteConfirmed olarak değiştirdik. Ancak Delete view'ı üzerinden form ile id verisini
                               // bu action'a Delete route'u üzerinden taşıyabilmek için ActionName selector'ı ile action çağrımını Delete olarak değiştirdik.
                               // Eğer ActionName kullanmasaydık bu action'ı DeleteConfirmed olarak çağırmamız gerekecekti.
        [Authorize(Roles = "Admin")] // controller üzerindeki Authorize sisteme giriş yapmış kullanıcılar içindi ancak biz bu action'ın
                                     // sadece Admin rolündekiler için çağrılabilmesini istiyoruz, bu yüzden action üzerinde
                                     // tekrar Authorize attribute'unu Admin rolüne göre tanımladık ve controller'dakini ezmiş olduk.
        public IActionResult DeleteConfirmed(int id)
        {
            Result result = _productService.Delete(id); // view'dan parametre olarak gelen id üzerinden ürün kaydını siliyoruz.

            TempData["Message"] = result.Message; // Index view'ında silme sonucunu gösterebilmek için dönen sonuç mesajını TempData'ya atıyoruz.
            
            return RedirectToAction(nameof(Index)); // Index action'ına yönlendirme yaparak verilerin tekrar veritabanından çekilip listelenmesini sağlıyoruz.
        }



        public IActionResult DeleteImage(int id) // ürün id'si üzerinden ürün imajını silme
        {
            Result result = _productService.DeleteImage(id); // servis üzerinden ürün imajını siliyoruz

            TempData["Message"] = result.Message; // servis methodu sonucu mesajını TempData olarak atıyoruz, Details view'ında kullanacağız

            return RedirectToAction(nameof(Details), new { id = id }); // kullanıcıyı bu action'a parametre olarak gelen id route value
                                                                       // üzerinden id parametreli Details action'ına yönlendiriyoruz
        }



        /// <summary>
        /// Referans olarak gönderilen resultModel'deki Image ve ImageExtension özelliklerini method içerisinde image
        /// parametresi üzerinden güncelleyen ve eğer yüklenen imajla ilgili bir validasyon hatası yoksa veya yüklenen imaj null'sa 
        /// SuccessResult, validasyon hatası varsa mesajıyla birlikte ErrorResult objesi dönen method.
        /// </summary>
        /// <param name="resultModel"></param>
        /// <param name="image"></param>
        /// <returns>Result</returns>
        private Result UpdateImage(ProductModel resultModel, IFormFile uploadedImage)
        {
            Result result = new SuccessResult(); // result'ın ilk değer atamasını SuccessResult objesi olarak yapıyoruz

            if (uploadedImage is not null && uploadedImage.Length > 0) // eğer uploadedImage içerisinde binary veri varsa
            {
                #region Dosya uzantı ve boyut validasyonları
                string uploadedFileName = uploadedImage.FileName; // yüklenen dosya üzerinden dosya adını alıyoruz, örneğin asusrog.jpg
                string uploadedFileExtension = Path.GetExtension(uploadedFileName); // yüklenen dosya adı üzerinden Path class'ının GetExtension
                                                                                    // methodu ile dosya uzantısını alıyoruz, örneğin .jpg

                // eğer yüklenen dosya uzantısı uygulamamızda kabul edilen dosya uzantıları içerisinde değilse 
                // (kabul edilen dosya uzantıları için aie delegesi üzerinden önce case insensitive yani büyük küçük harf duyarsız yaparak
                // sonra da boşlukları temizleyerek yüklenen dosya uzantısını da case insensitive yapıp Any methodu ile
                // herhangi bir eşleşme var mı diye kontrol edip if içerisindeki koşulun komple değilini alıyoruz)
                if (!AppSettings.AcceptedImageExtensions.Split(',').Any(aie => aie.ToLower().Trim() == uploadedFileExtension.ToLower())) 
                {
                    // yüklenen imaj uzantısının kabul edilen imaj uzantıları içerisinde olmadığı mesajını ErrorResult objesi oluşturarak result'a atıyoruz
                    result = new ErrorResult("Image can't be uploaded because image extension is not in \"" + ".jpg, .jpeg, .png" + "\"!"); // \": çift tırnak escape sequence
                }

                // result başarılı mı diye kontrol ediyoruz
                if (result.IsSuccessful)
                {
                    double acceptedFileLength = AppSettings.AcceptedImageLength; // uygulamamızda kabul edilen dosya boyutu, mega bytes (Mb) cinsinden
                    double acceptedFileLengthInBytes = acceptedFileLength * Math.Pow(1024, 2); // megabyte (Mb) cinsinden kabul edilen dosya boyutunu byte'a dönüştürüyoruz,
                                                                                               // 1 byte = 8 bits
                                                                                               // 1 kilo byte (Kb) = 1024 bytes
                                                                                               // 1 mega byte (Mb) = 1024 kilo bytes (Kb) = 1024 * 1024 bytes = 1.048.576 bytes

                    // eğer yüklenen imaj dosya boyutu uygulamamızda kabul edilen dosya boyutundan büyükse
                    if (uploadedImage.Length > acceptedFileLengthInBytes)
                    {
                        // yüklenen imaj dosya boyutu kabul edilen dosya boyutundan büyük mesajını ErrorResult objesi oluşturarak result'a atıyoruz,
                        // N1: N sayı formatı, 1: ondalıktan sonra 1 hane
                        result = new ErrorResult("Image can't be uploaded because image file length is greater than " + acceptedFileLength.ToString("N1") + " mega bytes!");
                    }
                }
                #endregion

                #region resultModel içerisindeki Image ve ImageExtension özellikleri güncellenmesi
                // result başarılı mı diye kontrol ediyoruz
                if (result.IsSuccessful)
                {
                    // using ile new'lenen obje kapanış süslü parantezinde dispose edilir
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // IFormFile CopyTo methodu ile binary veriyi memoryStream objesine kopyalıyoruz
                        uploadedImage.CopyTo(memoryStream);

                        // referans olarak methoddan döneceğimiz resultModel içerisindeki Image özelliğini memoryStream'in ToArray methodu ile
                        // byte[] olarak, ImageExtension özelliğini de yukarıda yüklenen dosya adı üzerinden aldığımız dosya uzantısı üzerinden
                        // set ediyoruz
                        resultModel.Image = memoryStream.ToArray();
                        resultModel.ImageExtension = uploadedFileExtension;
                    }
                }
                #endregion
            }

            // eğer uploadedImage içerisinde binary veri yoksa veya herhangi bir dosya boyutu veya dosya uzantısı validasyon hatası yoksa
            // SuccessResult objesi dönüyoruz
            return result;
        }



		#region IActionResult'ı implemente eden class'lar ve bu class tiplerini dönen methodlar
		/*
        IActionResult
        |
        ActionResult
        |
        ViewResult (View()) - ContentResult (Content()) - EmptyResult - FileContentResult (File()) - HttpResults - JavaScriptResult (JavaScript()) - JsonResult (Json()) - RedirectResults
        */
		public ContentResult GetHtmlContent() // tarayıcıda çağrılması: ~/Products/GetHtmlContent
        {
            //return new ContentResult(); // ContentResult objesini new'leyerek dönmek yerine aşağıdaki methodu kullanılmalıdır.
            return Content("<b><i>Content result.</i></b>", "text/html"); // içerik tipi text/html belirtilmelidir ki tarayıcı HTML olarak yorumlayabilsin,
                                                                          // Türkçe karakterlerde problem olursa son parametre olarak Encoding.UTF8 de kullanılabilir.
        }

        public ActionResult GetProductsXmlContent() // tarayıcıda çağrılması: ~/Products/GetProductsXmlContent, XML döndürme işlemleri genelde bu şekilde yapılmaz, web servisler üzerinden döndürülür.
        {
            List<ProductModel> products = _productService.Query().ToList();
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            xml += "<Products>";
            foreach (ProductModel product in products)
            {
                xml += "<Product>";
                xml += "<Id>" + product.Id + "</Id>";
                xml += "<Name>" + product.Name + "</Name>";
                xml += "<Description>" + product.Description + "</Description>";
                xml += "<UnitPrice>" + product.UnitPriceDisplay + "</UnitPrice>";
                xml += "<StockAmount>" + product.StockAmount + "</StockAmount>";
                xml += "<ExpirationDate>" + product.ExpirationDateDisplay + "</ExpirationDate>";
                xml += "<Category>" + product.CategoryNameDisplay + "</Category>";
                xml += "</Product>";
            }
            xml += "</Products>";
            return Content(xml, "application/xml"); // XML verileri için içerik tipi application/xml belirtilmelidir ki tarayıcı XML olarak yorumlayabilsin.
        }

        public string GetString() // tarayıcıda çağrılması: ~/Products/GetString
        {
            return "String."; // sayfaya "String." düz yazısını (plain text) döner 
        }

        public EmptyResult GetEmpty() // tarayıcıda çağrılması: ~/Products/GetEmpty
        {
            return new EmptyResult(); // içerisinde hiç bir veri olmayan boş bir sayfa döner
        }

        public RedirectResult RedirectToMicrosoft() // tarayıcıda çağrılması: ~/Products/RedirectToMicrosoft
        {
            //return new RedirectResult(); // RedirectResult objesini new'leyerek dönmek yerine aşağıdaki methodu kullanılmalıdır.
            return Redirect("https://microsoft.com"); // parametre olarak belirtilen adrese yönlendirir
        }
        #endregion
    }
}

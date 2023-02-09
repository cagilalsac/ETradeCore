#nullable disable
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    // MvcWebUI projesinde Controllers klasörü seçilerek fareye sağ tıklanıp Add -> Controller -> MVC Controller with views, using Entity Framework
    // seçildikten sonra Model class olarak Category (mutlaka entity seçilmeli), Data context class olarak da ETicaretContext seçildikten sonra
    // action view'larının da oluşturulması için Generate views işaretlenir, ilk aşamada client side validation yapmayacağımız için Reference script libraries
    // seçilmeden Use a layout page işaretlenip projenin tanımlanmış layout view'ının kullanılması için boş bırakılarak, son olarak da Controller name
    // istenilirse değiştirilip scaffolding ile controller, action'ları ve view'larının oluşturulması sağlanabilir.
    // Daha sonra da controller'da belirtilen yönlendirmeler üzerinden kodlar yazılır.

    public class CategoriesController : Controller
    {
        // Add service injections here
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        // Action veya Controller üzerinde Authorize attribute'u yazılmadığı için sisteme giriş yapan veya yapmayan herkes bu action'ı çağırabilir
        public IActionResult Index()
        {
            List<CategoryModel> categoryList = _categoryService.Query().ToList(); // Add get list service logic here
            return View(categoryList);
        }

        // GET: Categories/Details/5
        [Authorize] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar bu action'ı çağırabilir,
                    // [Authorize(Roles = "Admin,User")] şeklinde Admin ve Kullanıcı rolundekiler bu action'ı çağırabilsin şeklinde de özelleştirilebilir,
                    // ancak uygulamamızda zaten Admin ve User olarak iki rol olduğundan sadece [Authorize] kullanmak yeterlidir,
                    // Account -> Controllers -> Users -> Login action'ında rol claim tipi olarak (ClaimTypes.Role) kullanıcı ile ilgili hangi
                    // rol verisi atandıysa örneğin biz Admin veya User atadık Authorize ile birlikte bu rol verisi veya verileri kullanılmalıdır (Admin,User)
        public IActionResult Details(int id)
        {
            CategoryModel category = _categoryService.Query().SingleOrDefault(c => c.Id == id); // Add get item service logic here
            if (category == null)
            {
                return View("_Error", "Category not found!");
            }
            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                     // ve Admin rolundekiler bu action'ı çağırabilir
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items

            // burada kategori dışında kullanacağımız herhangi bir model verimiz olmadığı ve bunları view'a taşımamız gerekmediği için ViewBag veya ViewData kullanmadık.

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                     // ve Admin rolundekiler bu action'ı çağırabilir, Authorize attribute'u hem get hem de post action'ları
                                     // üzerinde tanımlanmalıdır
        public IActionResult Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Add insert service logic here
                var result = _categoryService.Add(category);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                     // ve Admin rolundekiler bu action'ı çağırabilir
        public IActionResult Edit(int id)
        {
            CategoryModel category = _categoryService.Query().SingleOrDefault(c => c.Id == id); // Add get item service logic here
            if (category == null)
            {
                return View("_Error", "Category not found!");
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(category);
        }

        // POST: Categories/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                     // ve Admin rolundekiler bu action'ı çağırabilir
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Add update service logic here
                var result = _categoryService.Update(category);
                if (result.IsSuccessful)
                    return RedirectToAction(nameof(Details), new { id = category.Id }); // Index action'ına dönmek yerine route value olarak id tanımlayarak Details action'ına dönüyoruz
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            // bazı ender durumlarda Authorize kullanmak yerine User üzerinden IsAuthenticated ile sisteme giriş yapıp yapmadığı IsInRole methodu
            // ile de rol veya rolleri kontrol edilerek buna göre işlemler yaptırılabilir
            if (!User.IsInRole("Admin")) // if koşulu !(User.Identity.IsAuthenticated && User.IsInRole("Admin")) şeklinde de yazılabilir ancak
                                         // IsInRole methodu zaten IsAuthenticated üzerinden authentication cookie'yi kontrol ettiğinden
                                         // buradaki if koşulu !User.IsInRole("Admin") yeterlidir,
                                         // eğer kullanıcı sisteme giriş yapmadıysa veya Admin rolunde değilse
                return View("_Error", "You are not authorized for this operation!"); // kullanıcıya bu işlem için yetkisi olmadığı bilgisini göster

            CategoryModel category = _categoryService.Query().SingleOrDefault(c => c.Id == id); // Add get item service logic here
            if (category == null)
            {
                return View("_Error", "Category not found!");
            }
            return View(category);
        }

        // POST: Categories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Action üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                     // ve Admin rolundekiler bu action'ı çağırabilir, genelde Authorize kullanılır ancak Delete get action'ında
                                     // olduğu gibi kullanıcının sisteme giriş yapıp yapmaması (User.Identity.IsAuthenticated)
                                     // veya rolü (User.IsInRole methodu) üzerinden yetkisi olmadığında
                                     // Authorize attribute'unun default davranışı (örneğin kullanıcı sisteme giriş yapmadıysa
                                     // Account -> Controllers -> Users -> Login action'ına eğer kullanıcı sisteme giriş yaptıysa
                                     // ve yetkisi olmayan bir action çağırdıysa Account -> Controllers -> Users -> AccessDenied
                                     // action'ına Program.cs'te Authentication region'ında yapılan konfigürasyona göre yönlendirir)
                                     // değiştirilmek istenirse action içerisinde User kullanılarak özelleştirilebilir
        public IActionResult DeleteConfirmed(int id)
        {
            // Add delete service logic here
            var result = _categoryService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}

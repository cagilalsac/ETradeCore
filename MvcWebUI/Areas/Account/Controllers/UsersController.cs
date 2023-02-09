#nullable disable
using Business.Models;
using Business.Models.Account;
using Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace MvcWebUI.Areas.Account.Controllers
{
    [Area("Account")]
    public class UsersController : Controller
    {
        // Add service injections here
        private readonly IAccountService _accountService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;

		public UsersController(IAccountService accountService, ICountryService countryService, ICityService cityService)
		{
			_accountService = accountService;
			_countryService = countryService;
			_cityService = cityService;
		}

		// GET: Account/Users/Register
		public IActionResult Register() // kayıt
        {
            ViewBag.Countries = new SelectList(_countryService.GetList(), "Id", "Name");
            // sadece ülke listesine göre bir SelectList oluşturuyoruz, şehir listesi ülke drop down list'inde yapılan ülke seçimine göre dolacak
           
            return View();
        }

        // POST: Account/Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AccountRegisterModel model) // kayıt
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.Register(model);
                if (result.IsSuccessful)
                    return RedirectToAction("Index", "Home", new { area = "" }); // projenin Home controller'ının Index action'ına dönebilmek için
                                                                                 // anonim bir objede area özelliğini "" atayarak yönlendirme yapmalıyız
                ModelState.AddModelError("", result.Message);
            }

			ViewBag.Countries = new SelectList(_countryService.GetList(), "Id", "Name", model.UserDetail.CountryId);
            // kullanıcının hatalı veri girişi yapması durumunda seçmiş olduğu ülke id'ye göre tekrar SelectList üzerinden bir drop down list oluşturuyoruz

            ViewBag.Cities = new SelectList(_cityService.GetList(model.UserDetail.CountryId ?? 0), "Id", "Name", model.UserDetail.CityId);
			// kullanıcının hatalı veri girişi yapması durumunda seçmiş olduğu ülke id'ye göre şehirleri tekrar doldurup
			// seçmiş olduğu şehir id'nin de drop down list'te seçili gelmesini sağlayan bir SelectList oluşturuyoruz,
			// eğer kullanıcı ülke seçimi yapmadıysa model.UserDetail.CountryId null geleceği için ?? operatörü ile null'sa 0 kullan diyerek
			// parametreyi GetList methoduna gönderiyoruz

			return View(model);
        }

        // GET: Account/Users/Login
        public IActionResult Login(string returnUrl) // giriş, returnUrl ile hangi sayfa üzerinden login'e geliniyorsa tekrar o sayfaya yönlendirilecek
        {
            AccountLoginModel model = new AccountLoginModel() // view'da UserName ve Password'ün boş, ReturnUrl'nin de aksiyona parametre olarak gelen
                                                              // returnUrl olacağı yeni bir model oluşturuyoruz
            {
                ReturnUrl = returnUrl
            };
            return View(model); // yukarıda new'lediğimiz modeli view'e gönderiyoruz
        }

        // POST: Account/Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginModel model) // giriş
        {
            if (ModelState.IsValid) 
            {
                UserModel userResult = new UserModel()
                {
                    Role = new RoleModel()
                }; 
                // UserModel tipindeki userResult'ı burada tanımlayıp new'liyoruz ki Login methodu  
                // başarılı olursa userResult referans tip olduğu için Login methodu içerisinde atansın 
                // ve bu methodda kullanabilelim, userResult içerisindeki Role referans özelliğini de new'liyoruz ki
                // methodda rolü de doldurabilelim

                var result = _accountService.Login(model, userResult); // modeldeki kullanıcı adı ve şifreyi aktiflik durumuyla birlikte kontrol ediyoruz
                if (result.IsSuccessful) // eğer modeldeki kullanıcı adı ve şifreye sahip aktif kullanıcı varsa
                {
                    // Login methodunda doldurulan userResult objesinin istediğimiz özelliklerindeki verileri bir claim (talep) listesinde dolduruyoruz ki
                    // bu liste üzerinden şifreli bir şekilde bir cookie (çerez) oluşup client'a geri dönülsün ve kullanıcı bilgilerini içeren
                    // bu cookie üzerinden web uygulamamızda authorization (yetki kontrülü) yapabilelim,
                    // claim'lerde asla şifre gibi kritik veriler saklanmamalıdır
                    List<Claim> claims = new List<Claim>()
                    {
                        //new Claim("Name", userResult.UserName), // Claim Dictionary veri tipine benzer bir tipi ve o tipe karşılık değeri olan bir yapıdır,
                                                                  // constructor'ının ilk parametresi olan tipi elle yazmak yerine ClaimTypes üzerinden
                                                                  // kullanmak daha uygundur, ikinci parametre ise bu tipe atanmak istenen değerdir
                        new Claim(ClaimTypes.Name, userResult.UserName),

                        new Claim(ClaimTypes.Role, userResult.Role.Name),

                        new Claim(ClaimTypes.Sid, userResult.Id.ToString()) // sepet (cart) işlemlerinde kullanabilmek için
                    };

                    // oluşturduğumuz claim listesi üzerinden cookie authentication default'ları ile bir identity (kimlik) oluşturuyoruz
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // oluşturduğumuz kimlik üzerinden de MVC'de authentication (kimlik doğrulama) için kullanacağımız bir principal oluşturuyoruz 
                    var principal = new ClaimsPrincipal(identity);

                    // son olarak oluşturduğumuz principal üzerinden cookie authentication default'ları ile MVC'de kimlik giriş işlemini tamamlıyoruz,
                    // SignInAsync methodu bir asenkron method olduğu için başına await (asynchronous wait) yazmalıyız
                    // ve methodun dönüş tipinin başına async yazarak dönüş tipini de bir Task tipi içerisinde tip olarak (IActionResult) tanımlamalıyız
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // giriş işlemi başarılı olduğu için kullanıcıyı ReturnUrl doluysa ReturnUrl üzerinden login'e geldiği controller ve action'a yönlendiriyoruz
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Home", new { area = "" }); // eğer ReturnUrl boşsa kullanıcıyı hoşgeldin view'ını
                                                                                 // dönen Home controller -> Index action'ına area'sı olmadığı için
                                                                                 // route value'da area = "" atayarak yönlendiriyoruz
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(); // view'a modeli göndermedik çünkü kullanıcının herhangi bir hatalı girişi durumunda sıfırdan sayfada kullanıcı adı ve şifresini girmesini istedik
        }

        public async Task<IActionResult> Logout() // çıkış
        {
            await HttpContext.SignOutAsync(); // Login aksiyonu ile oluşan çerezi (cookie) kaldırır

            // projenin Home controller -> Index action'ı bir area'nın içerisinde olmadığı için area özelliğini içeren anonim tipteki objeyi
            // route value parametresi olarak ve "" atayarak oluşturuyoruz
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult AccessDenied() // kullanıcı giriş yaptı ancak yetkisi olmayan bir controller action'ını çağırdı
        {
            return View("_Error", "Access is denied to this page!");
        }
    }
}

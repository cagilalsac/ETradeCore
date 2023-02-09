using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Models;
using System.Diagnostics;

namespace MvcWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Eğer controller'larda loglama (kayıt tutma) işlemleri yapılmak istenirse bu alan tanımlanıp constructor üzerinden enjekte
                                                          // edilerek bu alan üzerinden methodlar çağrılarak loglama işlemleri gerçekleştirilebilir.
                                                          // Loglar Properties klasöründeki launchSettings.json dosyasında konfigüre edilen development (geliştirme) profilinde proje
                                                          // çalıştırıldığında Visual Studio'nun Output penceresinde ASP.NET Core Web Server seçilerek,
                                                          // production (canlı) profilde çalıştırıldığında ise Console üzerinden gösterilecektir.
                                                          // Aşağıdan yukarıya gösterilme sırasına göre log seviyeleri: Sadece appsettings.json veya isteğe göre
                                                          // appsettings.Development.json dosyalarında belirtilen minimum seviyeye göre bu seviye ve üst seviyelerdeki loglar gösterilir.
                                                          // Trace (0) -> Debug (1) -> Information (2) -> Warning (3) -> Error (4) -> Critical (5)

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() // genelde karşılama için Index aksiyonu kullanılır, tarayıcıda adres olarak ~/Home/Index veya Program.cs altında default route tanımı
                                     // yani {controller=Home}/{action=Index}/{id?} (?: opsiyonel) yapıldığından ~/Home ya da ~/ yazılarak
                                     // veya bu adres için herhangi bir view'da link oluşturularak (örneğin Views -> Shared klasörü altındaki _Layout.cshtml) bu aksiyon çağrılabilir
                                     // (~: web sunucusunun adresi, örneğin https://localhost:44320 veya IP numarası).
        {
            _logger.LogInformation($"Home Controller -> Index Action executed on {DateTime.Now}.");
            // appsettings.json dosyasında LogLevel Default özelliği Information, appsettings.Development.json dosyasında da Debug atandığından
            // proje production profilinde çalıştırıldığında bu method ile belirtilen Information log seviyesi LogLevel Default özellik seviyesine (Information) eşit olduğu için
            // bu log gösterilecek, aynı şekilde proje development profilinde çalıştırıldığında bu method ile belirtilen Information log seviyesi
            // LogLevel Default özelliğinden (Debug) daha üst bir seviye olduğu için bu log yine gösterilecektir.

            //return View(); // Views klasörü altındaki controller adına sahip Home klasöründeki aksiyon adına sahip Index.cshtml'i döner
            return View("Welcome"); // Views klasörü altındaki controller adına sahip Home klasöründeki Welcome.cshtml'i döner
        }

        public IActionResult Privacy() // tarayıcı üzerinden adres kısmına ~/Home/Privacy yazılarak veya bu adres için herhangi bir view'da link oluşturularak
                                       // (örneğin Views -> Shared klasörü altındaki _Layout.cshtml) bu aksiyon çağrılabilir.
        {
            _logger.LogDebug($"Home Controller -> Privacy Action executed on {DateTime.Now}.");
            // appsettings.json dosyasında LogLevel Default özelliği Information, appsettings.Development.json dosyasında da Debug atandığından
            // proje production profilinde çalıştırıldığında bu method ile belirtilen Debug log seviyesi LogLevel Default özelliğinden (Information) daha alt bir seviye olduğu için
            // bu log gösterilmeyecek, ancak proje development profilinde çalıştırıldığında bu method ile belirtilen Debug log seviyesi
            // LogLevel Default özellik seviyesine (Debug) eşit olduğu için bu log gösterilecektir.

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // şu an için önemli değil, başka örnek üzerinden gösterilecek!
        public IActionResult Error() // tarayıcı üzerinden adres kısmına ~/Home/Error yazılarak veya herhangi bir controller'ın herhangi bir action'ında
                                     // hata (exception) alındığında bu aksiyona yönlendirme yapılarak çağrılabilir.
        {
            _logger.LogError($"Home Controller -> Error Action executed on {DateTime.Now}.");
            // appsettings.json dosyasında LogLevel Default özelliği Information, appsettings.Development.json dosyasında da Debug atandığından
            // proje production profilinde çalıştırıldığında bu method ile belirtilen Error log seviyesi LogLevel Default özelliğinden (Information) daha üst bir seviye olduğu için
            // bu log gösterilecek, aynı şekilde proje development profilinde çalıştırıldığında bu method ile belirtilen Error log seviyesi
            // LogLevel Default özelliğinden (Debug) daha üst bir seviye olduğu için bu log yine gösterilecektir.

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // şu an için önemli değil, başka örnek üzerinden gösterilecek!
        }
    }
}
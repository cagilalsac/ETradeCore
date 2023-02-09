using Business.Models.Cart;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MvcWebUI.Areas.Cart.Controllers
{
    [Area("Cart")]
    [Authorize(Roles = "User")] // sadece kullanıcılar sepet işlemi yapabilecek
    public class HomeController : Controller
    {
        // Session (oturum da denebilir) ASP.NET'te verilerin geçici olarak tarayıcı bazlı (kullanıcı bazlı) sunucu hafızasında 
        // saklanmasını sağlar. ASP.NET Core'da kullanabilmek için Program.cs'de konfigürasyonunun yapılması gerekir,
        // session'da sadece byte[], string ve int tiplerinde veri saklanabilir.

        

        private readonly IProductService _productService; // ürün id üzerinden ürünün adı ve birim fiyatına ihtiyacımız olduğundan
                                                          // ürün servisini constructor üzerinden inject ediyoruz

        int userId; // login olmuş kullanıcının Id'si

        const string SESSIONKEY = "cart"; // session'da tarayıcı bazlı (kullanıcı bazlı) veri saklamak için kullanacağımız anahtar

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult GetCart() // sepetteki elemanları listeleyecek action
        {
            // kullanıcı uygulamaya login olurkenki oluşturduğumuz user claim listesi üzerinden claim type'ı (tip) Sid olan
            // claim'in value'suna (değer) ulaşıp int'e dönüştürerek userId değişkenine atıyoruz
            userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var cart = GetSession(userId); // sepet elemanlarını model olarak methoddan session üzerinden alıyoruz

            // artık sepetteki elemanların gruplanmış halini kullanacağız
            //return View("Cart", cart); // aldığımız model'i Cart view'ına gönderiyoruz
            return View("CartGroupBy", GroupBy(cart)); // GroupBy methodundan dönen modeli CartGroupBy view'ına gönderiyoruz
        }

        public IActionResult AddToCart(int productId) // parametre olarak gönderilen productId üzerinden ürün adı ve birim fiyatı
                                                      // ile userId kullanarak oluşturulan sepet elemanının sepete eklenmesini sağlar
        {
            // kullanıcı uygulamaya login olurkenki oluşturduğumuz user claim listesi üzerinden claim type'ı (tip) Sid olan
            // claim'in value'suna (değer) ulaşıp int'e dönüştürerek userId değişkenine atıyoruz
            userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var product = _productService.Query().SingleOrDefault(p => p.Id == productId); // ürünü servisten productId üzerinden çekiyoruz

            var cart = GetSession(userId); // ürünü eklemeden önce session'dan kullanıcı sepet eleman listesini alıyoruz

            // sepete eklemek için sepet elemanını oluşturuyoruz
            var cartItem = new CartItemModel(productId, userId, product.UnitPrice ?? 0, product.Name); // ??: ürünün birim fiyatı null'sa 0,
                                                                                                       // değilse birim fiyatını kullan
           
            cart.Add(cartItem); // sepete sepet elemanını ekliyoruz

            SetSession(cart); // session'ı yeni eklenen sepet elemanı üzerinden güncelliyoruz

            TempData["Message"] = $"{cart.Count(c => c.ProductId == productId)} \"{product.Name}\" added to cart successfully."; 
            // Products controller'ın Index View'ında sepete ekleme sonucunu gösterebilmek için TempData'yı atıyoruz

            return RedirectToAction("Index", "Products", new { area = "" }); // kullanıcıyı Products controller'ının Index action'ına yönlendiriyoruz,
                                                                             // Products controller herhangi bir area içerisinde olmadığı için
                                                                             // area = "" yazdık
        }

        public IActionResult RemoveFromCart(int productId)
        {
            // kullanıcı uygulamaya login olurkenki oluşturduğumuz user claim listesi üzerinden claim type'ı (tip) Sid olan
            // claim'in value'suna (değer) ulaşıp int'e dönüştürerek userId değişkenine atıyoruz
            userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var cart = GetSession(userId); // kullanıcı sepetini session'dan çekiyoruz

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId); // sepetteki productId'si parametre olarak gelen productId'ye 
                                                                               // sahip ilk kaydı cartItem'a atıyoruz

            if (cartItem is not null) // eğer atadığımız sepet elemanı null değilse
            {
                cart.Remove(cartItem); // sepetten sepet elemanını çıkarıyoruz

                SetSession(cart); // session'ı kullanıcı sepetine göre güncelliyoruz

                TempData["Message"] = $"\"{cartItem.ProductName}\" removed from cart successfully.";
                // Cart area'sının Home controller'ının GetCart action'ının Cart View'ında sepetten çıkarma sonucunu gösterebilmek için TempData'yı atıyoruz
            }

            return RedirectToAction(nameof(GetCart)); // kullanıcıyı tekrar sepetini görmesi için GetCart action'ına yönlendiriyoruz
        }

        public IActionResult ClearCart()
        {
            // kullanıcı uygulamaya login olurkenki oluşturduğumuz user claim listesi üzerinden claim type'ı (tip) Sid olan
            // claim'in value'suna (değer) ulaşıp int'e dönüştürerek userId değişkenine atıyoruz
            userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var cart = GetSession(userId); // session'dan kullanıcı Id'ye göre sepet elemanlarını çekiyoruz

            cart.RemoveAll(c => c.UserId == userId); // sepetten kullanıcı Id'ye sahip tüm sepet elemanlarını siliyoruz

            SetSession(cart); // elemanlarını sildiğimiz listeye göre session'ı güncelliyoruz

            TempData["Message"] = "Cart cleared successfully.";
            // Cart area'sının Home controller'ının GetCart action'ının Cart View'ında sepetin temizlenmesi sonucunu gösterebilmek için TempData'yı atıyoruz

            return RedirectToAction(nameof(GetCart)); // kullanıcıyı tekrar sepetini görmesi için GetCart action'ına yönlendiriyoruz
        }



        private List<CartItemModel> GetSession(int userId) // session'dan kullanıcı sepetindeki elemanların listesini userId parametresine göre dönen method
        {
            var cart = new List<CartItemModel>(); // methoddan döneceğimiz listeyi new'liyoruz, eğer session'da
                                                  // kullanıcı sepet verisi yoksa içinde eleman olmayan bu listeyi methoddan döneceğiz

            var cartJson = HttpContext.Session.GetString(SESSIONKEY); // HttpContext'in Session referans objesi üzerinden
                                                                      // GetString methodu ile veriyi JSON formatında cartJson
                                                                      // değişkenine atıyoruz,
                                                                      // Session üzerinden diğer kullanabileceğimiz get methodları:
                                                                      // Get: byte[] döner,
                                                                      // GetInt32: int döner

            if (!string.IsNullOrWhiteSpace(cartJson)) // eğer cartJson'da veri varsa
            {
                cart = JsonConvert.DeserializeObject<List<CartItemModel>>(cartJson);
                // Newtonsoft.Json kütüphanesinin Deserialize methodunu kullanarak JSON formatındaki metinsel veriyi
                // List<CartItemModel> tipindeki C# objemize dönüştürüyoruz, JSON -> C#: Deserialize

                cart = cart.Where(c => c.UserId == userId).ToList();
                // sepetteki elemanları parametre olarak gelen kullanıcının userId'sine göre filtreliyoruz
            }

            return cart; // içinde eleman olmayan veya session'dan dolan listemizi dönüyoruz
        }

        private void SetSession(List<CartItemModel> cart) 
        {
            var cartJson = JsonConvert.SerializeObject(cart); // cart eleman listesini JSON formatına dönüştürüyoruz,
                                                              // C# -> JSON: Serialize

            HttpContext.Session.SetString(SESSIONKEY, cartJson); // HttpContext'in Session referans objesi üzerinden
                                                                 // SetString methodu ile veriyi JSON formatında Session'a atıyoruz,
                                                                 // Session üzerinden diğer kullanabileceğimiz set methodları:
                                                                 // Set: parametre olarak byte[] alır,
                                                                 // SetInt32: parametre olarak int alır
        }

        // parametre olarak gönderilen sepet eleman listesine göre ürün Id, kullanıcı Id ve ürün adına göre gruplama yaparak
        // toplam birim fiyat ve toplam sayıyı hesaplar ve gruplanmış model tipindeki listeyi döner,
        // LINQ GroupBy da Join vb. methodlarda olduğu gibi tüm collection'lar için (List, IEnumerable, IQueryable, vb.) kullanılabilir
        private List<CartItemGroupByModel> GroupBy(List<CartItemModel> cart)
        {
            var cartGroupBy = (from c in cart // c: sepetteki her bir eleman delegesi

                              group c by new { c.ProductId, c.UserId, c.ProductName } // c delegesi üzerinden ProductId, UserId ve ProductName
                                                                                      // özelliklerine göre grupluyoruz,
                                                                                      // eğer tek bir özelliğe göre gruplayacak olsaydık
                                                                                      // örneğin productName, new { } kullanmamıza gerek yoktu,
                                                                                      // direkt group c by c.ProductName yazacaktık

                              into cGroupBy // cGroupBy: gruplanmış delege

                              select new CartItemGroupByModel() // cGroupBy delegesi üzerinden gruplanmış model projeksiyonu
                              {
                                  ProductId = cGroupBy.Key.ProductId, // gruplanmış ProductId özelliğine Key üzerinden ulaşıyoruz

                                  UserId = cGroupBy.Key.UserId, // gruplanmış UserId özelliğine Key üzerinden ulaşıyoruz

                                  ProductName = cGroupBy.Key.ProductName, // gruplanmış ProductName özelliğine Key üzerinden ulaşıyoruz

                                  TotalUnitPriceValue = cGroupBy.Sum(cgb => cgb.UnitPrice), // gruplanmış özellikler üzerinden birim fiyat toplam değeri,
                                                                                            // cgb: cGroupBy için delege

                                  TotalUnitCountValue = cGroupBy.Count(), // gruplanmış özellikler üzerinden toplam sayı değeri

                                  TotalPrice = cGroupBy.Sum(cgb => cgb.UnitPrice).ToString("C2"), // gruplanmış özellikler üzerinden formatlanmış birim fiyat toplamı

                                  TotalCount = cGroupBy.Count() + " " + (cGroupBy.Count() == 1 ? "unit" : "units") // gruplanmış özellikler üzerinden formatlanmış toplam sayı
                              }).ToList();

            // Tüm toplamlar:
            var totalItem = new CartItemGroupByModel(); // toplamları göstereceğimiz eleman

            totalItem.ProductName = "Total"; // ürün adı sütununa Total yazdırıyoruz

            totalItem.TotalPrice = cartGroupBy.Sum(cgb => cgb.TotalUnitPriceValue).ToString("C2"); // toplam fiyatı TotalUnitPriceValue'ya göre hesaplatıyoruz
            
            totalItem.TotalCount = cartGroupBy.Sum(cgb => cgb.TotalUnitCountValue).ToString(); // toplam sayıyı TotalUnitCountValue'ya göre hesaplatıyoruz

            if (totalItem.TotalCount == "0") // TotalCount sonuna toplam sayıya göre no units, unit veya units ekliyoruz 
                totalItem.TotalCount = "No units";
            else if (totalItem.TotalCount == "1")
                totalItem.TotalCount += " unit";
            else
                totalItem.TotalCount += " units";
            
            cartGroupBy.Add(totalItem); // cartGroupBy listesine toplam elemanını ekliyoruz

            return cartGroupBy; // cartGrouBy listesini methoddan dönüyoruz
        }
    }
}

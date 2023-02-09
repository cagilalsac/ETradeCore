using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;

namespace MvcWebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent // view component'lar ViewComponent class'ından miras almalıdır,
                                                         // kullanım amacı ortak paylaşılan bir komponent oluşturup ihtiyaç
                                                         // duyulan projenin view'larında çağırmaktır,
                                                         // yapı olarak controller'a benzer ancak farkı içerisinde action
                                                         // yerine Invoke methodu bulunmasıdır ve istenilen view'da Invoke
                                                         // methodu çağırılarak komponent kullanılabilir,
                                                         // asenkron (bir işlemin sonucunu beklemeden başka bir işlem yaptırma)
                                                         // olarak kullanılır,
                                                         // Invoke methodu sonucunda dönecek view
                                                         // MvcWebUI -> Views -> Shared -> Components
                                                         // -> Categories (view component adı) -> Default.cshtml altında oluşturulmalıdır
    {

        private readonly ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ViewViewComponentResult Invoke()
        {
            List<CategoryModel> categories;
            Task<List<CategoryModel>> task;

            // Senkron:
            //categories = _categoryService.Query().ToList(); // *1

            // Asenkron:
            //task = _categoryService.Query().ToListAsync(); // *2, ToList, SingleOrDefault ve FirstOrDefault gibi methodların asenkron (Async)
                                                             // versiyonları asenkron işlemler için kullanılabilir,
                                                             // sonucunda bir task (görev) döner

            // Asenkron:
            task = _categoryService.GetListAsync(); // *3, kategori listesini getiren görev,
                                                    // asenkron methodlar bizim tarafımızdan da oluşturulup kullanılabilir

            categories = task.Result; // görev Result ile tamamlandıktan sonra ulaşılan sonuç kategoriler verisi

            return View(categories);
        }
    }
}

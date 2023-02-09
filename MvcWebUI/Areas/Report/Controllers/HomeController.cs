using Business.Services;
using Business.Services.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebUI.Areas.Report.Models;

namespace MvcWebUI.Areas.Report.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Report")]
    public class HomeController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;

        public HomeController(IReportService reportService, ICategoryService categoryService, IStoreService storeService)
        {
            _reportService = reportService;
            _categoryService = categoryService;
            _storeService = storeService;
        }

        public IActionResult Index(HomeIndexViewModel viewModel) // Bir action'dan view'e tek model objesi gönderilebildiği için
                                                                 // içerisinde birden çok obje tutabilen bir view model
                                                                 // objesi kullanılarak view'da farklı tiplerde birden çok obje kullanılabilir.
                                                                 // View model'i MvcWebUI -> Report -> Models -> HomeIndexViewModel class'ında
                                                                 // önce controller sonra action adı belirterek oluşturuyoruz.
                                                                 // Index action'ı ilk kez çağrıldığında viewModel objesi new'lenmiş
                                                                 // olarak gelecek ancak içerisindeki referanslar özellikleri null olacaktır,
                                                                 // bu action'da bu referans özelliklerini service'lerden doldurup
                                                                 // view model objesini tekrar view'a gönderiyoruz.
        {
            viewModel.Report = _reportService.GetList(viewModel.Filter, false); // Business katmanındaki servis methoduna yine aynı
                                                                                // katmandaki ReportFilterModel tipindeki Filter
                                                                                // parametresini gönderiyoruz ki filtrelenerek
                                                                                // sonuç dönsün.
                                                                                // viewModel asla bu methodda parametre olarak
                                                                                // kullanılmamalıdır çünkü Business katmanı
                                                                                // MvcWebUI katmanına ait olan HomeIndexViewModel
                                                                                // tipine bağımlı hale gelmiş olur (MvcWebUI -> Business).
                                                                                // Bağımlılık her zaman
                                                                                // AppCore -> DataAccess -> Business -> MvcWebUI
                                                                                // yönünde olmalıdır.

            viewModel.Categories = new SelectList(_categoryService.Query().ToList(), "Id", "Name");
            // view'ın filtreleme kısmında kategori drop down list'i için oluşturuyoruz,
            // view'da kullanıcının seçtiği kategori arama yapıldıktan sonra seçili geliyorsa
            // 4. parametre olan selectedValue'yu her zaman göndermeye gerek yok

            viewModel.Stores = new MultiSelectList(_storeService.Query().ToList(), "Id", "Name");
            // view'ın filtreleme kısmındaki mağazalar list box'ı için MultiSelectList oluşturduk

            return View(viewModel);
        }
    }
}

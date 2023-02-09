#nullable disable // view model'lerde de nullable disable'ı model ve entity'lerde olduğu gibi kullanmak daha uygundur

using Business.Models.Report;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcWebUI.Areas.Report.Models
{
    public class HomeIndexViewModel
    {
        public List<ReportItemModel> Report { get; set; } // rapor view'ında servis üzerinden join kullanarak çektiğimiz rapor elemanları,
                                                          // Business -> Models -> Report -> ReportItemModel class'ını oluşturuyoruz

        public ReportFilterModel Filter { get; set; } // içerisinde raporu filtrelemek için kullanacağımız özelliklerin bulunduğu referans özellik

        public SelectList Categories { get; set; } // view'ın filtreleme kısmında kategori drop down list'i için kullanacağız

        public MultiSelectList Stores { get; set; } // view'ın filtreleme kısmında mağaza list box'ı için kullanacağız
    }
}

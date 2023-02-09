#nullable disable

using System.ComponentModel;

namespace Business.Models.Report
{
    public class ReportFilterModel // view'da filtreleme kısmında kullanıcıdan alacağımız input'lar
    {
        [DisplayName("Category")]
        public int? CategoryId { get; set; } // view'daki tüm kategoriler işlemi için null geleceğinden nullable tanımladık

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Unit Price")]
        public double? UnitPriceBegin { get; set; }

        public double? UnitPriceEnd { get; set; }

        [DisplayName("Stock Amount")]
        public int? StockAmountBegin { get; set; }

        public int? StockAmountEnd { get; set; }

        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDateBegin { get; set; }

        public DateTime? ExpirationDateEnd { get; set; }

        [DisplayName("Store")]
        public List<int> StoreIds { get; set; }
    }
}

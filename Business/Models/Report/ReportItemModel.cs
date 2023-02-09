#nullable disable

using System.ComponentModel;

namespace Business.Models.Report
{
    public class ReportItemModel // bu modeli sadece rapor için kullanacağımızdan ve üzerinden CRUD işlemleri yapmayacağımızdan
                                 // RecordBase'den miras almamıza gerek yok
    {
		#region Rapor view'ında gösterim için kullanacağımız özellikler
		[DisplayName("Product Name")]
        public string ProductName { get; set; }

		// view'da ProductDescription'ı HTML title attribute'u üzerinden göstereceğimizden DisplayName'e gerek yok
		public string ProductDescription { get; set; }

		[DisplayName("Unit Price")]
		public string UnitPrice { get; set; }

		[DisplayName("Stock Amount")]
		public string StockAmount { get; set; }

		[DisplayName("Expiration Date")]
		public string ExpirationDate { get; set; }

		[DisplayName("Category")]
		public string CategoryName { get; set; }

		// view'da CategoryDescription'ı HTML title attribute'u üzerinden göstereceğimizden DisplayName'e gerek yok
		public string CategoryDescription { get; set; }

		[DisplayName("Store")]
		public string StoreName { get; set; }
		#endregion

		#region Rapor view'ında sorgu üzerinden filtreleme için kullanacağımız özellikler
		public int? CategoryId { get; set; } // tüm kategoriler için null geleceğinden int? tipini kullandık

		public int? StoreId { get; set; } // tüm mağazalar için null geleceğinden int? tipini kullandık

		public double UnitPriceValue { get; set; }

		public int StockAmountValue { get; set; }

		public DateTime? ExpirationDateValue { get; set; }
		#endregion
	}
}

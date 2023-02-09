#nullable disable

using System.ComponentModel;

namespace Business.Models.Cart
{
    // CartItemModel tipindeki liste üzerinden gruplama yaparak toplam birim fiyat ve toplam birim sayılarını göstereceğimiz model
    public class CartItemGroupByModel 
    {
        public int ProductId { get; set; } // gruplayacağımız ürün Id
        public int UserId { get; set; } // gruplayacağımız kullanıcı Id

        [DisplayName("Product Name")]
        public string ProductName { get; set; } // gruplayacağımız ürün adı

        public string TotalUnitPrice { get; set; } // gruplamaya göre hesaplayacağımız toplam birim fiyat
        public string TotalUnitCount { get; set; } // gruplamaya göre hesaplayacağımız toplam birim sayısı



        // Tüm toplamlar:
        public double TotalUnitPriceValue { get; set; } // TotalPrice özelliğine bu özellik üzerinden toplam alıp atama yapacağız
        public int TotalUnitCountValue { get; set; } // TotalCount özelliğine bu özellik üzerinden toplam alıp atama yapacağız

        [DisplayName("Total Price")]
        public string TotalPrice { get; set; } // view'da kullanıcının ödemesi gereken toplam fiyatı göstereceğimiz özellik

        [DisplayName("Total Count")]
        public string TotalCount { get; set; } // view'da kullanıcıya toplam sayıyı göstereceğimiz özellik
    }
}

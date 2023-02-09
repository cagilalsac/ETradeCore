#nullable disable

using System.ComponentModel;

namespace Business.Models.Cart
{
    public class CartItemModel // sepet elemanı, üzerinden CRUD işlemleri yapmayacağımız için RecordBase'den miras almıyor
    {
        public int ProductId { get; set; } // sepete eklenen ürünün Id'si
        public int UserId { get; set; } // sepete ürün ekleyen kullanıcının Id'si

        [DisplayName("Unit Price")]
        public double UnitPrice { get; set; } // sepete eklenen ürünün birim fiyatı

        [DisplayName("Product Name")]
        public string ProductName { get; set; } // sepete eklenen ürünün adı

        // objeyi daha kolay new'leyebilmek için parametreli constructor'ını oluşturuyoruz (opsiyonel)
        public CartItemModel(int productId, int userId, double unitPrice, string productName)
        {
            ProductId = productId;
            UserId = userId;
            UnitPrice = unitPrice;
            ProductName = productName;
        }
    }
}

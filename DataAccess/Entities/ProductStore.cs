#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class ProductStore // Ürün Mağaza ara ilişki entity'si, ürün ve mağaza arasında many to many ilişki olduğu için bu ara ilişkileri tutan entity'i oluşturuyoruz
    {
        [Key]
        [Column(Order = 0)]
        public int ProductId { get; set; } // Ürün entity'sinden ürün id'yi taşıyoruz ve mağaza id ile birlikte primary key olarak ilk sırada set ediyoruz

        public Product Product { get; set; }

        [Key]
        [Column(Order = 1)]
        public int StoreId { get; set; } // Mağaza entity'sinden mağaza id'yi taşıyoruz ve ürün id ile birlikte primary key olarak ikinci sırada set ediyoruz

        public Store Store { get; set; }
    }
}

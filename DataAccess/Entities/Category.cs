#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Category : RecordBase // Kategori
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; } // 1 to many ilişkili başka entity kolleksiyonuna referans, 1 kategorinin 0 veya daha çok ürünleri olabilir
    }
}

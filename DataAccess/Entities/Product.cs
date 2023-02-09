#nullable disable // null değer atanabilen referans tiplerin sonuna ? yazma uyarısını devre dışı bırakmak 
                  // ve ? konmadığında zorunlu hale gelmelerini engellemek için sadece entity ve modellerde kullanılmalıdır

using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    //[Table("ETradeProducts")] // Product entity'sinin karşılığında kullanılacak tablo ismini değiştirmek için kullanılabilir, ETradeContext OnModelCreating methodunda yazmak daha uygundur
    public class Product : RecordBase // Ürün
    {
        // SOLID prensipleri gereği özellikler üzerinden data annotation'larla tablo sütun özelliklerini belirlemek yerine
        // proje DbContext class'ında (ETradeContext) OnModelCreating methodunu ezerek bu method içerisinde 
        // başka bir class üzerinden bu sütun özelliklerini tanımlamak daha uygun olacaktır.

        [Required] // zorunlu (tabloya null kaydedilemeyen) özellik
        [StringLength(200)] // Name özelliğinin veritabanı tablosundaki sütun tipi nvarchar(200) olacaktır
        public string Name { get; set; } 

        [StringLength(500)] // Description özelliğinin veritabanı tablosundaki sütun tipi nvarchar(500) olacaktır
        public string Description { get; set; } // zorunlu olmayan (tabloya null kaydedilebilen) özellik

        public double UnitPrice { get; set; } // zorunlu özellik

        public int StockAmount { get; set; } // zorunlu özellik

        public DateTime? ExpirationDate { get; set; } // zorunlu olmayan özellik

        public int? CategoryId { get; set; } // zorunlu olmayan özellik, 1 to many ilişki (1 ürünün 0 veya 1 kategorisi olmalı)

        public Category Category { get; set; } // ilişkili entity'e referans özelliği

        public List<ProductStore> ProductStores { get; set; } // many to many ilişki için ürün mağaza kolleksiyon referansı

        [Column(TypeName = "image")]
        public byte[] Image { get; set; } // byte[] olarak binary veri saklayacağımız, veritabanındaki tabloda sütun tipi image olacak ve null'a izin veren özellik,
                                          // eğer null'a izin vermeyeceksek Required attribute'u kullanılmalıdır

        [StringLength(5)]
        public string ImageExtension { get; set; } // binary veri olarak saklayacağımız Image için dosya uzantısı, örneğin .jpg, .png, vb,
												   // dosya uzantısını Image'ı çekerken içerik tipini belirlemek için kullanacağız,
												   // örneğin image/jpeg, image/png,
		                                           // eğer null'a izin vermeyeceksek Required attribute'u kullanılmalıdır
	}
}

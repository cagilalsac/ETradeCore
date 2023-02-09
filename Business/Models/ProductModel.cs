#nullable disable // null değer atanabilen referans tiplerin sonuna ? yazma uyarısını devre dışı bırakmak 
                  // ve ? konmadığında zorunlu hale gelmelerini engellemek için sadece entity ve modellerde kullanılmalıdır

using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{
    public class ProductModel : RecordBase // modeller de RecordBase'den miras almalıdır ki hem Id ve Guid alanlarını
                                           // miras alsın hem de servislerde tip olarak kullanılabilsin.
    {
        // ilgili entity'de referans olmayan özellikler veya başka bir deyişle veritabanındaki ilgili tablosundaki
        // sütun karşılığı olan özellikler entity'den kopyalanır.

        // SOLID prensipleri gereği bu class'ta validasyon için data annotation'lar kullanmak yerine MVC FluentValidation
        // gibi bir kütüphane üzerinden başka bir class'ta validasyonları yönetmek daha uygun olacaktır.

        #region Entity'den Kopyalanan Özellikler
        //[Required] // kullanıcıdan gelen model verisi validasyonu için zorunlu olacağını belirtir
        [Required(ErrorMessage = "{0} is required!")] // istenirse view'larda gösterilecek validasyon mesajları bu şekilde özelleştirilebilir,
                                                      // 0: varsa DisplayName'i (Product Name) yoksa özellik ismini (Name) kullanır

        //[StringLength(200)] // kullanıcıdan gelen model verisi validasyonu için verinin maksimum 200 karakter olacağını belirtir 
        
        //[MinLength(3)] // alternatif olarak kullanıcıdan gelen model verisi validasyonu için verinin minimum 3 karakter olacağını belirten data annotation kullanılabilir
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} characters!")] // 0: varsa DisplayName'i (Product Name) yoksa özellik ismini (Name) kullanır,
                                                                             // 1: MinLength attribute'unun ilk parametresi olan 3'ü kullanır

        //[MaxLength(200)] // alternatif olarak kullanıcıdan gelen model verisi validasyonu için verinin maksimum 200 karakter olacağını belirten data annotation kullanılabilir
        [MaxLength(200, ErrorMessage = "{0} must be maximum {1} characters!")] // 0: varsa DisplayName'i (Product Name) yoksa özellik ismini (Name) kullanır,
                                                                               // 1: MaxLength attribute'unun ilk parametresi olan 200'ü kullanır

        [DisplayName("Product Name")]  // her view'da elle Product Name yazmak yerine model üzerinden bu özelliğin DisplayName'ini kullanacağız,
                                       // eğer yazılmazsa DisplayName üzerinden özelliğin adı (Name) kullanılır.  
        public string Name { get; set; }



        //[StringLength(500)] // kullanıcıdan gelen model verisi validasyonu için verinin maksimum 500 karakter olacağını belirtir 
        [StringLength(500, ErrorMessage = "{0} must be maximum {1} characters!")] // 0: varsa DisplayName'i yoksa özellik ismini (Description) kullanır,
                                                                                  // 1: StringLength attribute'unun ilk parametresi olan 500'ü kullanır
        public string Description { get; set; } // kullanıcıdan gelen model verisi validasyonu için zorunlu olmayan özellik



        //[Range(0, double.MaxValue)] // isteğe göre kullanıcıdan gelen model verisi validasyonu için verinin pozitif bir double değer olacağını belirten data annotation kullanılabilir
        [Range(0, double.MaxValue, ErrorMessage = "{0} must be zero or positive!")] // 0: varsa DisplayName'i (Unit Price) yoksa özellik ismini (UnitPrice) kullanır

        [DisplayName("Unit Price")]

        [Required(ErrorMessage = "{0} is required!")]  // view'da text box üzerinden birim fiyat girileceği ve boş girildiğinde "The value '' is invalid." validasyon mesajı
                                                       // gösterileceği için UnitPrice'ı nullable yaptık ve istediğimiz mesajın gösterimini sağlamak için de Required
                                                       // data annotation'ını kullandık 
        public double? UnitPrice { get; set; } // kullanıcıdan gelen model verisi validasyonu için zorunlu özellik



        //[Range(0, 1000000)] // isteğe göre kullanıcıdan gelen model verisi validasyonu için verinin 0 ile 1000000 arasında bir integer değer olacağını belirten data annotation kullanılabilir
        [Range(0, 1000000, ErrorMessage = "{0} must be between {1} and {2}!")] // 0: varsa DisplayName'i (Stock Amount) yoksa özellik ismini (StockAmount) kullanır,
                                                                               // 1: Range attribute'unun ilk parametresi olan 0'ı kullanır,
                                                                               // 2: Range attribute'unun ikinci parametresi olan 1000000'u kullanır

        [DisplayName("Stock Amount")]

        [Required(ErrorMessage = "{0} is required!")]  // view'da numeric up down üzerinden stok miktarı girileceği ve boş girildiğinde "The value '' is invalid." validasyon mesajı
                                                       // gösterileceği için StockAmount'u nullable yaptık ve istediğimiz mesajın gösterimini sağlamak için de Required
                                                       // data annotation'ını kullandık 

        public int? StockAmount { get; set; } // kullanıcıdan gelen model verisi validasyonu için zorunlu özellik



        [DisplayName("Expiration Date")]  
        public DateTime? ExpirationDate { get; set; } // kullanıcıdan gelen model verisi validasyonu için zorunlu olmayan özellik




        [DisplayName("Category")]  
        public int? CategoryId { get; set; } // ürünün kategorisi view'da bir drop down list üzerinden seçileceği ve Seçiniz item'ı üzerinden null gönderilebileceği
                                             // için CategoryId nullable yapılmalıdır ve eğer mutlaka seçim yapılması isteniyorsa Required attribute'u
                                             // (data annotation) ile işaretlenmelidir, bu örnekte seçim yapılmasa da olur.



		public byte[] Image { get; set; } // ilgili controller action'ında parametre olarak aldığımız IFormFile tipindeki imaj (image) verisini kopyalayacağımız
                                          // binary veriyi tipindeki özellik



		[StringLength(5, ErrorMessage = "{0} must be maximum {1} characters!")]
		public string ImageExtension { get; set; } // ilgili controller action'ında parametre olarak aldığımız IFormFile tipindeki imaj (image)
                                                   // üzerinden kullanıcının yüklediği dosya uzantısını saklayacağımız özellik
		#endregion



		// ihtiyaç halinde view'larda gösterim veya veri girişi için entity verilerini özelleştirip (formatlama, ilişkili entity referansı üzerinden özellik kullanma, vb.) kullanacağımız yeni özellikler oluşturulabilir.
		#region View'larda Gösterim veya Veri Girişi için Kullanacağımız Özellikler
		[DisplayName("Unit Price")] 
        public string UnitPriceDisplay { get; set; } // Obje üzerinden kolay bir şekilde gösterim amaçlı kullanım için kullanacağımız özellikleri belirtmek ve
                                                     // yukarıda UnitPrice özelliği olduğundan ve aynı özellik ismini kullanamayacağımızdan kendi belirlediğimiz
                                                     // bir son eki (Display) özellik adında kullanıyoruz.
                                                     // Display ile biten özelliklerin verilerini özelleştirerek (formatlama, ilişkili entity referansı üzerinden
                                                     // özellik kullanma, vb.) ilgili servisin Query methodunda atayacağız.



        [DisplayName("Expiration Date")] 
        public string ExpirationDateDisplay { get; set; }



        [DisplayName("Category")]
        public string CategoryNameDisplay { get; set; }



        [DisplayName("Stores")]
        public string StoreNamesDisplay { get; set; } // bir ürünün birden çok mağazası olabileceği için ilişkili mağaza adlarını bir ayraç üzerinden
                                                      // tek bir string olarak birleştirip bu özelliğe ürün servisinde atıyoruz ve kullanıcıya gösteriyoruz


        [DisplayName("Stores")]
        public List<int> StoreIds { get; set; } // kullanıcıdan ürün ekleme ve güncelleme işlemlerinde mağaza verilerini alırken view'da list box kullanarak
                                                // mağaza listesi üzerinden kullanıcının seçtiği mağazaların id'lerini bu özellikte saklıyoruz,
                                                // eğer bir ürünün mutlaka bir veya daha çok mağazası olacaksa Required attribute'u kullanılmalıdır



        [DisplayName("Image")]
        public string ImgSrcDisplay { get; set; } // Image binary verisini view'da img HTML tag'inin src attribute'una ilgili servisin Query methodunda
                                                  // dönüşümünü yapıp bu özelliği atayarak kullanıcıya göstereceğiz
        #endregion



        /*
        Entity ve model özelliklerinde kullanılabilecek bazı genel data annotation'lar (attribute): 
        NOT: Data annotation'lar ile sadece model verisi üzerinden basit validasyonlar yapılabilir, örneğin veritabanındaki bir tablo üzerinden 
        validasyon gerekiyorsa bu validasyon service class'larında yapılmalıdır.

        Key (Entity): Özelliğin birincil anahtar olduğunu belirtir ve veritabanı oluşturulurken tablodaki sütun karşılığı otomatik artan sayı olarak ayarlanır.
        Required (Entity ve Model): Özelliğin zorunlu olduğunu belirtir.
        Column (Entity): Özelliğin veritabanı tablosundaki sütunu ile ilgili ayarlarını belirtir, örneğin sütun adı (Name), sütun veri tipi (TypeName) ve sütun sırası (Order: çoklu key için kullanılır).
        DataType (Model): Özelliğin veri tipi için kullanılır, örneğin Text, Date, Time, DateTime, Currency, EmailAddress, PhoneNumber, Password, v.b.
        ReadOnly (Model): Özelliğin sadece okunabilir olması için kullanılır.
        DisplayFormat (Model): Metinsel veri gösteriminde kullanılacak format'ı belirtir ve genellikle tarih, ondalık sayı, v.b. formatlama işlemleri için kullanılır.
        Table (Entity): Veritabanında oluşacak tablonun adını (Name) değiştirmek için kullanılır.
        StringLength (Entity ve Model): Metinsel tipte özellikler için girilecek karakter sayısının maksimumunu belirtmede kullanılır.
        MinLength (Model): Metinsel tipte özellikler için girilecek karakter sayısının minimumunu belirtmede kullanılır.
        MaxLength (Model): Metinsel tipte özellikler için girilecek karakter sayısının maksimumunu belirtmede kullanılır.
        Compare (Model): Tanımlandığı özelliğin başka bir özellik üzerinden verilerinin karşılaştırılması için kullanılır.
        RegularExpression (Model): Verilerin daha detaylı validasyonu için öğrenilip kullanılabilecek bir doğrulama desenidir.
        Range (Model): Sayısal değerler için aralık belirtmede kullanılır.
        EmailAddress (Model): Özellik verisinin e-posta formatında olması için kullanılır.
        Phone (Model): Özellik verisinin telefon formatında olması için kullanılır.
        NotMapped (Entity): Özelliğin veritabanında ilgili tablosunda sütununun oluşturulmaması için kullanılır.
        JsonIgnore (Model): Özelliğin oluşturulacak JSON formatındaki veriye dahil edilmemesini sağlar.
        */
    }
}

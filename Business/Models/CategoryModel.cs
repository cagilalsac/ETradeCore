#nullable disable 

using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class CategoryModel : RecordBase
    {
        #region Entity'den Kopyalanan Özellikler
        [Required(ErrorMessage = "{0} is required!")] 
        [StringLength(100, ErrorMessage = "{0} must be maximum {1} characters!")]
        [DisplayName("Category Name")]   
        public string Name { get; set; }



        // StringLength veya MaxLength kullanılmadığından kullanıcıdan gelen model verisi validasyonu için veri maksimum nvarchar(MAX) üzerinden MAX ile tanımlanmış karakter olacaktır 
        public string Description { get; set; }
        #endregion



        #region View'larda Gösterim veya Veri Girişi için Kullanacağımız Özellikler
        [DisplayName("Products Count")]
        public int ProductsCount { get; set; }
        #endregion
    }
}

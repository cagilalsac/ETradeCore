#nullable disable

using DataAccess.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
	public class UserDetailModel
	{
		#region Entity'den Kopyalanan Özellikler
		public int UserId { get; set; }



		public Sex Sex { get; set; }



		[Required(ErrorMessage = "{0} is required!")]
		[StringLength(250, ErrorMessage = "{0} must be maximum {1} characters!")]
		[EmailAddress(ErrorMessage = "{0} must be in e-mail format!")]
		[DisplayName("E-Mail")]
		public string Email { get; set; }



		[StringLength(25, ErrorMessage = "{0} must be maximum {1} characters!")]
		[Phone(ErrorMessage = "{0} must be in phone number format!")]
		public string Phone { get; set; }



		[Required(ErrorMessage = "{0} is required!")]
		[StringLength(750, ErrorMessage = "{0} must be maximum {1} characters!")]
		public string Address { get; set; }



		[DisplayName("Country")]
		[Required(ErrorMessage = "{0} is required!")]
		public int? CountryId { get; set; }



		[DisplayName("City")]
		[Required(ErrorMessage = "{0} is required!")]
		public int? CityId { get; set; }
		#endregion



		#region Entity Referans Özelliklerine Karşılık Kullanacağımız Özellikler
		public CountryModel Country { get; set; }



		public CityModel City { get; set; }
		#endregion
	}
}

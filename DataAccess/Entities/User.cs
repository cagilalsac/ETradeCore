#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
	public class User : RecordBase // Kullanıcı
	{
		[Required]
		[StringLength(15)]
		public string UserName { get; set; } // zorunlu

		[Required]
		[StringLength(10)]
		public string Password { get; set; } // zorunlu

		public bool IsActive { get; set; } // zorunlu, sadece aktif kullanıcıların (IsActive = true) uygulamaya giriş yapmasını sağlamak için bu özellik kullanılır,
										   // kullanıcının uygulama üzerinden aktifliği kaldırıldığında (IsActive = false) artık uygulamaya giriş yapamayacaktır

		public int RoleId { get; set; } // zorunlu, 1 to many ilişki (1 kullanıcının mutlaka 1 rolü olmalı)

		public Role	Role { get; set; }

		public UserDetail UserDetail { get; set; } // 1 kullanıcının 0 veya 1 ilişkili kullanıcı detayı olabilir
	}
}

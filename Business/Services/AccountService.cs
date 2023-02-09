using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using Business.Models.Account;
using DataAccess.Enums;

namespace Business.Services
{
    public interface IAccountService // IAccountService'i IService'ten implemente etmiyoruz çünkü bu servis UserService enjeksiyonu üzerinden login ve register işlerini yapacak,
									 // CRUD işlemlerinin hepsini yapmayacak, bu yüzden Login ve Register methodlarını içerisinde tanımlıyoruz
	{
		Result Login(AccountLoginModel accountLoginModel, UserModel userResultModel); // kullanıcıların kullanıcı girişi için
		// accountLoginModel view üzerinden kullanıcıdan aldığımız verilerdir,
		// userResultModel ise accountLoginModel'deki doğru verilere göre kullanıcıyı veritabanından çektikten sonra method içerisinde atayacağımız ve
		// referans tip olduğu için de Login methodunu çağırdığımız yerde kullanabileceğimiz sonuç kullanıcı objesidir,
		// böylelikle Login methodundan hem login işlem sonucunu Result olarak hem de işlem başarılıysa kullanıcı objesini UserModel objesi olarak dönebiliyoruz

		Result Register(AccountRegisterModel accountRegisterModel); // kullanıcıların yeni kullanıcı kaydı için
	}

	public class AccountService : IAccountService
	{
		private readonly IUserService _userService; // CRUD işlemlerini yaptığımız UserService objesini bu servise enjekte ediyoruz ki Query methodu üzerinden Login,
													// Add methodu üzerinden de Register işlemleri yapabilelim

		public AccountService(IUserService userService)
		{
			_userService = userService;
		}

		public Result Login(AccountLoginModel accountLoginModel, UserModel userResultModel) // kullanıcı girişi
		{
			// önce accountLoginModel üzerinden kullanıcının girmiş olduğu kullanıcı adı ve şifreye sahip aktif kullanıcı sorgusu üzerinden veriyi çekip user'a atıyoruz,
			// kullanıcı adı ve şifre hassas veri olduğu için trim'lemiyoruz ve büyük küçük harf duyarlılığını da ortadan kaldırmıyoruz
			var user = _userService.Query().SingleOrDefault(u => u.UserName == accountLoginModel.UserName && u.Password == accountLoginModel.Password && u.IsActive);

			if (user is null) // eğer böyle bir kullanıcı bulunamadıysa
				return new ErrorResult("Invalid user name or password!"); // kullanıcı adı veya şifre hatalı sonucunu dönüyoruz

			// burada kullanıcı bulunmuş demektir dolayısıyla referans tip olduğu için userResultModel'i yukarıda çektiğimiz user'a göre dolduruyoruz,
			// dolayısıyla hem sorgulanan kullanıcı objesi (userResultModel) hem de işlem sonucunu SuccessResult objesi olarak methoddan dönüyoruz,
			// Account area -> Users controller -> Login action'ında sadece kullanıcı adı ve role ihtiyacımız olduğu için objemizi bu özellikler üzerinden dolduruyoruz
			userResultModel.UserName = user.UserName;
			userResultModel.Role.Name = user.Role.Name;

			userResultModel.Id = user.Id; // sepet (cart) işlemlerinde kullanabilmek için

            return new SuccessResult(); 
		}

		public Result Register(AccountRegisterModel accountRegisterModel) // yeni kullanıcı kaydı
		{
			var user = new UserModel()
			{
				IsActive = true, // istenirse burada olduğu gibi tüm kayıt yapan kullanıcılar aktif yapılabilir veya aktif yapılmayarak örneğin e-posta gönderimi
								 // sağlanıp gönderilen link'e tıklandıktan sonra kullanıcının aktiflik durumu aktif olarak güncellenebilir

				Password = accountRegisterModel.Password,
				UserName = accountRegisterModel.UserName,

                RoleId = (int)Roles.User, // Roles enum'ı üzerinden RoleId'yi atamak hem veritabanındaki rol tablosundaki id'ler güncellenirse bu enum üzerinden
										  // kolayca bu değişikliğin uygulanabilmesini hem de her bir rolün id'si neydi diye veritabanındaki tabloya
										  // sürekli bakılmasından kurtulmamızı sağlar

				UserDetail = new UserDetailModel() // kullanıcının detay verilerini içeren UserDetail referans özelliğini de new'leyip
												   // accountRegisterModel'a göre dolduruyoruz
				{
					Address = accountRegisterModel.UserDetail.Address,
					CityId = accountRegisterModel.UserDetail.CityId,
					CountryId = accountRegisterModel.UserDetail.CountryId,
					Email = accountRegisterModel.UserDetail.Email,
					Phone = accountRegisterModel.UserDetail.Phone,
					Sex = accountRegisterModel.UserDetail.Sex
				}
            };

			// 1. yöntem:
			//return _userService.Add(user); // UserService'teki Add methodu bize sonuç döndüğünden ve bu sonucu dönerek Register methodunu
											 // çağırdığımız yerde kullanabileceğimizden UserService'teki Add methodundan dönen sonucu Register
											 // methodu sonucu olarak dönebiliriz
			// 2. yöntem:
			var result = _userService.Add(user); // UserService Add methodunun sonucunu bir result objesine atıyoruz
			if (!result.IsSuccessful) // eğer işlem başarılı değilse
				result.Message = "User can't be registered becuase user with the same name exists!"; // dönen ErrorResult objesinin mesajını burada değiştiriyoruz
			return result; // SuccessResult için bir değişiklik yapmadık, ya SuccessResult objesini ya da mesajını değiştirdiğimiz ErrorResult objesini dönüyoruz
		}
	}
}

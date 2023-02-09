using AppCore.Business.Services.Bases;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Repositories;

namespace Business.Services
{
	public interface ICountryService : IService<CountryModel>
	{
		List<CountryModel> GetList(); // IService'teki method tanımları dışında eğer istenirse yeni method tanımları bu interface
								      // içerisinde yapılıp bu interface'in implemente edildiği class'ta bu tanımlanan methodlar yazılabilir,
								      // kolayca ülke listesini tek method üzerinden CountryModel listesi olarak çekebilmek için
								      // bu methodu tanımladık ve aşağıdaki class'ta implemente ettik,
								      // bu interface'te bu method tanımı mutlaka yapılmalıdır yoksa ilgili controller'da 
								      // bu method çağrılamaz

		CountryModel GetItem(int id); // id parametresine göre ülkeyi dönen method tanımı
	}

	public class CountryService : ICountryService
	{
		private readonly CountryRepoBase _countryRepo;

		public CountryService(CountryRepoBase countryRepo)
		{
			_countryRepo = countryRepo;
		}

		public Result Add(CountryModel model)
		{
			throw new NotImplementedException();
		}

		public Result Delete(int id)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_countryRepo.Dispose();
		}

		public List<CountryModel> GetList()
		{
			return Query().ToList();
		}

		public CountryModel GetItem(int id) 
		{
			return Query().SingleOrDefault(c => c.Id == id);
		}

		public IQueryable<CountryModel> Query()
		{
			return _countryRepo.Query().OrderBy(c => c.Name).Select(c => new CountryModel 
			{ 
				Name = c.Name, 
				Guid = c.Guid,
				Id = c.Id
			});
		}

		public Result Update(CountryModel model)
		{
			throw new NotImplementedException();
		}
	}
}

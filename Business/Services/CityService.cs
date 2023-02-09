using AppCore.Business.Services.Bases;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Repositories;

namespace Business.Services
{
	public interface ICityService : IService<CityModel>
	{
		List<CityModel> GetList(int countryId); // gönderilen ülke id parametresine göre şehirleri ülkelere göre filtreleyerek liste olarak dönen method tanımı
		List<CityModel> GetList(); // tüm şehirleri liste olarak dönen method tanımı
		CityModel GetItem(int id); // id üzerinden şehri dönen method tanımı
	}

	public class CityService : ICityService
	{
		private readonly CityRepoBase _cityRepo;

		public CityService(CityRepoBase cityRepo)
		{
			_cityRepo = cityRepo;
		}

		public Result Add(CityModel model)
		{
			throw new NotImplementedException();
		}

		public Result Delete(int id)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_cityRepo.Dispose();
		}

		public CityModel GetItem(int id)
		{
			return Query().SingleOrDefault(c => c.Id == id);
		}

		public List<CityModel> GetList(int countryId)
		{
			return Query().Where(c => c.CountryId == countryId).ToList();
		}

		public List<CityModel> GetList()
		{
			return Query().ToList();
		}

		public IQueryable<CityModel> Query()
		{
			return _cityRepo.Query().OrderBy(c => c.Name).Select(c => new CityModel()
			{
				CountryId = c.CountryId,
				Name = c.Name,
				Guid = c.Guid,
				Id = c.Id
			});
		}

		public Result Update(CityModel model)
		{
			throw new NotImplementedException();
		}
	}
}

using AppCore.Records.Bases;
using AppCore.Results.Bases;

namespace AppCore.Business.Services.Bases
{
    // kullanıcı ile etkileşimde bulunacak model ile veritabanındaki entity dönüşümlerinin gerçekleştirildiği,
    // aynı zamanda veri formatlama ve validasyon gibi işlemlerin yapılabileceği servis class'larının interface'i.



    // public interface IService<TModel>
    // interface'in implemente edileceği class'larda herhangi bir TModel tipi üzerinden işlemlerin yapılabileceği interface.

    // public interface IService<TModel> : where TModel : class
    // interface'in implemente edileceği class'larda referans bir TModel tipi üzerinden işlemlerin yapılabileceği interface.

    // public interface IService<TModel> : where TModel : class, new()
    // interface'in implemente edileceği class'larda new'lenebilen bir referans TModel tipi üzerinden işlemlerin yapılabileceği interface.



    // new'lenebilen ve RecordBase'den miras alan tipler üzerinden işlemlerin yapılabileceği ve IDisposable interface'ini implemente edecek interface. 
    public interface IService<TModel> : IDisposable where TModel : RecordBase, new()
    {
        IQueryable<TModel> Query(); // Read işlemleri
        Result Add(TModel model); // Create işlemleri
        Result Update(TModel model); // Update işlemleri
        Result Delete(int id); // Delete işlemleri
    }
}

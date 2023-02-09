#nullable disable

namespace AppCore.Results.Bases
{
    public abstract class Result // servis class'larında çeşitli methodlardan sonuç olarak dönmek, ve başarılı ile başarısız işlem sonuçları için kullanılacak base class
    {
        public bool IsSuccessful { get; } // get; ile readonly yani sadece constructor üzerinden veya bu satırda set edilebilir
        public string Message { get; set; }

        public Result(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }
}

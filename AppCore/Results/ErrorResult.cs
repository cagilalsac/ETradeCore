using AppCore.Results.Bases;

namespace AppCore.Results
{
    public class ErrorResult : Result // servis class'larında çeşitli methodlardan başarısız olarak dönecek işlem sonucu class'ı
    {
        public ErrorResult(string message) : base(false, message) // Result class'ının constructor'ına isSuccessful parametresini false gönderiyoruz ki sonuç başarısız olsun
        {
        }

        public ErrorResult() : base(false, "") // mesaj göndermeden başarısız işlem sonucu oluşturmak için
        {
        }
    }
}

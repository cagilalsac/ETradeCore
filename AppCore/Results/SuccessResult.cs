using AppCore.Results.Bases;

namespace AppCore.Results
{
    public class SuccessResult : Result // servis class'larında çeşitli methodlardan başarılı olarak dönecek işlem sonucu class'ı
    {
        public SuccessResult(string message) : base(true, message) // Result class'ının constructor'ına isSuccessful parametresini true gönderiyoruz ki sonuç başarılı olsun
        {
        }

        public SuccessResult() : base(true, "") // mesaj göndermeden başarılı işlem sonucu oluşturmak için
        {
        }
    }
}

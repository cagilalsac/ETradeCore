namespace MvcWebUI.Settings
{
    // Program.cs'te geliştirme veya canlı ortamda uygulamanın çalıştırılmasına göre appsettings.json veya appsettings.Development.json
    // içerisindeki AppSettings section'ı (bölümü) üzerinden new'lenerek section içerisindeki özelliklerin new'lenmiş AppSettings tipindeki
    // objeye atanması ve bu class'taki atanan static özellikler üzerinden de MVC uygulamasında ihtiyaç olan herhangi bir yerde
    // (örneğin controller veya view) kullanılması sağlanır.
    // Section adı class adı ile aynı, section içerisindeki özellik adları da class özellik adları ile aynı olmalıdır.
    public class AppSettings
    {
        public static string Title { get; set; } // uygulama başlığı
        public static string AcceptedImageExtensions { get; set; } // kabul edilen imaj dosya uzantıları
        public static double AcceptedImageLength { get; set; } // Mb (mega byte), kabul edilen imaj dosya boyutu
    }
}

namespace AppCore.App
{
	public static class Environment // MvcWebUI (web uygulaması) veya WebApi (web servis) projelerinin geliştirme (development)
									// veya canlı (production) ortamda çalışma bilgisini tutan sınıf
	{
		public static bool IsDevelopment { get; set; }
	}
}

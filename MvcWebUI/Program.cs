#nullable disable

using Business.Services;
using Business.Services.Report;
using DataAccess.Contexts;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MvcWebUI.Settings;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#region Localization
// Web uygulamasýnýn bölgesel ayarý aþaðýdaki þekilde tek seferde konfigüre edilerek tüm projenin bu ayarý kullanmasý saðlanabilir,
// dolayýsýyla veri formatlama veya dönüþtürme gibi iþlemlerde her seferinde CultureInfo objesinin kullaným gereksinimi ortadan kalkar.
// Bu þekilde sadece tek bir bölgesel ayar projede kullanýlabilir.
List<CultureInfo> cultures = new List<CultureInfo>()
{
    new CultureInfo("en-US") // eðer uygulama Türkçe olacaksa CultureInfo constructor'ýnýn parametresini ("tr-TR") yapmak yeterlidir.
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name);
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
    // projeye Cookie authentication default'larýný kullanarak kimlik doðrulama ekliyoruz
	
    .AddCookie(config => 
    // oluþturulacak cookie'yi config action delegesi üzerinden konfigüre ediyoruz, action delegeleri func delegeleri gibi bir sonuç dönmez,
    // üzerlerinden burada olduðu gibi genelde konfigürasyon iþlemleri yapýlýr
	
    {
		config.LoginPath = "/Account/Users/Login"; 
        // eðer sisteme giriþ yapýlmadan bir iþlem yapýlmaya çalýþýlýrsa kullanýcýyý Account area -> Users controller -> Login action'ýna yönlendir
		
        config.AccessDeniedPath = "/Account/Users/AccessDenied"; 
        // eðer sisteme giriþ yapýldýktan sonra yetki dýþý bir iþlem yapýlmaya çalýþýlýrsa kullanýcýyý Account area -> Users controller -> AccessDenied
        // action'ýna yönlendir
		
        config.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
        // sisteme giriþ yapýldýktan sonra oluþan cookie 30 dakika boyunca kullanýlabilsin
		
        config.SlidingExpiration = true; 
        // SlidingExpiration true yapýlarak kullanýcý sistemde her iþlem yaptýðýnda cookie kullaným süresi yukarýda belirtilen 30 dakika uzatýlýr,
        // eðer false atanýrsa kullanýcýnýn cookie ömrü yukarýda belirtilen 30 dakika sonra sona erer ve yeniden giriþ yapmak zorunda kalýr
	});
#endregion

#region Session
builder.Services.AddSession(config => // projeye config action delegesi konfigürasyonlarý üzerinden session ekliyoruz
{
    config.IdleTimeout = TimeSpan.FromMinutes(30); // kullanýcýnýn web uygulamasýný kullanmadýðýnda session'ýn temizleneceði süre (30 dakika)
                                                   // default'u 20 dakika
    config.IOTimeout = Timeout.InfiniteTimeSpan; // sadece IdleTimeout'un kullanýlmasý için IOTimeout deðerini sonsuz yapýyoruz
});
#endregion

#region IoC Container : Inversion of Control Container (Baðýmlýlýklarýn Yönetimi) 
// Alternatif olarak Business katmanýnda Autofac ve Ninject gibi kütüphaneler de kullanýlabilir.

// Unable to resolve service hatalarý burada çözümlenmelidir.

// AddScoped: istek (request) boyunca objenin referansýný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak)
// bir kere oluþturulur ve yanýt (response) dönene kadar bu obje hayatta kalýr.
// AddSingleton: web uygulamasý baþladýðýnda objenin referansný (genelde interface veya abstract class) kullandýðýmýz yerde obje (somut class'tan oluþturulacak)
// bir kere oluþturulur ve uygulama çalýþtýðý (IIS üzerinden uygulama durdurulmadýðý veya yeniden baþlatýlmadýðý) sürece bu obje hayatta kalýr.
// AddTransient: istek (request) baðýmsýz ihtiyaç olan objenin referansýný (genelde interface veya abstract class) kullandýðýmýz her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullanýlýr.

string connectionString = builder.Configuration.GetConnectionString("ETradeDb"); // appsettings.json veya appsettings.Development.json dosyalarýndaki isim üzerinden atanan
                                                                                 // veritabaný baðlantý string'ini döner.

builder.Services.AddDbContext<ETradeContext>(options => options.UseSqlServer(connectionString)); // projede herhangi bir class'ta ETradeContext tipinde 
                                                                                                 // constructor injection yapýldýðýnda ETradeContext objesini new'leyerek
                                                                                                 // o class'a enjekte eder.       

builder.Services.AddScoped<ProductRepoBase, ProductRepo>(); // projede herhangi bir class'ta ProductRepoBase tipinde constructor injection yapýldýðýnda
                                                            // ProductRepo objesini new'leyerek o class'a enjekte eder.

builder.Services.AddScoped<CategoryRepoBase, CategoryRepo>(); 
builder.Services.AddScoped<StoreRepoBase, StoreRepo>(); 
builder.Services.AddScoped<UserRepoBase, UserRepo>(); 
builder.Services.AddScoped<CountryRepoBase, CountryRepo>(); 
builder.Services.AddScoped<CityRepoBase, CityRepo>(); 

builder.Services.AddScoped<IProductService, ProductService>(); // projede herhangi bir class'ta IProductService tipinde constructor injection yapýldýðýnda
                                                               // ProductService objesini new'leyerek o class'a enjekte eder.

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IReportService, ReportService>();
#endregion

#region AppSettings
// Önce appsettings.json veya isteðe göre appsettings.Development.json dosyalarýndaki AppSettings bölümü (section) alýnýr,
// elle string olarak AppSettings yazmak yerine AppSettings class'ýnýn nameof ile adýný kullanmak hata riskini ortadan kaldýrýr.
//var section = builder.Configuration.GetSection("AppSettings");
var section = builder.Configuration.GetSection(nameof(AppSettings)); 

// Sonra bu section'daki özellikler ile new'lenen AppSettings tipindeki obje özellikleri baðlanýr yani obje özellikleri doldurulur.
section.Bind(new AppSettings());
#endregion

var app = builder.Build();

#region Localization
app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
});
#endregion

// Eðer istenirse AppCore -> App -> Environment class'ý altýndaki IsDevelopment özelliði burada atanarak örneðin Views -> Shared -> _Layout.cshtml
// view'ýnda sadece development ortamý için Seed Database link'inin gelmesi saðlanabilir.
AppCore.App.Environment.IsDevelopment = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

#region Authentication
app.UseAuthentication(); // sen kimsin?
#endregion

app.UseAuthorization(); // sen iþlem için yetkili misin?

#region Session
app.UseSession();
#endregion

#region Area
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // MVC default route tanýmý

app.Run();

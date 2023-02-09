#nullable disable
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")] // Controller üzerinde tanımlandığından sadece sisteme giriş yapmış yani authentication cookie'si olanlar
                                 // ve Admin rolündekiler bu controller'ın tüm action'larını çağırabilir
    public class StoresController : Controller
    {
        // Add service injections here
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // GET: Stores
        public IActionResult Index()
        {
            List<StoreModel> storeList = _storeService.Query().ToList(); // Add get list service logic here
            return View(storeList);
        }

        // GET: Stores/Details/5
        public IActionResult Details(int id)
        {
            StoreModel store = _storeService.Query().SingleOrDefault(s => s.Id == id); // Add get item service logic here
            if (store == null)
            {
                return NotFound("Store not found!"); // eğer mağaza bulunamazsa NotFound dönüyoruz ki mesajını Index view'ındaki
                                                     // modal içerisine yazdırabilelim
            }
            return PartialView("_Details", store); // mağazayı bir partial view üzerinden dönüyoruz ki Index view'ındaki modal içerisine doldurabilelim
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return PartialView("_CreateEdit"); // yeni mağaza form'unu bir partial view üzerinden dönüyoruz ki Index view'ındaki
                                               // modal içerisine doldurabilelim
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                // Add insert service logic here
                var result = _storeService.Add(store);
                if (result.IsSuccessful)
                    return Ok(); // işlem başarılı olduğundan herhangi bir mesaj içermeyen Ok sonucunu dönüyoruz ki
                                 // Index.cshtml'deki script üzerinden Index action'ına yönlendirebilelim
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return PartialView("_CreateEdit", store); // yeni mağaza form'unu kullanıcı tarafından gönderilen store ile birlikte
                                                      // bir partial view üzerinden dönüyoruz ki Index view'ındaki modal içerisine doldurabilelim
        }

        // GET: Stores/Edit/5
        public IActionResult Edit(int id)
        {
            StoreModel store = _storeService.Query().SingleOrDefault(s => s.Id == id); // Add get item service logic here
            if (store == null)
            {
                return NotFound("Store not found!"); // eğer mağaza bulunamazsa NotFound dönüyoruz ki mesajını Index view'ındaki
                                                     // modal içerisine yazdırabilelim
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return PartialView("_CreateEdit", store); // mağaza düzenleme form'unu veritabanından çektiğimiz store ile birlikte
                                                      // bir partial view üzerinden dönüyoruz ki Index view'ındaki modal içerisine doldurabilelim
        }

        // POST: Stores/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                // Add update service logic here
                var result = _storeService.Update(store);
                if (result.IsSuccessful)
                    return Ok(); // işlem başarılı olduğundan herhangi bir mesaj içermeyen Ok sonucunu dönüyoruz ki
                                 // Index.cshtml'deki script üzerinden Index action'ına yönlendirebilelim
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return PartialView("_CreateEdit", store); // mağaza düzenleme form'unu kullanıcı tarafından gönderilen store ile birlikte
                                                      // bir partial view üzerinden dönüyoruz ki Index view'ındaki modal içerisine doldurabilelim
        }

        // GET: Stores/Delete/5
        public IActionResult Delete(int id) // Index view'ında Delete link'i için AlertifyJS üzerinden bir konfirmasyon pop up'ı göstereceğiz
                                            // ve bu pop up üzerinden silme işlemi onaylanırsa bu get action'ına mağazanın id'sini
                                            // göndererek silinmesini sağlayacağız
        {
			// Add delete service logic here
			var result = _storeService.Delete(id);
			TempData["Message"] = result.Message;
			return RedirectToAction(nameof(Index));
		}
	}
}

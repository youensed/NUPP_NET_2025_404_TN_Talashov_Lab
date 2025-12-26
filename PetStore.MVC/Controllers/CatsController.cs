using Microsoft.AspNetCore.Mvc;
using PetStore.MVC.Models;
using PetStore.MVC.Services;

namespace PetStore.MVC.Controllers
{
    public class CatsController : Controller
    {
        private readonly IPetStoreApiClient _apiClient;
        private readonly ILogger<CatsController> _logger;

        public CatsController(IPetStoreApiClient apiClient, ILogger<CatsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        // GET: Cats
        public async Task<IActionResult> Index()
        {
            try
            {
                var cats = await _apiClient.GetListAsync<CatViewModel>("api/cats");
                return View(cats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cats");
                TempData["Error"] = "Не вдалося завантажити список котів. Переконайтеся, що REST API запущено.";
                return View(new List<CatViewModel>());
            }
        }

        // GET: Cats/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var cat = await _apiClient.GetAsync<CatViewModel>($"api/cats/{id}");
                if (cat == null)
                {
                    return NotFound();
                }
                return View(cat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cat details");
                return NotFound();
            }
        }

        // GET: Cats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cats/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CatCreateViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return View(cat);
            }

            try
            {
                var success = await _apiClient.PostAsync("api/cats", cat);
                if (success)
                {
                    TempData["Success"] = "Кота успішно додано!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося створити кота");
                    return View(cat);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cat");
                ModelState.AddModelError("", "Помилка при створенні кота");
                return View(cat);
            }
        }

        // GET: Cats/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var cat = await _apiClient.GetAsync<CatViewModel>($"api/cats/{id}");
                if (cat == null)
                {
                    return NotFound();
                }
                return View(cat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cat for edit");
                return NotFound();
            }
        }

        // POST: Cats/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CatViewModel cat)
        {
            if (id != cat.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(cat);
            }

            try
            {
                var success = await _apiClient.PutAsync($"api/cats/{id}", cat);
                if (success)
                {
                    TempData["Success"] = "Кота успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося оновити кота");
                    return View(cat);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cat");
                ModelState.AddModelError("", "Помилка при оновленні кота");
                return View(cat);
            }
        }

        // GET: Cats/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var cat = await _apiClient.GetAsync<CatViewModel>($"api/cats/{id}");
                if (cat == null)
                {
                    return NotFound();
                }
                return View(cat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cat for delete");
                return NotFound();
            }
        }

        // POST: Cats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiClient.DeleteAsync($"api/cats/{id}");
                if (success)
                {
                    TempData["Success"] = "Кота успішно видалено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Не вдалося видалити кота";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cat");
                TempData["Error"] = "Помилка при видаленні кота";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


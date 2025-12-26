using Microsoft.AspNetCore.Mvc;
using PetStore.MVC.Models;
using PetStore.MVC.Services;

namespace PetStore.MVC.Controllers
{
    public class DogsController : Controller
    {
        private readonly IPetStoreApiClient _apiClient;
        private readonly ILogger<DogsController> _logger;

        public DogsController(IPetStoreApiClient apiClient, ILogger<DogsController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        // GET: Dogs
        public async Task<IActionResult> Index()
        {
            try
            {
                var dogs = await _apiClient.GetListAsync<DogViewModel>("api/dogs");
                return View(dogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dogs");
                TempData["Error"] = "Не вдалося завантажити список собак. Переконайтеся, що REST API запущено.";
                return View(new List<DogViewModel>());
            }
        }

        // GET: Dogs/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var dog = await _apiClient.GetAsync<DogViewModel>($"api/dogs/{id}");
                if (dog == null)
                {
                    return NotFound();
                }
                return View(dog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dog details");
                return NotFound();
            }
        }

        // GET: Dogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DogCreateViewModel dog)
        {
            if (!ModelState.IsValid)
            {
                return View(dog);
            }

            try
            {
                var success = await _apiClient.PostAsync("api/dogs", dog);
                if (success)
                {
                    TempData["Success"] = "Собаку успішно додано!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося створити собаку");
                    return View(dog);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dog");
                ModelState.AddModelError("", "Помилка при створенні собаки");
                return View(dog);
            }
        }

        // GET: Dogs/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var dog = await _apiClient.GetAsync<DogViewModel>($"api/dogs/{id}");
                if (dog == null)
                {
                    return NotFound();
                }
                return View(dog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dog for edit");
                return NotFound();
            }
        }

        // POST: Dogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DogViewModel dog)
        {
            if (id != dog.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dog);
            }

            try
            {
                var success = await _apiClient.PutAsync($"api/dogs/{id}", dog);
                if (success)
                {
                    TempData["Success"] = "Собаку успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося оновити собаку");
                    return View(dog);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dog");
                ModelState.AddModelError("", "Помилка при оновленні собаки");
                return View(dog);
            }
        }

        // GET: Dogs/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dog = await _apiClient.GetAsync<DogViewModel>($"api/dogs/{id}");
                if (dog == null)
                {
                    return NotFound();
                }
                return View(dog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dog for delete");
                return NotFound();
            }
        }

        // POST: Dogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiClient.DeleteAsync($"api/dogs/{id}");
                if (success)
                {
                    TempData["Success"] = "Собаку успішно видалено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Не вдалося видалити собаку";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dog");
                TempData["Error"] = "Помилка при видаленні собаки";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using PetStore.MVC.Models;
using PetStore.MVC.Services;

namespace PetStore.MVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IPetStoreApiClient _apiClient;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(IPetStoreApiClient apiClient, ILogger<CustomersController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _apiClient.GetListAsync<CustomerViewModel>("api/customers");
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers");
                TempData["Error"] = "Не вдалося завантажити список клієнтів. Переконайтеся, що REST API запущено.";
                return View(new List<CustomerViewModel>());
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var customer = await _apiClient.GetAsync<CustomerViewModel>($"api/customers/{id}");
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer details");
                return NotFound();
            }
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                var success = await _apiClient.PostAsync("api/customers", customer);
                if (success)
                {
                    TempData["Success"] = "Клієнта успішно додано!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося створити клієнта");
                    return View(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                ModelState.AddModelError("", "Помилка при створенні клієнта");
                return View(customer);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var customer = await _apiClient.GetAsync<CustomerViewModel>($"api/customers/{id}");
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer for edit");
                return NotFound();
            }
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerViewModel customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                var success = await _apiClient.PutAsync($"api/customers/{id}", customer);
                if (success)
                {
                    TempData["Success"] = "Клієнта успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Не вдалося оновити клієнта");
                    return View(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                ModelState.AddModelError("", "Помилка при оновленні клієнта");
                return View(customer);
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var customer = await _apiClient.GetAsync<CustomerViewModel>($"api/customers/{id}");
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer for delete");
                return NotFound();
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiClient.DeleteAsync($"api/customers/{id}");
                if (success)
                {
                    TempData["Success"] = "Клієнта успішно видалено!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Не вдалося видалити клієнта";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                TempData["Error"] = "Помилка при видаленні клієнта";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


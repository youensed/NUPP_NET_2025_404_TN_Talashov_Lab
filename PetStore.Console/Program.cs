using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetStore.Infrastructure;
using PetStore.Infrastructure.Models;
using PetStore.Infrastructure.Repository;
using PetStore.Common.Services;

namespace PetStore.ConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №3 ===");
            Console.WriteLine("Робота із базами даних. Entity Framework Core. PostgreSQL.\n");

            // PostgreSQL connection string
            string connectionString = "Host=localhost;Database=PetStoreDB;Username=postgres;Password=b27g12dan";
            
            try
            {
                // Initialize context
                var context = new PetStoreContext(connectionString);
                Console.WriteLine("Підключення до PostgreSQL...");
                
                // Ensure database is created and migrations are applied
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("База даних готова!\n");

                // Create repositories
                var dogRepository = new EfCoreRepository<DogModel>(context);
                var catRepository = new EfCoreRepository<CatModel>(context);
                var customerRepository = new EfCoreRepository<CustomerModel>(context);
                var vaccineRepository = new EfCoreRepository<VaccineModel>(context);

                // Create adapters for CRUD services
                var dogAdapter = new RepositoryAdapter<DogModel>(dogRepository);
                var catAdapter = new RepositoryAdapter<CatModel>(catRepository);
                var customerAdapter = new RepositoryAdapter<CustomerModel>(customerRepository);
                var vaccineAdapter = new RepositoryAdapter<VaccineModel>(vaccineRepository);

                // Create CRUD services
                var dogService = new CrudServiceAsyncRepository<DogModel>(dogAdapter);
                var catService = new CrudServiceAsyncRepository<CatModel>(catAdapter);
                var customerService = new CrudServiceAsyncRepository<CustomerModel>(customerAdapter);
                var vaccineService = new CrudServiceAsyncRepository<VaccineModel>(vaccineAdapter);

                // Clear existing data for demo
                Console.WriteLine("Очищення попередніх даних...");
                var existingDogs = await dogService.ReadAllAsync();
                foreach (var dog in existingDogs)
                {
                    await dogService.RemoveAsync(dog);
                }
                var existingCats = await catService.ReadAllAsync();
                foreach (var cat in existingCats)
                {
                    await catService.RemoveAsync(cat);
                }
                var existingCustomers = await customerService.ReadAllAsync();
                foreach (var customer in existingCustomers)
                {
                    await customerService.RemoveAsync(customer);
                }
                var existingVaccines = await vaccineService.ReadAllAsync();
                foreach (var vaccine in existingVaccines)
                {
                    await vaccineService.RemoveAsync(vaccine);
                }
                Console.WriteLine("Дані очищено\n");

                // ===== CRUD Operations Demo =====
                Console.WriteLine("=== Демонстрація CRUD операцій ===\n");

                // CREATE: Add customers first (for foreign key relationships)
                Console.WriteLine("1. CREATE - Створення клієнтів:");
                var customer1 = new CustomerModel("Іван Петренко", 30);
                var customer2 = new CustomerModel("Марія Коваленко", 25);
                
                await customerService.CreateAsync(customer1);
                await customerService.CreateAsync(customer2);
                Console.WriteLine($"    Додано клієнта: {customer1.Name} (ID: {customer1.Id})");
                Console.WriteLine($"    Додано клієнта: {customer2.Name} (ID: {customer2.Id})\n");

                // CREATE: Add dogs
                Console.WriteLine("2. CREATE - Додавання собак:");
                var dog1 = new DogModel("Бобік", 3, "Лабрадор", true);
                var dog2 = new DogModel("Рекс", 5, "Овчарка", true);
                var dog3 = new DogModel("Макс", 2, "Хаскі", false);
                
                await dogService.CreateAsync(dog1);
                await dogService.CreateAsync(dog2);
                await dogService.CreateAsync(dog3);
                Console.WriteLine($"    Додано: {dog1.Name} ({dog1.Breed}) - ID: {dog1.Id}");
                Console.WriteLine($"    Додано: {dog2.Name} ({dog2.Breed}) - ID: {dog2.Id}");
                Console.WriteLine($"    Додано: {dog3.Name} ({dog3.Breed}) - ID: {dog3.Id}\n");

                // CREATE: Add cats
                Console.WriteLine("3. CREATE - Додавання котів:");
                var cat1 = new CatModel("Мурка", 4, "Сірий", true);
                var cat2 = new CatModel("Сніжок", 2, "Білий", true);
                
                await catService.CreateAsync(cat1);
                await catService.CreateAsync(cat2);
                Console.WriteLine($"    Додано: {cat1.Name} ({cat1.Color}) - ID: {cat1.Id}");
                Console.WriteLine($"    Додано: {cat2.Name} ({cat2.Color}) - ID: {cat2.Id}\n");

                // READ: Get all dogs
                Console.WriteLine("4. READ - Читання всіх собак:");
                var allDogs = await dogService.ReadAllAsync();
                foreach (var dog in allDogs)
                {
                    Console.WriteLine($"   - {dog.Name}, {dog.Age} років, порода: {dog.Breed}, тренований: {(dog.IsTrained ? "Так" : "Ні")}");
                }
                Console.WriteLine();

                // READ: Get dog by ID
                Console.WriteLine("5. READ BY ID - Читання собаки за ID:");
                var foundDog = await dogService.ReadAsync(dog1.Id);
                if (foundDog != null)
                {
                    Console.WriteLine($"   Знайдено: {foundDog.Name} (ID: {foundDog.Id})\n");
                }

                // UPDATE: Update dog
                Console.WriteLine("6. UPDATE - Оновлення даних собаки:");
                dog1.Age = 4;
                dog1.IsTrained = true;
                await dogService.UpdateAsync(dog1);
                var updatedDog = await dogService.ReadAsync(dog1.Id);
                Console.WriteLine($"    Оновлено: {updatedDog.Name}, новий вік: {updatedDog.Age}\n");

                // ===== Relationships Demo =====
                Console.WriteLine("=== Демонстрація зв'язків між сутностями ===\n");

                // ONE-TO-MANY: Assign pets to customers
                Console.WriteLine("7. ONE-TO-MANY зв'язок (Клієнт → Багато тварин):");
                dog1.OwnerId = customer1.Id;
                await dogService.UpdateAsync(dog1);
                cat1.OwnerId = customer1.Id;
                await catService.UpdateAsync(cat1);
                dog2.OwnerId = customer2.Id;
                await dogService.UpdateAsync(dog2);
                Console.WriteLine($"    {dog1.Name} тепер належить {customer1.Name}");
                Console.WriteLine($"    {cat1.Name} тепер належить {customer1.Name}");
                Console.WriteLine($"    {dog2.Name} тепер належить {customer2.Name}\n");

                // Query relationships with Include (EF Core feature)
                Console.WriteLine("8. Запит зв'язків - Тварини клієнта (з Include):");
                var customerWithPets = await context.Customers
                    .Include(c => c.Pets)
                    .FirstOrDefaultAsync(c => c.Id == customer1.Id);
                
                if (customerWithPets != null)
                {
                    Console.WriteLine($"   Клієнт: {customerWithPets.Name}");
                    Console.WriteLine($"   Тварини клієнта: {customerWithPets.Pets.Count}");
                    foreach (var pet in customerWithPets.Pets)
                    {
                        if (pet is DogModel dogPet)
                        {
                            Console.WriteLine($"      - Собака: {dogPet.Name} ({dogPet.Breed})");
                        }
                        else if (pet is CatModel catPet)
                        {
                            Console.WriteLine($"      - Кіт: {catPet.Name} ({catPet.Color})");
                        }
                    }
                }
                Console.WriteLine();

                // MANY-TO-MANY: Create vaccines and assign to pets
                Console.WriteLine("9. MANY-TO-MANY зв'язок (Тварини ↔ Вакцини):");
                var vaccine1 = new VaccineModel("Сказ", "Вакцина проти сказу");
                var vaccine2 = new VaccineModel("Чумка", "Вакцина проти чумки");
                var vaccine3 = new VaccineModel("Лептоспіроз", "Вакцина проти лептоспірозу");
                
                await vaccineService.CreateAsync(vaccine1);
                await vaccineService.CreateAsync(vaccine2);
                await vaccineService.CreateAsync(vaccine3);
                Console.WriteLine($"    Створено вакцину: {vaccine1.Name}");
                Console.WriteLine($"    Створено вакцину: {vaccine2.Name}");
                Console.WriteLine($"    Створено вакцину: {vaccine3.Name}\n");

                // Assign vaccines to pets using EF Core navigation properties
                Console.WriteLine("10. Призначення вакцин тваринам:");
                var dog1FromDb = await context.Dogs
                    .Include(d => d.Vaccines)
                    .FirstOrDefaultAsync(d => d.Id == dog1.Id);
                var dog2FromDb = await context.Dogs
                    .Include(d => d.Vaccines)
                    .FirstOrDefaultAsync(d => d.Id == dog2.Id);
                var cat1FromDb = await context.Cats
                    .Include(c => c.Vaccines)
                    .FirstOrDefaultAsync(c => c.Id == cat1.Id);

                if (dog1FromDb != null)
                {
                    dog1FromDb.Vaccines.Add(vaccine1);
                    dog1FromDb.Vaccines.Add(vaccine2);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"    {dog1FromDb.Name} отримав вакцини: {vaccine1.Name}, {vaccine2.Name}");
                }

                if (dog2FromDb != null)
                {
                    dog2FromDb.Vaccines.Add(vaccine1);
                    dog2FromDb.Vaccines.Add(vaccine3);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"    {dog2FromDb.Name} отримав вакцини: {vaccine1.Name}, {vaccine3.Name}");
                }

                if (cat1FromDb != null)
                {
                    cat1FromDb.Vaccines.Add(vaccine1);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"    {cat1FromDb.Name} отримав вакцину: {vaccine1.Name}");
                }
                Console.WriteLine();

                // Query many-to-many relationships
                Console.WriteLine("11. Запит багато-до-багатьох зв'язків:");
                var dogsWithVaccines = await context.Dogs
                    .Include(d => d.Vaccines)
                    .ToListAsync();
                
                foreach (var dog in dogsWithVaccines)
                {
                    if (dog.Vaccines.Any())
                    {
                        Console.WriteLine($"   {dog.Name} має вакцини:");
                        foreach (var vaccine in dog.Vaccines)
                        {
                            Console.WriteLine($"      - {vaccine.Name}");
                        }
                    }
                }
                Console.WriteLine();

                // Query vaccines with pets
                Console.WriteLine("12. Вакцини та їх застосування:");
                var vaccinesWithPets = await context.Vaccines
                    .Include(v => v.Pets)
                    .ToListAsync();
                
                foreach (var vaccine in vaccinesWithPets)
                {
                    Console.WriteLine($"   {vaccine.Name}: застосовано для {vaccine.Pets.Count} тварин");
                }
                Console.WriteLine();

                // DELETE: Remove a pet
                Console.WriteLine("13. DELETE - Видалення тварини:");
                await dogService.RemoveAsync(dog3);
                Console.WriteLine($"    Видалено: {dog3.Name}\n");

                // Pagination demo
                Console.WriteLine("14. Пагінація (перша сторінка, 2 елементи):");
                var page0 = await dogService.ReadAllAsync(0, 2);
                foreach (var dog in page0)
                {
                    Console.WriteLine($"   - {dog.Name} ({dog.Breed})");
                }
                Console.WriteLine();

                // Statistics with LINQ
                Console.WriteLine("=== Статистика (LINQ) ===");
                var allDogsForStats = (await dogService.ReadAllAsync()).ToList();
                if (allDogsForStats.Any())
                {
                    Console.WriteLine($"Загальна кількість собак: {allDogsForStats.Count}");
                    Console.WriteLine($"Середній вік: {allDogsForStats.Average(d => d.Age):F2} років");
                    Console.WriteLine($"Тренованих собак: {allDogsForStats.Count(d => d.IsTrained)}");
                    
                    var breedGroups = allDogsForStats.GroupBy(d => d.Breed)
                        .Select(g => new { Breed = g.Key, Count = g.Count() });
                    Console.WriteLine("Собаки по породах:");
                    foreach (var group in breedGroups)
                    {
                        Console.WriteLine($"   {group.Breed}: {group.Count}");
                    }
                }
                Console.WriteLine();

                // Table-per-Type (TPT) demonstration
                Console.WriteLine("=== Демонстрація Table-per-Type (TPT) ===");
                var allPets = await context.Pets.ToListAsync();
                Console.WriteLine($"Всього тварин у базі: {allPets.Count}");
                Console.WriteLine($"  - Собак: {allPets.OfType<DogModel>().Count()}");
                Console.WriteLine($"  - Котів: {allPets.OfType<CatModel>().Count()}");
                Console.WriteLine();

                Console.WriteLine(" Програма успішно завершена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
                Console.WriteLine($"Деталі: {ex.InnerException?.Message}");
                Console.WriteLine("\nПереконайтеся, що PostgreSQL запущено на localhost:5432");
                Console.WriteLine("Перевірте правильність логіну та пароля у рядку підключення");
                Console.WriteLine("\nДля створення бази даних виконайте:");
                Console.WriteLine("  dotnet ef migrations add InitialCreate --project PetStore.Infrastructure");
                Console.WriteLine("  dotnet ef database update --project PetStore.Infrastructure");
            }
        }
    }
}

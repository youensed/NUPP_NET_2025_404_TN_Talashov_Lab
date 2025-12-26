using System;
using System.Linq;
using System.Threading.Tasks;
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
            Console.WriteLine("Робота із базами даних. MongoDB. Шаблон Репозиторій.\n");

            // MongoDB connection string (localhost)
            string connectionString = "mongodb+srv://specialforbohdan:1b2g3d4n@cluster0.n1d9gpq.mongodb.net/";
            
            try
            {
                // Initialize context
                var context = new PetStoreContext(connectionString);
                Console.WriteLine("Підключення до MongoDB успішне!\n");

                // Create repositories
                var dogRepository = new MongoRepository<DogModel>(context.Dogs);
                var catRepository = new MongoRepository<CatModel>(context.Cats);
                var customerRepository = new MongoRepository<CustomerModel>(context.Customers);

                // Create adapters for CRUD services
                var dogAdapter = new RepositoryAdapter<DogModel>(dogRepository);
                var catAdapter = new RepositoryAdapter<CatModel>(catRepository);
                var customerAdapter = new RepositoryAdapter<CustomerModel>(customerRepository);

                // Create CRUD services
                var dogService = new CrudServiceAsyncRepository<DogModel>(dogAdapter);
                var catService = new CrudServiceAsyncRepository<CatModel>(catAdapter);
                var customerService = new CrudServiceAsyncRepository<CustomerModel>(customerAdapter);

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
                Console.WriteLine(" Дані очищено\n");

                // ===== CRUD Operations Demo =====
                Console.WriteLine("=== Демонстрація CRUD операцій ===\n");

                // CREATE: Add dogs
                Console.WriteLine("1. CREATE - Додавання собак:");
                var dog1 = new DogModel("Бобік", 3, "Лабрадор", true);
                var dog2 = new DogModel("Рекс", 5, "Овчарка", true);
                var dog3 = new DogModel("Макс", 2, "Хаскі", false);
                
                await dogService.CreateAsync(dog1);
                await dogService.CreateAsync(dog2);
                await dogService.CreateAsync(dog3);
                Console.WriteLine($"    Додано: {dog1.Name} ({dog1.Breed})");
                Console.WriteLine($"    Додано: {dog2.Name} ({dog2.Breed})");
                Console.WriteLine($"    Додано: {dog3.Name} ({dog3.Breed})\n");

                // CREATE: Add cats
                Console.WriteLine("   Додавання котів:");
                var cat1 = new CatModel("Мурка", 4, "Сірий", true);
                var cat2 = new CatModel("Сніжок", 2, "Білий", true);
                
                await catService.CreateAsync(cat1);
                await catService.CreateAsync(cat2);
                Console.WriteLine($"    Додано: {cat1.Name} ({cat1.Color})");
                Console.WriteLine($"    Додано: {cat2.Name} ({cat2.Color})\n");

                // READ: Get all dogs
                Console.WriteLine("2. READ - Читання всіх собак:");
                var allDogs = await dogService.ReadAllAsync();
                foreach (var dog in allDogs)
                {
                    Console.WriteLine($"   - {dog.Name}, {dog.Age} років, порода: {dog.Breed}, тренований: {(dog.IsTrained ? "Так" : "Ні")}");
                }
                Console.WriteLine();

                // READ: Get dog by ID
                Console.WriteLine("3. READ BY ID - Читання собаки за ID:");
                var foundDog = await dogService.ReadAsync(dog1.Id);
                if (foundDog != null)
                {
                    Console.WriteLine($"   Знайдено: {foundDog.Name} (ID: {foundDog.Id})\n");
                }

                // UPDATE: Update dog
                Console.WriteLine("4. UPDATE - Оновлення даних собаки:");
                dog1.Age = 4;
                dog1.IsTrained = true;
                await dogService.UpdateAsync(dog1);
                var updatedDog = await dogService.ReadAsync(dog1.Id);
                Console.WriteLine($"    Оновлено: {updatedDog.Name}, новий вік: {updatedDog.Age}\n");

                // ===== Relationships Demo =====
                Console.WriteLine("=== Демонстрація зв'язків між сутностями ===\n");

                // Create customers
                Console.WriteLine("5. Створення клієнтів:");
                var customer1 = new CustomerModel("Іван Петренко", 30);
                var customer2 = new CustomerModel("Марія Коваленко", 25);
                
                await customerService.CreateAsync(customer1);
                await customerService.CreateAsync(customer2);
                Console.WriteLine($"    Додано клієнта: {customer1.Name}");
                Console.WriteLine($"    Додано клієнта: {customer2.Name}\n");

                // ONE-TO-ONE: Assign owner to pet
                Console.WriteLine("6. ONE-TO-ONE зв'язок (Власник → Тварина):");
                dog1.OwnerId = customer1.Id;
                await dogService.UpdateAsync(dog1);
                cat1.OwnerId = customer1.Id;
                await catService.UpdateAsync(cat1);
                dog2.OwnerId = customer2.Id;
                await dogService.UpdateAsync(dog2);
                Console.WriteLine($"    {dog1.Name} тепер належить {customer1.Name}");
                Console.WriteLine($"    {cat1.Name} тепер належить {customer1.Name}");
                Console.WriteLine($"    {dog2.Name} тепер належить {customer2.Name}\n");

                // ONE-TO-MANY: Customer has multiple pets
                Console.WriteLine("7. ONE-TO-MANY зв'язок (Клієнт → Багато тварин):");
                customer1.PetIds.Add(dog1.Id);
                customer1.PetIds.Add(cat1.Id);
                await customerService.UpdateAsync(customer1);
                
                customer2.PetIds.Add(dog2.Id);
                await customerService.UpdateAsync(customer2);
                
                Console.WriteLine($"    {customer1.Name} має {customer1.PetIds.Count} тварин");
                Console.WriteLine($"    {customer2.Name} має {customer2.PetIds.Count} тварин\n");

                // Query relationships
                Console.WriteLine("8. Запит зв'язків - Тварини клієнта:");
                var customerWithPets = await customerService.ReadAsync(customer1.Id);
                Console.WriteLine($"   Клієнт: {customerWithPets.Name}");
                Console.WriteLine($"   Тварини клієнта:");
                
                foreach (var petId in customerWithPets.PetIds)
                {
                    var dog = await dogService.ReadAsync(petId);
                    if (dog != null)
                    {
                        Console.WriteLine($"      - Собака: {dog.Name} ({dog.Breed})");
                    }
                    else
                    {
                        var cat = await catService.ReadAsync(petId);
                        if (cat != null)
                        {
                            Console.WriteLine($"      - Кіт: {cat.Name} ({cat.Color})");
                        }
                    }
                }
                Console.WriteLine();

                // DELETE: Remove a pet
                Console.WriteLine("9. DELETE - Видалення тварини:");
                await dogService.RemoveAsync(dog3);
                Console.WriteLine($"    Видалено: {dog3.Name}\n");

                // Pagination demo
                Console.WriteLine("10. Пагінація (перша сторінка, 2 елементи):");
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

                Console.WriteLine(" Програма успішно завершена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
                Console.WriteLine("\nПереконайтеся, що MongoDB запущено на localhost:27017");
                Console.WriteLine("Для запуску MongoDB використайте: mongod");
            }
        }
    }
}

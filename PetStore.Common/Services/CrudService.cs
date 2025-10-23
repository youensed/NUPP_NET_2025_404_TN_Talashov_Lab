using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace PetStore.Common.Services
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private readonly List<T> _storage = new();

        public void Create(T element) => _storage.Add(element);

        public T Read(Guid id)
        {
            var prop = typeof(T).GetProperty("Id");
            return _storage.FirstOrDefault(x => (Guid)prop.GetValue(x)! == id)!;
        }

        public IEnumerable<T> ReadAll() => _storage;

        public void Update(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            var id = (Guid)prop.GetValue(element)!;
            var existing = Read(id);
            if (existing != null)
            {
                _storage.Remove(existing);
                _storage.Add(element);
            }
        }

        public void Remove(T element) => _storage.Remove(element);

        // 🔹 Додаткове завдання: Збереження даних у файл
        public void Save(string filePath)
        {
            var json = JsonSerializer.Serialize(_storage, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // 🔹 Додаткове завдання: Завантаження даних із файлу
        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не знайдено", filePath);

            var json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);

            if (items != null)
            {
                _storage.Clear();
                _storage.AddRange(items);
            }
        }
    }
}

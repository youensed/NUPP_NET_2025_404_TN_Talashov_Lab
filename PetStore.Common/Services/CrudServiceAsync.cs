using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PetStore.Common.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _storage = new();
        private readonly SemaphoreSlim _fileSemaphore = new(1, 1);
        private readonly string _filePath;

        public CrudServiceAsync(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<bool> CreateAsync(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) return false;

            var id = (Guid)prop.GetValue(element)!;
            return _storage.TryAdd(id, element);
        }

        public async Task<T> ReadAsync(Guid id)
        {
            _storage.TryGetValue(id, out var element);
            return await Task.FromResult(element);
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await Task.FromResult(_storage.Values.ToList());
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var skip = page * amount;
            var result = _storage.Values.Skip(skip).Take(amount).ToList();
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdateAsync(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) return false;

            var id = (Guid)prop.GetValue(element)!;
            
            if (_storage.ContainsKey(id))
            {
                _storage[id] = element;
                return await Task.FromResult(true);
            }
            
            return await Task.FromResult(false);
        }

        public async Task<bool> RemoveAsync(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) return false;

            var id = (Guid)prop.GetValue(element)!;
            return await Task.FromResult(_storage.TryRemove(id, out _));
        }

        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();
            try
            {
                var items = _storage.Values.ToList();
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        public async Task<bool> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return false;

            await _fileSemaphore.WaitAsync();
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var items = JsonSerializer.Deserialize<List<T>>(json);

                if (items != null)
                {
                    _storage.Clear();
                    var prop = typeof(T).GetProperty("Id");
                    foreach (var item in items)
                    {
                        var id = (Guid)prop.GetValue(item)!;
                        _storage.TryAdd(id, item);
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        // IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetStore.Common.Services
{
    // Repository-based implementation of ICrudServiceAsync
    public class CrudServiceAsyncRepository<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly IRepositoryAdapter<T> _repository;

        public CrudServiceAsyncRepository(IRepositoryAdapter<T> repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateAsync(T element)
        {
            try
            {
                await _repository.AddAsync(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var all = await _repository.GetAllAsync();
            var skip = page * amount;
            return all.Skip(skip).Take(amount).ToList();
        }

        public async Task<bool> UpdateAsync(T element)
        {
            try
            {
                await _repository.UpdateAsync(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(T element)
        {
            try
            {
                await _repository.DeleteAsync(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            // For database-backed repositories, save is implicit
            return await Task.FromResult(true);
        }

        // IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator()
        {
            var all = _repository.GetAllAsync().GetAwaiter().GetResult();
            return all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // Adapter interface to bridge between our repository and the service
    public interface IRepositoryAdapter<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}


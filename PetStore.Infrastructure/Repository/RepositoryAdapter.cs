using PetStore.Common.Services;

namespace PetStore.Infrastructure.Repository
{
    // Adapter to make EfCoreRepository compatible with CrudServiceAsync
    public class RepositoryAdapter<T> : IRepositoryAdapter<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public RepositoryAdapter(IRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }
    }
}


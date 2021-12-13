using BookStore.Data;

namespace BookStore.Services
{
    public abstract class ServiceBase<T>
    {
        protected IRepository<T> Repository { get; set; }

        protected ServiceBase(IRepository<T> repository)
        {
            Repository = repository;
        }

        public IQueryable<T> AsQueryable()
        {
            return Repository.AsQueryable();
        }

        public Task InsertAsync(T entity)
        {
            return Repository.InsertAsync(entity);
        }

        public Task UpdateAsync(object id, T entity)
        {
            return Repository.UpdateAsync(id, entity);
        }

        public Task PartialUpdateAsync(object id, Action<IPartialUpdateBuilder<T>> builder)
        {
            return Repository.PartialUpdateAsync(id, builder);
        }

        public Task DeleteAsync(object id)
        {
            return Repository.DeleteAsync(id);
        }
    }
}

using System.Linq.Expressions;

namespace BookStore.Data
{
    public interface IRepository<T>
    {
        IQueryable<T> AsQueryable();

        Task<T> FindByIdAsync(object id);

        Task<T> FindOneAsync(Expression<Func<T, bool>> filter);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        Task InsertAsync(T entity);

        Task UpdateAsync(object id, T entity);

        Task PartialUpdateAsync(object id, Action<IPartialUpdateBuilder<T>> builder);

        Task UpsertAsync(Expression<Func<T, bool>> filter, T entity);

        Task DeleteAsync(object id);

        Task DeleteManyAsync(Expression<Func<T, bool>> filter);

        Task IncrementAsync<TField>(object id, Expression<Func<T, TField>> field, TField value);

        Task IncrementAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value);
    }

    public interface IPartialUpdateBuilder<TDocument>
    {
        void SetField<TField>(Expression<Func<TDocument, TField>> field, TField fieldValue);
    }
}

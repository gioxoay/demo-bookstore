using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace BookStore.Data
{
    public class MongoDBRepository<T> : IRepository<T>
    {
        private readonly IMongoCollection<T> collection;

        public MongoDBRepository(IMongoCollection<T> collection)
        {
            this.collection = collection;
        }

        public IQueryable<T> AsQueryable()
        {
            return collection.AsQueryable();
        }

        public async Task<T> FindByIdAsync(object id)
        {
            FilterDefinition<T> filter;

            if (id is string idString && ObjectId.TryParse(idString, out ObjectId idObject))
            {
                filter = Builders<T>.Filter.Eq("_id", idObject);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> filter)
        {
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await collection.Find(filter).AnyAsync();
        }

        public Task InsertAsync(T entity)
        {
            return collection.InsertOneAsync(entity);
        }

        public Task UpdateAsync(object id, T entity)
        {
            FilterDefinition<T> filter;

            if (id is string idString && ObjectId.TryParse(idString, out ObjectId idObject))
            {
                filter = Builders<T>.Filter.Eq("_id", idObject);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            return collection.FindOneAndReplaceAsync(filter, entity);
        }

        public async Task PartialUpdateAsync(object id, Action<IPartialUpdateBuilder<T>> builder)
        {
            var update = Builders<T>.Update;
            var updates = new List<UpdateDefinition<T>>();
            builder(new PartialUpdateBuilder(update, updates));

            if (updates.Count > 0)
            {
                FilterDefinition<T> filter;

                if (id is string idString && ObjectId.TryParse(idString, out ObjectId idObject))
                {
                    filter = Builders<T>.Filter.Eq("_id", idObject);
                }
                else
                {
                    filter = Builders<T>.Filter.Eq("_id", id);
                }

                await collection.UpdateOneAsync(filter, update.Combine(updates));
            }
        }

        public async Task DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            await collection.DeleteManyAsync(filter);
        }

        public async Task DeleteAsync(object id)
        {
            FilterDefinition<T> filter;

            if (id is string idString && ObjectId.TryParse(idString, out ObjectId idObject))
            {
                filter = Builders<T>.Filter.Eq("_id", idObject);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            await collection.FindOneAndDeleteAsync(filter);
        }

        public async Task IncrementAsync<TField>(object id, Expression<Func<T, TField>> field, TField value)
        {
            FilterDefinition<T> filter;

            if (id is string idString && ObjectId.TryParse(idString, out ObjectId idObject))
            {
                filter = Builders<T>.Filter.Eq("_id", idObject);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            var update = Builders<T>.Update.Inc(field, value);
            await collection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task IncrementAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value)
        {
            var update = Builders<T>.Update.Inc(field, value);
            await collection.UpdateManyAsync(filter, update);
        }

        private class PartialUpdateBuilder : IPartialUpdateBuilder<T>
        {
            private readonly UpdateDefinitionBuilder<T> updateDefinition;
            private readonly List<UpdateDefinition<T>> updates;

            public PartialUpdateBuilder(UpdateDefinitionBuilder<T> updateDefinition, List<UpdateDefinition<T>> updates)
            {
                this.updateDefinition = updateDefinition;
                this.updates = updates;
            }

            public void SetField<TField>(System.Linq.Expressions.Expression<Func<T, TField>> field, TField fieldValue)
            {
                updates.Add(updateDefinition.Set(field, fieldValue));
            }
        }
    }
}

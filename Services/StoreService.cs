using BookStore.Data;
using BookStore.Data.Domain;

namespace BookStore.Services
{
    public class StoreService : ServiceBase<Store>
    {
        public StoreService(IRepository<Store> repository) : base(repository)
        {
        }
    }
}

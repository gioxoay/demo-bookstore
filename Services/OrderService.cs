using BookStore.Data;
using BookStore.Data.Domain;

namespace BookStore.Services
{
    public class OrderService : ServiceBase<Order>
    {
        public OrderService(IRepository<Order> repository) : base(repository)
        {
        }
    }
}

using BookStore.Data;
using BookStore.Data.Domain;
using BookStore.Models;
using BookStore.Models.Dto;

namespace BookStore.Services
{
    public class BookService : ServiceBase<Book>
    {
        private IRepository<Book> bookRepository;
        private IRepository<Store> storeRepository;
        private IRepository<BookInStore> bookInStoreRepository;
        private IRepository<Order> orderRepository;

        public BookService(IRepository<Book> bookRepository,
        IRepository<Store> storeRepository,
        IRepository<BookInStore> bookInStoreRepository,
        IRepository<Order> orderRepository) : base(bookRepository)
        {
            this.bookRepository = bookRepository;
            this.storeRepository = storeRepository;
            this.bookInStoreRepository = bookInStoreRepository;
            this.orderRepository = orderRepository;
        }

        public List<BookDto> GetBooks(BookParameters parameters)
        {
            var bookQuery = bookRepository.AsQueryable();
            var bookInStoreQuery = bookInStoreRepository.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                var lName = parameters.Name.ToLowerInvariant().Trim();
                bookQuery = bookQuery.Where(x => x.Name.ToLowerInvariant().Contains(lName));
            }

            var query = from b in bookQuery
                        join s in bookInStoreQuery on b.Id equals s.BookId into booksInStore
                        where booksInStore.Any()
                        select new BookDto
                        {
                            Name = b.Name,
                            Description = b.Description,
                            ISBN = b.ISBN,
                            Author = b.Author,
                            MinPrice = booksInStore.Min(x => x.Price),
                            MaxPrice = booksInStore.Max(x => x.Price),
                            QuantityInStocks = booksInStore.Sum(x => x.Quantity)
                        };

            return query.ToList();
        }

        public BookDto? GetBook(string isbn)
        {
            var bookQuery = bookRepository.AsQueryable().Where(x => x.ISBN == isbn);
            var storeQuery = bookInStoreRepository.AsQueryable();
            var bookInStoreQuery = bookInStoreRepository.AsQueryable();

            var query = from b in bookQuery
                        join bs in bookInStoreQuery on b.Id equals bs.BookId into booksInStore
                        select new BookDto
                        {
                            Name = b.Name,
                            Description = b.Description,
                            ISBN = b.ISBN,
                            Author = b.Author,
                            MinPrice = booksInStore.Min(x => x.Price),
                            MaxPrice = booksInStore.Max(x => x.Price),
                            QuantityInStocks = booksInStore.Sum(x => x.Quantity),
                            Stores = booksInStore
                                .Where(x => x.Quantity > 0)
                                .Select(x => new BookInStoreDto
                                {
                                    StoreId = x.StoreId,
                                    Price = x.Price,
                                    Quantity = x.Quantity
                                })
                        };

            var book = query.FirstOrDefault();

            // Todo: Need optimize this code
            if (book != null && book.Stores != null)
            {
                var storeIds = book.Stores.Select(x => x.StoreId).ToList();
                if (storeIds.Count > 0)
                {
                    var stores = storeRepository.AsQueryable().Where(x => storeIds.Contains(x.Id)).ToList();
                    foreach (var item in book.Stores)
                    {
                        var store = stores.FirstOrDefault(x => x.Id == item.StoreId);
                        if (store != null)
                        {
                            item.StoreName = store.Name;
                        }
                    }
                }
            }

            return book;
        }

        public async Task<PlaceOrderResult> PlaceOrder(PlaceOrderRequest request)
        {
            var book = bookRepository.AsQueryable().Where(x => x.ISBN == request.ISBN).FirstOrDefault();

            if (book == null)
            {
                return new PlaceOrderResult
                {
                    Success = false,
                    Message = "Book not found."
                };
            }

            // Todo: Need apply transaction and decrease book quantity

            var order = new Order
            {
                BookId = book.Id,
                BookName = book.Name,
                BookISBN = book.ISBN,
                StoreId = request.StoreId,
                ContactEmail = request.ContactEmail,
                CreatedAt = DateTime.UtcNow,
            };

            await orderRepository.InsertAsync(order);

            return new PlaceOrderResult
            {
                Success = true,
                Message = "Order success."
            };
        }
    }
}

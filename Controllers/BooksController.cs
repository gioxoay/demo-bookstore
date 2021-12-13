using BookStore.Models;
using BookStore.Models.Dto;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService bookService;

        public BooksController(BookService bookService)
        {
            this.bookService = bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<BookDto>> Get([FromQuery] BookParameters parameters)
        {
            return bookService.GetBooks(parameters);
        }

        [HttpGet("{isbn}")]
        public ActionResult<BookDto> GetBookDetails(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return NotFound();
            }

            var book = bookService.GetBook(isbn);

            if (book == null)
            {
                return NotFound();
            }

            return new JsonResult(book);
        }

        [HttpPost("place-order")]
        public async Task<ActionResult<PlaceOrderResult>> PlaceOrder(PlaceOrderRequest request)
        {
            var result = await bookService.PlaceOrder(request);

            return new JsonResult(result);
        }
    }
}

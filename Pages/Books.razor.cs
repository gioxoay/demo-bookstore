using Microsoft.AspNetCore.Components;
using BookStore.Shared;
using TabBlazor;
using TabBlazor.Components.Modals;
using TabBlazor.Services;
using BookStore.Services;
using BookStore.Data.Domain;

namespace BookStore.Pages
{
    public partial class Books
    {
        [Inject] IModalService ModalService { get; set; }

        [Inject] BookService BookService { get; set; }

        [Inject] IServiceProvider ServiceProvider { get; set; }

        private static List<Book> books = new();

        protected override async Task OnInitializedAsync()
        {
            await FetchBooks();
            await base.OnInitializedAsync();
        }

        private async Task FetchBooks()
        {
            books = BookService
                .AsQueryable()
                .OrderBy(x => x.Name)
                .ToList();
            await Task.CompletedTask;
        }

        public async Task OnItemAdd()
        {
            await ShowModal(new Book());
        }

        public async Task OnItemEdit(Book book)
        {
            await ShowModal(book);
        }

        public async Task ShowModal(Book book)
        {
            var component = new RenderComponent<EditBookModal>();
            component.Set(x => x.Model, book);

            var modalOptions = new ModalOptions
            {
                Size = ModalSize.XLarge
            };

            var result = await ModalService.ShowAsync("Book", component, modalOptions);

            if (!result.Cancelled)
            {
                book.ISBN = book.ISBN.ToUpperInvariant().Trim();

                if (string.IsNullOrEmpty(book.Id))
                {
                    await BookService.InsertAsync(book);
                }
                else
                {
                    await BookService.UpdateAsync(book.Id, book);
                }

                await FetchBooks();
            }
        }

        public async Task OnItemDelete(Book book)
        {
            var result = await ModalService.ShowDialogAsync(new DialogOptions
            {
                MainText = Constants.DeleteConfirmTitle,
                SubText = Constants.DeleteConfirmMessage,
                IconElements = AppIcons.Alert_Triangle,
                StatusColor = TablerColor.Danger
            });

            if (result)
            {
                await BookService.DeleteAsync(book.Id);
                await FetchBooks();
            }
        }
    }
}

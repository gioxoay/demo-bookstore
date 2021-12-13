using BookStore.Data.Domain;
using BookStore.Services;
using Microsoft.AspNetCore.Components;
using TabBlazor;
using TabBlazor.Services;

namespace BookStore.Pages
{
    public partial class EditBookModal
    {
        [Inject]
        public IModalService ModalService { get; set; }

        [Inject]
        public StoreService StoreService { get; set; }

        [Parameter]
        public Book Model { get; set; }

        private void Cancel()
        {
            ModalService.Close(ModalResult.Cancel());
        }

        private void Save()
        {
            ModalService.Close(ModalResult.Ok());
        }
    }
}
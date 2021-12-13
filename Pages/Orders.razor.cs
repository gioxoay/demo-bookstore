using Microsoft.AspNetCore.Components;
using TabBlazor.Services;
using BookStore.Services;
using BookStore.Data.Domain;

namespace BookStore.Pages
{
    public partial class Orders
    {
        [Inject] IModalService ModalService { get; set; }

        [Inject] OrderService OrderService { get; set; }

        private static List<Order> items = new();

        protected override async Task OnInitializedAsync()
        {
            await FetchItems();
            await base.OnInitializedAsync();
        }

        private async Task FetchItems()
        {
            items = OrderService
                .AsQueryable()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            await Task.CompletedTask;
        }
    }
}

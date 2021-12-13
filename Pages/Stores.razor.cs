using Microsoft.AspNetCore.Components;
using BookStore.Shared;
using TabBlazor;
using TabBlazor.Components.Modals;
using TabBlazor.Services;
using BookStore.Services;
using BookStore.Data.Domain;

namespace BookStore.Pages
{
    public partial class Stores
    {
        [Inject] IModalService ModalService { get; set; }

        [Inject] StoreService StoreService { get; set; }

        private static List<Store> items = new();

        protected override async Task OnInitializedAsync()
        {
            await FetchItems();
            await base.OnInitializedAsync();
        }

        private async Task FetchItems()
        {
            items = StoreService
                .AsQueryable()
                .OrderBy(x => x.Name)
                .ToList();
            await Task.CompletedTask;
        }

        public async Task OnItemAdd()
        {
            await ShowModal(new Store());
        }

        public async Task OnItemEdit(Store airdrop)
        {
            await ShowModal(airdrop);
        }

        public async Task ShowModal(Store item)
        {
            var component = new RenderComponent<EditStoreModal>();
            component.Set(x => x.Model, item);

            var modalOptions = new ModalOptions
            {
                Size = ModalSize.Medium
            };

            var result = await ModalService.ShowAsync("Store", component, modalOptions);

            if (!result.Cancelled)
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    await StoreService.InsertAsync(item);
                }
                else
                {
                    await StoreService.UpdateAsync(item.Id, item);
                }

                await FetchItems();
            }
        }

        public async Task OnItemDelete(Store item)
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
                await StoreService.DeleteAsync(item.Id);
                await FetchItems();
            }
        }
    }
}

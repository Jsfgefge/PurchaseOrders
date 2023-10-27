using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;

namespace BlazorPurchaseOrders.Pages {
    public partial class Index : ComponentBase {

        [Inject]
        IPOHeaderService POHeaderService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        IEnumerable<POHeader> poheader;
        private List<ItemModel> Toolbaritems = new List<ItemModel>();

        protected override async Task OnInitializedAsync() {
            poheader = await POHeaderService.POHeaderList();

            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new order", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a new order", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a new order", PrefixIcon = "e-delete" });
        }

        public void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
            }
            if (args.Item.Text == "Edit") {
                //Code for editing
            }
            if (args.Item.Text == "Delete") {
                //Code for deleting
            }
        }
    }


}
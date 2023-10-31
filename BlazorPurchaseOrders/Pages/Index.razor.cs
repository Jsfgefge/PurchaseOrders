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

        private int selectedPOHeaderID { get; set; } = 0;
        IEnumerable<POHeader> poheader;
        private List<ItemModel> Toolbaritems = new List<ItemModel>();

        private int POHeaderID;
        WarningPage Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        protected override async Task OnInitializedAsync() {
            poheader = await POHeaderService.POHeaderList();

            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new order", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a new order", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a new order", PrefixIcon = "e-delete" });
        }
        public void RowSelectHandler(RowSelectEventArgs<POHeader> args) {
            selectedPOHeaderID = args.Data.POHeaderID;
        }
        public void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                POHeaderID = 0;
                NavigationManager.NavigateTo($"/purchaseorder/{POHeaderID}");

            }
            if (args.Item.Text == "Edit") {
                //Code for editing
                if (selectedPOHeaderID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please Select an Order from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    NavigationManager.NavigateTo($"/purchaseorder/{selectedPOHeaderID}");
                }
            }
            if (args.Item.Text == "Delete") {
                //Code for deleting
            }
        }
    }


}
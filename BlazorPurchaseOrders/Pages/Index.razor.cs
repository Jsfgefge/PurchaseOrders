using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;
using System.Runtime.InteropServices;
using System.Data;

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
        ConfirmPage ConfirmOrderDelete;
        string ConfirmHeaderMessage = "";
        string ConfirmContentMessage = "";
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        POHeader orderHeader = new POHeader();
        public bool ConfirmationChanged { get; set; } = false;

        protected override async Task OnInitializedAsync() {
            poheader = await POHeaderService.POHeaderList();

            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new order", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a new order", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a new order", PrefixIcon = "e-delete" });
        }
        public void RowSelectHandler(RowSelectEventArgs<POHeader> args) {
            selectedPOHeaderID = args.Data.POHeaderID;
        }
        public async void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
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
                if (selectedPOHeaderID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select an Order grom the grid.";
                    Warning.OpenDialog();
                }
                else {
                    orderHeader = await POHeaderService.POHeader_GetOne(selectedPOHeaderID);
                    ConfirmHeaderMessage = "Confirm Deletion";
                    ConfirmContentMessage = "Please confirm that this rder should be deleted.";
                    ConfirmOrderDelete.OpenDialog();

                }
            }
        }
        protected async Task ConfirmOrderArchive(bool archiveConfirmed) {
            if (archiveConfirmed) {
                orderHeader.POHeaderIsArchived = true;
                bool Success = await POHeaderService.POHeaderUpdate(orderHeader);
                poheader = await POHeaderService.POHeaderList();
                StateHasChanged();
                selectedPOHeaderID = 0;
            }
        }
    }


}
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;
using System.Runtime.InteropServices;
using System.Data;
using Microsoft.JSInterop;
using System;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorPurchaseOrders.Pages {
    public partial class Index : ComponentBase {

        [Inject]
        IPOHeaderService POHeaderService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime IJS { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        private Guid selectedPOHeaderGuid { get; set; }

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
            //poheader = await POHeaderService.POHeaderList();
            await GetOrderList();
            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new order", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a new order", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a new order", PrefixIcon = "e-delete" });
            Toolbaritems.Add(new ItemModel() { Text = "Preview", TooltipText = "Preview selected order", PrefixIcon = "e-print" });
        }
        public void RowSelectHandler(RowSelectEventArgs<POHeader> args) {
            selectedPOHeaderID = args.Data.POHeaderID;
            selectedPOHeaderGuid = args.Data.POHeaderGuid;
        }
        public async void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                POHeaderID = 0;
                NavigationManager.NavigateTo($"/purchaseorder/{Guid.Empty}");

            }
            if (args.Item.Text == "Edit") {
                //Code for editing
                if (selectedPOHeaderID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please Select an Order from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    NavigationManager.NavigateTo($"/purchaseorder/{selectedPOHeaderGuid}");
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
            if (args.Item.Text == "Preview") {
                //Code for editing - Check that an order haws been selected from the grid
                if (selectedPOHeaderID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please Select an Order from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    //NavigationManager.NavigateTo($"/previeworder/");
                    //NavigationManager.NavigateTo($"/previeworder/{selectedPOHeaderID}");
                    await IJS.InvokeVoidAsync("open", $"/previeworder/{selectedPOHeaderGuid}", "_blank");
                }
            }
        }
        protected async Task GetOrderList() {
            var user = (await authenticationStateTask).User;
            if (user.IsInRole("Admin") || user.IsInRole("Manager")) {
                poheader = await POHeaderService.POHeaderList(null);   //leave user name blank
            }
            else {
                poheader = await POHeaderService.POHeaderList(user.Identity.Name); //pass user name
            }
        }
        protected async Task ConfirmOrderArchive(bool archiveConfirmed) {
            if (archiveConfirmed) {
                orderHeader.POHeaderIsArchived = true;
                bool Success = await POHeaderService.POHeaderUpdate(orderHeader);
                //poheader = await POHeaderService.POHeaderList();
                await GetOrderList();
                StateHasChanged();
                selectedPOHeaderID = 0;
            }
        }
    }


}
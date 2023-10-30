using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;
using Syncfusion.Blazor.Inputs;

namespace BlazorPurchaseOrders.Pages {
    public partial class PurchaseOrderPage : ComponentBase {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] ISupplierService SupplierService { get; set; }
        [Inject] IPOHeaderService POHeaderService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        POHeader orderaddedit = new POHeader();
        IEnumerable<Supplier> supplier;

        [Parameter]
        public int POHeaderID { get; set; }
        private string UserName;

        string pagetitle = "Add an Order";

        

        //Executes on page open, sets headings and gets data in the case of edit
        protected override async Task OnInitializedAsync() {
            supplier = await SupplierService.SupplierList();
            orderaddedit.POHeaderOrderDate = DateTime.Now;
            var authSatate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authSatate.User;

            if (user.Identity.IsAuthenticated) {
                UserName = user.Identity.Name;
            }
            else {
                UserName = "The user is NOT Authenticated";
            }
            orderaddedit.POHeaderRequestedBy = UserName;


            if (POHeaderID == 0) {
                pagetitle = "Add an Order";
            }
            else {
                pagetitle = "Edit an Order";
            }
        }
        protected async Task OrderSave() {
            if (POHeaderID == 0) {
                //Save the record
                bool Success = await POHeaderService.POHeaderInsert(orderaddedit);
                NavigationManager.NavigateTo("/");
            }
            else {
                NavigationManager.NavigateTo("/");
            }
        }

        //Executes if user clivjs the Cancel button.
        void Cancel() {
            NavigationManager.NavigateTo("/");
        }

        //Populates the supplier address  when Supplier drop-down changed.
        private void OnChangeSupplier(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Supplier> args) {
            this.orderaddedit.POHeaderSupplierAddress1 = args.ItemData.SupplierAddress1;
            this.orderaddedit.POHeaderSupplierAddress2 = args.ItemData.SupplierAddress2;
            this.orderaddedit.POHeaderSupplierAddress3 = args.ItemData.SupplierAddress3;
            this.orderaddedit.POHeaderSupplierPostCode = args.ItemData.SupplierPostCode;
            this.orderaddedit.POHeaderSupplierEmail = args.ItemData.SupplierEmail;
        }
    }
}

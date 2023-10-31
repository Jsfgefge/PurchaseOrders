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
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Syncfusion.Blazor.Layouts;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.ComponentModel.Design;

namespace BlazorPurchaseOrders.Pages {
    public partial class PurchaseOrderPage : ComponentBase {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] ISupplierService SupplierService { get; set; }
        [Inject] IPOHeaderService POHeaderService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] IProductService ProductService { get; set; }
        [Inject] ITaxService TaxService { get; set; }
        [Inject] IPOLineService POLineService { get; set; }

        public bool supplierEneabled { get; set; } = true;


        POHeader orderaddedit = new POHeader();
        IEnumerable<Supplier> supplier;
        IEnumerable<Product> product;
        IEnumerable<Tax> tax;
        IEnumerable<POLine> orderLinesByPOHeader;

        [Parameter]
        public int POHeaderID { get; set; }
        private string UserName;

        WarningPage Warning;
        string pagetitle = "Add an Order";
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        string dialogHeaderText = "";
        int selectedPOLineID = -999;
        int currentNewRow = 0;

        SfGrid<POLine> OrderLinesGrid;
        SfDialog DialogAddEditOrderLine;
        SfDialog DialogDeleteOrderLine;

        public POLine addeditOrderLine = new POLine();
        public List<POLine> orderLines = new List<POLine>();
        private List<ItemModel> Toolbaritems = new List<ItemModel>();

        private List<int> OrderLinesToBeDeleted = new List<int>();

        //Executes on page open, sets headings and gets data in the case of edit
        protected override async Task OnInitializedAsync() {
            supplier = await SupplierService.SupplierList();
            //product = await ProductService.ProductList();
            tax = await TaxService.TaxList();

            orderaddedit.POHeaderOrderDate = DateTime.Now;
            var authSatate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authSatate.User;

            //Items in the toolbar that i want
            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new order linbe", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a order line", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a order line", PrefixIcon = "e-delete" });


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
                orderaddedit = await POHeaderService.POHeader_GetOne(POHeaderID);
                orderLinesByPOHeader = await POLineService.POLine_GetByPOHeader(POHeaderID);
                orderLines = orderLinesByPOHeader.ToList();
                supplierEneabled = false;
            }
        }
        protected async Task OrderSave() {
            if (orderaddedit.POHeaderSupplierID == 0) {
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Please select a supplier before adding order lines";
                Warning.OpenDialog();
            }
            else {
                if (POHeaderID == 0) {
                    //Save the record
                    //bool Success = await POHeaderService.POHeaderInsert(orderaddedit);

                    int HeaderID = await POHeaderService.POHeaderInsert(
                        orderaddedit.POHeaderOrderDate,
                        orderaddedit.POHeaderSupplierID,
                        orderaddedit.POHeaderSupplierAddress1,
                        orderaddedit.POHeaderSupplierAddress2,
                        orderaddedit.POHeaderSupplierAddress3,
                        orderaddedit.POHeaderSupplierPostCode,
                        orderaddedit.POHeaderSupplierEmail,
                        orderaddedit.POHeaderRequestedBy
                        );
                    foreach (var individualPOLine in orderLines) {
                        individualPOLine.POLineHeaderID = HeaderID;
                        bool Success = await POLineService.POLineInsert(individualPOLine);
                    }
                    NavigationManager.NavigateTo("/");
                }
                else {
                    bool Success = await POHeaderService.POHeaderUpdate(orderaddedit);
                    foreach(var individualPOLine in orderLines) {
                        //If POLineHeaderID is positive it means it has been edited during the edit of this order
                        if (individualPOLine.POLineID > 0) {
                            Success = await POLineService.POLineUpdate(individualPOLine);
                        }
                        else {
                            individualPOLine.POLineHeaderID = POHeaderID;
                            Success = await POLineService.POLineInsert(individualPOLine);
                            }
                    }
                    foreach(var individualPOLine in OrderLinesToBeDeleted) {
                        Success = await POLineService.POLineDeleteOne(individualPOLine);
                    }
                    //Clear the list of POLines to be deleted
                    OrderLinesToBeDeleted.Clear();
                    NavigationManager.NavigateTo("/");
                }
                //addeditOrderLine = new POLine(); //Ensures a blank form when adding
                //addeditOrderLine.POLineNetPrice = 0;
                //addeditOrderLine.POLineTaxID = 0;
                //addeditOrderLine.POLineProductID = 0;
                //await this.DialogAddEditOrderLine.Show();
            }
        }

        //Executes if user clivjs the Cancel button.
        void Cancel() {
            NavigationManager.NavigateTo("/");
        }

        //Populates the supplier address  when Supplier drop-down changed.
        private async Task OnChangeSupplier(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, Supplier> args) {
            this.orderaddedit.POHeaderSupplierAddress1 = args.ItemData.SupplierAddress1;
            this.orderaddedit.POHeaderSupplierAddress2 = args.ItemData.SupplierAddress2;
            this.orderaddedit.POHeaderSupplierAddress3 = args.ItemData.SupplierAddress3;
            this.orderaddedit.POHeaderSupplierPostCode = args.ItemData.SupplierPostCode;
            this.orderaddedit.POHeaderSupplierEmail = args.ItemData.SupplierEmail;

            //Refresh product to select only those products for this supplier (and products with null suppliers)
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            product = await ProductService.ProductListBySupplier(orderaddedit.POHeaderSupplierID);
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                //Check taht a supplier has been selected formn the dorpdown list
                if (orderaddedit.POHeaderSupplierID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a supplier before adding order lines";
                    Warning.OpenDialog();
                }
                else {
                    addeditOrderLine = new POLine(); //Ensures a blank form when adding
                    addeditOrderLine.POLineNetPrice = 0;
                    addeditOrderLine.POLineTaxID = 1;
                    addeditOrderLine.POLineTaxRate = 0;
                    addeditOrderLine.POLineProductID = 0;
                    dialogHeaderText = "Add an Order Line";
                    await this.DialogAddEditOrderLine.Show();
                }
            }
            if (args.Item.Text == "Edit") {
                //Code for editing goes here
                if (selectedPOLineID == -999) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select an Order Line from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    //Populate addeditOrderLine (temporary dat set used for the editing process)
                    addeditOrderLine = orderLines.Where(x => x.POLineID == selectedPOLineID).FirstOrDefault();
                    StateHasChanged();
                    dialogHeaderText = "Edit an Order Line";
                    await this.DialogAddEditOrderLine.Show();
                }
            }
            if (args.Item.Text == "Delete") {
                //Code for deleting goes here
                if (selectedPOLineID == -999) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select an Order Line from the grid";
                    Warning.OpenDialog();
                }
                else{
                    await this.DialogDeleteOrderLine.Show();
                }
            }

        }

        public void RowSelectHandler(RowSelectEventArgs<POLine> args) {
            selectedPOLineID = args.Data.POLineID;
        }

        private void OnChangeProduct(Syncfusion.Blazor.DropDowns.SelectEventArgs<Product> args) {
            this.addeditOrderLine.POLineProductCode = args.ItemData.ProductCode;
            this.addeditOrderLine.POLineProductDescription = args.ItemData.ProductDescription;
            this.addeditOrderLine.POLineProductUnitPrice = args.ItemData.ProductUnitPrice;
            POLineCalc();
        }

        private void OnChangeTax(Syncfusion.Blazor.DropDowns.SelectEventArgs<Tax> args) {
            int testTaxID = args.ItemData.TaxID;
            this.addeditOrderLine.POLineTaxRate = args.ItemData.TaxRate;
            POLineCalc();
        }

        private void POLineCalc() {
            addeditOrderLine.POLineNetPrice = addeditOrderLine.POLineProductUnitPrice * addeditOrderLine.POLineProductQuantity;
            addeditOrderLine.POLineTaxAmount = addeditOrderLine.POLineNetPrice.Value * addeditOrderLine.POLineTaxRate;
            addeditOrderLine.POLineGrossPrice = addeditOrderLine.POLineNetPrice.Value * (1 + addeditOrderLine.POLineTaxRate);
        }

        private async Task OrderLineSave() {
            if (addeditOrderLine.POLineID == 0) {
                if (addeditOrderLine.POLineProductCode == null || addeditOrderLine.POLineProductCode == "") {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Product.";
                    Warning.OpenDialog();
                }
                else if (addeditOrderLine.POLineTaxID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Tax Rate.";
                    Warning.OpenDialog();
                }
                else {
                    currentNewRow = currentNewRow - 1;
                    orderLines.Add(new POLine {
                        POLineID = currentNewRow,
                        POLineHeaderID = 0,
                        POLineProductID = addeditOrderLine.POLineProductID,
                        POLineProductCode = addeditOrderLine.POLineProductCode,
                        POLineProductDescription = addeditOrderLine.POLineProductDescription,
                        POLineProductQuantity = addeditOrderLine.POLineProductQuantity,
                        POLineProductUnitPrice = addeditOrderLine.POLineProductUnitPrice,
                        POLineNetPrice = addeditOrderLine.POLineNetPrice,
                        POLineTaxRate = addeditOrderLine.POLineTaxRate,
                        POLineTaxAmount = addeditOrderLine.POLineTaxAmount,
                        POLineGrossPrice = addeditOrderLine.POLineGrossPrice,
                        POLineTaxID=addeditOrderLine.POLineTaxID,
                    });
                    OrderLinesGrid.Refresh();
                    StateHasChanged();

                    //Reseting the POLine, addeditOrderLine = new POline(); gives error
                    addeditOrderLine.POLineProductID = 0;
                    addeditOrderLine.POLineProductCode = "";
                    addeditOrderLine.POLineProductDescription = "";
                    addeditOrderLine.POLineProductQuantity = 0;
                    addeditOrderLine.POLineProductUnitPrice = 0;
                    addeditOrderLine.POLineNetPrice = 0;
                    //addeditOrderLine.POLineTaxID = 0;
                    //addeditOrderLine.POLineTaxRate = 0;
                    addeditOrderLine.POLineTaxAmount = 0;
                    addeditOrderLine.POLineGrossPrice = 0;
                    supplierEneabled = false;
                }
            }
            else {
                //An order is being edited
                //Chech taht a product has been selected from the drop-down list
                if (addeditOrderLine.POLineProductCode == null || addeditOrderLine.POLineProductCode == "") {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Product.";
                    Warning.OpenDialog();
                }
                else if (addeditOrderLine.POLineTaxID == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Tax Rate.";
                    Warning.OpenDialog();
                }
                else {
                    OrderLinesGrid.Refresh();
                    StateHasChanged();
                    await CloseDialog();
                    selectedPOLineID = -999;
                }
            }
        }
        public async void ConfirmDeleteNo() {
            await this.DialogDeleteOrderLine.Hide();
            selectedPOLineID = -999;
        }
        private void OrderLineDelete() {
            if (selectedPOLineID > 0) {
                //Order line has already been saved to the database, an was present at start of this order edit
                //Add to list of order to be deleted when order is saved
                OrderLinesToBeDeleted.Add(selectedPOLineID);
            }
            var itemToRemove = orderLines.Single(x => x.POLineID == selectedPOLineID);
            orderLines.Remove(itemToRemove);
            OrderLinesGrid.Refresh();
        }
        public async void ConfirmDeleteYes() {
            OrderLineDelete();
            await this.DialogDeleteOrderLine.Hide();
            selectedPOLineID = -999;
        }
        private async Task CloseDialog() {
            await this.DialogAddEditOrderLine.Hide();
        }
    }
}

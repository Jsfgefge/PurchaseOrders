using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;

namespace BlazorPurchaseOrders.Pages {
    public partial class ProductPage : ComponentBase {
        [Inject] IProductService ProductService { get; set; }
        [Inject] ISupplierService SupplierService { get; set; }

        IEnumerable<Supplier> supplier;
        IEnumerable<Product> product;
        private List<ItemModel> Toolbaritems = new List<ItemModel>();
        public int SelectedProductId { get; set; } = 0;

        SfDialog DialogAddEditProduct;
        SfDialog DialogDeleteProduct;
        Product addeditProduct = new Product();
        string HeaderText = "";

        WarningPage Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        protected override async Task OnInitializedAsync() {
            //Populate the list of VAT object from the VAT table
            product = await ProductService.ProductList();
            supplier = await SupplierService.SupplierList();
            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Product", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit Selected Product", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete Selected Product", PrefixIcon = "e-delete" });
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                addeditProduct = new Product(); // Ensures a blank form when adding
                HeaderText = "Add Product";
                await this.DialogAddEditProduct.Show();
            }
            if (args.Item.Text == "Edit") {
                //Code for editing goes here
                if (SelectedProductId == 0) {
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "Please selecte a Product from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditProduct (temporary data set used for the editing process)
                    HeaderText = "Edit Product";
                    addeditProduct = await ProductService.Product_GetOne(SelectedProductId);
                    await this.DialogAddEditProduct.Show();
                }
            }
            if (args.Item.Text == "Delete") {
                //code for deleting goes here
                if (SelectedProductId == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Product from the grid";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditProduct
                    HeaderText = "Delete Product";
                    addeditProduct = await ProductService.Product_GetOne(SelectedProductId);
                    await this.DialogDeleteProduct.Show();
                }
            }
        }

        public void RowSelectedHandler(RowSelectEventArgs<Product> args) {
            SelectedProductId = args.Data.ProductID;
        }

        protected async Task ProductSave() {
            if (addeditProduct.ProductID == 0) {
                int Succes = await ProductService.ProductInsert(addeditProduct.ProductCode, addeditProduct.ProductDescription, addeditProduct.ProductUnitPrice, addeditProduct.ProductSupplierID);
                if (Succes != 0) {
                    //Product already exists
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "This Product already exists; it cannot be added again";
                    Warning.OpenDialog();
                    //Data is left in the dialog so the ser can see the problem.
                }
                else {
                    //Clear te dialog and is ready for another entre 
                    //User myst specifically close or cancel the dialog
                    addeditProduct = new Product();
                }
            }
            else {
                //Item is being edited
                int Success = await ProductService.ProductUpdate(addeditProduct.ProductID, addeditProduct.ProductCode, addeditProduct.ProductDescription, addeditProduct.ProductUnitPrice, addeditProduct.ProductSupplierID, addeditProduct.ProductIsArchived);
                if (Success != 0) {
                    //Product already exists
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "This Product already exists; it cannot be added again.";
                    Warning.OpenDialog();
                }
                else {
                    await this.DialogAddEditProduct.Hide();
                    this.StateHasChanged();
                    addeditProduct = new Product();
                    SelectedProductId = 0;
                }
            }

            //Close Dialog
            //await CloseDialog();

            //Refresh datagrid
            product = await ProductService.ProductList();
            StateHasChanged();
        }

        private async Task CloseDialog() {
            await this.DialogAddEditProduct.Hide();
        }

        public async void ConfirmDeleteNo() {
            await DialogDeleteProduct.Hide();
            SelectedProductId = 0;
        }
        public async void ConfirmDeleteYes() {
            int Success = await ProductService.ProductUpdate(addeditProduct.ProductID, addeditProduct.ProductCode, addeditProduct.ProductDescription, addeditProduct.ProductUnitPrice, addeditProduct.ProductSupplierID, addeditProduct.ProductIsArchived=true);
            if (Success != 0) {
                //Product already exists
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Unknown error has ocurred - the record hasnot been deleted!";
                Warning.OpenDialog();
            }
            else {
                await this.DialogDeleteProduct.Hide();
                product = await ProductService.ProductList();
                this.StateHasChanged();
                addeditProduct = new Product();
                SelectedProductId = 0;
            }
        }

    }
}

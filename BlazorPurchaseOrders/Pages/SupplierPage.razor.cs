using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;

namespace BlazorPurchaseOrders.Pages {
    public partial class SupplierPage : ComponentBase {
        [Inject] ISupplierService SupplierService { get; set; }

        IEnumerable<Supplier> supplier;
        private List<ItemModel> Toolbaritems = new List<ItemModel>();
        public int SelectedSupplierId { get; set; } = 0;

        SfDialog DialogAddEditSupplier;
        SfDialog DialogDeleteSupplier;
        Supplier addeditSupplier = new Supplier();
        string HeaderText = "";

        WarningPage Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        protected override async Task OnInitializedAsync() {
            //Populate the list of VAT object from the VAT table
            supplier = await SupplierService.SupplierList();
            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Supplier", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit Selected Supplier", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete Selected Supplier", PrefixIcon = "e-delete" });
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                addeditSupplier = new Supplier(); // Ensures a blank form when adding
                HeaderText = "Add Supplier";
                await this.DialogAddEditSupplier.Show();
            }
            if (args.Item.Text == "Edit") {
                //Code for editing goes here
                if (SelectedSupplierId == 0) {
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "Please selecte a Supplier from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditSupplier (temporary data set used for the editing process)
                    HeaderText = "Edit Supplier";
                    addeditSupplier = await SupplierService.Supplier_GetOne(SelectedSupplierId);
                    await this.DialogAddEditSupplier.Show();
                }
            }
            if (args.Item.Text == "Delete") {
                //code for deleting goes here
                if (SelectedSupplierId == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Supplier from the grid";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditSupplier
                    HeaderText = "Delete Supplier";
                    addeditSupplier = await SupplierService.Supplier_GetOne(SelectedSupplierId);
                    await this.DialogDeleteSupplier.Show();
                }
            }
        }

        public void RowSelectedHandler(RowSelectEventArgs<Supplier> args) {
            SelectedSupplierId = args.Data.SupplierID;
        }

        protected async Task SupplierSave() {
            if (addeditSupplier.SupplierID == 0) {
                int Succes = await SupplierService.SupplierInsert(addeditSupplier.SupplierName, addeditSupplier.SupplierAddress1, addeditSupplier.SupplierAddress2, addeditSupplier.SupplierAddress3, addeditSupplier.SupplierPostCode, addeditSupplier.SupplierEmail);
                if (Succes != 0) {
                    //Supplier already exists
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "This Supplier already exists; it cannot be added again";
                    Warning.OpenDialog();
                    //Data is left in the dialog so the ser can see the problem.
                }
                else {
                    //Clear te dialog and is ready for another entre 
                    //User myst specifically close or cancel the dialog
                    addeditSupplier = new Supplier();
                }
            }
            else {
                //Item is being edited
                int Success = await SupplierService.SupplierUpdate(addeditSupplier.SupplierID, addeditSupplier.SupplierName, addeditSupplier.SupplierAddress1, addeditSupplier.SupplierAddress2, addeditSupplier.SupplierAddress3, addeditSupplier.SupplierPostCode, addeditSupplier.SupplierEmail, addeditSupplier.SupplierIsArchived);
                if (Success != 0) {
                    //Supplier already exists
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "This Supplier already exists; it cannot be added again.";
                    Warning.OpenDialog();
                }
                else {
                    await this.DialogAddEditSupplier.Hide();
                    this.StateHasChanged();
                    addeditSupplier = new Supplier();
                    SelectedSupplierId = 0;
                }
            }

            //Close Dialog
            //await CloseDialog();

            //Refresh datagrid
            supplier = await SupplierService.SupplierList();
            StateHasChanged();
        }

        private async Task CloseDialog() {
            await this.DialogAddEditSupplier.Hide();
        }

        public async void ConfirmDeleteNo() {
            await DialogDeleteSupplier.Hide();
            SelectedSupplierId = 0;
        }
        public async void ConfirmDeleteYes() {
            int Success = await SupplierService.SupplierUpdate(addeditSupplier.SupplierID, addeditSupplier.SupplierName, addeditSupplier.SupplierAddress1, addeditSupplier.SupplierAddress2, addeditSupplier.SupplierAddress3, addeditSupplier.SupplierPostCode, addeditSupplier.SupplierEmail, addeditSupplier.SupplierIsArchived=true);
            if (Success != 0) {
                //Supplier already exists
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Unknown error has ocurred - the record hasnot been deleted!";
                Warning.OpenDialog();
            }
            else {
                await this.DialogDeleteSupplier.Hide();
                supplier = await SupplierService.SupplierList();
                this.StateHasChanged();
                addeditSupplier = new Supplier();
                SelectedSupplierId = 0;
            }
        }

    }
}

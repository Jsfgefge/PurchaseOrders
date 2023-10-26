using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;

namespace BlazorPurchaseOrders.Pages {
    public partial class TaxPage : ComponentBase{
        [Inject] ITaxService TaxService { get; set; }

        IEnumerable<Tax> tax;
        private List<ItemModel> Toolbaritems = new List<ItemModel>();
        public int SelectedTaxId { get; set; } = 0;

        SfDialog DialogAddEditTax;
        SfDialog DialogDeleteTax;
        Tax addeditTax = new Tax();
        string HeaderText = "";

        WarningPage Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        protected override async Task OnInitializedAsync() {
            //Populate the list of VAT object from the VAT table
            tax = await TaxService.TaxList();
            Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Tax Rate", PrefixIcon = "e-add" });
            Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit Selected Tax Rate", PrefixIcon = "e-edit" });
            Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete Selected Tax Rate", PrefixIcon = "e-delete" });
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args) {
            if (args.Item.Text == "Add") {
                //Code for adding goes here
                addeditTax = new Tax(); // Ensures a blank form when adding
                HeaderText = "Add Tax Rate";
                await this.DialogAddEditTax.Show();
            }
            if (args.Item.Text == "Edit") {
                //Code for editing goes here
                if (SelectedTaxId == 0) {
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "Please selecte a Tax Rate from the grid.";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditTax (temporary data set used for the editing process)
                    HeaderText = "Edit Tax Rate";
                    addeditTax = await TaxService.Tax_GetOne(SelectedTaxId);
                    await this.DialogAddEditTax.Show();
                }
            }
            if (args.Item.Text == "Delete") {
                //code for deleting goes here
                if (SelectedTaxId == 0) {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Tax Rate from the grid";
                    Warning.OpenDialog();
                }
                else {
                    //populate addeditTax
                    HeaderText = "Delete Tax Rate";
                    addeditTax = await TaxService.Tax_GetOne(SelectedTaxId);
                    await this.DialogDeleteTax.Show();
                }
            }
        }

        public void RowSelectedHandler(RowSelectEventArgs<Tax> args) {
            SelectedTaxId = args.Data.TaxID;
        }

        protected async Task TaxSave() {
            if (addeditTax.TaxID == 0) {
                int Succes = await TaxService.TaxInsert(addeditTax.TaxDescription, addeditTax.TaxRate);
                if (Succes != 0) {
                    //Tax Rate already exists
                    WarningHeaderMessage = "Warning";
                    WarningContentMessage = "This Tax Description already exists; it cannot be added again";
                    Warning.OpenDialog();
                    //Data is left in the dialog so the ser can see the problem.
                }
                else {
                    //Clear te dialog and is ready for another entre 
                    //User myst specifically close or cancel the dialog
                    addeditTax = new Tax();
                }
            }
            else {
                //Item is being edited
                int Success = await TaxService.TaxUpdate(addeditTax.TaxDescription, addeditTax.TaxRate, SelectedTaxId, addeditTax.TaxIsArchived);
                if (Success != 0) {
                    //Tax Rate already exists
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "This Tax Description already exists; it cannot be added again.";
                    Warning.OpenDialog();
                }
                else {
                    await this.DialogAddEditTax.Hide();
                    this.StateHasChanged();
                    addeditTax = new Tax();
                    SelectedTaxId = 0;
                }
            }

            //Close Dialog
            //await CloseDialog();

            //Refresh datagrid
            tax = await TaxService.TaxList();
            StateHasChanged();
        }

        private async Task CloseDialog() {
            await this.DialogAddEditTax.Hide();
        }

        public async void ConfirmDeleteNo() {
            await DialogDeleteTax.Hide();
            SelectedTaxId = 0;
        }
        public async void ConfirmDeleteYes() {
            int Success = await TaxService.TaxUpdate(addeditTax.TaxDescription, addeditTax.TaxRate, SelectedTaxId, addeditTax.TaxIsArchived = true);
            if (Success != 0) {
                //Tax rate already exists
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Unknown error has ocurred - the record hasnot been deleted!";
                Warning.OpenDialog();
            }
            else {
                await this.DialogDeleteTax.Hide();
                tax = await TaxService.TaxList();
                this.StateHasChanged();
                addeditTax = new Tax();
                SelectedTaxId = 0;
            }
        }

    }
}

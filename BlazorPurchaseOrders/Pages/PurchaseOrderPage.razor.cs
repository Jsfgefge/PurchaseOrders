using Microsoft.AspNetCore.Components;
using BlazorPurchaseOrders.Data;

namespace BlazorPurchaseOrders.Pages {
    public partial class PurchaseOrderPage : ComponentBase {
        [Inject] NavigationManager navigationManager { get; set; }

        string pagetitle = "Add an Order";
        [Parameter]
        public int POHeaderID { get; set; }
    }
}

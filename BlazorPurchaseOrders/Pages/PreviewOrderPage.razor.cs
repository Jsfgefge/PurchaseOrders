using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using BlazorPurchaseOrders.Data;
using Syncfusion.Blazor.Popups;
using BlazorPurchaseOrders.Shared;
using Syncfusion.Blazor.Grids;

namespace BlazorPurchaseOrders.Pages {
	public partial class PreviewOrderPage : ComponentBase{
		[Inject] 
		IPOHeaderService POHeaderService { get; set; }
		[Inject]
		IPOLineService POLineService { get; set; }
		IEnumerable<POLine> orderLinesByPOHeader;
		public List<POLine> orderLines = new List<POLine>();
		POHeader orderHeader = new POHeader();
        public int POHeaderID { get; set; }
		[Parameter]
        public Guid POHeaderGuid { get; set; }
		protected override async Task OnInitializedAsync() {
            orderHeader = await POHeaderService.POHeader_GetOneByGuid(POHeaderGuid);
            POHeaderID = orderHeader.POHeaderID;
            orderLinesByPOHeader = await POLineService.POLine_GetByPOHeader(POHeaderID);
            orderLines = orderLinesByPOHeader.ToList(); //Convert from IEnumable to List
        }
	}
}

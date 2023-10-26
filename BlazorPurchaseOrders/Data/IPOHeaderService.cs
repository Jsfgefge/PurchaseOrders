using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    public interface IPOHeaderService {
        Task<bool> POHeaderInsert(POHeader poheader);
        Task<IEnumerable<POHeader>> POHeaderList();
        Task<POHeader> POHeader_GetOne(int POHeaderID);
        Task<bool> POHeaderUpdate(POHeader poheader);
    }
}

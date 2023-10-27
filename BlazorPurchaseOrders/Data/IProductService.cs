// This is the Product Interface
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    // Each item below provides an interface to a method in ProductServices.cs
    public interface IProductService {
        Task<int> ProductInsert(string ProductCode,
                                             string ProductDescription,
                                             decimal ProductUnitPrice,
                                             Int32 ProductSupplierID);
        Task<IEnumerable<Product>> ProductList();
        Task<Product> Product_GetOne(int ProductID);
        Task<int> ProductUpdate(int ProductID,
                                             string ProductCode,
                                             string ProductDescription,
                                             decimal ProductUnitPrice,
                                             Int32 ProductSupplierID,
                                             bool ProductIsArchived);
    }
}
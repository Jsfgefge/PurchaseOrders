// This is the Product Interface
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    // Each item below provides an interface to a method in ProductServices.cs
    public interface IProductService {
        Task<bool> ProductInsert(Product product);
        Task<IEnumerable<Product>> ProductList();
        Task<Product> Product_GetOne(int ProductID);
        Task<bool> ProductUpdate(Product product);
    }
}
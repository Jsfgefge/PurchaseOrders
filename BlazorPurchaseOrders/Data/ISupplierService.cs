// This is the Supplier Interface
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    // Each item below provides an interface to a method in SupplierServices.cs
    public interface ISupplierService {
        Task<bool> SupplierInsert(Supplier supplier);
        Task<IEnumerable<Supplier>> SupplierList();
        Task<Supplier> Supplier_GetOne(int SupplierID);
        Task<bool> SupplierUpdate(Supplier supplier);
    }
}
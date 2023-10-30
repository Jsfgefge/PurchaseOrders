using System;
using System.ComponentModel.DataAnnotations;
// This is the model for one row in the database table. You may need to make some adjustments.
namespace BlazorPurchaseOrders.Data {
    public class POLine {
        [Required]
        public int POLineID { get; set; }
        [Required]
        public int POLineHeaderID { get; set; }
        [Required]
        public int POLineProductID { get; set; }
        [Required]
        [StringLength(50)]
        public string POLineProductDescription { get; set; }
        [Required]
        public decimal POLineProductQuantity { get; set; }
        [Required]
        public decimal POLineProductUnitPrice { get; set; }
        [Required]
        public decimal POLineTaxRate { get; set; }

        //The following are not save to database - just for the DataGrid
        public decimal? POLineNetPrice { get; set; }
        public decimal POLineTaxAmount { get; set; }
        public decimal POLineGrossPrice { get; set; }
        public string POLineProductCode { get; set; }

        //POLineTaxID is not saved to the database, but is needed fro the Tax Rate drop-down list
        //It woulkd be more usual to save the TaxID to POLine in the databasem but
        //Tax rate percentage mught change in the future for a particular 'rate' we don't want
        //historic tax amount to be recalculated if re-displayed in the future
        public int POLineTaxID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    public class ServiceActionProduct
    {
        public long Id { set; get; }
        public long ActionId { set; get; }
        public long ServiceId { set; get; }
        public string SupplierId { set; get; }
        public string ServiceName { set; get; }
        public string Branch { set; get; }
        public string ActionName { set; get; }
        public string Name { set; get; }
        public string ItemCode { set; get; }
        public string Description { set; get; }
        public string Unit { set; get; }
        public long Count { set; get; }
        public decimal Amount { set; get; }
        public decimal CollectionAmount { set; get; }
        public decimal Quantity { set; get; }
        public decimal Collected { set; get; }
        public long Reorder { set; get; }
        public decimal Price { set; get; }
        public decimal TaxPercentage { set; get; }
        public decimal TaxPrice { set; get; }
        public string Currency { set; get; }
        public string ServiceType { set; get; }
        public string ServiceCategory { set; get; }
        public string Image { set; get; }
        public string AmountRequiredIn { set; get; }
        public decimal MaxSale { set; get; }
        public decimal MinSale { set; get; }
        public string Status { set; get; }
        public bool IsExternal { set; get; }
        public string ExternalServiceId { set; get; }
        public string ExternalURL { set; get; }
        public string Base { set; get; }
        public List<string> PropotionProducts { set; get; }
        public bool HasRecipe { set; get; }
        public bool HasFiles { set; get; }
        public List<ServiceFile> Files { set; get; }
        public bool DontDisplayRecipe { set; get; }
        public bool NotForSale { set; get; }

    }
}

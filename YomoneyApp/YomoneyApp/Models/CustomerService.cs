using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    public class CustomerService
    {
        public long Id { set; get; }
        public string CustomerId { set; get; }
        public string CustomerName { set; get; }
        public string CustomerMobileNumber { set; get; }
        public string CustomerCardNumber { set; get; }
        public long ServiceId { set; get; }
        public long TransactionType { set; get; }
        public string ServiceName { set; get; }
        public string ServiceType { set; get; }
        public decimal? Quantity { set; get; }
        public bool HasProduct { set; get; }
        public bool IsActive { set; get; }
        public bool HasRecords { set; get; }
        public bool AllowPartPayment { set; get; }
        public bool DeactivateOnAuthorisation { set; get; }
        public string ProductName { set; get; }
        public string ProductDetails { set; get; }
        public string ServiceProvider { set; get; }
        public string SupplierId { set; get; }
        public string ServiceAgentId { set; get; }
        public string SupplierName { set; get; }
        public string Description { set; get; }
        public decimal? Balance { set; get; }
        public decimal? SuspenseBalance { set; get; }
        public string TransactionCode { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime DatelastAccess { set; get; }
        public DateTime SubDue { set; get; }
        public string BillingCycle { set; get; }
        public string ReceiverMobile { set; get; }
        public string ResponseCode { set; get; }
        public string ResponseDescription { set; get; }
        public string logo { set; get; }
        public string ReceiversName { set; get; }
        public string ReceiversSurname { set; get; }
        public string ReceiversIdentification { set; get; }
        public string ReceiversGender { set; get; }
        public string ServiceRegion { set; get; }// district
        public string ServiceProvince { set; get; }
        public string ServiceCountry { set; get; }
        public string Currency { set; get; }
        public string Information1 { set; get; }
        public string Information2 { set; get; }
        public string Cashier { set; get; }
        public string Authoriser { set; get; }
        public string LocationCode { set; get; }
        public string JsonProducts { set; get; }
        public string InitialProducts { set; get; }
        public List<ServiceActionProduct> Products { set; get; }
    }
}

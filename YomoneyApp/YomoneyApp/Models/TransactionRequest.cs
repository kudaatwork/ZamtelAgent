using System;
using static System.DateTime;


namespace RetailKing.Models
    {
        public class TransactionRequest
        {
            public string AgentCode { get; set; }
            public string Mpin { get; set; }
            public decimal Amount { get; set; }
            public decimal MaxSale { get; set; }
            public decimal MinSale { get; set; }
            public string CustomerMSISDN { get; set; }
            public long ServiceId { get; set; }
            public long ActionId { get; set; }
            public string MTI { get; set; } //Message type indicator
            public string TerminalId { get; set; }
            public long TransactionType { get; set; }
            public string TransactionRef { get; set; }
            public string CustomerAccount { get; set; }
            public string CustomerData { get; set; }
            public string Product { get; set; }
            public string Source { get; set; }
            public string ServiceProvider { get; set; }
            public int Quantity { get; set; }
            public string Action { get; set; }
            public string ProcessingCode { get; set; }    
            public string OrderLines { get; set; }
            public string Note { get; set; }
            public string Narrative { get; set; }
            public string PaymentMethod { get; set; }
            public string Currency { get; set; }
            public string Country { get; set; }
        }
   
    }

  

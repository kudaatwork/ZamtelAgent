using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    namespace RetailKing.Models
    {
        public class TransactionResponse
        {
            public string ResponseCode { get; set; }
            public string Description { get; set; }
            public string Country { get; set; }
            public string Balance { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string TransactionCode { get; set; }
            public List<Vouchers> vouchers { get; set; }
            public string AgentCode { get; set; }
            public string Mpin { get; set; }
            public decimal Amount { get; set; }
            public decimal MaxSale { get; set; }
            public decimal MinSale { get; set; }
            public string CustomerMSISDN { get; set; }
            public long ServiceId { get; set; }
            public string MTI { get; set; } //Message type indicator
            public string TerminalId { get; set; }
            public string TransactionRef { get; set; }
            public long TransactionType { get; set; }
            public string CustomerAccount { get; set; }
            public string CustomerData { get; set; }
            public string Product { get; set; }
            public int Quantity { get; set; }
            public string Action { get; set; }
            public string ProcessingCode { get; set; }
            public string Note { get; set; }
            public string Narrative { get; set; }
        }


     
    }

    


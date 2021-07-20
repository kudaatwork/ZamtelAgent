using System;
using Newtonsoft.Json;
using static System.DateTime;

namespace YomoneyApp
{
    public class Loyalty
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string ContactPerson { set; get; }
        public string SupplierId { set; get; }
        public string SupplierName { set; get; }
        public string Branch { set; get; }
        public long SchemeId { set; get; }
        public int Points { set; get; }
        public decimal MonetaryValue { set; get; }
        public string DOB { set; get; }
        public string Email { set; get; }
        public string CardNumber { set; get; }
        public string PhoneNumber { set; get; }
        public DateTime LastAccess { set; get; }

    }
}


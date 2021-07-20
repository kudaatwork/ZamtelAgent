using System;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;


namespace RetailKing.Models
{
    public class Vouchers
    {
        public long Id { get; set; }
        public decimal Denomination { get; set; }
        public string BatchNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ExpireryDate { get; set; }
        public string Dated { get; set; }
        public long VID { get; set; }
        public string Status { get; set; }
        public string Product { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string VoucherKey { get; set; }
        public string Destination { get; set; }
        public string Miscellenious { get; set; }

    }
}

    


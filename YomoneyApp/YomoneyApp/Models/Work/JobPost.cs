using System;
using Newtonsoft.Json;
using static System.DateTime;
using System.Collections.Generic;

namespace YomoneyApp
{
    public class JobPost
    {

        public long Id { set; get; }
        public string CustomerId { set; get; }
        public string CustomerName { set; get; }
        public string Title { set; get; }
        public string Category { set; get; }
        public string Tags { set; get; }
        public string Description { set; get; }
        public Decimal Budget { set; get; }
        public Decimal ClosingAmount { set; get; }
        public long BidCount { set; get; }
        public string Status { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime ClosingDate { set; get; }
        public DateTime DateClosed { set; get; }
        public List<JobBid> Jobbids { set; get; }

    }
}


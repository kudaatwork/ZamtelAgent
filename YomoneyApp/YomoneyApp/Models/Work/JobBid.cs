using System;
using static System.DateTime;

namespace YomoneyApp
{
    public class JobBid
    {
        public String CustomerId { set; get; }
        public String CustomerName { set; get; }
        public String JobPostId { set; get; }
        public String RequesterId { set; get; }
        public String Description { set; get; }
        public String Currency { set; get; }
        public Decimal Amount { set; get; }
    }
}


using System;
using Newtonsoft.Json;
using static System.DateTime;

namespace YomoneyApp
{
    public class Rating
    {
        public String CustomerId { set; get; }
        public String Name { set; get; }
        public Int32 RateScore { set; get; }
        public String Description { set; get; }
    }
}


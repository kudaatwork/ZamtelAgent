using System;
using Newtonsoft.Json;
using static System.DateTime;

namespace YomoneyApp
{
    public class JobProfile
    {

        public String CustomerId { set; get; }
        public String CustomerName { set; get; }
        public String JobCategory { set; get; }
        public String SubCategory { set; get; }
        public String Description { set; get; }
        public String Address { set; get; }
        public String Image { set; get; }
        public String ProfileRating { set; get; }
        public Int32 RatingCount { set; get; }
        public Int32 JobsDone { set; get; }
        public DateTime ExpireryDate { set; get; }
        public Int32 CustomerRatings { set; get; }

    }
}


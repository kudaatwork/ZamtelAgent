using System;
using Newtonsoft.Json;
using static System.DateTime;

namespace YomoneyApp
{
    public class Advert
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SupplierId { get; set; }
        public string Image { get; set; }
        public Nullable<decimal> DailyMax { get; set; }
        public Nullable<decimal> DailyUsage { get; set; }
        public string Sex { get; set; }
        public Nullable<int> MinAge { get; set; }
        public Nullable<int> MaxAge { get; set; }
        public string Location { get; set; }
        public string AdPosition { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> ExpireryDate { get; set; }
        public string Status { get; set; }
        public string Placements { get; set; }
        public string City { get; set; }
        public Nullable<int> Radius { get; set; }
        public string Timeslots { get; set; }
        public Nullable<double> minLatitude { get; set; }
        public Nullable<double> maxLatitude { get; set; }
        public Nullable<double> minLongitude { get; set; }
        public Nullable<double> maxLongitude { get; set; }
        public string SiteUrl { get; set; }
        public Nullable<long> AdPossitionId { get; set; }
        public string Adtype { get; set; }
        public string LinkParameterName { get; set; }
        public Nullable<bool> HasParameter { get; set; }
        public string BackgroundImage { get; set; }
        public string PageLayout { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public Nullable<long> Clicks { get; set; }
        public Nullable<long> Likes { get; set; }
        public Nullable<long> Shares { get; set; }
        public Nullable<long> Impressions { get; set; }
        public string MediaType { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    class MapRoute
    {
        public long Id { get; set; }
        public string SupplierId { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public Nullable<long> ActionId { get; set; }
        public Nullable<long> TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TransportDetails { get; set; }
        public string PackageDetails { get; set; }
        public Nullable<System.DateTime> Datecreated { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public string StartPoint { get; set; }
        public string Destinations { get; set; }
        public string DestinationTags { get; set; }
        public string Status { get; set; }
        public string PackageSatus { get; set; }
        public Nullable<bool> IsPublic { get; set; }
        public string EndPoint { get; set; }
        public Nullable<double> StartLongitude { get; set; }
        public Nullable<double> StartLatitude { get; set; }
        public Nullable<double> EndLongitude { get; set; }
        public Nullable<double> EndLatitude { get; set; }
        public Nullable<decimal> ActivityBudget { get; set; }
        public Nullable<decimal> ActivityActual { get; set; }
        public Nullable<decimal> PercentageCompleted { get; set; }
        public Nullable<decimal> DistanceCovered { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<bool> UseTicketRate { get; set; }
        public Nullable<bool> Recurring { get; set; }
    }
}

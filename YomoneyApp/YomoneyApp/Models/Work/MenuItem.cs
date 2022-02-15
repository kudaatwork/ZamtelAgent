using System;
using Newtonsoft.Json;
using ZXing.Net.Mobile.Forms;
using static System.DateTime;

namespace YomoneyApp
{
    public class MenuItem
    {
        public string Id { set; get; }
        public string Image { set; get; }
        public string Media { set; get; }
        public string MediaType { set; get; }
        public string ThemeColor { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public string Section { set; get; }// subcategory
        public string Note { set; get; }
        public string Currency { set; get; }
        public long ServiceId { set; get; }
        public long ActionId { set; get; }
        public long TransactionType { set; get; }
        public string SupplierId { set; get; }
        public string Status { get; set; }
        public string UserImage { set; get; }
        public string Amount { set; get; }
        public string WebLink { set; get; }
        public bool IsDelivered { get; set; }
        public int Count { set; get; }
        public DateTime date { set; get; }
        public ZXingBarcodeImageView Barcode { set; get; }
        public bool HasProducts { set; get; }
        public bool IsAdvert { set; get; }
        public bool IsShare { set; get; }
        public bool IsNotAdvert { set; get; }
        public bool IsEmptyList { set; get; }
        public bool RequiresRegistration { set; get; }
        public bool RequiresAmount { set; get; } // call pop up for entering amount
    }
}


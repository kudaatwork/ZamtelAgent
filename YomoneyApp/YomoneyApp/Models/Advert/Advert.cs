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
    }
}


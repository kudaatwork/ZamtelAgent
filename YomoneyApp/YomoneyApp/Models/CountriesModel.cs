using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    public class CountriesModel
    {
        public string name { get; set; }
        public string alpha2Code { get; set; }
        public List<string> callingCodes { get; set; }
        public Flags flags { get; set; }
        public bool independent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models.Supervisor
{
    public class Supervisor
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public DateTime? DateTimeModified { get; set; }
        public int? ModifiedByUserId { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? ProvinceId { get; set; }
    }
}

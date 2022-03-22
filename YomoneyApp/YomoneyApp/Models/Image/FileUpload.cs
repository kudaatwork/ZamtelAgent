using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models.Image
{
    public class FileUpload
    {
        public string PhoneNumber { get; set; }      
        public string Image { get; set; }       
        public string Type { get; set; }        
        public string Purpose { get; set; }
        public string Name { get; set; }       
        public string SupplierId { get; set; }
        public long ServiceId { get; set; }
        public long ActionId { get; set; }
        public string FormId { get; set; }
        public string FieldId { get; set; }
        public string RecordId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    public class ServiceFile
    {
        // model for a file upload
        public long Id { set; get; }
        public long ServiceId { set; get; }
        public long ActionId { set; get; }
        public long ProductId { set; get; }
        public string SupplierId { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string FileType { set; get; }
        public string path { set; get; }
        public bool IsDownloadable { set; get; }

    }
}

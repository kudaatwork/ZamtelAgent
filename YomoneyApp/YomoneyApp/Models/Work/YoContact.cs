using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models.Work
{
    public class YoContact
    {
        public int Id { get; set; }
        public string YoId { get; set; }
        public long JobId { get; set; }
        public long AgentId { set; get; }
        public long BidId { set; get; }
        public long ServiceId { set; get; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public string Avator { get; set; }
        public long UnreadCount { set; get; }
        public DateTime DateTime { get; set; }
    }
}

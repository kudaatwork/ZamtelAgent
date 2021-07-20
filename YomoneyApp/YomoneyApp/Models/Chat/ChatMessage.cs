using System;

namespace YomoneyApp
{
	public class ChatMessage
	{

        public long Id { get; set; }
        public Nullable<long> msgId { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public string avatar { get; set; }
        public string message { get; set; }
        public DateTime Date { get; set; }
        public string time { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public bool unread { get; set; }
        public bool IsMine { get; set; }
        public bool Sent { get; set; }
        public bool Received { get; set; }
        public long unreadCount { set; get; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string image { get; set; }
        public string RequestType { set; get; }
        public long JobId { set; get; }
        public long AgentId { set; get; }
        public long BidId { set; get; }
        public long ServiceId { set; get; }
    }
}


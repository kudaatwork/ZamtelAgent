using System;
using System.Threading.Tasks;

namespace YomoneyApp
{
	public interface IChatServices
    {
		Task Connect();
		Task Send(ChatMessage message);
        Task NotificationReceived(ChatMessage message);
        Task GetConversation(ChatMessage message);
        Task markRead(ChatMessage message);
        Task GetUnread(string receiverId);
        Task GetContactsUnread(string receiverId);
        Task GetUnreadCount(string receiverId);
        Task GetSupportList(string receiverId);
        Task JoinRoom(string roomName);
		event EventHandler<string> OnMessageReceived;
        event EventHandler<string> onConnected;
        event EventHandler<string> onNotification;
        event EventHandler<string> onUnread;
        event EventHandler<string> onSupportList;
        event EventHandler<string> onContactsUnread;
        event EventHandler<string> onUnreadCount;
        event EventHandler<string> onRead;
        event EventHandler<string> onOTP;
        event EventHandler<string> onFileUpload;
        event EventHandler<string> onCsAction;
        event EventHandler<string> onSignaturePad;
        event EventHandler<string> onConversation;
        
    }
}


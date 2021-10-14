using System;
using System.Threading.Tasks;
using YomoneyApp.Models;

namespace YomoneyApp
{
	public interface IChatServices
    {
		Task Connect();
		Task Send(ChatMessage message);
        Task NotificationReceived(ChatMessage message);
        Task JoinLocationPointsGroup(RoutesInfo routesInfo);
        Task SendLocationPoints(RoutesInfo routesInfo);
        Task GetConversation(ChatMessage message);
        Task GetLocationPoints(RoutesInfo routesInfo);
        Task markRead(ChatMessage message);
        Task GetUnread(string receiverId);
        Task GetContactsUnread(string receiverId);
        Task GetUnreadCount(string receiverId);
        Task GetSupportList(string receiverId);
        Task JoinRoom(string roomName);

		event EventHandler<string> OnMessageReceived;
        event EventHandler<string> OnLocationPointsUpdate;
        event EventHandler<string> OnJoinLocationPointsGroup;
        event EventHandler<string> OnLocationPointsReceived;
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


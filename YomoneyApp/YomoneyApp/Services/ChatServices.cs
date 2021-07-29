using System;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;
using XamarinChat;
using System.Threading.Tasks;
using YomoneyApp;
using YomoneyApp.Services;
using Newtonsoft.Json;
using System.Linq;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using YomoneyApp.Models.Work;
using System.Collections.Generic;
using System.ComponentModel;

[assembly: Dependency (typeof(ChatServices))]
namespace YomoneyApp
{
	public class ChatServices : IChatServices 
    {
		private readonly HubConnection _connection;
		private readonly IHubProxy _proxy;

		public event EventHandler<string> OnMessageReceived;
        public event EventHandler<string> onNotification;
        public event EventHandler<string> onConnected;
        public event EventHandler<string> onUnread;
        public event EventHandler<string> onSupportList;
        public event EventHandler<string> onContactsUnread;
        public event EventHandler<string> onUnreadCount;
        public event EventHandler<string> onRead;
        public event EventHandler<string> onOTP;
        public event EventHandler<string> onFileUpload;
        public event EventHandler<string> onCsAction;
        public event EventHandler<string> onSignaturePad;
        public event EventHandler<string> onConversation;
        AccessSettings acnt = new Services.AccessSettings();
        
        public ChatServices ()
		{
			_connection = new HubConnection ("https://www.yomoneyservice.com");
			_proxy = _connection.CreateHubProxy ("ChatHub");
		}

        #region IChatServices implementation
        #region Reconnection 
        public ConnectionState ConnectionState { get { return _connection.State; } }
        public bool IsConnectedOrConnecting
        {
            get
            {
                return _connection.State != ConnectionState.Disconnected;
            }
        }
      
        #endregion
        public async Task Connect ()
		{

            if (Device.RuntimePlatform == Device.Android)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Contacts);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Contacts);
                } 
            }
            string uname = acnt.UserName;
            
            
            await _connection.Start();
            
            await _proxy.Invoke("Connect", uname);
            //await _proxy.Invoke("getUnread", uname);
            
            _proxy.On("sendPrivateMessage", (string message, string IsMine) => OnMessageReceived(this, message));

            _proxy.On("sendNotification", (string message) => onNotification(this, message));

            _proxy.On ("sendPrivateOffline", (string name, string message) => onConnected(this, message));

            _proxy.On("onConnected", (string name, string msg) => onConnected(this, msg));

            _proxy.On("onUnread", (string Id, string msgs) => onUnread(this,  msgs));

            _proxy.On("onSupportList", (string Id, string msgs) => onSupportList(this, msgs));

            _proxy.On("onContactsUnread", (string Id, string msgs) => onContactsUnread(this, msgs));

            _proxy.On("onUnreadCount", (string Id, string msgs) => onUnreadCount(this, msgs));

            _proxy.On("onRead", (string Id, string msgs) => onRead(this, msgs));

            _proxy.On("onCsAction", (string Id, string msgs) => onRead(this, msgs));

            _proxy.On("onFileUpload", (string Id, string msgs) => onRead(this, msgs));

            _proxy.On("onOTP", (string Id, string msgs) => onRead(this, msgs));

            _proxy.On("onSignaturePad", (string Id, string msgs) => onRead(this, msgs));

            _proxy.On("onConversation", (string Id, string msgs) => onConversation(this, msgs));
        }

		public async Task Send (ChatMessage message)
		{
			await _proxy.Invoke ("sendPrivateMessage", message.ReceiverId , message.message,message.RequestType );
		}

        public async Task NotificationReceived(ChatMessage message)
        {
            await _proxy.Invoke("sendNotificationReceived", message.ReceiverId, message.Id, message.RequestType);
        }
        public async Task GetContactsUnread(string UserId)
        {
            AccessSettings ac = new AccessSettings();
            string dbPath = ac.GetSetting("dbPath").Result;
            string clist = "";
            List<YoContact> conts = new List<YoContact>();
            var db = new YomoneyRepository(dbPath);
            
            try
            {
                var yoconts = await db.GetContactsAsync();
                
                var contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();

                foreach (var contact in contacts)
                {
                    if (contact.Number != null)
                    {
                        contact.Number = GetNumbers(contact.Number);
                        //if(yoconts != null && yoconts.Count > 0)
                        //{
                            var rcont = yoconts.Where(u => u.Phone == contact.Number).FirstOrDefault();
                            if (rcont != null)
                            {
                                contacts.Remove(contact);
                            }
                            else
                            {
                                YoContact cntct = new YoContact();
                                cntct.Name = contact.Name;
                                cntct.Phone = contact.Number;
                                cntct.Email = contact.Email;
                                if (cntct.Avator == null)
                                {
                                    Random rr = new Random();
                                    var r = rr.Next(1, 14);
                                    cntct.Avator = "Tile" + r + ".png";
                                    cntct.Address = cntct.Name.Substring(0, 1);
                                }
                            conts.Add(cntct);
                            }
                        //}
                    }
                }
                //var msgs = JsonConvert.DeserializeObject<List<YoContact>>(messages);

                //string dbPath = acnt.GetSetting("dbPath").Result;
                //var db = new YomoneyRepository(dbPath);
                var succ = await db.AddRangeContactAsync(conts);
                clist = JsonConvert.SerializeObject(conts);
               await _proxy.Invoke("getContacts", UserId, clist);
            }
            catch (Exception e)
            {
                // var yoconts = await db.GetContactsAsync();
                await _connection.Start();
                await _proxy.Invoke("getContacts", UserId, clist);
                // await GetUnread(UserId);
            }
        }

        public async Task GetSupportList(string UserId)
        {
            try
            {
                await _proxy.Invoke("getSupportList", UserId);
            }
            catch (Exception e)
            {

            }
        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public async Task GetUnread(string UserId)
        {
            try
            {
                await _proxy.Invoke("getUnread", UserId);
            }
            catch (Exception e)
            {

            }
        }

        public async Task GetUnreadCount(string UserId)
        {
            try
            {
                await _proxy.Invoke("getUnreadCount", UserId);
            }
            catch { }
        }

        public async Task markRead(ChatMessage message)
        {
            try
            {
                var msgs = JsonConvert.SerializeObject(message);
                await _proxy.Invoke("markRead", msgs);
            }
            catch { }
        }

        public async Task GetConversation(ChatMessage message)
        {
            try
            {
                var msgs = JsonConvert.SerializeObject(message);
                await _proxy.Invoke("getConversation", msgs);
            }
            catch { }
        }

        public async Task JoinRoom(string roomName)
		{
            try
            {
                await _proxy.Invoke("JoinRoom", roomName);
            }
            catch { }
		}

		#endregion
	}
}


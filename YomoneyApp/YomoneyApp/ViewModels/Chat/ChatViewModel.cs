using MvvmHelpers;
using YomoneyApp.Services;
using YomoneyApp.Views.Chat;
using YomoneyApp.Views.TemplatePages;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using System.IO;
using Plugin.Toasts;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Webview;
using YomoneyApp;
using YomoneyApp.Models.Work;
using YomoneyApp.Models;
using Xamarin.Forms.PlatformConfiguration;
using static Android.App.ActivityManager;

namespace YomoneyApp
{
    public class ChatViewModel : ViewModelBase
    {
        bool showAlert = false;
        private IChatServices _chatServices;
        private string _roomName = "PrivateRoom";
        public string HostDomain = "https://www.yomoneyservice.com";
        AccessSettings acnt = new Services.AccessSettings();
        string dbPath = "";
        #region ViewModel Properties

        private ObservableRangeCollection<LocationPoints> _polyLinePoints;
        public ObservableRangeCollection<LocationPoints> PolyLinePoints
        {
            get { return _polyLinePoints; }
            set
            {
                _polyLinePoints = value;
                OnPropertyChanged("PolyLinePoints");
            }
        }

        private ObservableRangeCollection<ChatMessage> _messages;
        public ObservableRangeCollection<ChatMessage> Messages {
            get { return _messages; }
            set {
                _messages = value;
                OnPropertyChanged("Messages");
            }
        }

        private ObservableRangeCollection<YoContact> _contacts;
        public ObservableRangeCollection<YoContact> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                OnPropertyChanged("Contacts");
            }
        }

        private ObservableRangeCollection<YoContact> _helpDesks;
        public ObservableRangeCollection<YoContact> HelpDesks
        {
            get { return _helpDesks; }
            set
            {
                _helpDesks = value;
                OnPropertyChanged("HelpDesks");
            }
        }

        private LocationPoints _locationPoints;
        public LocationPoints LocationPoints
        {
            get { return _locationPoints; }
            set
            {
                _locationPoints = value;
                OnPropertyChanged("LocationPoints");
            }
        }

        private ChatMessage _chatMessage;
        public ChatMessage ChatMessage {
            get { return _chatMessage; }
            set {
                _chatMessage = value;
                OnPropertyChanged("ChatMessage");
            }
        }

        private ChatMessage _lastMessage;
        public ChatMessage LastMessage
        {
            get { return _lastMessage; }
            set
            {
                _lastMessage = value;
                OnPropertyChanged("ChatMessage"); // to be tested
            }
        }

        string chatText = string.Empty;
        public string ChatText
        {
            get { return chatText; }
            set { SetProperty(ref chatText, value); }
        }

        string contactSearch = string.Empty;
        public string ContactSearch
        {
            get { return ContactSearch; }
            set { SetProperty(ref contactSearch, value); }
        }

        string messageCount = string.Empty;
        public string MessageCount
        {
            get { return messageCount; }
            set { SetProperty(ref messageCount, value); }
        }

        bool hasMessages = false;
        public bool HasMessages
        {
            get { return hasMessages; }
            set { SetProperty(ref hasMessages, value); }
        }

        #endregion

        public ChatViewModel(Page page) : base(page)
        {
            dbPath = acnt.GetSetting("dbPath").Result;
            _chatServices = DependencyService.Get<IChatServices>();
            _lastMessage = new ChatMessage();
            _chatMessage = new ChatMessage();
            _locationPoints = new LocationPoints();
            _polyLinePoints = new ObservableRangeCollection<LocationPoints>();
            //_locationPoints = new LocationPoints();
            _messages = new ObservableRangeCollection<ChatMessage>();
            _contacts = new ObservableRangeCollection<YoContact>();
            _helpDesks = new ObservableRangeCollection<YoContact>();

            _chatServices.Connect();

            _chatServices.OnLocationPointsUpdate += _chatServices_OnLocationUpdate;
            _chatServices.OnMessageReceived += _chatServices_OnMessageReceived;
            _chatServices.onNotification += _chatServices_onNotification;
            _chatServices.onConnected += _chatServices_onConnected;
            _chatServices.onUnread += _chatServices_onUnread;
            _chatServices.onContactsUnread += _chatServices_onContactsUnread;
            _chatServices.onUnreadCount += _chatServices_onUnreadCount;
            _chatServices.onSupportList += _chatServices_onSupportList;
            _chatServices.onRead += _chatServices_onRead;
            _chatServices.onOTP += _chatServices_onOTP;
            _chatServices.onFileUpload += _chatServices_onFileUpload;
            _chatServices.onCsAction += _chatServices_onCsAction;
            _chatServices.onSignaturePad += _chatServices_onSignaturePad;
            _chatServices.onConversation += _chatServices_onConversation;
        }

        public Action<ChatMessage> ItemSelected { get; set; }
        public Action<YoContact> ContactSelected { get; set; }
        public Action<RoutesInfo> PointSelected { get; set; }

        ChatMessage selectedChat;

        public ChatMessage SelectedChat
        {
            get { return selectedChat; }
            set
            {
                selectedChat = value;
                OnPropertyChanged("SelectedChat");

                if (selectedChat == null)
                    return;

                if (ItemSelected == null)
                {
                    //selected chat bid
                    //var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;
                    var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;
                    try
                    {
                        if (SelectedChat.ReceiverName != "You have no active yomoney contacts")
                        {
                            page.Navigation.PushModalAsync(new MassagingPagexaml(Id));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    SelectedChat = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedChat);
                }
            }
        }

        YoContact selectedContact;

        public YoContact SelectedContact
        {
            get { return selectedContact; }
            set
            {
                selectedContact = value;
                OnPropertyChanged("SelectedContact");

                if (selectedContact == null)
                    return;

                if (ContactSelected == null)
                {
                    //selected chat bid
                    //var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;
                    #region getmsg page
                    var Id = SelectedContact.Phone + "_" + SelectedContact.Name + "_" + SelectedContact.Phone + "_" + "Job" + "_" + SelectedContact.BidId + "_" + SelectedContact.JobId;
                    try
                    {
                        // page.Navigation.PushModalAsync(new MassagingPagexaml(Id));
                        if (page.Navigation.NavigationStack.Count == 0 ||
                        page.Navigation.NavigationStack.Last().GetType() != typeof(WebviewPage))
                        {
                            page.Navigation.PushModalAsync(new WebviewPage("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + SelectedContact.Phone.ToString(), "Job Profile", true, null));
                            page.Navigation.PopAsync();
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion
                    SelectedContact = null;
                }
                else
                {
                    ContactSelected.Invoke(selectedContact);
                }
            }
        }

        YoContact selectedSupport;

        public YoContact SelectedSupport
        {
            get { return selectedSupport; }
            set
            {
                selectedSupport = value;
                OnPropertyChanged("SelectedSupport");

                if (selectedSupport == null)
                    return;

                if (ContactSelected == null)
                {
                    //selected chat bid
                    //var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;
                    var Id = SelectedSupport.Phone + "_" + SelectedSupport.Name + "_" + SelectedSupport.AgentId + "_" + "Support" + "_" + SelectedSupport.BidId + "_" + SelectedSupport.JobId;
                    try
                    {
                        page.Navigation.PushModalAsync(new MassagingPagexaml(Id));
                    }
                    catch (Exception ex)
                    {

                    }
                    SelectedContact = null;
                }
                else
                {
                    ContactSelected.Invoke(selectedContact);
                }
            }
        }

        RoutesInfo selectedPoint;

        public RoutesInfo SelectedPoint
        {
            get { return selectedPoint; }
            set
            {
                selectedPoint = value;
                OnPropertyChanged("SelectedPoint");

                if (selectedPoint == null)
                    return;

                if (SelectedPoint == null)
                {
                    //selected chat bid
                    //var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;

                    SelectedPoint = null;
                    selectedPoint = null;
                }
                else
                {
                    PointSelected.Invoke(selectedPoint);
                }
            }
        }

        void _chatServices_OnMessageReceived(object sender, string e)
        {
            var msg = JsonConvert.DeserializeObject<ChatMessage>(e);

            var db = new YomoneyRepository(dbPath);

            var edate = msg.Date.AddMinutes(1);

           var today = DateTime.Now.Date;
            var dbmm = db.GetMessagesAsync().Result.ToList();
            _messages.Clear();
            _messages.AddRange(dbmm);

            var dm = db.GetMessagesAsync().Result.Where(u => u.message == msg.message && (u.Date >= msg.Date && u.Date <= edate)).FirstOrDefault();

            if (msg.IsMine)
            {
                msg.Sent = true;
                msg.Received = false;
            }
            else
            {
                msg.Sent = false;
                msg.Received = true;
            }

            if (msg.Date.Date.ToUniversalTime() == DateTime.Now.Date.ToUniversalTime())
            {
                msg.time = msg.Date.ToString("HH:mm");
            }
            else
            {
                msg.time = msg.Date.ToString("dd MMM yyyy HH:mm");
            }

            //var dml =_messages.Where(u => u.message == msg.message && (u.Date >= msg.Date && u.Date <= edate)).FirstOrDefault();

            if (dm == null)
            {
                _messages.Add(msg);

                try
                {
                    var msdAdded = db.AddMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (string.IsNullOrEmpty(messageCount)) messageCount = "0";

                messageCount = (long.Parse(messageCount) + 1).ToString();
                hasMessages = true;
                ChatText = "";

                if (msg.Received)
                {
                    if (isApplicationInTheBackground())
                    {
                        try
                        {
                            var notificator = DependencyService.Get<IToastNotificator>();

                            var options = new NotificationOptions()
                            {
                                Title = msg.SenderName,
                                Description = msg.message,
                                AllowTapInNotificationCenter = true,
                                IsClickable = true
                            };

                            var result = notificator.Notify(options);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        #region Sound Notification
                        try
                        {
                            var assembly = typeof(App).GetTypeInfo().Assembly;
                            Stream audioStream = assembly.GetManifestResourceStream("YomoneyApp." + "notify.wav");
                            // var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                            // player.Load(audioStream);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        #endregion

                    }
                    else
                    {
                        try
                        {
                            var assembly = typeof(App).GetTypeInfo().Assembly;
                            Stream audioStream = assembly.GetManifestResourceStream("YomoneyApp." + "notify.wav");
                            // var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                            // player.Load(audioStream);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            //LastMessage = msg;

            // MessageList.ScrollTo(LastMessage, ScrollToPosition.MakeVisible, false);
        }

        void _chatServices_onNotification(object sender, string e)
        {

            var msg = JsonConvert.DeserializeObject<ChatMessage>(e);
            if (msg.IsMine)
            {
                msg.Sent = true;
                msg.Received = false;
            }
            else
            {
                msg.Sent = false;
                msg.Received = true;
            }

            if (msg.Date.Date.ToUniversalTime() == DateTime.Now.Date.ToUniversalTime())
            {
                msg.time = msg.Date.ToString("HH:mm");
            }
            else
            {
                msg.time = msg.Date.ToString("dd MMM yyyy HH:mm");
            }
            var dm = _messages.Where(u => u.message == msg.message && u.Date == msg.Date).FirstOrDefault();
            if (dm == null)
            {
                _messages.Add(msg);
                if (string.IsNullOrEmpty(messageCount)) messageCount = "0";
                messageCount = (long.Parse(messageCount) + 1).ToString();
                hasMessages = true;

                try
                {
                    var assembly = typeof(App).GetTypeInfo().Assembly;
                    Stream audioStream = assembly.GetManifestResourceStream("YomoneyApp." + "notify.wav");
                    var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                    player.Load(audioStream);
                }
                catch (Exception s) { }

                try
                {
                    var notificator = DependencyService.Get<IToastNotificator>();

                    var options = new NotificationOptions()
                    {
                        Title = msg.SenderName,
                        Description = msg.message
                    };

                    var result = notificator.Notify(options);
                }
                catch (Exception n) { }
                _chatServices.NotificationReceived(dm);

            }
            //LastMessage = msg;

            //MessageList.ScrollTo(LastMessage, ScrollToPosition.MakeVisible, false);
        }

        void _chatServices_onConnected(object sender, string e)
        {
            var msgs = new ChatMessage();
            try
            {
                msgs = JsonConvert.DeserializeObject<ChatMessage>(e);
            }
            catch
            {
                msgs.message = e;
            }
            _messages.Add(msgs);
        }

        async void _chatServices_onUnread(object sender, string messages)
        {
            var nw = DateTime.Now;
            if (nw.Subtract(contactReceive).TotalMilliseconds > 100)
            {
                contactReceive = DateTime.Now;
                #region message
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                // string uname = acnt.UserName;
                var msgs = JsonConvert.DeserializeObject<List<ChatMessage>>(messages);

                var nn = msgs.GroupBy(u => u.JobId)
                        .Select(grp => grp.First()).ToList();
                List<YoContact> resp = new List<YoContact>();
                if (nn.Count == 0)
                {
                    YoContact mn = new YoContact();
                    mn.Name = "You have no contacts on yoapp";
                    mn.Avator = "https://www.yomoneyservice.com/Content/Spani/Images/avator.jpg";

                    //mn.IsMine = true;
                    resp.Add(mn);
                    _contacts.ReplaceRange(resp);
                }
                else
                {
                    dbPath = acnt.GetSetting("dbPath").Result;
                    var db = new YomoneyRepository(dbPath);
                    foreach (var m in nn)
                    {
                        YoContact mn = new YoContact();
                        mn.Name = m.ReceiverName;
                        if (m.avatar != null)
                        {
                            // mn.Avator = m.avatar;
                            Random rr = new Random();
                            var r = rr.Next(1, 14);
                            mn.Avator = "Tile" + r + ".png";
                            mn.Address = mn.Name.Substring(0, 1);
                        }
                        else
                        {
                            Random rr = new Random();
                            var r = rr.Next(1, 14);
                            mn.Avator = "Tile" + r + ".png";
                            mn.Address = mn.Name.Substring(0, 1);

                        }
                        mn.Phone = m.ReceiverId;
                        mn.Skills = m.message;
                        var yoconts = await db.AddContactAsync(mn);
                    }
                    resp = await db.GetContactsAsync();

                    _contacts.ReplaceRange(resp);
                }
                #endregion
            }

        }

        void _chatServices_onSupportList(object sender, string messages)
        {
            var nw = DateTime.Now;
            if (nw.Subtract(contactReceive).TotalMilliseconds > 100)
            {
                contactReceive = DateTime.Now;
                #region message
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                // string uname = acnt.UserName;
                var nn = JsonConvert.DeserializeObject<List<YoContact>>(messages);

                //var nn = msgs.GroupBy(u => u.JobId)
                //        .Select(grp => grp.First()).ToList();
                if (nn.Count == 0)
                {
                    YoContact mn = new YoContact();
                    mn.Skills = "You have no active service support centers";
                    mn.Avator = HostDomain + "/Content/Spani/Images/myServices.jpg";
                    List<YoContact> resp = new List<YoContact>();

                    resp.Add(mn);
                    _helpDesks.ReplaceRange(resp);
                }
                else
                {
                    _helpDesks.ReplaceRange(nn);
                }
                #endregion
            }

        }

        async void _chatServices_onContactsUnread(object sender, string messages)
        {
            var nw = DateTime.Now;
            if (nw.Subtract(contactReceive).TotalMilliseconds > 100)
            {
                contactReceive = DateTime.Now;
                #region message

                var msgs = JsonConvert.DeserializeObject<List<YoContact>>(messages);

                //string dbPath = acnt.GetSetting("dbPath").Result;
                var db = new YomoneyRepository(dbPath);
                var yoconts = await db.AddRangeContactAsync(msgs);

                var nn = await db.GetContactsAsync();
                //var nn = msgs.GroupBy(u => u.JobId)
                //    .Select(grp => grp.First()).ToList();
                if (nn.Count == 0)
                {
                    YoContact mn = new YoContact();
                    mn.Skills = "You have no active yomoney contacts";
                    mn.Avator = "https://www.yomoneyservice.com/Content/Spani/Images/Contactchat.jpg";
                    List<YoContact> resp = new List<YoContact>();
                    resp.Add(mn);
                    _contacts.ReplaceRange(resp);
                }
                else
                {
                    foreach (var mn in nn)
                    {
                        if (mn.Avator != null)
                        {
                            // mn.Avator = m.avatar;
                            Random rr = new Random();
                            var r = rr.Next(1, 14);
                            mn.Avator = "Tile" + r + ".png";
                            mn.Address = mn.Name.Substring(0, 1);
                        }
                        else
                        {
                            Random rr = new Random();
                            var r = rr.Next(1, 14);
                            mn.Avator = "Tile" + r + ".png";
                            mn.Address = mn.Name.Substring(0, 1);

                        }

                    }
                    _contacts.ReplaceRange(nn);
                }
                #endregion
            }

        }

        void _chatServices_onUnreadCount(object sender, string Count)
        {
            messageCount = Count;
            if (!string.IsNullOrEmpty(Count) && long.Parse(Count) > 0)
            {
                HasMessages = true;
            }
        }

        void _chatServices_onRead(object sender, string messages)
        {
            AccessSettings acnt = new Services.AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;
            // string uname = acnt.UserName;
            var msgs = JsonConvert.DeserializeObject<List<ChatMessage>>(messages);
            foreach (var msg in msgs)
            {
                if (msg.SenderId == uname)
                {
                    msg.Sent = true;
                    msg.Received = false;

                }
                else
                {
                    msg.Sent = false;
                    msg.Received = true;
                }
                if (msg.avatar == null)
                {
                    msg.avatar = "https://www.yomoneyservice.com/Content/Administration/images/user.png";
                }
                if (msg.Date.Date.ToUniversalTime() == DateTime.Now.Date.ToUniversalTime())
                {
                    msg.time = msg.Date.ToString("HH:mm");
                }
                else
                {
                    msg.time = msg.Date.ToString("dd MMM yyyy HH:mm");
                }
            }
            var nn = msgs.GroupBy(u => u.JobId)
                    .Select(grp => grp.First()).ToList();
            _messages.ReplaceRange(nn);
        }

        void _chatServices_onFileUpload(object sender, string messages)
        {
            var px = JsonConvert.DeserializeObject<MenuItem>(messages);
            page.Navigation.PushAsync(new FileUploadPage(px));
        }

        void _chatServices_OnLocationUpdate(object sender, string e)
        {
            var location = JsonConvert.DeserializeObject<LocationPoints>(e);

            if (String.IsNullOrEmpty(location.Latitude.ToString()) && String.IsNullOrEmpty(location.Longitude.ToString()))
            {
                var db = new YomoneyRepository(dbPath);

                _polyLinePoints.Add(location);
            }
        }

        void _chatServices_onCsAction(object sender, string message)
        {

            string[] part = message.Split('_');
            //                                    name    method   arg
            MessagingCenter.Send<string, string>(part[0], part[1], part[2]);
        }

        void _chatServices_onSignaturePad(object sender, string messages)
        {
            var px = JsonConvert.DeserializeObject<MenuItem>(messages);
            page.Navigation.PushAsync(new SignaturePage(px));
        }

        async void _chatServices_onOTP(object sender, string messages)
        {
            var px = JsonConvert.DeserializeObject<MenuItem>(messages);

            switch (px.TransactionType)
            {
                case 00: // webpay respons   
                    if (px.Description == "Success" || px.Description == "00000")
                    {
                        MessagingCenter.Send<string, string>("PaymentRequest", "NotifyMsg", "Payment Success");
                    }
                    else
                    {
                        await page.DisplayAlert("Payment Error", px.Description, "OK");
                    }
                    break;
                case 10:
                case 9: // webview  
                    if (px.Note == "External")
                    {
                        await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false, px.ThemeColor));
                    }
                    else
                    {
                        await page.Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                    }
                    break;
                case 3:// "Payment":
                    await page.Navigation.PushAsync(new PaymentPage(px));
                    break;
                case 13: // OTP        
                    await page.Navigation.PushAsync(new OTPPage(px));
                    break;
                case 14: // file upload       
                    await page.Navigation.PushAsync(new FileUploadPage(px));
                    break;
                case 15: // Signature 
                    await page.Navigation.PushAsync(new SignaturePage(px));
                    break;
                case 16: // Signature 
                    await page.Navigation.PushAsync(new SharePage(px));
                    break;
            }
        }

        void _chatServices_onConversation(object sender, string messages)
        {
            AccessSettings acnt = new Services.AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;
            IsBusy = true;
            //string uname = acnt.UserName;
            var msgs = JsonConvert.DeserializeObject<List<ChatMessage>>(messages);

            foreach (var msg in msgs)
            {
                if (msg.SenderId == uname)
                {
                    msg.Sent = true;
                    msg.Received = false;
                }
                else
                {
                    msg.Sent = false;
                    msg.Received = true;
                }
                if (msg.avatar == null)
                {
                    msg.avatar = "https://www.yomoneyservice.com/Content/Administration/images/user.png";
                }
                if (msg.Date.Date.ToUniversalTime() == DateTime.Now.Date.ToUniversalTime())
                {
                    msg.time = msg.Date.ToString("HH:mm");
                }
                else
                {
                    msg.time = msg.Date.ToString("dd MMM yyyy HH:mm");
                }
            }

            LastMessage = msgs.OrderBy(x => x.Id).Last();
            _messages.ReplaceRange(msgs);
        }

        private bool isApplicationInTheBackground()
        {
            bool isInBackground = true;

            RunningAppProcessInfo myProcess = new RunningAppProcessInfo();
            Android.App.ActivityManager.GetMyMemoryState(myProcess);
            isInBackground = myProcess.Importance != Android.App.Importance.Foreground;

            return isInBackground;
        }

        #region Send Message Command

        Command sendMessageCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command SendMessageCommand {
            get {
                /*return sendMessageCommand ??
                    (sendMessageCommand = new Command(async () => await ExecuteSendMessageCommand(), () => { return !IsBusy; }));*/
                return sendMessageCommand ??
                (sendMessageCommand = new Command(ExecuteSendMessageCommand));
            }
        }



        async void ExecuteSendMessageCommand()
        {
            if (IsBusy)
                return;

            Message = "Sending Message...";
            IsBusy = true;
            sendMessageCommand?.ChangeCanExecute();

            try
            {
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                //string uname = acnt.UserName;

                IsBusy = false;
                await _chatServices.Send(new ChatMessage { SenderId = acnt.UserName, ReceiverId = _chatMessage.ReceiverId, message = ChatText, RequestType = _chatMessage.RequestType });
                ChatText = "";
            }
            catch { }
        }

        #endregion

        #region markRead Command

        Command markReadCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command MarkReadCommand
        {
            get
            {
                return markReadCommand ??
                (markReadCommand = new Command(ExecuteMarkReadCommand));
            }
        }

        void ExecuteMarkReadCommand()
        {
            try
            {
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                //string uname = acnt.UserName;

                IsBusy = false;
                _chatServices.markRead(new ChatMessage { SenderId = acnt.UserName, ReceiverId = _chatMessage.ReceiverId, message = ChatText, RequestType = _chatMessage.RequestType });
                ChatText = "";
            }
            catch { }
        }

        #endregion

        #region Get unReadCount Command

        Command getUnreadCountCommand;

        public Command GetUnreadCountCommand
        {
            get
            {
                return getUnreadCountCommand ??
                (getUnreadCountCommand = new Command(ExecuteGetUnreadCountCommand));
            }
        }

        async void ExecuteGetUnreadCountCommand()
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                //string pass = acnt.Password;
                string uname = acnt.UserName;
                // string uname = acnt.UserName;
                await _chatServices.GetUnreadCount(uname);
                // IsBusy = false;
            }
            catch { }
        }

        #endregion

        #region Get Messages Command

        Command getUnreadCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetUnreadCommand
        {
            get
            {
                return getUnreadCommand ??
                (getUnreadCommand = new Command(ExecuteGetUnreadCommand));
            }
        }

        async void ExecuteGetUnreadCommand()
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                await _chatServices.GetUnread(uname);
                // IsBusy = false;
            }
            catch { }
        }

        #endregion

        #region Get Sync Contacts Command

        Command getContactsCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetContactsCommand
        {
            get
            {
                return getContactsCommand ??
                (getContactsCommand = new Command(async () => await ExecuteGetContactsCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetContactsCommand()
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();

                string dbPath = acnt.GetSetting("dbPath").Result;

                string uname = acnt.UserName;
                var db = new YomoneyRepository(dbPath);
                var yoconts = await db.GetContactsAsync();
                foreach (var contact in yoconts)
                {
                    if (contact.Avator == null)
                    {
                        Random rr = new Random();
                        var r = rr.Next(1, 14);
                        contact.Avator = "Tile" + r + ".png";
                        contact.Address = contact.Name.Substring(0, 1);
                    }
                }
                _contacts.ReplaceRange(yoconts);

                await _chatServices.GetContactsUnread(uname);
                // IsBusy = false;
            }
            catch { }
        }

        #endregion

        #region Get Get Support Command

        Command getSupportCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetSupportCommand
        {
            get
            {
                return getSupportCommand ??
                (getSupportCommand = new Command(ExecuteGetSupportCommand));
            }
        }

        async void ExecuteGetSupportCommand()
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                await _chatServices.GetSupportList(uname);
                // IsBusy = false;
            }
            catch { }
        }

        #endregion

        #region Get Local Contacts List 

        Command getContactsListCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetContactsListCommand
        {
            get
            {
                return getContactsListCommand ??
                (getContactsListCommand = new Command(ExecuteGetContactsListCommand));
            }
        }

        async void ExecuteGetContactsListCommand()
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string uname = acnt.UserName;
                string dbPath = acnt.GetSetting("dbPath").Result;
                var db = new YomoneyRepository(dbPath);
                var yoconts = await db.GetContactsAsync();
                List<YoContact> nn = new List<YoContact>();
                if (yoconts.Count > 0)
                {
                    _contacts.ReplaceRange(yoconts);
                }
                else
                {
                    await _chatServices.GetUnread(uname);
                }

                // IsBusy = false;
            }
            catch (Exception e) {

            }
        }

        #endregion

        #region Get Chats Command
        Command getChatsCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetChatsCommand
        {
            get
            {
                return getChatsCommand ??
                (getChatsCommand = new Command(ExecuteGetChatsCommand));
            }
        }

        async void ExecuteGetChatsCommand()
        {

            try
            {
                //IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                var db = new YomoneyRepository(dbPath);
                
                var today = DateTime.Now.Date;
                var dbmm = db.GetMessagesAsync().Result.ToList();
                _messages.Clear();
                _messages.AddRange(dbmm);
                                               
            }
            catch (Exception ex)
            {
                showAlert = true;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                getChatsCommand.ChangeCanExecute();
            }                      
                if (showAlert)
                    await page.DisplayAlert("Oh Oooh", "There has been an error in gathering messages.", "OK");
        }

        #endregion

        #region Get Conversation Command

        Command getConversationCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetConversationCommand
        {
            get
            {
                return getConversationCommand ??
                (getConversationCommand = new Command(ExecuteGetConversationCommand));
            }
        }

        async void ExecuteGetConversationCommand()
        {

            try
            {
                IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                //string uname = acnt.UserName;
                _chatMessage.SenderId = uname;
                await _chatServices.GetConversation(_chatMessage);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                showAlert = true;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                getConversationCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh", "There has been an error in gathering messages.", "OK");
        }

        #endregion

        #region send location

        Command sendLocationPointCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command SendLocationPointCommand
        {
            get
            {
                return sendLocationPointCommand ??
                (sendLocationPointCommand = new Command(async () => ExecuteSendLocationPointCommand(selectedPoint), () => { return !IsBusy; }));
            }
        }
        public void SendLocationPoint(RoutesInfo lp)
        {

            selectedPoint = lp;
            OnPropertyChanged("SelectedPoint");
            if (selectedPoint == null)
                return;

            if (ItemSelected == null)
            {
                selectedPoint = null;
                SelectedPoint = null;
            }
            else
            {
                PointSelected.Invoke(selectedPoint);
            }
        }

        public async void ExecuteSendLocationPointCommand(RoutesInfo lp)
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                await _chatServices.SendLocationPoints(lp);
                // IsBusy = false;
            }
            catch { }
        }


        Command joinLocationPointGroupCommand;

        public Command JoinLocationPointGroupCommand
        {
            get
            {
                return joinLocationPointGroupCommand ??
                (joinLocationPointGroupCommand = new Command(async () => ExecuteJoinLocationPointGroupCommand(selectedPoint), () => { return !IsBusy; }));
            }
        }
        public async void ExecuteJoinLocationPointGroupCommand(RoutesInfo lp)
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                await _chatServices.JoinLocationPointsGroup(lp);
                // IsBusy = false;
            }
            catch { }
        }

        #endregion

        #region GetLocationPoints
        Command getLocationPointCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command GetLocationPointCommand
        {
            get
            {
                return getLocationPointCommand ??
                (getLocationPointCommand = new Command(async () => ExecuteGetLocationPointsCommand(selectedPoint), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetLocationPointsCommand(RoutesInfo lp)
        {
            try
            {
                // IsBusy = true;
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                await _chatServices.GetLocationPoints(lp);

                // IsBusy = false;
            }
            catch(Exception ex)            
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Join Room Command

        Command joinRoomCommand;

        /// <summary>
        /// Command to Send Message
        /// </summary>
        public Command JoinRoomCommand {
            get {
                return joinRoomCommand ??
                    (joinRoomCommand = new Command(ExecuteJoinRoomCommand));
            }
        }

        async void ExecuteJoinRoomCommand()
        {
            IsBusy = true;
            await _chatServices.JoinRoom(_roomName);
            IsBusy = false;
        }

        #endregion

        #region cancel 
        private Command getCancelCommand;

        public Command GetCancelCommand
        {
            get
            {
                return getCancelCommand ??
                    (getCancelCommand = new Command(async () => await ExecuteGetCancelCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetCancelCommand()
        {
            await page.Navigation.PopAsync();
        }
        #endregion

        #region Object 

        bool qrpay = false;
        public bool QrPay
        {
            get { return qrpay; }
            set { SetProperty(ref qrpay, value); }
        }

        string message = "Loading...";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }


        DateTime contactReceive;
        public DateTime ContactReceive
        {
            get { return contactReceive; }
            set { SetProperty(ref contactReceive, value); }
        }

        DateTime massageReceive;
        public DateTime MessageReceive
        {
            get { return massageReceive; }
            set { SetProperty(ref massageReceive, value); }
        }

        public Action RefreshScrollDown { get; internal set; }
        #endregion
    }  

}


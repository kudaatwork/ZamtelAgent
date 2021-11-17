using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using YomoneyApp.Models;
using YomoneyApp.Services;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        #region ViewModel Properties & Variables
      
        public string HostDomain = "http://192.168.100.150:5000";

        AccountViewModel accountViewModel = new AccountViewModel(null);
     
        #endregion

        public MapPageViewModel(Page page) : base(page)
        {
        }
        
    }
}

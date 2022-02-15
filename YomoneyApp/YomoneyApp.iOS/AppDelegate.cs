using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using MediaManager;
using Plugin.Toasts;
using UIKit;
using Xamarin.Forms.PlatformConfiguration;
using CarouselView.FormsPlugin.iOS;
using YomoneyApp.Constants;
using FFImageLoading.Forms.Platform;

namespace YomoneyApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            SQLitePCL.Batteries.Init();

           // Xamarin.FormsMaps.Init();
            var dbPath = Path.Combine(System.Environment.GetFolderPath
              (System.Environment.SpecialFolder.Personal), "YomoneyDB.db");
            ToastNotification.Init();
            ImageCircleRenderer.Init();
            //var yomoneyRepository = new YomoneyRepository(dbPath);
            //LibVLCSharpFormsRenderer.Init();
            CrossMediaManager.Current.Init();
            global::Xamarin.Forms.Forms.Init();
            Rg.Plugins.Popup.Popup.Init();
            CachedImageRenderer.Init();
            Xamarin.FormsGoogleMaps.Init(AppConstants.GoogleMapsApiKey);
            CarouselViewRenderer.Init();
            LoadApplication(new App(dbPath));

            return base.FinishedLaunching(app, options);
        }
    }
}

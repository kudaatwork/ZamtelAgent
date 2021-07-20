using System;
using Xamarin.Android;
using Android.Webkit;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using ImageCircle.Forms.Plugin.Droid;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Plugin.Toasts;
using Xam.Plugin.WebView.Droid;
using System.IO;

using MediaManager;
using CarouselView.FormsPlugin.Droid;

//using MediaManager;

namespace YomoneyApp.Droid
{
    [Activity(Label = "YoApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
           
            //CrossCurrentActivity.Current.Init();
            
            DependencyService.Register<ToastNotification>(); // Register your dependency

            // If you are using Android you must pass through the activity
           // builder.setSmallIcon(R.drawable.ic_launcher);
            ToastNotification.Init(this, new PlatformOptions() { SmallIconDrawable = Android.Resource.Drawable.IcDialogInfo });
            ImageCircleRenderer.Init();
            //LibVLCSharpFormsRenderer.Init();
            CrossMediaManager.Current.Init();
            var dbPath = Path.Combine(System.Environment.GetFolderPath
               (System.Environment.SpecialFolder.Personal), "YomoneyDB.db");

            //var yomoneyRepository = new YomoneyRepository(dbPath);
            CarouselViewRenderer.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(dbPath));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

}
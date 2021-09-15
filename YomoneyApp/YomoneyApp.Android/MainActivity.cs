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
using Android;

//using MediaManager;

namespace YomoneyApp.Droid
{
    [Activity(Label = "YoApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;

        readonly string[] LocationPermmissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermmissions, RequestLocationId);
                }
                else
                {
                    // Permissions already granted - Display Message
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {        

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Xamarin.FormsMaps.Init(this, savedInstanceState);

            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);

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
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState); // initialize for Xamarin.Forms.GoogleMaps
            LoadApplication(new App(dbPath));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode == RequestLocationId)
            {
                if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted))
                {
                    //Permissions Granted - Display a Message
                }
                else
                {
                    // Permissions Denied - Display a Message
                }
            }
            else
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }            
        }
    }

}
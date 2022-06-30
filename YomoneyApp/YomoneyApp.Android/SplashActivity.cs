using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using YomoneyApp.Droid;

namespace MyShop.Droid
{
   // [Activity(Label = "SplashActivity")]
    [Activity(Label = "Zamtel App", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            System.Threading.Thread.Sleep(1500);
            StartActivity(typeof(MainActivity));
        }                
    }
}
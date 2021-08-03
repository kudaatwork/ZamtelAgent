using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YomoneyApp.CustomeRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YomoneyApp.Droid.CustomRenderer;

[assembly: ExportRenderer(typeof(MyCustomeEntry), typeof(MyEntryRenderer))]
namespace YomoneyApp.Droid.CustomRenderer
{    
    class MyEntryRenderer: EntryRenderer
    {
        public MyEntryRenderer(Context context) : base(context)
        {

        }
        

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
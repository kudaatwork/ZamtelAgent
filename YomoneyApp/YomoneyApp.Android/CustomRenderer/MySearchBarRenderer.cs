using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Service.Controls;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YomoneyApp.CustomeRenderer;
using YomoneyApp.Droid.CustomRenderer;

[assembly: ExportRenderer(typeof(MySearchBar), typeof(MySearchBarRenderer))]

namespace YomoneyApp.Droid.CustomRenderer
{
    public class MySearchBarRenderer: SearchBarRenderer
    {
        public MySearchBarRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
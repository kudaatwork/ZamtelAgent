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
using Xamarin.Forms;
using YomoneyApp.CustomeRenderer;
using Xamarin.Forms.Platform.Android;
using YomoneyApp.Droid.CustomRenderer;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(MyButton), typeof(MyButtonRenderer))]
namespace YomoneyApp.Droid.CustomRenderer
{
    class MyButtonRenderer: ButtonRenderer
    {
        public MyButtonRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //base.OnElementChanged(e);
            if (Control != null)
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            //Control.SetBackgroundResource(Android.Graphics.ImageDecoder.)
        }
    }
}
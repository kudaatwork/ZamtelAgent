using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YomoneyApp.CustomeRenderer;
using YomoneyApp.Droid.CustomRenderer;

[assembly: ExportRenderer(typeof(MyDataPicker), typeof(MyDataPickerRender))]
namespace YomoneyApp.Droid.CustomRenderer
{
    class MyDataPickerRender : DatePickerRenderer
    {
        public MyDataPickerRender(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
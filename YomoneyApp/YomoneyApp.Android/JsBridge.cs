using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using Xamarin.Forms;


namespace YomoneyApp.Droid
{
    public class JSBridge : Java.Lang.Object, IRunnable
    {
        Android.Content.Context _context;

        public JSBridge(Android.Content.Context context)
        {
            this._context = context;
        }
        public void Run()
        {
            // you need to implment this func because of IRunnable interface but I didn't use it 
            //as I need to send parameter from client side
        }

        [JavascriptInterface]
        [Export("callCSAction")]
        public void CallCSAction(Java.Lang.String MessageName)
        {
            
          
            string[] part = MessageName.Split("-");
            //                                    name    method   arg
            MessagingCenter.Send<string, string>(part[0], part[1], part[2]);
            
        }
    }
}
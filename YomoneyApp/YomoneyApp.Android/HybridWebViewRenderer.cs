using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YomoneyApp.Droid;
using YomoneyApp.Views.TemplatePages;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace YomoneyApp.Droid
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        const string routesFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

        //const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

        Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");

                ((HybridWebView)Element).Cleanup();
            }

            if (e.NewElement != null)
            {
                Control.SetWebViewClient(new JavascriptWebViewClient(this, $"javascript: {routesFunction}"));

                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");

                Control.LoadUrl($"{((HybridWebView)Element).Uri}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((HybridWebView)Element).Cleanup();
            }

            base.Dispose(disposing);
        }
    }
}
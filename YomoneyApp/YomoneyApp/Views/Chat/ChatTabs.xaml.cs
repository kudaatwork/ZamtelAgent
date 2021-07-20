using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatTabs : TabbedPage
    {
        public ChatTabs()
        {
            InitializeComponent();
        }
    }
}
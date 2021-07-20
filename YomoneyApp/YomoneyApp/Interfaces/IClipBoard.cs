using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YomoneyApp.Interfaces
{
    public interface IClipBoard
    {
        String GetTextFromClipBoard();
        void OnCopy(string text);
    }
}

// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Kikitori.GUI
{
    public static class ClipboardHelper
    {
        public static void RunInSta(Action action)
        {
            Thread thread = new Thread(() => action());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        public static object GetClipboardData()
        {
            object result = null;
            RunInSta(() =>
            {
                IDataObject clipboardObject = Clipboard.GetDataObject();
                if (Clipboard.ContainsText())
                {
                    result = clipboardObject.GetData("System.String");
                }
                if (Clipboard.ContainsFileDropList())
                {
                    result = File.ReadAllBytes(((string[])(clipboardObject.GetData("FileName")))[0]);
                }
            });
            return result;
        }
    }
}

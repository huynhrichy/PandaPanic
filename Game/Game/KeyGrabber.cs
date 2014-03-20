using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Game
{
    public class KeyGrabber
    {
        public class KeyFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                /*
                    These are the message constants we will be watching for.
                */
                const int WM_KEYDOWN = 0x0100;
                const int WCHAR_EVENT = 0x0102;

                if (m.Msg == WM_KEYDOWN)
                {
                    /*
                        The TranslateMessage function requires a pointer to be passed to it.
                        Since C# doesn't typically use pointers, we have to make use of the Marshal
                        class to store the Message into a pointer. We can then pass this pointer
                        over to the native function.
                    */
                    IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf(m));
                    Marshal.StructureToPtr(m, pointer, true);
                    TranslateMessage(pointer);
                }
                else if (m.Msg == WCHAR_EVENT)
                {
                    //The WParam parameter contains the true character value
                    //we are after. Print this out to the screen and call the
                    //InboundCharEvent so any events hooked up to this will be
                    //notifed that there is a char ready to be processed.
                    char trueCharacter = (char)m.WParam;
                    Console.WriteLine(trueCharacter);

                    if (InboundCharEvent != null)
                        InboundCharEvent(trueCharacter);
                    System.Threading.Thread.Sleep(100);
                }

                //Returning false allows the message to continue to the next filter or control.
                return false;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern bool TranslateMessage(IntPtr message);
        }

        public static event Action<char> InboundCharEvent;
        static KeyGrabber()
        {
            Application.AddMessageFilter(new KeyFilter());
        }
    }
}
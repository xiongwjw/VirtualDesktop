using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;

namespace VirtualDesktop
{
    public delegate void HotkeyEventHandler(int hotKeyID);

    public class HotkeyHelper : IMessageFilter
    {
        public event HotkeyEventHandler OnHotkey;

        public enum KeyFlags
        {
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        class NativeMethods
        {
            private NativeMethods() { }

            #region WIN32 API

            [DllImport("user32.dll")]
            public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

            [DllImport("user32.dll")]
            public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalAddAtom(String lpString);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);

            #endregion
        }

        Hashtable keyIDs = new Hashtable();
        IntPtr hWnd;

        public HotkeyHelper(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            Application.AddMessageFilter(this);
        }

        public int RegisterHotkey(Keys Key, KeyFlags keyflags)
        {
            UInt32 hotkeyid = NativeMethods.GlobalAddAtom(System.Guid.NewGuid().ToString());
            NativeMethods.RegisterHotKey((IntPtr)hWnd, hotkeyid, (UInt32)keyflags, (UInt32)Key);
            keyIDs.Add(hotkeyid, hotkeyid);
            return (int)hotkeyid;
        }

        public void UnregisterHotkeys()
        {
            Application.RemoveMessageFilter(this);
            foreach (UInt32 key in keyIDs.Values)
            {
                NativeMethods.UnregisterHotKey(hWnd, key);
                NativeMethods.GlobalDeleteAtom(key);
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x312)
            {
                if (OnHotkey != null)
                {
                    foreach (UInt32 key in keyIDs.Values)
                    {
                        if ((UInt32)m.WParam == key)
                        {
                            OnHotkey((int)m.WParam);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace JobClockAdmin
{
    /// <summary>
    /// Used to retrieve key state info
    /// </summary>
    public class KeyboardInfo
    {
        private KeyboardInfo() { }

        [DllImport("user32.dll")]
        public static extern Int16 GetAsyncKeyState(int vKey);

        public static bool IsPressed(Keys key)
        {
            return GetAsyncKeyState((int)key) < 0;

        }

        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);
        public static KeyStateInfo GetKeyState(Keys key)
        {
            short keyState = GetKeyState((int)key);
            byte[] bits = BitConverter.GetBytes(keyState);
            bool toggled = bits[0] == 1, pressed = bits[1] == 1;
            return new KeyStateInfo(key, pressed, toggled);
        }
    }


    public struct KeyStateInfo
    {
        Keys _key;
        bool _isPressed,
            _isToggled;
        public KeyStateInfo(Keys key,
                        bool ispressed,
                        bool istoggled)
        {
            _key = key;
            _isPressed = ispressed;
            _isToggled = istoggled;
        }
        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None,
                                            false,
                                            false);
            }
        }
        public Keys Key
        {
            get { return _key; }
        }
        public bool IsPressed
        {
            get { return _isPressed; }
        }
        public bool IsToggled
        {
            get { return _isToggled; }
        }
    }
}

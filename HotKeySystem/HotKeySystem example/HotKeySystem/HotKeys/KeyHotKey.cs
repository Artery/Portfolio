using System;
using System.Globalization;
using System.Threading;
using System.Windows.Input;

namespace HotKeySystem_example.HotKeySystem
{
    //HotKey-class for a KeyGesture-HotKey
    public class KeyHotKey : HotKeyBase
    {
        //Converter to convert strings to KeyGestures
        protected static readonly KeyGestureConverter s_KeyConverter = new KeyGestureConverter();

        public KeyHotKey()
        {
            KeyGesture = new KeyGesture(Key.None);
        }

        public KeyHotKey(HotKeyConfig config)
            : this(config.CommandType, config.hotkeyGestureString, config.Description)
        {
        }

        public KeyHotKey(Type commandType, string hotkeyGestureString, string description)
        {
            CommandType = commandType;
            Description = description;

            KeyGesture = CreateGestureFromString<KeyGesture>(hotkeyGestureString);
            HotKeyDisplayString = GetDisplayStringFromGesture(KeyGesture);
        }

        public KeyHotKey(Type commandType, KeyGesture keyGesture, string description)
        {
            CommandType = commandType;
            Description = description;
            KeyGesture = keyGesture;

            HotKeyDisplayString = GetDisplayStringFromGesture(KeyGesture);
        }

        public virtual KeyGesture KeyGesture { get; set; }

        public virtual string GetDisplayStringFromGesture(KeyGesture gesture, CultureInfo cultureInfo = null)
        {
            return gesture.GetDisplayStringForCulture(cultureInfo ?? Thread.CurrentThread.CurrentUICulture);
        }
    }
}

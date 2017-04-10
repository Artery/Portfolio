using System;
using System.Windows.Input;

namespace HotKeySystem_example.HotKeySystem
{
    //HotKey-class for a MouseGesture-HotKey
    public class MouseHotKey : HotKeyBase
    {
        protected static readonly MouseGestureConverter s_MouseConverter = new MouseGestureConverter();

        protected MouseGesture m_MouseGesture;

        public MouseHotKey()
        {
            new MouseGesture(MouseAction.None);
        }

        public MouseHotKey(HotKeyConfig config)
            : this(config.CommandType, config.hotkeyGestureString, config.Description)
        {
        }

        public MouseHotKey(Type commandType, string hotkeyGestureString, string description)
        {
            CommandType = commandType;
            Description = description;

            MouseGesture = CreateGestureFromString<MouseGesture>(hotkeyGestureString);
            HotKeyDisplayString = GetDisplayStringFromGesture(MouseGesture);
        }

        public MouseHotKey(Type commandType, MouseGesture mouseGesture, string description)
        {
            CommandType = commandType;
            Description = description;
            MouseGesture = mouseGesture;

            HotKeyDisplayString = GetDisplayStringFromGesture(m_MouseGesture);
        }

        public virtual MouseGesture MouseGesture { get; set; }

        public virtual string GetDisplayStringFromGesture(MouseGesture gesture)
        {
            //There is no build-in "GetDisplayString"-Method, so the displayString needs to be composed
            var displayString = gesture.MouseAction.ToString();

            if(!gesture.Modifiers.ToString().Equals(ModifierKeys.None))
            {
                displayString = gesture.Modifiers.ToString() + "+" + displayString;
            }

            return displayString;
        }
    }
}

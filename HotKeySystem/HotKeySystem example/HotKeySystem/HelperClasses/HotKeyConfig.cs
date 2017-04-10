using System;

namespace HotKeySystem_example.HotKeySystem
{
    //Initializer-class for HotKeys
    public class HotKeyConfig
    {
        public Type CommandType { get; set; }
        public string Description { get; set; }
        public string hotkeyGestureString { get; set; }

        public HotKeyConfig(Type commandType, string hotkeyGestureString, string description)
        {
            this.CommandType = commandType;
            this.Description = description;
            this.hotkeyGestureString = hotkeyGestureString;
        }
    }
}

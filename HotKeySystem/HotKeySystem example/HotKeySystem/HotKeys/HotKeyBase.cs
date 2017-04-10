using System;
using System.Windows.Input;

namespace HotKeySystem_example.HotKeySystem
{
    //Base class for a HotKey, which is linked to a specific command
    public abstract class HotKeyBase
    {
        //Type of the linked command
        public virtual Type CommandType { get; set; }
        //Displayed shortcut e.g.: 'Ctrl+A' or 'Umschalttaste+Linksklick'
        public virtual string HotKeyDisplayString { get; set; }
        //Description what the HotKeyNEW does e.g.: 'Close current project'
        public virtual string Description { get; set; }

        public virtual T CreateGestureFromString<T>(string gestureString)
            where T : InputGesture
        {
            return GestureConverterAdapter.GetGestureConverter(typeof(T)).ConvertFromString(gestureString) as T;
        }
    }
}

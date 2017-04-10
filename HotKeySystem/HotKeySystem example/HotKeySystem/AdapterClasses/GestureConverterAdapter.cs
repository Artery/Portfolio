using System;
using System.ComponentModel;
using System.Windows.Input;

namespace HotKeySystem_example.HotKeySystem
{
    //Adapter-class for GestureConverters to provide GestureConverter-object for specific HotKey-Types
    public static class GestureConverterAdapter
    {
        public static TypeConverter GetGestureConverter(Type gestureType)
        {
            TypeConverter typeConverter = null;

            if (gestureType.Equals(typeof(KeyGesture)))
            {
                typeConverter = new KeyGestureConverter();
            }
            else if (gestureType.Equals(typeof(MouseGesture)))
            {
                typeConverter = new MouseGestureConverter();
            }
            else
            {
                throw new NotImplementedException("GestureConverterAdapter could not provide requested GestureConverter for Gesture-Type: " + gestureType.FullName);
            }
            
            return typeConverter;
        }
    }
}

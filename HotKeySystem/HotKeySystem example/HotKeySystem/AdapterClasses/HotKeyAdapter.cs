using System;

namespace HotKeySystem_example.HotKeySystem
{
    //Adapter-class for HotKeys to provide HotKey-object of specific type
    public static class HotKeyAdapter
    {
        public static HotKeyBase GetNewHotKey(Type hotkeyType, HotKeyConfig config)
        {
            HotKeyBase hotkey = null;

            if (hotkeyType.Equals(typeof(KeyHotKey)))
            {
                hotkey = new KeyHotKey(config);
            }
            else if (hotkeyType.Equals(typeof(MouseHotKey)))
            {
                hotkey = new MouseHotKey(config);
            }
            else
            {
                throw new NotImplementedException("HotKeyAdapter could not provide requested HotKey of Type: " + hotkeyType.FullName);
            }

            return hotkey;
        }

        public static T GetNewHotKey<T>(HotKeyConfig config)
            where T : HotKeyBase, new()
        {
            return Activator.CreateInstance(typeof(T), config) as T;
        }
    }
}

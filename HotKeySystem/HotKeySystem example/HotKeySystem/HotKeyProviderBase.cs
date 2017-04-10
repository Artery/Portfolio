using log4net;
using System;
using System.Linq;

namespace HotKeySystem_example.HotKeySystem
{
    //Provider class for creating, storing and providing HotKeys
    public abstract class HotKeyProviderBase
    {
        //Enum to specify LoggerWarnMessage
        protected enum DefaultReturnTypes { NULL = 0, DEFAULT_GESTURE };


        protected HotKeyList<HotKeyBase> m_HotKeys = new HotKeyList<HotKeyBase>();

        public virtual HotKeyList<HotKeyBase> HotKeys
        {
            get { return m_HotKeys; }
        }


        //Returns exisiting hotkey, otherwise tries to create new one
        public virtual T GetHotKey<T>(Type commandType)
            where T : HotKeyBase, new()
        {
            var hotkey = m_HotKeys.FirstOrDefault<HotKeyBase>(item => item.CommandType.Equals(commandType) && item is T)
                            ?? CreateHotKey<T>(commandType);
            
            if(hotkey == null)
            {
                throw new ArgumentNullException("HotKeyProviderBase - GetHotKey<" 
                    + typeof(T).FullName + ">(" + commandType.FullName 
                    + "): Fetching hotkey from internal List or trying to create it resulted in null argument!");
            }

            return hotkey as T;
        }

        //Returns exisiting hotkey, otherwise tries to create new one or returns default-value null
        public virtual T GetHotKeyOrNull<T>(Type commandType)
            where T : HotKeyBase, new()
        {
            return TryGetHotKey<T>(commandType, DefaultReturnTypes.NULL);
        }

        //Returns exisiting hotkey, otherwise tries to create new one or returns default-gesture 'None'
        public virtual T GetHotKeyOrDefault<T>(Type commandType)
            where T : HotKeyBase, new()
        {
            return TryGetHotKey<T>(commandType, DefaultReturnTypes.DEFAULT_GESTURE) ?? new T();
        }

        protected virtual T TryGetHotKey<T>(Type commandType, DefaultReturnTypes defaultReturnType)
            where T : HotKeyBase, new()
        {
            T hotkey = null;
            
            try
            {
                hotkey = GetHotKey<T>(commandType);
            }
            catch(Exception e)
            {
                LogManager.GetLogger("HotKeyProviderBase").Warn(
                    GetWarnMessage("TryGetHotKey", commandType, typeof(T), e.Message) 
                    + "\n" 
                    + GetDefaultTypeMessage(defaultReturnType));
            }

            return hotkey;
        }

        protected virtual string GetWarnMessage(string subMethodName, Type commandType, Type hotkeyType, string ExceptionMessage)
        {
            return "HotKeyProviderBase - " + subMethodName + ": Could not create HotKey of Type " + hotkeyType.FullName
                    + " and for CommandType: " + commandType.FullName
                    + "\nInnerExceptionMessage: " + ExceptionMessage;
        }

        protected virtual string GetDefaultTypeMessage(DefaultReturnTypes defaultReturnType)
        {
            var defaultTypeMessage = "Returning ";

            //For more specified log-message
            switch (defaultReturnType)
            {
                case DefaultReturnTypes.NULL:
                    defaultTypeMessage += "null";
                    break;
                case DefaultReturnTypes.DEFAULT_GESTURE:
                    defaultTypeMessage += "default('none')-gesture";
                    break;
                default:
                    defaultTypeMessage += defaultReturnType.ToString();
                    break;
            }

            return defaultTypeMessage;
        }


        protected abstract T CreateHotKey<T>(Type commandType) where T : HotKeyBase, new();
    }
}

using System;

namespace HotKeySystem_example.HotKeySystem
{
    //HotKeyProvider for MapEditor
    public class HotKeyProvider : HotKeyProviderBase
    {
        //Singleton-Pattern based on MapInputTracker
        #region Singleton

        public static HotKeyProvider Instance
        {
            get { return Nested.Instance; }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
        {
            internal static readonly HotKeyProvider Instance = new HotKeyProvider();
        }

        #endregion

        protected override T CreateHotKey<T>(Type commandType)
        {
            var commandName = commandType.Name;
            var typeName = typeof(T).Name;

            //Fetch hotkey data from resource and config files
            var description = Properties.HotKeysDescriptions.ResourceManager.GetString(commandName) ?? String.Empty;
            string hotkeyGestureString = null;
            
            try
            {
                hotkeyGestureString = Properties.HotKeySettings.Default[commandName + typeName] as string;
            }
            catch(Exception)
            {
                throw new HotKeyNotDeclaredException("MapEditorHotKeyProvider - CreateHotKey<" 
                    + typeName + ">(" + commandName
                    + "): Requested HotKey is not declared in HotKeySettings/App.config!");
            }
            
            //Create new hotkey
            var newHotKey = HotKeyAdapter.GetNewHotKey<T>(new HotKeyConfig(commandType, hotkeyGestureString, description));

            if(newHotKey != null)
            {
                //Localize DisplayString of new hotkey
                newHotKey.HotKeyDisplayString = HotKeyLocalizer.LocalizeDisplayString(newHotKey.HotKeyDisplayString);
                m_HotKeys.Add(newHotKey);
            }

            return newHotKey;
        }
    }
}

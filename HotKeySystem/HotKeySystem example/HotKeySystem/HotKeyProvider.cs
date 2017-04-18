using System;

namespace HotKeySystem_example.HotKeySystem
{
    //Specified HotKeyProvider
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

        //Fetches the gestureString from HotKeySettings (part of the app.config)
        protected string FetchGestureString<T>(Type commandType)
            where T : HotKeyBase, new()
        {
            var commandName = commandType.Name;
            var typeName = typeof(T).Name;

            //Fetch hotkey data from resource and config files
            string hotkeyGestureString = null;

            try
            {
                hotkeyGestureString = Properties.HotKeySettings.Default[commandName + typeName] as string;
            }
            catch (Exception)
            {
                throw new HotKeyNotDeclaredException("MapEditorHotKeyProvider - CreateHotKey<"
                    + typeName + ">(" + commandName
                    + "): Requested HotKey is not declared in HotKeySettings/App.config!");
            }

            return hotkeyGestureString;
        }

        //Fetches the description from HotKeysDescriptions (part of the app.config)
        protected string FetchDescription<T>(Type commandType)
            where T : HotKeyBase, new()
        {
            return Properties.HotKeysDescriptions.ResourceManager.GetString(commandType.Name) ?? String.Empty;
        }

        //Create a single HotKey
        protected override T CreateHotKey<T>(Type commandType)
        {
            var description = FetchDescription<T>(commandType);
            var hotkeyGestureString = FetchGestureString<T>(commandType);

            return InternCreateHotKey<T>(commandType, hotkeyGestureString, description);
        }

        //Create multiple (list) of hotkeys
        protected override List<T> CreateMultipleHotKeys<T>(Type commandType)
        {
            var description = FetchDescription<T>(commandType);
            //Delimiter is hardcoded, another approach should be considered!
            var hotkeyGestureStrings = FetchGestureString<T>(commandType).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            List<T> hotkeys = new List<T>();

            foreach (var gestureString in hotkeyGestureStrings)
            {
                hotkeys.Add(InternCreateHotKey<T>(commandType, gestureString, description));
            }

            return hotkeys;
        }

        //Method that actually creates a hotkey
        protected T InternCreateHotKey<T>(Type commandType, string hotkeyGestureString, string description)
            where T : HotKeyBase, new()
        {
            //Create new hotkey
            var newHotKey = HotKeyAdapter.GetNewHotKey<T>(new HotKeyConfig(commandType, hotkeyGestureString, description));

            if (newHotKey != null)
            {
                //Localize DisplayString of new hotkey
                newHotKey.HotKeyDisplayString = HotKeyLocalizer.LocalizeDisplayString(newHotKey.HotKeyDisplayString);
                m_HotKeys.Add(newHotKey);
            }

            return newHotKey;
        }
    }
}

using System;

namespace HotKeySystem_example.HotKeySystem
{
    //Provides simple and primitive localization for HotKeys
    public static class HotKeyLocalizer
    {
        public static string LocalizeDisplayString(string displayString)
        {
            var displayStringParts = displayString.Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);

            //Replace all substrings which could be localized
            foreach(string substring in displayStringParts)
            {
                var localizedString = Properties.LocalizedKeyNames.ResourceManager.GetString(substring);

                if(localizedString != null)
                {
                    displayString = displayString.Replace(substring, localizedString);
                }
            }

            return displayString;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Configuration;
using HotKeySystem_example.Commands.ProjectCommands;
using HotKeySystem_example.HotKeySystem;

namespace HotKeySystem_example.ViewModels
{
    public class HelpWindowViewModel
    {
        //Package-Class for a specific hotkey-group-tab e.g. "Hilfe"
        public class HotKeyCategoryTab
        {
            public HotKeyCategoryTab(string header, ICollectionView hotkeys)
            {
                Header = header;
                HotKeys = hotkeys;
            }
            public string Header { get; set; }

            public ICollectionView HotKeys { get; private set; }
        }

        public class HotKeyInfo
        {
            public HotKeyInfo(string displayString, string description)
            {
                DisplayString = displayString;
                Description = description;
            }

            public string DisplayString {get; set;}
            public string Description { get; set; }
        }
        
        public ICollectionView Tabs { get; private set; }
        private readonly CreateHotKeyListTempFileCommand m_CreateHotKeyListTempFileCommand;
        private int m_HotKeyDisplayStringMaxLength = 0;


        public HelpWindowViewModel()
        {
            m_CreateHotKeyListTempFileCommand = new CreateHotKeyListTempFileCommand(this);

            Tabs = CreateHotKeyCategoryTabsCollection(ref m_HotKeyDisplayStringMaxLength);
        }

        public CreateHotKeyListTempFileCommand CreateHotKeyListTempFileCommand
        {
            get { return m_CreateHotKeyListTempFileCommand; }
        }

        public int HotKeyDisplayStringMaxLength
        {
            get
            {
                return m_HotKeyDisplayStringMaxLength;
            }
        }

        //Creates the whole HotKeyCategoryTabCollection which is bound in the HelpWindow
        private static ICollectionView CreateHotKeyCategoryTabsCollection(ref int hotKeyDisplayStringMaxLength)
        {
            var groupedHotkeys = new Dictionary<string, List<HotKeyInfo>>();
            var tabs = new List<HotKeyCategoryTab>();
            var hotkeys = Properties.HotKeySettings.Default.Properties;
                //MapEditorHotKeyProvider.Instance.HotKeys;

            foreach (SettingsProperty item in hotkeys)
            {
                var commandName = item.Name.Split(new string[] { "Command" }, StringSplitOptions.RemoveEmptyEntries)[0] + "Command";
                var displayString = HotKeyLocalizer.LocalizeDisplayString(new SettingsPropertyValue(item).PropertyValue as string);
                var description = Properties.HotKeysDescriptions.ResourceManager.GetString(commandName);
                var category = Properties.HotKeysCategories.ResourceManager.GetString(commandName);
                
                if (description != null && category != null)
                {
                    var hotkeyInfo = new HotKeyInfo(displayString, description);

                    //Create new HotKeyCategory, if not existing
                    if (!groupedHotkeys.ContainsKey(category))
                    {
                        groupedHotkeys.Add(category, new List<HotKeyInfo>());
                        tabs.Add(new HotKeyCategoryTab(category,
                            CollectionViewSource.GetDefaultView(groupedHotkeys[category])));
                    }

                    groupedHotkeys[category].Add(hotkeyInfo);

                    //Used to determine the with and tabs for the outputfile
                    if (hotkeyInfo.DisplayString.Length > hotKeyDisplayStringMaxLength)
                    {
                        hotKeyDisplayStringMaxLength = hotkeyInfo.DisplayString.Length;
                    }
                }
            }

            //Sorting alphabetically
            SortHotKeyCategory(groupedHotkeys);

            return CollectionViewSource.GetDefaultView(tabs);
        }

        private static void SortHotKeyCategory(Dictionary<string, List<HotKeyInfo>> groupedHotkeys)
        {
            foreach (var item in groupedHotkeys)
            {
                item.Value.Sort((x, y) => string.Compare(x.DisplayString, y.DisplayString));
            }
        }
    }
}

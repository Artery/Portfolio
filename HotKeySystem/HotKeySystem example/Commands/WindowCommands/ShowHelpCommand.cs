using HotKeySystem_example.HotKeySystem;
using HotKeySystem_example.Utility;
using HotKeySystem_example.ViewModels;
using HotKeySystem_example.Windows;

namespace HotKeySystem_example.Commands.WindowCommands
{
    public class ShowHelpCommand : ShowDialogCommand<HelpWindow>
    {
        private KeyHotKey m_KeyHotKey;

        public KeyHotKey KeyHotKey
        {
            get { return m_KeyHotKey; }
            set { m_KeyHotKey = value; }
        }

        public ShowHelpCommand()

            : base(new HelpWindowViewModel())
        {
            m_KeyHotKey = HotKeyProvider.Instance.GetHotKeyOrDefault<KeyHotKey>(this.GetType());
        }
    }
}
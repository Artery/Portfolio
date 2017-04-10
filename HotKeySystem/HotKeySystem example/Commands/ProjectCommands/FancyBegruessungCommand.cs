using HotKeySystem_example.HotKeySystem;
using HotKeySystem_example.Utility;
using HotKeySystem_example.ViewModels;

namespace HotKeySystem_example.Commands.ProjectCommands
{
    public class FancyBegruessungCommand : CommandBase
    {
        private MainWindowViewModel m_ViewModel;
        private KeyHotKey m_KeyHotKey;

        public KeyHotKey KeyHotKey
        {
            get { return m_KeyHotKey; }
            set { m_KeyHotKey = value; }
        }

        public FancyBegruessungCommand(MainWindowViewModel viewModel)
        {
            m_KeyHotKey = HotKeyProvider.Instance.GetHotKeyOrDefault<KeyHotKey>(this.GetType());
            m_ViewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            m_ViewModel.FancyBegruessungVisibility = System.Windows.Visibility.Visible;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
using HotKeySystem_example.Commands.ProjectCommands;
using HotKeySystem_example.Commands.WindowCommands;
using System.ComponentModel;
using System.Windows;

namespace HotKeySystem_example.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ShowHelpCommand m_ShowHelp;
        private readonly FancyBegruessungCommand m_FancyBegruessung;
        private Visibility m_FancyBegruessungVisibility;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            m_ShowHelp = new ShowHelpCommand();
            m_FancyBegruessung = new FancyBegruessungCommand(this);
            m_FancyBegruessungVisibility = Visibility.Hidden;
        }

        public ShowHelpCommand ShowHelp
        {
            get { return m_ShowHelp; }
        }

        public FancyBegruessungCommand FancyBegruessung
        {
            get { return m_FancyBegruessung; }
        }

        public Visibility FancyBegruessungVisibility
        {
            get { return m_FancyBegruessungVisibility; }
            set 
            {
                if (m_FancyBegruessungVisibility != value)
                {
                    m_FancyBegruessungVisibility = value;
                    OnPropertyChanged("FancyBegruessungVisibility");
                }
                
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

using System;
using System.Windows;

namespace HotKeySystem_example.Utility
{
    [Serializable]
    public class ShowDialogCommand<T> : CommandBase where T : Window, new()
    {
        public ShowDialogCommand()
        {
        }

        public ShowDialogCommand(object dataContext)
        {
            DataContext = dataContext;
        }

        public object DataContext { get; set; }
        public bool? DialogResult { get; set; }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var window = Find();

            if (window == null)
            {
                window = new T();
            }

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (DataContext != null)
            {
                window.DataContext = DataContext;
            }

            if (window.IsLoaded)
            {
                window.Activate();
            }
            else
            {
                DialogResult = window.ShowDialog();
            }
        }

        protected Window Find()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is T)
                {
                    return window;
                }
            }

            var window1 = new T();

            return window1;
        }
    }
}
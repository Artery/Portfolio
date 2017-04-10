using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using HotKeySystem_example.Utility;
using log4net;
using HotKeySystem_example.ViewModels;

namespace HotKeySystem_example.Commands.ProjectCommands
{
    public class CreateHotKeyListTempFileCommand : CommandBase
    {
        private readonly HelpWindowViewModel m_HelpWindowViewModel;
        private static bool s_HotKeyListCreated = false;
        private static bool s_fileNameCreated = false;
        private static string s_fileExtension = ".txt";
        private static string s_fileName = "HotKeyList" + s_fileExtension;

        public CreateHotKeyListTempFileCommand(HelpWindowViewModel helpWindowViewModel)
        {
            if (!s_fileNameCreated)
            {
                GenerateFileName();
            }

            m_HelpWindowViewModel = helpWindowViewModel;
        }

        private static void GenerateFileName()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (assemblyPath == null)
            {
                s_fileName = Path.GetTempPath() + s_fileName;
            }
            else
            {
                var uri = new Uri(assemblyPath);
                assemblyPath = uri.LocalPath;

                s_fileName = assemblyPath + "\\" + s_fileName;
            }

            s_fileNameCreated = true;
        }

        public override void Execute(object parameter)
        {
            var logger = LogManager.GetLogger("CreateHotKeyListTempFileCommand");
            string errorMessage = String.Empty;
            bool errorOccured = false;

            if (!CreateHotKeyListFile(logger))
            {
                errorMessage += "Fehler beim Erstellen der " + s_fileName + "!\n";
                errorOccured = true;
            }

            if (!OpenHotKeyListInTxtEditor(logger))
            {
                if (!File.Exists(s_fileName))
                {
                    s_HotKeyListCreated = false;
                }

                errorMessage += "Fehler beim Öffnen der " + s_fileName + "!\n";
                errorOccured = true;
            }

            if (errorOccured)
            {
                errorMessage += "Bitte versuchen Sie es erneut!";
                MessageBoxResult result = MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CreateHotKeyListFile(ILog logger)
        {
            if (!s_HotKeyListCreated)
            {
                try
                {
                    System.IO.File.Delete(s_fileName);

                    string fileContent = GenerateHotKeyListFileContent();

                    System.IO.File.WriteAllText(@s_fileName, fileContent);
                    logger.Info("Generated new " + s_fileName);

                    s_HotKeyListCreated = true;
                }
                catch (Exception)
                {
                    logger.Error("Failed generating new " + s_fileName);
                    return false;
                }
            }

            return true;
        }

        private static bool OpenHotKeyListInTxtEditor(ILog logger)
        {
            try
            {
                if (!s_HotKeyListCreated)
                {
                    throw new Exception();
                }
                
                Process.Start(@s_fileName);
            }
            catch (Exception)
            {
                logger.Error("Failed to open " + s_fileName);
                return false;
            }

            return true;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        private string GenerateHotKeyListFileContent()
        {
            string fileContent = "HotKeySystem example - Tastenkürzel";
            var Tabs = m_HelpWindowViewModel.Tabs;

            Tabs.MoveCurrentToFirst();
            do
            {
                fileContent += "\n\n";
                var tab = (HelpWindowViewModel.HotKeyCategoryTab) Tabs.CurrentItem;

                fileContent += tab.Header + "\n\n";
                var hotkeys = tab.HotKeys;
                string fullTabString = String.Empty;

                hotkeys.MoveCurrentToFirst();
                do
                {
                    var hotkey = hotkeys.CurrentItem as HelpWindowViewModel.HotKeyInfo;
                    fullTabString += "\t" +
                                     string.Format(
                                         "{0,-" + (m_HelpWindowViewModel.HotKeyDisplayStringMaxLength + 5).ToString() +
                                         "}",
                                         hotkey.DisplayString) + hotkey.Description + "\n";
                } while (hotkeys.MoveCurrentToNext());
                hotkeys.MoveCurrentToFirst();

                fileContent += fullTabString;
            } while (Tabs.MoveCurrentToNext());
            Tabs.MoveCurrentToFirst();

            return fileContent;
        }

        public static bool HotKeyListCreated
        {
            get
            {
                return s_HotKeyListCreated;
            }
        }
    }
}
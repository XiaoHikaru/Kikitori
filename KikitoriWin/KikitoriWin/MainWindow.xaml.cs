// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Kikitori
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ViewModel.MainWindowVM vm;

        public MainWindow()
        {
            vm = new Kikitori.ViewModel.MainWindowVM();
            DataContext = vm;
            InitializeComponent();
        }

        private void OnEndApplication()
        {
            vm.TransferToDB();
            vm.SaveAndCloseDB();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnEndApplication();
        }

        private static string GetFileName<T>()
            where T : FileDialog, new()
        {
            T fileDialog = new T();
            fileDialog.DefaultExt = ".db";
            fileDialog.Filter = @"SQLite DB files (*.db)|*.db";

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            return null;
        }

        private void OnClickNewDatabase(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileName<SaveFileDialog>();
            if (fileName != null)
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                vm.LoadOrCreate(fileName);
            }
        }

        private void OnClickOpenDatabase(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileName<OpenFileDialog>();
            if (fileName != null)
            {
                vm.LoadOrCreate(fileName);
            }
        }

        private void OnClickExit(object sender, RoutedEventArgs e)
        {
            OnEndApplication();
            Close();
        }

        private void ButtonAddMediaClick(object sender, RoutedEventArgs e)
        {
            vm.NewMedium();
        }

        private void ButtonDeleteMediaClick(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedMediumIndex == -1)
            {
                return;
            }
            MessageBoxResult messageBoxResult
                = System.Windows.MessageBox.Show($"This will also delete all exercise items of medium \"{vm.SelectedMediumTitle}\". Are you sure?",
                "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                vm.DeleteSelectedMedium();
            }
        }

        private void ButtonResetGapQuizClick(object sender, RoutedEventArgs e)
        {
            vm.ResetTrainingData();
        }

        private void CellEdited(object sender, EventArgs e)
        {
            vm.TransferToDB();
        }

        private void ButtonAddExerciseEntryClick(object sender, RoutedEventArgs e)
        {
            var entryInput = new EntryInput(this.vm);
            entryInput.ShowDialog();
        }

        private void ButtonStartGapQuizClick(object sender, RoutedEventArgs e)
        {
            var gapQuiz = new GapQuiz(vm);
            if (!gapQuiz.QuizCompleted)
            {
                gapQuiz.ShowDialog();
            }
        }
    }
}

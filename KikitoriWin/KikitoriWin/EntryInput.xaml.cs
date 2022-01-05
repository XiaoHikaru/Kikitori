// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Windows;

namespace Kikitori
{
    /// <summary>
    /// Interaktionslogik für EntryInput.xaml
    /// </summary>
    public partial class EntryInput : Window
    {
        ViewModel.EntryInputVM vm;

        public EntryInput(Kikitori.ViewModel.MainWindowVM mainWindowVM)
        {
            vm = new Kikitori.ViewModel.EntryInputVM(mainWindowVM);
            DataContext = vm;
            InitializeComponent();
        }

        private void ItemEdited(object sender, EventArgs e)
        {
            vm.TransferToDB(true);
        }

        private async void ButtonAddClipboardContentClick(object sender, RoutedEventArgs e)
        {
            object clipboardObject = GUI.ClipboardHelper.GetClipboardData();
            if (clipboardObject is byte[] copiedBytes)
            {
                vm.CurrentMP3Audio = copiedBytes;
            }
            else if (clipboardObject is string copiedSentence)
            {
                vm.CurrentSentence = await Kikitori.Kanji.Furiganas.GetSeparation(copiedSentence, false);
                vm.CurrentFurigana = await Kikitori.Kanji.Furiganas.GetSeparation(copiedSentence, true);
            }
        }

        private async void ButtonAudioPlayClick(object sender, RoutedEventArgs e)
        {
            var player = new Kikitori.Audio.AudioPlayer();
            await player.Play(vm.CurrentMP3Audio);
        }

        private void ButtonAddEntryClick(object sender, RoutedEventArgs e)
        {
            if (!vm.CheckValidityOfNewEntry())
            {
                MessageBox.Show("The number of separators ('*') is not the same for sentence and furigana entry.");
                return;
            }
            vm.AddCurrentItem();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.TransferToDB(false);
        }

        private void ButtonDeleteEntryClick(object sender, RoutedEventArgs e)
        {
            vm.DeleteSelectedIndex();
        }

        private async void ButtonMakeFuriganaPropositionClick(object sender, RoutedEventArgs e)
        {
            vm.CurrentFurigana = await Kikitori.Kanji.Furiganas.GetFuriganaProposition(vm.CurrentSentence);
        }
    }

}

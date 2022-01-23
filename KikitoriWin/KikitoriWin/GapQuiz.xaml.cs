// Kikitori
// (C) 2021, Andreas Gaiser

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Nito.AsyncEx;

namespace Kikitori
{
    /// <summary>
    /// Interaktionslogik für GapQuiz.xaml
    /// </summary>
    public partial class GapQuiz : Window
    {
        Kikitori.ViewModel.GapQuizVM vm;
        public bool QuizCompleted = false;

        public bool NewQuizItem()
        {
            QuizCompleted = !vm.GetNewItem();
            if (QuizCompleted)
            {
                MessageBox.Show("Congratulations, you know every token.", "Congrats");
                Close();
                return false;
            }
            return true;
        }

        public GapQuiz(Kikitori.ViewModel.MainWindowVM mainWindowVM)
        {
            vm = new Kikitori.ViewModel.GapQuizVM(mainWindowVM);
            InitializeComponent();
            DataContext = vm;
            NewQuizItem();
            if (QuizCompleted)
            {
                Close();
            }
            else
            {
                var player = new Kikitori.Audio.AudioPlayer();
                player.NonAsyncPlay(vm.CurrentMP3Audio);
            }

        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.CloseQuiz();
        }

        private async Task PlaySentence()
        {
            ButtonAudioPlay.IsEnabled = false;
            if (vm.CurrentMP3Audio != null)
            {
                var player = new Kikitori.Audio.AudioPlayer();
                await player.Play(vm.CurrentMP3Audio);
            }
            ButtonAudioPlay.IsEnabled = true;
        }

        private async void ButtonAudioPlayClick(object sender, RoutedEventArgs e)
        {
            await PlaySentence();
        }

        private async Task WaitABit(int milisecondsDelay)
        {
            await Task.Delay(milisecondsDelay);
        }

        private async void ButtonCheckClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            if (vm.CheckAnswer())
            {
                TextBlockSolution.Background = Brushes.LightGreen;
                TextBlockSolutionFurigana.Background = Brushes.White;
                TextBlockSolutionRomaji.Background = Brushes.White;
                await WaitABit(500);
            }
            else
            {
                TextBlockSolution.Background = Brushes.OrangeRed;
                TextBlockSolutionFurigana.Background = Brushes.OrangeRed;
                TextBlockSolutionRomaji.Background = Brushes.OrangeRed;
                await PlaySentence();
                await WaitABit(500);
                MessageBox.Show("分かりましたか。", "Confirmation", MessageBoxButton.OK);
            }
            TextBlockSolution.Background = Brushes.White;
            TextBlockSolutionFurigana.Background = Brushes.White;
            TextBlockSolutionRomaji.Background = Brushes.White;
            vm.CurrentCompleteSolutionHint = "";
            vm.CurrentCompleteSolutionHintFurigana = "";
            IsEnabled = true;
            NewQuizItem();
            if (!QuizCompleted)
            {
                await PlaySentence();
            }

        }
    }
}

// Kikitori
// (C) 2021, Andreas Gaiser

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Kikitori
{
    /// <summary>
    /// Interaktionslogik für GapQuiz.xaml
    /// </summary>
    public partial class GapQuiz : Window
    {
        Kikitori.ViewModel.GapQuizVM vm;

        public void NewQuizItem()
        {
            if (!vm.GetNewItem())
            {
                MessageBox.Show("Congratulations, you know every token.", "Congrats");
                Close();
            }
        }

        public GapQuiz(Kikitori.ViewModel.MainWindowVM mainWindowVM)
        {
            vm = new Kikitori.ViewModel.GapQuizVM(mainWindowVM);
            DataContext = vm;
            InitializeComponent();
            NewQuizItem();
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

        private async void ButtonCheckClick(object sender, RoutedEventArgs e)
        {
            if (vm.CheckAnswer())
            {
                TextBlockSolution.Background = Brushes.LightGreen;
            }
            else
            {
                TextBlockSolution.Background = Brushes.OrangeRed;
            }
            NewQuizItem();
            await PlaySentence();
        }
    }
}

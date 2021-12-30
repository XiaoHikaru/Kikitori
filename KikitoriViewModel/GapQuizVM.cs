// Kikitori
// (C) 2021, Andreas Gaiser

using System.ComponentModel;
using System.Linq;
using DB = Kikitori.Data.Database;
using Medium = Kikitori.Data.Medium;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.ViewModel
{
    public class GapQuizVM : VM
    {
        MainWindowVM mainWindow;
        Medium currentMedium;
        SentenceItem currentItem;
        Games.GapQuiz currentQuiz;

        public GapQuizVM(MainWindowVM mainWindow)
        {
            this.mainWindow = mainWindow;
            currentMedium = mainWindow.Media[mainWindow.SelectedMediumIndex];
            currentQuiz = new Games.GapQuiz(DB.Instance.GetItems<SentenceItem>().Where(item => item.ExerciseMediumLink == currentMedium.ID).ToList());
            NotifyAll();
        }

        public override void NotifyAll()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(CurrentSentenceSuffix));
            OnPropertyChanged(nameof(CurrentSentencePrefix));
            OnPropertyChanged(nameof(CurrentSolutionCandidate));
            OnPropertyChanged(nameof(CurrentMP3Audio));
            OnPropertyChanged(nameof(currentCompleteSolutionHint));
        }

        public bool GetNewItem()
        {
            bool result = currentQuiz.GetNextExerciseToken(out string prefix, out string suffix, out SentenceItem sentenceItem);
            if (result)
            {
                CurrentSentencePrefix = prefix;
                CurrentSentenceSuffix = suffix;
                currentItem = sentenceItem;
                CurrentMP3Audio = sentenceItem.MP3Audio;
                CurrentSolutionCandidate = "";
            }
            return result;
        }

        public bool CheckAnswer()
        {
            bool isCorrect = currentQuiz.CheckInput(CurrentSolutionCandidate);
            CurrentCompleteSolutionHint = currentItem.Sentence.Replace("*", " ");
            if (isCorrect)
            {
                currentItem.TrainedKnownTokens = currentQuiz.GetKnownTokens();
                DB.Instance.Update(currentItem);

            }
            return isCorrect;
        }

        #region Properties

        private string currentSentencePrefix;
        public string CurrentSentencePrefix
        {
            get { return currentSentencePrefix; }
            set
            {
                SetField(ref currentSentencePrefix, value); NotifyAll();
            }
        }

        private string currentSentenceSuffix;
        public string CurrentSentenceSuffix
        {
            get { return currentSentenceSuffix; }
            set
            {
                SetField(ref currentSentenceSuffix, value); NotifyAll();
            }
        }

        private string currentSolutionCandidate;
        public string CurrentSolutionCandidate
        {
            get { return currentSolutionCandidate; }
            set
            {
                SetField(ref currentSolutionCandidate, value); NotifyAll();
            }
        }

        private string currentCompleteSolutionHint;
        public string CurrentCompleteSolutionHint
        {
            get { return currentCompleteSolutionHint; }
            set
            {
                SetField(ref currentCompleteSolutionHint, value); NotifyAll();
            }
        }

        private byte[] currentMP3Audio;
        public byte[] CurrentMP3Audio
        {
            get { return currentMP3Audio; }
            set
            {
                SetField(ref currentMP3Audio, value); NotifyAll();
            }
        }

        public string Title
        {
            get => "Gap Quiz for medium " + mainWindow.SelectedMediumTitle;
        }

        #endregion
    }
}

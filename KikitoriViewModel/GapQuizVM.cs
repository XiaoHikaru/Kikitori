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

        public GapQuizVM(MainWindowVM mainWindow, int mediumID)
        {
            this.mainWindow = mainWindow;
            this.currentMedium = DB.Instance.GetItems<Medium>().Where(medium => medium.ID == mediumID).FirstOrDefault();
            if (currentMedium == null)
            {
                throw new System.Exception("Medium with ID " + mediumID + " not found.");
            }
            var theItems = DB.Instance.GetItems<SentenceItem>().Where(item => item.ExerciseMediumLink == currentMedium.ID).ToList();
            currentQuiz = new Games.GapQuiz(theItems);
            NotifyAll();
        }

        public void CloseQuiz()
        {
            mainWindow.QuizCount++;
        }


        public GapQuizVM(MainWindowVM mainWindow)
        {
            this.mainWindow = mainWindow;
            currentMedium = mainWindow.Media[mainWindow.SelectedMediumIndex];
            var theItems = DB.Instance.GetItems<SentenceItem>().Where(item => item.ExerciseMediumLink == currentMedium.ID).ToList();
            currentQuiz = new Games.GapQuiz(theItems);
            NotifyAll();
        }


        public override void NotifyAll()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(CurrentSentenceSuffix));
            OnPropertyChanged(nameof(CurrentSentencePrefix));
            OnPropertyChanged(nameof(CurrentSolutionCandidate));
            OnPropertyChanged(nameof(CurrentMP3Audio));
            OnPropertyChanged(nameof(CurrentCompleteSolutionHint));
            OnPropertyChanged(nameof(CurrentCompleteSolutionHintFurigana));
            OnPropertyChanged(nameof(CurrentCompleteSolutionHintRomaji));
            OnPropertyChanged(nameof(NumberOfCorrectTokens));
            OnPropertyChanged(nameof(NumberOfTokens));
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
            var (isCorrect, diffText) = currentQuiz.CheckInput(CurrentSolutionCandidate);
            if (isCorrect)
            {
                CurrentCompleteSolutionHint = "すごい！";
                CurrentCompleteSolutionHintFurigana = "";
                currentItem.TrainedKnownTokens = currentQuiz.GetKnownTokens();
                DB.Instance.Update(currentItem);

            }
            else
            {
                CurrentCompleteSolutionHint = "違う: " + diffText.Replace("*", " ");
                CurrentCompleteSolutionHintFurigana = currentItem.Furigana.Replace("*", " ");
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

        private string currentCompleteSolutionHintFurigana;
        public string CurrentCompleteSolutionHintFurigana
        {
            get { return currentCompleteSolutionHintFurigana; }
            set
            {
                SetField(ref currentCompleteSolutionHintFurigana, value); NotifyAll();
            }
        }

        public string CurrentCompleteSolutionHintRomaji
        {
            get
            {
                if (CurrentCompleteSolutionHintFurigana == null) { return ""; }
                else
                {
                    return Kanji.Furiganas.GetRomajiNonAsync(CurrentCompleteSolutionHintFurigana);
                }
            }
        }

        public int NumberOfCorrectTokens
        {
            get
            {
                return currentQuiz.CorrectTokensCount;
            }
        }

        public int NumberOfTokens
        {
            get
            {
                return currentQuiz.TokensCount;
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
            get => "Gap Quiz for medium " + currentMedium?.Title;
        }

        #endregion
    }
}

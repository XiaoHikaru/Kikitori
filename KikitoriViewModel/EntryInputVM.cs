// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using DB = Kikitori.Data.Database;
using Medium = Kikitori.Data.Medium;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.ViewModel
{
    public class EntryInputVM : VM
    {
        private MainWindowVM mainWindow;
        private Medium currentMedium;


        public EntryInputVM(MainWindowVM mainWindow)
        {
            this.mainWindow = mainWindow;
            currentMedium = mainWindow.Media[mainWindow.SelectedMediumIndex];
            SentenceItems = new SentenceItemsList();
            SentenceItems.Reload(currentMedium);
        }

        public override void NotifyAll()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(SentenceItems));
            OnPropertyChanged(nameof(CurrentSentence));
            OnPropertyChanged(nameof(CurrentFurigana));
            OnPropertyChanged(nameof(CurrentRomaji));
            OnPropertyChanged(nameof(CurrentMP3Audio));
            OnPropertyChanged(nameof(CurrentInputIsComplete));
            OnPropertyChanged(nameof(CurrentMP3AudioPresent));
            OnPropertyChanged(nameof(SelectedItemIndex));

        }

        #region Properties

        private SentenceItemsList sentenceItems;
        public SentenceItemsList SentenceItems
        {
            get { return sentenceItems; }
            set
            {
                SetField(ref sentenceItems, value); NotifyAll();
            }
        }

        private string currentSentence;
        public string CurrentSentence
        {
            get { return currentSentence; }
            set
            {
                SetField(ref currentSentence, value); NotifyAll();
            }
        }

        private string currentFurigana;
        public string CurrentFurigana
        {
            get { return currentFurigana; }
            set { SetField(ref currentFurigana, value); NotifyAll(); }
        }

        public string CurrentRomaji
        {
            get
            {
                if (CurrentFurigana == null) { return ""; }
                else
                {
                    return Kanji.Furiganas.GetRomajiNonAsync(CurrentFurigana);
                }
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

        private int selectedItemIndex = -1;
        public int SelectedItemIndex
        {
            get { return selectedItemIndex; }
            set
            {
                selectedItemIndex = value; NotifyAll();
            }
        }

        public bool CurrentInputIsComplete
        {
            get => !String.IsNullOrEmpty(currentSentence) && !String.IsNullOrEmpty(currentFurigana) && currentMP3Audio != null;
        }

        public bool CurrentMP3AudioPresent
        {
            get => currentMP3Audio != null;
        }

        public string Title
        {
            get => "Entries for medium " + mainWindow.SelectedMediumTitle;
        }

        #endregion

        public static bool CheckValidityOfEntry(string sentence, string furigana)
        {
            return sentence.Count(c => c == '*') == furigana.Count(c => c == '*');
        }

        public bool CheckValidityOfNewEntry()
        {
            return CheckValidityOfEntry(CurrentSentence, currentFurigana);
        }

        public void AddCurrentItem()
        {
            SentenceItem item = new SentenceItem
            {
                Sentence = CurrentSentence,
                Furigana = CurrentFurigana,
                MP3Audio = CurrentMP3Audio,
                ExerciseMediumLink = currentMedium.ID
            };
            DB.Instance.Save(item);
            SentenceItems.Add(item);
            CurrentSentence = "";
            CurrentFurigana = "";
            CurrentMP3Audio = null;
        }

        public void TransferToDB(bool deleteTrainingData)
        {
            SentenceItems.TransferToDB(currentMedium, deleteTrainingData);
        }

        public void DeleteSelectedIndex()
        {
            if (SelectedItemIndex != -1)
            {
                try
                {
                    SentenceItems.RemoveSentenceItem(SelectedItemIndex);
                }
                catch (System.ArgumentOutOfRangeException)
                { }
                NotifyAll();
            }
        }
    }

    public class SentenceItemValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            SentenceItem item = (value as BindingGroup).Items[0] as SentenceItem;
            if (!EntryInputVM.CheckValidityOfEntry(item.Sentence, item.Furigana))
            {
                return new ValidationResult(false,
                    "Number of separators (*) does not match between sentence and furigana transcription.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }

}

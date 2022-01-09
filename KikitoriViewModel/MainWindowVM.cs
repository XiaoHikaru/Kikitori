// Kikitori
// (C) 2021, Andreas Gaiser

using System.ComponentModel;
using DB = Kikitori.Data.Database;

namespace Kikitori.ViewModel
{
    public class MainWindowVM : VM, INotifyPropertyChanged
    {
        public MainWindowVM()
        {
            Media = new MediaList();
        }

        public override void NotifyAll()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(DatabaseLoaded));
            OnPropertyChanged(nameof(Media));
            OnPropertyChanged(nameof(SelectedMediumIndex));
            OnPropertyChanged(nameof(MediumSelected));
            OnPropertyChanged(nameof(SelectedMediumTitle));
        }

        public void LoadOrCreate(string dbPath)
        {
            DB.Instance.LoadOrCreateDatabase(dbPath);
            Media.Reload();
            NotifyAll();
        }

        public bool DBIsLoaded()
        {
            return DB.Instance.IsLoaded();
        }

        public void SaveAndCloseDB()
        {
            DB.Instance.SaveAndClose();
            NotifyAll();
        }

        #region Properties

        public bool DatabaseLoaded
        {
            get => DB.Instance.IsLoaded();
        }

        public string Title
        {
            get => "Kikitori - " + DB.Instance.Path;
        }

        MediaList media;
        public MediaList Media
        {
            get { return media; }
            set { media = value; SetField(ref media, value); }
        }

        public void NewMedium()
        {
            var newMedium = new Data.Medium { Title = "New Title", Description = "Give Description" };
            Media.InsertAndUpdateDB(Media.Count, newMedium);
            NotifyAll();
        }

        public void Reset()
        {
            DB.Instance.Dispose();
        }

        public void DeleteSelectedMedium()
        {
            if (SelectedMediumIndex != -1)
            {
                try
                {
                    Media.RemoveMedium(SelectedMediumIndex);
                }
                catch (System.ArgumentOutOfRangeException)
                { }
                NotifyAll();
            }
        }

        public void ResetTrainingData()
        {
            if (SelectedMediumIndex != -1)
            {
                Media.ResetTrainingData(SelectedMediumIndex);
            }
        }

        public void TransferToDB()
        {
            Media.TransferToDB();
        }

        private int selectedMediumIndex = -1;
        public int SelectedMediumIndex
        {
            get { return selectedMediumIndex; }
            set
            {
                selectedMediumIndex = value; NotifyAll();
            }
        }

        public bool MediumSelected
        {
            get { return DatabaseLoaded && SelectedMediumIndex != -1; }
        }

        public string SelectedMediumTitle
        {
            get
            {
                if (MediumSelected)
                {
                    return Media[SelectedMediumIndex].Title;
                }
                else
                {
                    return "";
                }
            }
        }

        private int quizCount = 0;
        public int QuizCount
        {
            get { return quizCount; }
            set
            {
                SetField(ref quizCount, value, nameof(QuizCount));
            }
        }

        #endregion

    }
}

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
            OnPropertyChanged("Title");
            OnPropertyChanged("DatabaseLoaded");
            OnPropertyChanged("Media");
            OnPropertyChanged("SelectedMediumIndex");
            OnPropertyChanged("MediumSelected");
            OnPropertyChanged("SelectedMediumTitle");
        }

        public void LoadOrCreate(string dbPath)
        {
            DB.Instance.LoadOrCreateDatabase(dbPath);
            Media.Reload();
            NotifyAll();
        }

        public void Save()
        {
            DB.Instance.SaveAll();
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
            Media.Insert(newMedium);
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
        #endregion

    }
}

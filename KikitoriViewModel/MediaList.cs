// Kikitori
// (C) 2021, Andreas Gaiser

using System.Linq;
using System.Collections.ObjectModel;
using DB = Kikitori.Data.Database;
using Medium = Kikitori.Data.Medium;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.ViewModel
{
    public class MediaList : ObservableCollection<Medium>
    {

        public void Reload()
        {
            Clear();
            foreach (var medium in DB.Instance.GetItems<Medium>())
            {
                Add(medium);
            }
        }

        public void TransferToDB()
        {
            foreach (var element in Items)
            {
                if (element is Medium medium)
                {
                    DB.Instance.Update(medium);
                }
            }
        }

        public void Insert(Medium medium)
        {
            Add(medium);
            DB.Instance.Save<Medium>(medium);
        }

        public void RemoveMedium(int index)
        {
            Medium mediumToDelete = this[index];
            RemoveAt(index);
            DB.Instance.Delete(mediumToDelete);
            // delete sentence items
            foreach (var item
                     in DB.Instance.GetItems<SentenceItem>().Where(item => item.ExerciseMediumLink == mediumToDelete.ID))
            {
                DB.Instance.Delete(item);
            }
        }
    }
}

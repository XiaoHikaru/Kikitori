// Kikitori
// (C) 2021, Andreas Gaiser

using System.Linq;
using System.Collections.ObjectModel;
using DB = Kikitori.Data.Database;
using Medium = Kikitori.Data.Medium;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.ViewModel
{
    public class SentenceItemsList : ObservableCollection<SentenceItem>
    {
        public void Reload(Medium medium)
        {
            Clear();
            foreach (var item in DB.Instance.GetItems<SentenceItem>().Where(item => item.ExerciseMediumLink == medium.ID).OrderBy(item => item.SequenceOrderIndex))
            {
                Add(item);
            }
        }

        public void TransferToDB(Medium medium, bool deleteTrainingData)
        {
            int sequenceOrderIndex = 1;
            foreach (var element in Items)
            {
                if (element is SentenceItem item)
                {
                    // TODO
                    if (!EntryInputVM.CheckValidityOfEntry(item.Sentence, item.Furigana))
                    {
                        item.Furigana = item.Sentence;
                    }
                    if (deleteTrainingData)
                    {
                        item.TrainedKnownTokens = "";
                    }
                    item.ExerciseMediumLink = medium.ID;
                    item.SequenceOrderIndex = sequenceOrderIndex++;
                    DB.Instance.Update(item);
                }
            }
        }

        public void Insert(SentenceItem item, Medium medium)
        {
            item.ExerciseMediumLink = medium.ID;
            Add(item);
            DB.Instance.Save<SentenceItem>(item);
        }

        public void RemoveSentenceItem(int index)
        {
            SentenceItem itemToDelete = this[index];
            RemoveAt(index);
            DB.Instance.Delete(itemToDelete);
        }


    }
}

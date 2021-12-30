// Kikitori
// (C) 2021, Andreas Gaiser

namespace Kikitori.Data
{

    public class SentenceItem : Entity
    {
        public byte[] MP3Audio { get; set; }

        public string Sentence { get; set; }

        public string Furigana { get; set; }

        // Example: 0;1;3
        public string TrainedKnownTokens { get; set; }

        public int SequenceOrderIndex { get; set; }

        // References Medium table
        public int ExerciseMediumLink { get; set; }

        // References training status database (TODO)
        public int TrainingStatusLink { get; set; }
    }
}

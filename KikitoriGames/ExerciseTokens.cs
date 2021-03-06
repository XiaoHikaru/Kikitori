// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Collections.Generic;
using System.Linq;
using Kikitori.Kanji;
using System.Threading.Tasks;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.Games
{
    internal class ExerciseTokens
    {
        string sentence;
        string furigana;
        string trainedKnownTokens;
        List<int> knownTokens;
        List<string> allTokensSentence;
        List<string> allTokensFurigana;

        public ExerciseTokens(SentenceItem item)
        {
            sentence = item.Sentence;
            furigana = item.Furigana;
            trainedKnownTokens = item.TrainedKnownTokens;
            allTokensSentence = sentence.Split('*').ToList();
            allTokensFurigana = furigana.Split('*').ToList();

            knownTokens = new List<int>();

            if (trainedKnownTokens != null)
            {
                foreach (var entry in trainedKnownTokens.Split(';'))
                {
                    if (Int32.TryParse(entry, out int result))
                    {
                        knownTokens.Add(result);
                    }
                }
            }
        }

        public int TokenCount
        {
            get { return allTokensSentence.Count; }
        }

        public int CorrectTokenCount
        {
            get
            {
                if (String.IsNullOrEmpty(trainedKnownTokens))
                {
                    return 0;
                }
                else
                {
                    return trainedKnownTokens.Split(';').Length;
                }
            }
        }

        public List<int> GetAllUnknownTokens()
        {
            List<int> result = new List<int>();
            for (int i = 0; i < allTokensSentence.Count; i++)
            {
                if (!IsKnown(i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public void SplitSentence(int tokenIndex, out string prefix, out string suffix)
        {
            prefix = null;
            suffix = null;
            for (int i = 0; i < allTokensSentence.Count; i++)
            {
                if (i < tokenIndex)
                {
                    if (prefix == null)
                    {
                        prefix = allTokensSentence[i];
                    }
                    else
                    {
                        prefix += " " + allTokensSentence[i];
                    }
                }
                else if (i > tokenIndex)
                {
                    if (suffix == null)
                    {
                        suffix = allTokensSentence[i];
                    }
                    else
                    {
                        suffix += " " + allTokensSentence[i];
                    }
                }
            }
        }

        public bool IsMastered()
        {
            return !GetAllUnknownTokens().Any();
        }

        public bool IsKnown(int tokenIndex)
        {
            return knownTokens.Contains(tokenIndex);
        }

        public void AddKnownToken(int tokenIndex)
        {
            if (!knownTokens.Contains(tokenIndex))
            {
                knownTokens.Add(tokenIndex);
            }
        }

        public string GetKnownTokenListAsString()
        {
            return String.Join(";", knownTokens);
        }

        private string Normalize(string s)
        {
            return s.Replace(" ", "").Replace("、", "").Replace("。", "").Replace("」", "").Replace("「", "").Replace("　", "").Replace(",", "").Replace(".", "").Replace("ー", "-");
        }

        private string ComputeMinimalDiff(string solution, string sentence, string furigana, string romaji)
        {
            solution = Normalize(solution);
            var sentenceDiff = new SimpleMyersDiff(solution, Normalize(sentence));
            var furiganaDiff = new SimpleMyersDiff(solution, Normalize(furigana));
            var romajiDiff = new SimpleMyersDiff(solution, Normalize(romaji));

            int countSentence = sentenceDiff.NumberOfDeletions(true);
            int countFurigana = furiganaDiff.NumberOfDeletions(true);
            int countRomaji = romajiDiff.NumberOfDeletions(true);

            if (countSentence <= countFurigana)
            {
                if (countSentence <= countRomaji)
                {
                    return sentenceDiff.GetDiffAsText(true) + "\n" + sentenceDiff.GetDiffAsText(false);
                }
                else
                {
                    return romajiDiff.GetDiffAsText(true) + "\n" + romajiDiff.GetDiffAsText(false);
                }
            }
            else
            {
                if (countFurigana <= countRomaji)
                {
                    return furiganaDiff.GetDiffAsText(true) + "\n" + furiganaDiff.GetDiffAsText(false);
                }
                else
                {
                    return romajiDiff.GetDiffAsText(true) + "\n" + romajiDiff.GetDiffAsText(false);
                }
            }
        }

        public (bool, string) IsCorrectAnswer(int tokenIndex, string answer)
        {
            answer = Normalize(answer);
            if (StringComparer.InvariantCultureIgnoreCase.Equals(answer, Normalize(allTokensFurigana[tokenIndex]))
                || StringComparer.InvariantCultureIgnoreCase.Equals(answer, Normalize(allTokensSentence[tokenIndex])))
            {
                return (true, "");
            }
            string asRomaji = Kanji.Furiganas.GetRomajiNonAsync(allTokensFurigana[tokenIndex]);
            if (StringComparer.InvariantCultureIgnoreCase.Equals(answer, Normalize(asRomaji)))
            {
                return (true, "");
            }
            string diffText = ComputeMinimalDiff(answer, allTokensSentence[tokenIndex], allTokensFurigana[tokenIndex], asRomaji);
            return (false, diffText);
        }
    }
}

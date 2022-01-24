// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Collections.Generic;
using System.Linq;
using SentenceItem = Kikitori.Data.SentenceItem;

namespace Kikitori.Games
{
    public class GapQuiz
    {

        List<SentenceItem> currentItems;
        int currentTokenIndex;
        ExerciseTokens currentExerciseTokens;

        public GapQuiz(List<SentenceItem> items)
        {
            currentItems = items;
        }

        public int TokensCount
        {
            get
            {
                int result = 0;
                foreach (var item in currentItems)
                {
                    var tokens = new ExerciseTokens(item);
                    result += tokens.TokenCount;
                }
                return result;
            }
        }

        public int CorrectTokensCount
        {
            get
            {
                int result = 0;
                foreach (var item in currentItems)
                {
                    var tokens = new ExerciseTokens(item);
                    result += tokens.CorrectTokenCount;
                }
                return result;
            }
        }

        /// <summary>
        /// Returns true iff there is at least one exercise token in <paramref name="item"/> which
        /// has not been entered correctly by the player yet.
        /// </summary>
        /// <param name="item"></param>
        public bool HasExerciseToken(SentenceItem item)
        {
            var tokens = new ExerciseTokens(item);
            return tokens.GetAllUnknownTokens().Any();
        }

        /// <summary>
        /// Returns false if no more exercise token could be found.
        /// </summary>
        public bool GetNextExerciseToken(out string prefix, out string suffix, out SentenceItem sentenceItem)
        {
            var random = new Random();
            prefix = null;
            suffix = null;
            sentenceItem = null;
            var candidates = currentItems.Where(item => HasExerciseToken(item)).ToList();
            if (!candidates.Any())
            {
                return false;
            }
            sentenceItem = candidates[random.Next(candidates.Count)];
            currentExerciseTokens = new ExerciseTokens(sentenceItem);
            var unknowTokens = currentExerciseTokens.GetAllUnknownTokens();
            int tokenIndex = unknowTokens[random.Next(unknowTokens.Count)];
            currentTokenIndex = tokenIndex;
            currentExerciseTokens.SplitSentence(tokenIndex, out prefix, out suffix);
            return true;
        }

        public (bool, string) CheckInput(string answer)
        {
            var (isCorrect, diffText) = currentExerciseTokens.IsCorrectAnswer(currentTokenIndex, answer);
            if (isCorrect)
            {
                currentExerciseTokens.AddKnownToken(currentTokenIndex);
            }
            return (isCorrect, diffText);
        }

        public string GetKnownTokens()
        {
            return currentExerciseTokens.GetKnownTokenListAsString();
        }
    }
}

// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Threading.Tasks;
using Kawazu;

namespace Kikitori.Kanji
{
    public class Furiganas
    {

        public static async Task<string> GetSeparation(string sentence, bool useHiragana)
        {
            if (sentence == null)
            {
                sentence = "";
            }
            var converter = new KawazuConverter();
            var divisions = await converter.GetDivisions(sentence, To.Hiragana, Mode.Normal, RomajiSystem.Hepburn, "(", ")");
            string result = "";
            foreach (Division d in divisions)
            {
                if (!String.IsNullOrEmpty(result))
                {
                    result += "*";
                }
                if (useHiragana)
                {
                    result += d.HiraReading;
                }
                else
                {
                    result += d.Surface;
                }
            }

            return result;
        }


        public static async Task<string> GetFuriganaProposition(string sentence)
        {
            if (sentence == null)
            {
                sentence = "";
            }
            var converter = new KawazuConverter();
            var result = await converter.Convert(sentence, To.Hiragana, Mode.Normal, RomajiSystem.Hepburn, "(", ")");
            return result;
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kikitori.Kanji;
using System;

namespace KikitoriKanjiTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var string1 = "引っ越しする時 てつだって いただけませんか";
            for (int i = 0; i < string1.Length; i++)
            {
                Console.WriteLine(i + ":" + string1[i]);
            }
            var string2 = "引っ越しするとき 手伝って いただけませんか";
            for (int i = 0; i < string2.Length; i++)
            {
                Console.WriteLine(i + ":" + string2[i]);
            }

            var diff = new SimpleMyersDiff(string1, string2);
            System.Console.WriteLine(diff.Diff());
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kikitori.Kanji;
using System;

namespace KikitoriKanjiTest
{
    [TestClass]
    public class SimpleMyersDiffTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var string1 = "引っ越しする時 てつだって いただけませんか";
            var string2 = "引っ越しするとき 手伝って いただけませんか";
            var expected1 = "引っ越しする<!!時!!> <!!てつだ!!>って いただけませんか";
            var expected2 = "引っ越しする<!!とき!!> <!!手伝!!>って いただけませんか";

            var diff = new SimpleMyersDiff(string1, string2);
            Assert.AreEqual(expected1, diff.GetDiffAsText(true));
            Assert.AreEqual(expected2, diff.GetDiffAsText(false));
        }

        [TestMethod]
        public void TestMethod2()
        {
            var string1 = "引っ越しする時 てつだって いただけませんか";
            var string2 = "引っ越しする時 てつだって いただけませんか";
            var expected1 = "引っ越しする時 てつだって いただけませんか";
            var expected2 = "引っ越しする時 てつだって いただけませんか";

            var diff = new SimpleMyersDiff(string1, string2);
            Assert.AreEqual(expected1, diff.GetDiffAsText(true));
            Assert.AreEqual(expected2, diff.GetDiffAsText(false));
        }

        [TestMethod]
        public void TestMethod3()
        {
            var string1 = "引っ越しする時 てつだって いただけませんか";
            var string2 = "引っ越しする時 てだって いただけませんか";
            var expected1 = "引っ越しする時 て<!!つ!!>だって いただけませんか";
            var expected2 = "引っ越しする時 てつだって いただけませんか";
            var diff = new SimpleMyersDiff(string1, string2);
            Assert.AreEqual(expected1, diff.GetDiffAsText(true));
            Assert.AreEqual(expected2, diff.GetDiffAsText(false));
        }
    }
}
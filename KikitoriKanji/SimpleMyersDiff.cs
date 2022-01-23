using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Kikitori.Kanji
{

    public enum DiffElementKind : byte
    {
        REMOVE_A = 0,
        REMOVE_B = 1,
        COMMON = 2,
    }

    public class DiffElement
    {
        public DiffElementKind Kind { get; set; }
        public string Segment { get; set; } = "";
        public int StartPos { get; set; } = 0; // for REMOVE_A, REMOVE_B

        public DiffElement(DiffElementKind kind)
        {
            Kind = kind;
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case DiffElementKind.REMOVE_A:
                    return "<remove from first string at position " + StartPos + ":" + Segment + ">";
                case DiffElementKind.REMOVE_B:
                    return "<remove from second string at position " + StartPos + ":" + Segment + ">";
                default:
                    return "<common string: " + Segment + ">";
            }
        }
    }

    public static class StringUtils
    {
        public static IEnumerable<string> GraphemeClusters(this string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }
        public static string ReverseGraphemeClusters(this string s)
        {
            return string.Join("", s.GraphemeClusters().Reverse().ToArray());
        }
    }


    public class SimpleMyersDiff
    {
        int maxValue;

        string a;
        int lengthA;

        string b;
        int lengthB;

        (int x, int y)[,] computedTable;

        (int x, int y) GetComputedTable(int D, int k)
        {
            return computedTable[D, k + maxValue];
        }
        void SetComputedTable(int D, int k, (int x, int y) value)
        {
            computedTable[D, k + maxValue] = value;
        }

        int[,] predecessor;


        int GetPredecessor(int D, int k)
        {
            return predecessor[D, k + maxValue];
        }
        void SetPredecessor(int D, int k, int value)
        {
            predecessor[D, k + maxValue] = value;
        }





        public SimpleMyersDiff(string a, string b)
        {
            this.a = a;
            this.b = b;
            lengthA = a.Length;
            lengthB = b.Length;
            maxValue = a.Length + b.Length;
            computedTable = new (int x, int y)[maxValue + 1, 2 * maxValue + 1];
            predecessor = new int[maxValue + 1, 2 * maxValue + 1];
        }


        void PushEntryIfNotNullAndResetSegment(DiffElement entry, List<DiffElement> result, StringBuilder segment, int startPos)
        {
            if (entry != null)
            {
                entry.Segment = segment.ToString();
                entry.StartPos = startPos;
                segment.Clear();
                result.Insert(0, entry);
            }
        }

        List<DiffElement> ComputeEdit(int finalK, int finalD)
        {
            var result = new List<DiffElement>();
            StringBuilder currentSegment = new StringBuilder();
            (int x, int y) currentPoint = (lengthA - 1, lengthB - 1);
            DiffElement editEntry = null;
            var currentK = finalK;
            var currentStringPos = 0;
            for (int backwardD = finalD; backwardD > 0; --backwardD)
            {
                (int x, int y) previousPoint = currentPoint;
                currentPoint = GetComputedTable(backwardD, currentK);
                Console.WriteLine("CURRENT POINT: " + currentPoint);

                if (GetPredecessor(backwardD, currentK) == 1)
                {
                    currentK++;
                    currentPoint = GetComputedTable(backwardD - 1, currentK);
                    Console.WriteLine("KILL b:" + b[currentPoint.y + 1]);
                    if (editEntry == null || editEntry.Kind != DiffElementKind.REMOVE_B)
                    {
                        PushEntryIfNotNullAndResetSegment(editEntry, result, currentSegment, currentStringPos);
                        editEntry = new DiffElement(DiffElementKind.REMOVE_B);
                    }
                    currentSegment.Insert(0, b[currentPoint.y + 1]);
                    currentStringPos = currentPoint.y + 1;
                }
                else
                {
                    currentK--;
                    currentPoint = GetComputedTable(backwardD - 1, currentK);
                    Console.WriteLine("KILL a:" + a[currentPoint.x + 1]);
                    if (editEntry == null || editEntry.Kind != DiffElementKind.REMOVE_A)
                    {
                        PushEntryIfNotNullAndResetSegment(editEntry, result, currentSegment, currentStringPos);
                        editEntry = new DiffElement(DiffElementKind.REMOVE_A);
                    }
                    currentSegment.Insert(0, a[currentPoint.x + 1]);
                    currentStringPos = currentPoint.x + 1;
                }
                Console.WriteLine(currentPoint + "");
            }
            if (currentSegment.Length != 0)
            {
                PushEntryIfNotNullAndResetSegment(editEntry, result, currentSegment, currentStringPos);
            }
            foreach (var entry in result)
            {
                Console.WriteLine(entry.ToString());
            }
            return result;
        }

        public (int x, int y) Diff()
        {
            // idea: compute (x,y) = max_end_point(D, k), where
            // - D is the number of non-diagonal edges
            // - k is the "diagonal" of (x,y), i.e. x-y = k
            // Rule: If (x, y) = max_end_point(D, k),
            // then either
            // (0,0) ->* (x', y') = max_end_point(D-1, k-1) -> (x'+1,y') -> (x'+1+n,y'+n) = (x,y)
            // or
            // (0,0) ->* (x', y') = max_end_point(D-1, k+1) -> (x',y'-1) -> (x'+n,y'-1+n) = (x,y)
            // is (x', y') in {max_end_point(D-1, k'), max_end_point(D, k')
            for (int D = 0; D <= maxValue; ++D)
            {
                for (int k = -D; k <= D; k += 1) // better: +=2
                {
                    (int x, int y) candidate = (0, 0);
                    if (D > 0)
                    {
                        if (k == -D || (k != D && GetComputedTable(D - 1, k - 1).x < GetComputedTable(D - 1, k + 1).x))
                        {
                            candidate = GetComputedTable(D - 1, k + 1);
                            candidate.y++;
                            SetPredecessor(D, k, 1);
                        }
                        else
                        {
                            candidate = GetComputedTable(D - 1, k - 1);
                            candidate.x++;
                            SetPredecessor(D, k, -1);
                        }
                    }
                    while (candidate.x < lengthA - 1 && candidate.y < lengthB - 1 && a[candidate.x + 1] == b[candidate.y + 1])
                    {
                        candidate = (candidate.x + 1, candidate.y + 1);
                    }
                    SetComputedTable(D, k, candidate);

                    if (candidate.x >= lengthA - 1 && candidate.y >= lengthB - 1)
                    {
                        ComputeEdit(k, D);
                        return candidate;
                    }
                }
            }
            throw new Exception("error");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpiKthElement.Domain;

namespace MpiKthElement
{
    public class Utilities
    {
        public static List<int> FillListWithRandomNumbers(int listSize)
        {
            Random rnd = new Random();
            List<int> randomList = new List<int>();
            for (int i = 0; i < listSize; i++)
            {
                var rand = rnd.Next(1, 10000);
                if (randomList.Contains(rand))
                {
                    i--;
                }
                else
                {
                    randomList.Add(rand);
                }
            }
            return randomList;
        }

        public static int ComputeMedian(int[] sourceNumbers)
        {
            return sourceNumbers.GetMedian();
        }

        /// <summary>
        /// http://stackoverflow.com/questions/9794558/weighted-median-computation
        /// </summary>
        public static int ComputeWeightedMedian(MedianWithElemCount[] source, int n)
        {
            var sorted = source.OrderBy(x => x.Median);

            var medians = sorted.Select(x => x.Median).ToArray();
            var weights = sorted.Select(x => (decimal)x.ElemCount / (decimal) n).ToArray();

            decimal S = weights.Sum();

            int k = 0;
            decimal sum = S - weights[0]; // sum is the total weight of all `x[i] > x[k]`

            while(sum > S/2)
            {
                ++k;
                sum -= weights[k];
            }

            return medians[k];
        }

        public static Leg ComputeLeg(int weightedMedian, int[] distributedList)
        {
            return new Leg()
            {
                Less = distributedList.Where(x => x < weightedMedian).Count(),
                Eq = distributedList.Where(x => x == weightedMedian).Count(),
                Greater = distributedList.Where(x => x > weightedMedian).Count()
            };
        }

        public static int[] CalcNoElems(int itemLen, int np)
        {
            int[] sendCounts = new int[np];
            int rem = itemLen % np;
            for (int i = 0; i < np; i++)
            {
                sendCounts[i] = itemLen / np;
                if (rem > 0)
                {
                    sendCounts[i]++;
                    rem--;
                }
            }
            return sendCounts;
        }

        public static int SolveSequentially(int[] nList, int k)
        {
            //Processor 1 solves the remaining problem sequentially
            var p1weightedMedian = nList.GetMedian();
            var p1Leg = Utilities.ComputeLeg(p1weightedMedian, nList);
            // if L<k<L+E then return solution M and stop
            if (p1Leg.Less < k && k <= (p1Leg.Less + p1Leg.Eq))
            {
                return p1weightedMedian;
            }
            else if (k <= p1Leg.Less)
            {
                nList = nList.Where(x => x < p1weightedMedian).ToArray();
                return SolveSequentially(nList, k);
            }
            else if (k > p1Leg.Less + p1Leg.Eq)
            {
                for (int i = 0; i < nList.Length; i++)
                {
                    nList = nList.Where(x => x > p1weightedMedian).ToArray();
                    k = k - (p1Leg.Less + p1Leg.Eq);
                    return SolveSequentially(nList, k);
                }
            }
            return p1weightedMedian;
        }

    }
}

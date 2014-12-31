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
                randomList.Add(rnd.Next(0, 100));
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
            var weights = sorted.Select(x => x.ElemCount / n).ToArray();

            double S = weights.Sum();

            int k = 0;
            double sum = S - weights[0]; // sum is the total weight of all `x[i] > x[k]`

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
                less = distributedList.Where(x => x < weightedMedian).Count(),
                eq = distributedList.Where(x => x == weightedMedian).Count(),
                greater = distributedList.Where(x => x > weightedMedian).Count()
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

    }
}

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
                randomList.Add(rnd.Next(0, 10));
            }
            return randomList;
        }

        public static int ComputeMedian(int[] sourceNumbers)
        {
            return sourceNumbers.GetMedian();
        }

        public static int ComputeWeightedMedian(IEnumerable<MedianWithElements> x)
        {
            //TODO: Compute weighted median
            return 8;
        }

        public static Leg ComputeLeg(int weightedMedian, int[] distributedList)
        {
            Leg leg = new Leg();
            leg.l = distributedList.Where(x => x < weightedMedian).Count();
            leg.e = distributedList.Where(x => x == weightedMedian).Count();
            leg.g = distributedList.Where(x => x > weightedMedian).Count();
            return leg;
        }
    }
}

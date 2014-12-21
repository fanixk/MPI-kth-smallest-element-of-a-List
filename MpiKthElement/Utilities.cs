using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                randomList.Add(rnd.Next(0, 99999));
            }
            return randomList;
        }
    }
}

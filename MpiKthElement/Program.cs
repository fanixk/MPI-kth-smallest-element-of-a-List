using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;
using MpiKthElement.Domain;

namespace MpiKthElement
{
    class Program
    {
        const int c = 1;

        static void Main(string[] args)
        {
           List<int> nList = new List<int>
                    {
                        7, 4, 6, 4, 2, 4, 2, 3
                    };
           nList = Utilities.FillListWithRandomNumbers(10);
           Console.WriteLine("list: {0}", String.Join(",", nList));

           for (int i = 1; i <= 10; i++)
           {
               var m = Utilities.SolveSequentially(nList.ToArray(), i);
               Console.WriteLine("Solution Found sequentially and is : {0} for k {1}", m, i);
           }

           nList.Sort();
           Console.WriteLine("Sorted List: {0}", String.Join(",", nList));

           Console.WriteLine("Press any key to exit...");
           Console.Read();

        }
    }
}

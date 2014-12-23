using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;

namespace MpiKthElement
{
    class Program
    {
        const int c = 1;

        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args, Threading.Multiple))
            {
                // MPI program goes here!

                int repeatTimes = 0;
                List<int> nList = new List<int>();
                Intracommunicator comm = Communicator.world;
                int[] distributedList;
                if (comm.Rank == 0)
                {
                    //set N:=n
                    Console.Write("Give N :");
                    string userInputN = Console.ReadLine();
                    int n;
                    if (!int.TryParse(userInputN, out n))
                    {
                        throw (new Exception("n must be integer"));
                    }


                    //find n/cp
                    repeatTimes = n / MPI.Communicator.world.Size;
                    //broadcast repeat times
                    comm.Broadcast<int>(ref repeatTimes, 0);
                    nList = Utilities.FillListWithRandomNumbers(n);
                }

                //scatter all n elements among the p processorsm each processor i with ni = n/p elements
                distributedList = new int[repeatTimes];
                distributedList = comm.ScatterFromFlattened<int>(nList.ToArray(), repeatTimes, 0);

                if (comm.Rank == 0)
                {
                    Console.WriteLine("Number of processes : {0}", MPI.Communicator.world.Size);
                    Console.WriteLine("Iteration will processed {0} times", repeatTimes);
                }
                comm.Barrier();
                Console.WriteLine("List from p:{0} {1}", Communicator.world.Rank, distributedList.Count());


            }

        }
    }
}

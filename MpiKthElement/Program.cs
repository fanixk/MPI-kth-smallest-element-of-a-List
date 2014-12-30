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
                int[] sendcounts = null;
                List<int> nList = new List<int>();
                Intracommunicator comm = Communicator.world;
                int[] distributedList = null;
                int n = 0; ;


                if (comm.Rank == 0)
                {
                    //set N:=n
                    Console.Write("Give N :");
                    string userInputN = Console.ReadLine();
                    if (!int.TryParse(userInputN, out n))
                    {
                        throw (new Exception("n must be integer"));
                    }
                    
                    //find n/cp
                    repeatTimes = n / MPI.Communicator.world.Size;
                    nList = Utilities.FillListWithRandomNumbers(n);
                   
                }

                sendcounts = new int[MPI.Communicator.world.Size];
                int rem = n % MPI.Communicator.world.Size;
                for (int i = 0; i < MPI.Communicator.world.Size; i++)
                {
                    sendcounts[i] = n / MPI.Communicator.world.Size;
                    if (rem > 0)
                    {
                        sendcounts[i]++;
                        rem--;
                    }


                }
                comm.Broadcast<int>(ref sendcounts, 0);

                //scatter all n elements among the p processorsm each processor i with ni = n/p elements
                // distributedList = new int[repeatTimes];
                //Console.WriteLine("repeat times {0}", repeatTimes);
                
                comm.ScatterFromFlattened<int>(nList.ToArray(), sendcounts, 0, ref distributedList);

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

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
            using (new MPI.Environment(ref args, Threading.Multiple))
            {
                // MPI program goes here!
                int np = MPI.Communicator.world.Size;
                int repeatTimes = 0;
                int[] sendcounts = null;
                List<int> nList = new List<int>();
                Intracommunicator comm = Communicator.world;
                int[] distributedList = null;
                int n = 0;
                
                if (comm.Rank == 0)
                {
                    //set N:=n
                    Console.Write("Give N :");
                    string userInputN = Console.ReadLine();
                    if (!int.TryParse(userInputN, out n))
                    {
                        throw (new Exception("n must be integer"));
                    }
                    
                    nList = Utilities.FillListWithRandomNumbers(n);
                }

                //ScatterV (for non divisible arrays)
                sendcounts = new int[np];
                int rem = n % np;
                for (int i = 0; i < np; i++)
                {
                    sendcounts[i] = n / np;
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

                //compute median
                //step 2.1 each processor computes the median mi of its ni elements
                var localMedian = distributedList.GetMedian();
                MedianWithElements medianWithElements = new MedianWithElements();
                medianWithElements.Median = localMedian;
                medianWithElements.Elements = distributedList;
                //step 2.2 each processor i sends mi and ni to processor 1
                var listOfMedians = comm.Gather(medianWithElements, 0);
                //step 3.3 Processor 1 computes the weighted median M
                int weightedMedian = 0;
                if (comm.Rank == 0)
                {
                    weightedMedian = Utilities.ComputeWeightedMedian(listOfMedians);
                }
                
                //step 2.4 processor 1 broadcast to all processors
                comm.Broadcast(ref weightedMedian, 0);
                //step 2.5 Each processor process l,e,g
                var localLeg = Utilities.ComputeLeg(weightedMedian, distributedList);
                //step 2.6 each processor sends l,e,g to processor 1
                var legs = comm.Gather(localLeg, 0);
                if (comm.Rank == 0)
                {
                    Console.WriteLine("l={0}, e={1}, g{2}", legs[0].l, legs[0].e, legs[0].g);
                }
            }

        }
    }
}

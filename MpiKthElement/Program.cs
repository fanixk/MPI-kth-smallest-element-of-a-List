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
                Intracommunicator comm = Communicator.world;
                int np = MPI.Communicator.world.Size;
                int[] sendcounts = null;
                List<int> nList = new List<int>();
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

                //ScatterV (calculate items for non divisible arrays)
                sendcounts = new int[np];
                int[] sendCounts = Utilities.calcNoElems(n, np);
                

                //send number of list items to each process
                comm.Broadcast<int>(ref sendCounts, 0);

                //scatter all n elements among the p processors each processor i with ni = n/p elements                
                comm.ScatterFromFlattened<int>(nList.ToArray(), sendCounts, 0, ref distributedList);

                if (comm.Rank == 0)
                {
                    Console.WriteLine("Number of processes : {0}", MPI.Communicator.world.Size);
                }
                comm.Barrier();
                Console.WriteLine("List from p:{0} {1}", Communicator.world.Rank, distributedList.Count());

                //compute median
                //step 2.1 each processor computes the median mi of its ni elements
                var localMedian = distributedList.GetMedian();
                MedianWithElemCount medianWithElemCount = new MedianWithElemCount()
                {
                    Median = localMedian,
                    ElemCount = distributedList.Count()
                };
           
                //step 2.2 each processor i sends mi and ni to processor 1
                var listOfMedians = comm.Gather(medianWithElemCount, 0);

                //step 3.3 Processor 1 computes the weighted median M
                int weightedMedian = 0;
                if (comm.Rank == 0)
                {
                    weightedMedian = Utilities.ComputeWeightedMedian(listOfMedians, n);
                    Console.WriteLine("weighted median = {0}", weightedMedian);
                }
                
                //step 2.4 processor 1 broadcast to all processors
                comm.Broadcast(ref weightedMedian, 0);
     
                //step 2.5 Each processor process l,e,g
                var localLeg = Utilities.ComputeLeg(weightedMedian, distributedList);

                //step 2.6 each processor sends l,e,g to processor 1
                var legs = comm.Gather(localLeg, 0);
                if (comm.Rank == 0)
                {
                    Console.WriteLine("l={0}, e={1}, g{2}", legs[0].less, legs[0].eq, legs[0].greater);
                }
            }

        }
    }
}

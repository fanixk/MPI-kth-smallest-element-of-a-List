﻿using System;
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
                int k = 0;

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
                    //nList = new List<int>
                    //{
                    //    7, 4, 6, 4, 2, 4, 2, 3
                    //};
                    //nList = new List<int>
                    //{
                    //    8,5,5,6,3,9,9,3,2,5
                    //};

                    Console.WriteLine("List : {0}", String.Join(",", nList));

                    //set k
                    Console.Write("Give k (kth element):");
                    string userInputK = Console.ReadLine();
                    if (!int.TryParse(userInputK, out k))
                    {
                        throw (new Exception("k must be integer"));
                    }

                }

                //ScatterV (calculate items for non divisible arrays)
                sendcounts = new int[np];
                int[] sendCounts = Utilities.CalcNoElems(n, np);


                //send number of list items to each process
                comm.Broadcast<int>(ref sendCounts, 0);

                //scatter all n elements among the p processors each processor i with ni = n/p elements                
                comm.ScatterFromFlattened<int>(nList.ToArray(), sendCounts, 0, ref distributedList);

                if (comm.Rank == 0)
                {
                    Console.WriteLine("Number of processes : {0}", MPI.Communicator.world.Size);
                }
                comm.Barrier();
                Console.WriteLine("List from p:{0} : {1}", Communicator.world.Rank, String.Join(",", distributedList));

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

                //step 2.3 Processor 1 computes the weighted median M
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

                //step 2.7 Processor 1 computes L,E,G Sums respectively the total numbers of elements less than, equal to, or greater than M
                var summLess = comm.Reduce(localLeg.Less, Operation<int>.Add, 0);
                var summEqual = comm.Reduce(localLeg.Eq, Operation<int>.Add, 0);
                var summGreater = comm.Reduce(localLeg.Greater, Operation<int>.Add, 0);

                //2.8 Processor 1 broadcasts L,E,G summaries to all other processors
                comm.Broadcast(ref summLess, 0);
                comm.Broadcast(ref summEqual, 0);
                comm.Broadcast(ref summGreater, 0);

                //step 2.9
                // if L<k<L+E then return solution M and stop
                string command = "";
                if (comm.Rank == 0)
                {
                    if (summLess < k && k <= (summLess + summEqual))
                    {
                        Console.WriteLine("Solution Found in M and is : {0}", weightedMedian);
                        comm.Abort(0);
                        return;

                    }
                    else if (k <= summLess)
                    {
                        //send command to discard all but those less
                        command = "<";
                    }
                    else if (k > (summLess + summEqual))
                    {
                        //send command to discard all but those greater
                        command = ">";
                    }

                }

                comm.Broadcast(ref command, 0);
                
                if (command == "<")
                {
                    distributedList = distributedList.Where(x => x < weightedMedian).ToArray();
                    n = summLess;
                }
                else if (command == ">")
                {
                    distributedList = distributedList.Where(x => x > weightedMedian).ToArray();
                    n = summGreater;
                    k = k - (summLess + summEqual);
                }

                //var totalNumberOfRemainingElements = comm.Reduce(distributedList.Length, Operation<int>.Add, 0);
                //Console.WriteLine("total remaining elements : {0} p {1}", totalNumberOfRemainingElements, Communicator.world.Rank);

                int[][] rem; 
                rem = comm.Gather(distributedList, 0);

                if (comm.Rank == 0)
                {

                    var tempList = rem.ToList();
                    int[] remainingElements = tempList.SelectMany(i => i).ToArray();
                    
                    var m = Utilities.SolveSequentially(remainingElements, k);
                    Console.WriteLine("Solution Found sequentially and is : {0}", m);
                    //Console.WriteLine("remaining elements : {0}", String.Join(",", remainingElements));
                }
               
            }

        }
    }
}

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

                    //set k
                    Console.Write("Give k (kth element):");
                    string userInputK = Console.ReadLine();
                    if (!int.TryParse(userInputK, out k))
                    {
                        throw (new Exception("k must be integer"));
                    }

                    nList = Utilities.FillListWithRandomNumbers(n);
                    Console.WriteLine("List : {0}", String.Join(",", nList));
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
                if (comm.Rank == 0)
                {
                    Console.WriteLine("l={0}, e={1}, g{2}", legs[0].Less, legs[0].Eq, legs[0].Greater);
                }

                //step 2.7 Processor 1 computes L,E,G Sums respectively the total numbers of elements less than, equal to, or greater than M
                var summLess = comm.Reduce(localLeg.Less, Operation<int>.Add, 0);
                var summEqual = comm.Reduce(localLeg.Eq, Operation<int>.Add, 0);
                var summGreater = comm.Reduce(localLeg.Greater, Operation<int>.Add, 0);
                //2.8 Processor 1 broadcasts L,E,G summaries to all other processors
                comm.Broadcast(ref summLess, 0);
                comm.Broadcast(ref summEqual, 0);
                comm.Broadcast(ref summGreater, 0);

                Console.WriteLine("sum less={0}, sum eq={1}, sum greater={2}", summLess, summEqual, summGreater);

                bool solutionFoundinM = false;
                //step 2.9
                // if L<k<L+E then return solution M and stop
                if (summLess < k && k <= (summLess + summEqual))
                {
                    solutionFoundinM = true;
                    if (comm.Rank != 0)
                    {
                        comm.Send<bool>(solutionFoundinM, 0, 0);
                    }
                }
                // if K <= L then each processor discards all but those elements less than M and set N:=L
                else if (k <= summLess)
                {
                    for (int i = 0; i < distributedList.Count(); i++)
                    {
                        //discart all those elements less than M
                        distributedList = distributedList.Where(x => x > weightedMedian).ToArray();
                        //set N:=L
                        n = summLess;
                    }
                    solutionFoundinM = false;
                    if (comm.Rank != 0)
                    {
                        comm.Send<bool>(solutionFoundinM, 0, 0);
                    }
                }
                // if k > L + E then each processor discards all but those elements greater than M and set N:=G and k:=k-(L+E) 
                else if (k > summLess + summEqual)
                {
                    for (int i = 0; i < distributedList.Count(); i++)
                    {
                        //discart all those elements less than M
                        distributedList = distributedList.Where(x => x < weightedMedian).ToArray();
                        //set N:=G and k:=k-(L+E)
                        n = summGreater;
                        k = k - (summLess + summEqual);
                    }
                    solutionFoundinM = false;
                    if (comm.Rank != 0)
                    {
                        comm.Send<bool>(solutionFoundinM, 0, 0);
                    }
                }

                //reduce sum total of elements number that are distributed among all processors
                var totalNumberOfRemainingElements = comm.Reduce(distributedList.Length, Operation<int>.Add, 0);
                int[] remainingElements = new int[totalNumberOfRemainingElements];
                if (!solutionFoundinM)
                {
                    //gather all remaining lists
                    comm.GatherFlattened(distributedList, 0, ref remainingElements);
                }
                if (comm.Rank == 0)
                {
                    comm.Receive(MPI.Communicator.anySource, 0, out solutionFoundinM);
                    if (solutionFoundinM)
                    {
                        Console.WriteLine("Solution Found in M and is : {0}", weightedMedian);
                    }
                    else
                    {
                        //Processor 1 solves the remaining problem sequentially
                        nList = Utilities.FillListWithRandomNumbers(n);
                        var p1weightedMedian = nList.GetMedian();
                        var p1Leg = Utilities.ComputeLeg(weightedMedian, distributedList);
                        // if L<k<L+E then return solution M and stop
                        if (p1Leg.Less < k && k < (p1Leg.Less + p1Leg.Eq))
                        {
                            Console.WriteLine("Solution Found in M and is : {0}", p1weightedMedian);
                        }
                        else
                        {

                            var m = Utilities.SolveSequentially(remainingElements, k);
                            Console.WriteLine("Solution Found sequentially and is : {0}", m);
                        }
                    }
                }
            }

        }
    }
}

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
                Console.WriteLine("Hello, World! from rank " + Communicator.world.Rank
                  + " (running on " + MPI.Environment.ProcessorName + ")");

                //set N:=n
                Console.Write("Give N :");
                string userInputN = Console.ReadLine();
                int n;
                if (!int.TryParse(userInputN, out n))
                {
                    throw (new Exception("n must be integer"));
                }

                
                //find n/cp
                int repeatTimes = n / MPI.Communicator.world.Size;
                Console.WriteLine("Number of processes : {0}", MPI.Communicator.world.Size);
                Console.WriteLine("Iteration will processed {0} times", repeatTimes);

                var nList = Utilities.FillListWithRandomNumbers(n);

                Console.ReadLine();
                /*
                Intracommunicator comm = Communicator.world;
                if (comm.Rank == 0)
                {
                    // program for rank 0
                    comm.Send("Rosie", 1, 0);

                    // receive the final message
                    string msg = comm.Receive<string>(Communicator.anySource, 0);

                    Console.WriteLine("Rank " + comm.Rank + " received message \"" + msg + "\".");
                }
                else // not rank 0
                {
                    // program for all other ranks
                    string msg = comm.Receive<string>(comm.Rank - 1, 0);

                    Console.WriteLine("Rank " + comm.Rank + " received message \"" + msg + "\".");

                    comm.Send(msg + ", " + comm.Rank, (comm.Rank + 1) % comm.Size, 0);
                }*/
            }

        }
    }
}

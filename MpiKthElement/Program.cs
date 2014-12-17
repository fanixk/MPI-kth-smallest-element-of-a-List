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
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args, Threading.Multiple))
            {
                // MPI program goes here!
                Console.WriteLine("Hello, World! from rank " + Communicator.world.Rank
                  + " (running on " + MPI.Environment.ProcessorName + ")");


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
                }
            }

        }
    }
}

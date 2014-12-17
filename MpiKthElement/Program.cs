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
            using (new MPI.Environment(ref args))
            {
                // MPI program goes here!
                Console.WriteLine("Hello, World! from rank " + Communicator.world.Rank
                  + " (running on " + MPI.Environment.ProcessorName + ")");
            }
            Console.ReadLine();
        }
    }
}

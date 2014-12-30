using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpiKthElement.Domain
{
    [Serializable]
    public class Leg
    {
        //Less
        public int less { get; set; }
        //Equal
        public int eq { get; set; }
        //Greater
        public int greater { get; set; }
    }
}

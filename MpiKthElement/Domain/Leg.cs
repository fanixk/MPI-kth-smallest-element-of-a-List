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
        public int l { get; set; }
        //Equal
        public int e { get; set; }
        //Greater
        public int g { get; set; }
    }
}

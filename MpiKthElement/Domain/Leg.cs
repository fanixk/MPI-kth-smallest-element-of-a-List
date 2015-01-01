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
        public int Less { get; set; }
        //Equal
        public int Eq { get; set; }
        //Greater
        public int Greater { get; set; }
    }
}

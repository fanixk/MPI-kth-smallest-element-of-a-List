using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpiKthElement.Domain
{
    [Serializable]
    public class MedianWithElemCount
    {
        public int Median { get; set; }
        public int ElemCount { get; set; }
    }
}

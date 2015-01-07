using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpiKthElement.Domain
{
    [Serializable]
    class Discard
    {
        public enum Type { None, AllButLesser, AllButGreater }
        public Type DiscardType { get; set; }
    }
}

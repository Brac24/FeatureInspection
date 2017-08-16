using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    class Inspection
    {
        private readonly int _opKey;
        private readonly int _lotSize;

        IList<Part> PartList { get; set; }

        int PartsInspected { get; set; }

        public Inspection (int lotSize, int opKey)
        {
            _lotSize = lotSize;
            _opKey = opKey;
        }
     }
}

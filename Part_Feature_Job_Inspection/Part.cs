using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    class Part
    {
        IList<Feature> Features { get; set; }

        bool ShouldInspect { get; set; }

        bool OutOfTolerance { get; set; }

        bool InProgress { get; set; }

    }
}

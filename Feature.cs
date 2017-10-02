using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public class Feature
    {
        public string FeatureType { get; set; }

        public string UnitOfMeasurement { get; set; }

        public double Nominal { get; set; }

        public double PlusTol { get; set; }

        public double MinusTol { get; set; }

        public double LowRange { get; set; }

        public double HighRange { get; set; }
        
        public double MeasuredValue { get; set; }
        
        public int InspectionToolSerial { get; set; }

        public string InspectionToolName { get; set; }

        public int PlacesToInspect { get; set; }
    }
}

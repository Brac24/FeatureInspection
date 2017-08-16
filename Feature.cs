using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public class Feature
    {
        string FeatureType { get; set; }

        string UnitOfMeasurement { get; set; }

        double Nominal { get; set; }

        double PlusTol { get; set; }

        double MinusTol { get; set; }

        double LowRange { get; set; }

        double HighRange { get; set; }

        double MeasuredValue { get; set; }

        int InspectionToolSerial { get; set; }

        string InspectionToolName { get; set; }

        int PlacesToInspect { get; set; }
    }
}

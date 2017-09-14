using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    interface IInspectionView
    {
        int OpKey { get; }
        string InspectionHeader { set; }

        int ListBoxIndex { get; set; }
        int ListBoxCount { get; }

        void BindDataGridViewInspection(DataTable featureTable);
    }
}

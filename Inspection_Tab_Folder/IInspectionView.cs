using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    interface IInspectionView
    {
        int OpKey { get; }
        string InspectionHeaderText { set; }

        int ListBoxIndex { get; set; }
        int ListBoxCount { get; }

        //void BindDataGridViewInspection(DataTable featureTable);

        BindingSource InspectionBindingSource { get; set; }

        DataGridView InspectionGrid { get; }

    }
}

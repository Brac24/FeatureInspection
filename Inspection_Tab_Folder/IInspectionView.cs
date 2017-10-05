using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
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

        BindingSource ListBoxBindingSource { get; set; }

        DataGridView InspectionGrid { get; }

        Chart InspectionChart { get; set; }

        ComboBox ChartFocusComboBox { get; set; }

        TextBox LotsizeTextBox { get; set; }

        TextBox OpKeyTextBox { get; set; }

        ListBox PartsListBox { get; set; }

        Label InspectionPageHeader { get; set; }

        Label PartNumberLabel { get; set; }

        Label JobLabel { get; set; }

        Label OperationLabel { get; set; }

        Label ChartLabel1 { get; set; }

        Label ChartLabel2 { get; set; }

    }
}

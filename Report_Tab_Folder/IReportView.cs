using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace Feature_Inspection
{
    interface IReportView
    {
        Chart ReportChart { get; set; }

        ComboBox ReportTypeComboBox { get; set; }

        TextBox ReportTB1 { get; set; }

        TextBox ReportTB2 { get; set; }

        Label ReportLB1 { get; set; }

        Label ReportLB2 { get; set; }
    }
}

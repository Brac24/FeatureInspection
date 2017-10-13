using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace Feature_Inspection
{
    class ReportPresenter
    {
        private IReportView view;
        private IFeaturesDataSource model;

        public ReportPresenter(IReportView view, IFeaturesDataSource model)
        {
            this.view = view;
            this.model = model;
            Initialize();
        }

        private void Initialize()
        {

        }

        public void ReportType_TextBoxVisibility()
        {
            if (view.ReportTypeComboBox.SelectedIndex == 0)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "OPKEY:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = false;
                view.ReportTB2.Visible = false;
            }
            if (view.ReportTypeComboBox.SelectedIndex == 1)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "PART:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = true;
                view.ReportTB2.Visible = true;
            }
        }

     
    }
}

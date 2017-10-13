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

        public void ReportScope_TextBoxVisibility()
        {
            if (view.ReportTypeComboBox.SelectedIndex == 0)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "OPKEY:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = false;
                view.ReportTB2.Visible = false;
                view.ReportTB2.Text = null;
                view.ReportTB1.Text = null;
                view.ReportTB1.Focus();
            }
            if (view.ReportTypeComboBox.SelectedIndex == 1)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "PART:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = true;
                view.ReportTB2.Visible = true;
                view.ReportTB2.Text = null;
                view.ReportTB1.Text = null;
                view.ReportTB1.Focus();
            }
        }

        public void check_ReportScope(object sender, KeyEventArgs e)
        {
            DataTable featureList;
            if (view.ReportTypeComboBox.SelectedIndex == 0)
            {
                filterTextBox(sender, e);
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    ValidateValidOpKey();
                    featureList = model.GetFeatureList(Int32.Parse(view.ReportTB1.Text));
                    BindFocusComboBox(featureList);
                }
            }
            if (view.ReportTypeComboBox.SelectedIndex == 1)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    //Some other method to check if part and operation exist.
                    OnEnterKeyInitializeDataGridView(sender, e);
                }
            }
        }

        public bool SetOpKeyReport()
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(Int32.Parse(view.ReportTB1.Text));

            if (info.Rows.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private void ValidateValidOpKey()
        {
            bool isValidOpKey = SetOpKeyReport();

            if (isValidOpKey)
            {
                ValidateReportExistForOpKey();
                MessageBox.Show("Valid OpKey");
            }
            else
            {
                InvalidOpKeyProcess();
            }

        }

        private bool ValidateReportExistForOpKey()
        {
            DataTable partList = null;
            bool inspectionExists = model.GetInspectionExistsOnOpKey(Int32.Parse(view.ReportTB1.Text));
            partList = model.GetPartsList(Int32.Parse(view.ReportTB1.Text));
            /*
            if (inspectionExists)
            {
                BeginInpsectionDataGridViewInitialization(partList);
            }
            else
            {
                CreateInspectionForOpKey();

                BeginInpsectionDataGridViewInitialization(partList);
            }
            */
            return inspectionExists;
        }

        private void InvalidOpKeyProcess()
        {
            MessageBox.Show(view.ReportTB1.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
            view.ReportTB1.Clear();
        }

        /// <summary>
        /// This method handles filtering non number characters out of the opKeyTextBox and lotSizeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void filterTextBox(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;
            int lotChars = textbox.Text.Length;
            //Block non-number characters
            char currentKey = (char)e.KeyCode;
            bool modifier = e.Control || e.Alt || e.Shift;
            bool nonNumber = char.IsLetter(currentKey) ||
                             char.IsSymbol(currentKey) ||
                             char.IsWhiteSpace(currentKey) ||
                             char.IsPunctuation(currentKey) ||
                             char.IsSeparator(currentKey) ||
                             char.IsUpper(currentKey);

            //Allow navigation keyboard arrows
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Delete:
                    e.SuppressKeyPress = false;
                    return;
                default:
                    break;
            }

            if (modifier || nonNumber || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Oemcomma)
                e.SuppressKeyPress = true;

            if (e.KeyCode >= (Keys)96 && e.KeyCode <= (Keys)105)
            {
                e.SuppressKeyPress = false;
            }

            if (lotChars == 0)
            {
                if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
                {
                    e.SuppressKeyPress = true;
                }
            }

            //Handle pasted Text
            if (e.Control && e.KeyCode == Keys.V)
            {
                //Preview paste data (removing non-number characters)
                string pasteText = Clipboard.GetText();
                string strippedText = "";
                for (int i = 0; i < pasteText.Length; i++)
                {
                    if (char.IsDigit(pasteText[i]))
                        strippedText += pasteText[i].ToString();
                }

                if (strippedText != pasteText)
                {
                    //There were non-numbers in the pasted text
                    e.SuppressKeyPress = true;
                }
                else
                    e.SuppressKeyPress = false;
            }
        }

        /// <summary>
        /// This event handler, when Enter or Tab are pressed, will call to a validation method and a page view update.
        /// </summary>
        /// <param name="e"></param>
        public bool OnEnterKeyInitializeDataGridView(object textbox, KeyEventArgs e)
        {

            SuppressKeyIfWhiteSpaceChar(e);

            if (e.KeyCode.Equals(Keys.Enter) || e.KeyCode.Equals(Keys.Tab))
            {
                ValidatePartAndOpNumberExistWhenEntered(textbox);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This event handler supresses any white space characters being entered.
        /// </summary>
        /// <param name="e"></param>
        private void SuppressKeyIfWhiteSpaceChar(KeyEventArgs e)
        {
            char currentKey = (char)e.KeyCode;
            bool nonNumber = char.IsWhiteSpace(currentKey); //Deals with all white space characters which inlcude tab, return, etc.

            if (nonNumber)
                e.SuppressKeyPress = true;
        }

        /// <summary>
        /// This method sees which textbox is in focus in the feature page, and validates that those values match the DB.
        /// </summary>
        //TODO: Always called with "InitializeFeatureGridView()", could they be combined or is there any redundancy?
        internal void ValidatePartAndOpNumberExistWhenEntered(object sender)
        {
            //Pressing enter key on part number text box
            if (sender.Equals(view.ReportTB1))//partBoxFeature.ContainsFocus
            {
                CheckPartNumberExists(view.ReportTB1.Text);
            }
            //Pressing enter on op number text box
            else if (sender.Equals(view.ReportTB2))
            {
                CheckOpNumberExists();
            }
        }

        /// <summary>
        /// This method validates that the part number exists.
        /// </summary>
        /// <param name="partNumber"></param>
        public void CheckPartNumberExists(string partNumber)
        {

            if (partNumber == "")
            {
                DataTable featureTable = model.GetFeaturesOnOpKey(view.ReportTB1.Text, view.ReportTB2.Text);
                DataBindReportPage(featureTable);
                MessageBox.Show("Please Enter a Part Number");
                view.ReportTB1.Text = null;
                view.ReportTB2.Text = null;
                view.ReportPageHeader.Text = "REPORT PAGE";
            }
            else if (model.PartNumberExists(partNumber)) //Check if part number entered exists
            {
                view.ReportTB2.Select();//opBoxFeature.Focus();

            }
            else
            {

                view.ReportPageHeader.Text = "REPORT PAGE";
                MessageBox.Show("Part Number does not exist");
                view.ReportTB1.Text = null; ;
                view.ReportTB2.Text = null;
                DataTable featureTable = model.GetFeaturesOnOpKey(view.ReportTB1.Text, view.ReportTB2.Text);
                DataBindReportPage(featureTable);


            }
        }

        /// <summary>
        /// This method validates that the op number exists.
        /// </summary>
        private void CheckOpNumberExists()
        {
            if (view.ReportTB2.Text == "")
            {
                view.ReportPageHeader.Text = "REPORT PAGE";
                view.ReportTB2.Clear();
                DataTable featureTable = model.GetFeaturesOnOpKey(view.ReportTB1.Text, view.ReportTB2.Text);
                DataBindReportPage(featureTable);
                MessageBox.Show("Please Enter an Operation Number");
            }
            else if (model.OpExists(view.ReportTB2.Text, view.ReportTB1.Text))
            {
                view.ReportPageHeader.Text = "PART " + view.ReportTB1.Text + " OP " + view.ReportTB2.Text + " REPORT";
                MessageBox.Show("This is a valid Part and Operation Number");
            }
            else
            {
                view.ReportPageHeader.Text = "REPORT PAGE";
                DataTable featureTable = model.GetFeaturesOnOpKey(view.ReportTB1.Text, view.ReportTB2.Text);
                DataBindReportPage(featureTable);
                view.ReportTB2.Clear();
                MessageBox.Show("Op Number does not exist for this Part Number");
            }
        }

        private void DataBindReportPage(DataTable featureTable)
        {

            /*
            view.FeatureGridView.Columns.Clear();

            view.BindingSource = new BindingSource();

            view.FeatureGridView.DataSource = null;

            view.BindingSource.DataSource = featureTable;

            view.FeatureGridView.DataSource = view.BindingSource;

            ConfigureFeatureDataGridView(featureTable);
            */
        }

        public void DataBindReportFocus()
        {
            bool caught = false;
            DataTable table;

            try
            {
                table = model.GetChartData(Int32.Parse(view.ReportTB1.Text), (int)view.ReportFocusComboBox.SelectedValue);
                view.ReportChart.DataSource = table;
            }
            catch
            {
                caught = true;
                view.ReportChart.Visible = false;
            }

            if (!caught)
            {
                view.ReportChart.Visible = true;
                view.ReportChart.DataBind();

                double max = view.ReportChart.Series["UpperToleranceSeries"].Points[0].YValues[0];
                double min = view.ReportChart.Series["LowerToleranceSeries"].Points[0].YValues[0];
                double nom = view.ReportChart.Series["NominalSeries"].Points[0].YValues[0];
                double tol = (max - min) / 4;
                string title = "NOMINAL: " + nom.ToString() + "   HIGH: " + (max).ToString() + "   LOW: " + (min).ToString();

                view.ReportChart.Titles[0].Text = title;
                view.ReportChart.Titles[0].BackColor = Color.FromArgb(15, 15, 15);
                view.ReportChart.ChartAreas[0].AxisY.Maximum = max + tol;
                view.ReportChart.ChartAreas[0].AxisY.Minimum = min - tol;
                view.ReportChart.ChartAreas[0].AxisY.Interval = tol;
                ChartDataLabeling();
                //TrimChartPartCount();
                //ChartDataWarning();
            }
        }

        /// <summary>
        /// This method creates the chart area that will display all run charts of recorded data.
        /// </summary>
        /// <param name="chart"></param>
        public void createGraphArea(Chart chart)
        {
            chart.ChartAreas.Add("InspectionChart");
            chart.Series.Add("NominalSeries");
            chart.Series.Add("UpperToleranceSeries");
            chart.Series.Add("LowerToleranceSeries");
            chart.Titles.Add("Title");

            chart.Series["NominalSeries"].XValueMember = "Piece_ID";
            chart.Series["NominalSeries"].YValueMembers = "Measured_Value";
            chart.Series["NominalSeries"].ChartType = SeriesChartType.Line;

            chart.Series["UpperToleranceSeries"].XValueMember = "Piece_ID";
            chart.Series["UpperToleranceSeries"].YValueMembers = "Upper_Tolerance";
            chart.Series["UpperToleranceSeries"].ChartType = SeriesChartType.Line;

            chart.Series["LowerToleranceSeries"].XValueMember = "Piece_ID";
            chart.Series["LowerToleranceSeries"].YValueMembers = "Lower_Tolerance";
            chart.Series["LowerToleranceSeries"].ChartType = SeriesChartType.Line;

            chart.Titles[0].Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.Titles[0].ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].BackColor = System.Drawing.Color.DimGray;
            chart.Series[0].Color = System.Drawing.Color.MediumSeaGreen;
            chart.Series[1].Color = System.Drawing.Color.DarkRed;
            chart.Series[2].Color = System.Drawing.Color.DarkRed;
            chart.Series[0].BorderWidth = 3;
            chart.Series[1].BorderWidth = 3;
            chart.Series[2].BorderWidth = 3;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.Series[0].MarkerStyle = MarkerStyle.Square;
            chart.Series[0].MarkerSize = 9;
        }

        /// <summary>
        /// This method binds nominal feature values to the graph focus combo box.
        /// </summary>
        /// <param name="featuresTable"></param>
        public void BindFocusComboBox(DataTable featuresTable)
        {
            view.ReportFocusComboBox.DisplayMember = "Nominal";
            view.ReportFocusComboBox.ValueMember = "Feature_Key";
            view.ReportFocusComboBox.DataSource = featuresTable;
        }

        public void ChartDataLabeling()
        {
            double max = view.ReportChart.Series["UpperToleranceSeries"].Points[0].YValues[0];
            double min = view.ReportChart.Series["LowerToleranceSeries"].Points[0].YValues[0];

            for (int i = 0; i < view.ReportChart.Series[0].Points.Count; i++)
            {

                if (view.ReportChart.Series[0].Points[i].YValues[0] > (max * .95) || view.ReportChart.Series[0].Points[i].YValues[0] < (min * 1.05))
                {
                    view.ReportChart.Series[0].Points[i].Color = Color.Orange;
                    DataPoint d = view.ReportChart.Series[0].Points[i];
                    d.Label = view.ReportChart.Series[0].Points[i].YValues[0].ToString();
                    d.LabelBackColor = Color.Gainsboro;
                }

                if (view.ReportChart.Series[0].Points[i].YValues[0] > (max) || view.ReportChart.Series[0].Points[i].YValues[0] < (min))
                {
                    view.ReportChart.Series[0].Points[i].Color = Color.Red;
                    DataPoint d = view.ReportChart.Series[0].Points[i];
                    d.Label = view.ReportChart.Series[0].Points[i].YValues[0].ToString();
                    d.LabelBackColor = Color.Black;
                    d.LabelForeColor = Color.Red;
                }

                if (view.ReportChart.Series[0].Points[i].YValues[0] == 0)
                {
                    view.ReportChart.Series[0].Points[i].IsEmpty = true;
                }
            }
        }
    }
}

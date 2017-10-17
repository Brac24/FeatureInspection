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
    class InspectionPresenter
    {
        private IInspectionView view;
        private IFeaturesDataSource model;

        public InspectionPresenter(IInspectionView view, IFeaturesDataSource model)
        {
            this.view = view;
            this.model = model;
            Initialize();
        }

        private void Initialize()
        {

        }

        /// <summary>
        /// This method makes all columns in a grid view not sortable.
        /// </summary>
        //TODO: exact same logic as in FeatureCreationPresenter. Can we consolidate this?
        public void DisableSortableColumns()
        {
            for (int j = 0; j < view.InspectionGrid.ColumnCount; j++)
            {
                view.InspectionGrid.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// TODO: create an accurate summary for this method.
        /// </summary>
        /// <param name="featuresTable"></param>
        public void BindDataGridViewInspection(DataTable featuresTable)
        {
            bool caught = false;
            view.InspectionGrid.Columns.Clear();

            view.InspectionGrid.DataSource = null;

            view.InspectionBindingSource = new BindingSource() { DataSource = featuresTable };

            view.InspectionGrid.DataSource = view.InspectionBindingSource;

            try
            {
                HideInspectionColumns();

                SetInspectionReadOnlyColumns();
            }

            catch
            {
                caught = true;
            }

            SetupRedoButtonColumn();

            Inspection_SetSelectedCell();

            DisableSortableColumns();

        }

        /// <summary>
        /// This method checks if the object InspectionGrid has any rows. If TRUE, then the first row of the "Measured Actual" column
        /// is set to selected.
        /// </summary>
        private void Inspection_SetSelectedCell()
        {
            if (view.InspectionGrid.RowCount != 0)
            {
                view.InspectionGrid.Rows[0].Cells["Measured Actual"].Selected = true;
            }

        }

        /// <summary>
        /// This method sets two columns in the InspectionGrid to read only.
        /// </summary>
        private void SetInspectionReadOnlyColumns()
        {
            view.InspectionGrid.Columns["Feature"].ReadOnly = true;
            view.InspectionGrid.Columns["Sketch Bubble"].ReadOnly = true;
        }

        /// <summary>
        /// This method hides a few columns in the InspectionGrid
        /// </summary>
        private void HideInspectionColumns()
        {
            view.InspectionGrid.Columns["Inspection_Key_FK"].Visible = false;
            view.InspectionGrid.Columns["Feature_Key"].Visible = false;
            view.InspectionGrid.Columns["Position_Key"].Visible = false;
            view.InspectionGrid.Columns["Old Value"].Visible = false;
            view.InspectionGrid.Columns["Oldest Value"].Visible = false;
            ;
        }

        /// <summary>
        /// This method sets up the Redo button column in the inspection entry grid.
        /// </summary>
        private void SetupRedoButtonColumn()
        {
            DataGridViewButtonColumn RedoButtonColumn = new DataGridViewButtonColumn();
            {
                RedoButtonColumn.FlatStyle = FlatStyle.Popup;
                RedoButtonColumn.CellTemplate.Style.BackColor = Color.DarkRed;
                RedoButtonColumn.CellTemplate.Style.SelectionBackColor = Color.DarkRed;
                RedoButtonColumn.HeaderText = "Redo Entry";
                RedoButtonColumn.Text = "Redo";
                RedoButtonColumn.Name = "Redo_Column";
                view.InspectionGrid.Columns.Insert(view.InspectionGrid.Columns.Count, RedoButtonColumn);
                RedoButtonColumn.UseColumnTextForButtonValue = true;
            }
        }

        /// <summary>
        /// This method reduces "graphics popping" by creating inspection headers on the inspectionEntryGridView 
        /// before any opkey has been entered.
        /// </summary>
        public void inspectionHeaderCreation()
        {
            createGridHeaders("Sketch Bubble", view.InspectionGrid);
            createGridHeaders("Feature", view.InspectionGrid);
            createGridHeaders("Measured Actual", view.InspectionGrid);
            createGridHeaders("Inspection Tool", view.InspectionGrid);
            createGridHeaders("Redo Entry", view.InspectionGrid);
        }

        /// <summary>
        /// This method assigns the features associated with a part index to a datatable.
        /// </summary>
        /// <param name="pieceID"></param>
        /// <returns></returns>
        public DataTable UpdateTable(int pieceID)
        {
            DataTable featureTable = model.GetFeaturesOnPartIndex(pieceID, view.OpKey);
            return featureTable;
        }

        /// <summary>
        /// This method deals with updating fields on the inspection page when the part number has changed.
        /// </summary>
        /// <param name="sender"></param>
        public void updateGridViewOnIndexChange(object sender)
        {
            //Use (listbox)sender.SelectedIndex
            var listBox = (ListBox)sender;
            DataTable featureTable;

            if (listBox.Text.Contains("Part"))
            {
                view.InspectionHeaderText = listBox.Text;

                //TODO: Refactor these 4 lines. Extract in to function. Same code appears in BeginInspetionDataGridView
                int pieceID = listBox.SelectedIndex + 1; //Due to 0 indexing
                featureTable = UpdateTable(pieceID);
                BindDataGridViewInspection(featureTable);
                IfInspectionCellEqualsZero_NoLock();
            }
        }

        /// <summary>
        /// This method checks to each row in the inspection grid to see if they equal 0 or not. If a value != 0, then the row locks.
        /// </summary>
        public void IfInspectionCellEqualsZero_NoLock()
        {
            for (int i = 0; i < view.InspectionGrid.RowCount; i++)
            {
                float test = float.Parse(view.InspectionGrid.Rows[i].Cells[5].Value.ToString());
                if (test != 0)
                {
                    view.InspectionGrid.Rows[i].ReadOnly = true;
                }
                else
                {
                    view.InspectionGrid.Rows[i].ReadOnly = false;
                }
            }
        }

        /// <summary>
        /// This method is called when the "Next Part" button is clicked, and will cycle up one index number to the next part in the listbox.
        /// </summary>
        public void GotToNextPart()
        {
            //+1 to selectedindex because we need to check what index it is going in to first
            if (view.ListBoxCount > 0)
            {
                view.ListBoxIndex = (view.ListBoxIndex + 1 < view.ListBoxCount) ?
                view.ListBoxIndex += 1 : view.ListBoxIndex = 0;
                DataTable featureList = model.GetFeatureList(view.OpKey);
                BindFocusComboBox(featureList);
                //string title = view.ChartFocusComboBox.Text;
                BindFocusCharts();
            }
        }

        /// <summary>
        /// This event handler transfers values in a the inspection grid view to an "older" position to keep track of redos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void RedoDataGridViewRow(object sender, DataGridViewCellMouseEventArgs e)
        {
            var table = (DataGridView)sender;

            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == table.Columns[table.ColumnCount - 1].Index)
                {
                    if (float.Parse(view.InspectionGrid.Rows[e.RowIndex].Cells["Oldest Value"].Value.ToString()) == 0)
                    {
                        view.InspectionGrid.Rows[e.RowIndex].Cells["Oldest Value"].Value = view.InspectionGrid.Rows[e.RowIndex].Cells["Old Value"].Value;
                        view.InspectionGrid.Rows[e.RowIndex].Cells["Old Value"].Value = view.InspectionGrid.Rows[e.RowIndex].Cells["Measured Actual"].Value;
                        table.Rows[e.RowIndex].ReadOnly = false;
                    }
                    else
                    {
                        MessageBox.Show("Maximum allowed of Redo's reached");
                    }
                }
            }
        }

        /// <summary>
        /// This method creates headers with a input name and datagrid to help avoid graphics popping.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="grid"></param>
        public void createGridHeaders(string name, DataGridView grid)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            {
                column.HeaderText = name;
                grid.Columns.Insert(grid.Columns.Count, column);
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
        /// This method binds the opkey and feature to the graph area and makes them visible.
        /// </summary>
        public void BindFocusCharts()
        {
            bool caught = false;
            DataTable table;
            
            try
            {
                table = model.GetChartData(view.OpKey, (int)view.ChartFocusComboBox.SelectedValue);
                view.InspectionChart.DataSource = table;
                view.InspectionChart.DataBind();
                double max = view.InspectionChart.Series["UpperToleranceSeries"].Points[0].YValues[0];
                double min = view.InspectionChart.Series["LowerToleranceSeries"].Points[0].YValues[0];
                double nom = Double.Parse(view.ChartFocusComboBox.Text.ToString());
                double tol = (max - min) / 4;
                string title = "NOMINAL: " + nom.ToString() + "   HIGH: " + (max).ToString() + "   LOW: " + (min).ToString();

                view.InspectionChart.Titles[0].Text = title;
                view.InspectionChart.Titles[0].BackColor = Color.FromArgb(15, 15, 15);
                view.InspectionChart.ChartAreas[0].AxisY.Maximum = max + tol;
                view.InspectionChart.ChartAreas[0].AxisY.Minimum = min - tol;
                view.InspectionChart.ChartAreas[0].AxisY.Interval = tol;
            }
            catch
            {
                caught = true;
                view.InspectionChart.Visible = false;
            }

            if (!caught)
            {
                view.InspectionChart.Visible = true; 
                ChartDataLabeling();
                //TrimChartPartCount();
                //ChartDataWarning();
            }
        }

        public void ChartDataWarning()
        {
            int j = 0;
            for (int i = 0; i < view.InspectionChart.Series[0].Points.Count; i++)
            {
                if (view.InspectionChart.Series[0].Points[i].IsEmpty == false)
                {
                    j++;
                }
            }

            if (view.InspectionChart.Series[0].Points[j - 1].Color.Name == "Orange")
            {
                MessageBox.Show("Warning");
            }

            if (view.InspectionChart.Series[0].Points[j - 1].Color.Name == "Red")
            {
                MessageBox.Show("You Done Goofed, Kid!");
            }
        }

        public void ChartDataLabeling()
        {
            double max = view.InspectionChart.Series["UpperToleranceSeries"].Points[0].YValues[0];
            double min = view.InspectionChart.Series["LowerToleranceSeries"].Points[0].YValues[0];
            double nom = Double.Parse(view.ChartFocusComboBox.Text.ToString());

            for (int i = 0; i < view.InspectionChart.Series[0].Points.Count; i++)
            {

                if (view.InspectionChart.Series[0].Points[i].YValues[0] > (((max - nom) * .85) + nom) || view.InspectionChart.Series[0].Points[i].YValues[0] < (nom - ((nom - min) * .85)))
                {
                    view.InspectionChart.Series[0].Points[i].Color = Color.Orange;
                    //DataPoint d = view.InspectionChart.Series[0].Points[i];
                    //d.Label = view.InspectionChart.Series[0].Points[i].YValues[0].ToString();
                    //d.LabelBackColor = Color.Gainsboro;
                }

                if (view.InspectionChart.Series[0].Points[i].YValues[0] > (max) || view.InspectionChart.Series[0].Points[i].YValues[0] < (min))
                {
                    view.InspectionChart.Series[0].Points[i].Color = Color.Red;
                    DataPoint d = view.InspectionChart.Series[0].Points[i];
                    d.Label = view.InspectionChart.Series[0].Points[i].YValues[0].ToString();
                    d.LabelBackColor = Color.Black;
                    d.LabelForeColor = Color.Red;
                }

                if (view.InspectionChart.Series[0].Points[i].YValues[0] == 0)
                {
                    view.InspectionChart.Series[0].Points[i].IsEmpty = true;
                    DataPoint d = view.InspectionChart.Series[0].Points[i];
                    d.LabelBackColor = Color.Transparent;
                    d.LabelForeColor = Color.Transparent;
                }
            }
        }



        public void TrimChartPartCount()
        {

            for (int i = 0; i < view.InspectionChart.Series[0].Points.Count - 1; i++)
            {
                if (view.InspectionChart.Series[0].Points[i].IsEmpty == false)
                {
                    view.InspectionChart.ChartAreas[0].AxisX.Maximum = i + 1;
                    view.InspectionChart.ChartAreas[0].AxisX.Minimum = i - 20;
                }
            }
        }

        public void noInspectionStartClear()
        {
            if (view.PartsListBox.Items.Count == 0)
            {
                view.ChartFocusComboBox.DataSource = null;
                view.InspectionHeaderText = "INSPECTION PAGE";
                view.PartsListBox.DataSource = null;
                view.LotsizeTextBox.Clear();
                HideInspectionColumns();
                SetupRedoButtonColumn();
                DisableSortableColumns();
                MessageBox.Show("Features have been added to this operation. Please enter your part count.");
                view.InspectionGrid.Columns.Remove("Redo_Column");
            }

        }

        /// <summary>
        /// This method clears some of the information being displayed on the inspection page.
        /// Best called on new inspections.
        /// </summary>
        public void smallInspectionPageClear()
        {
            view.InspectionChart.Visible = false;
            view.ChartFocusComboBox.DataSource = null;
            view.ChartFocusComboBox.Items.Clear();
            view.ChartFocusComboBox.Text = null;
            view.LotsizeTextBox.Clear();
            view.OpKeyTextBox.Focus();
            view.PartsListBox.DataSource = null;
            view.InspectionGrid.DataSource = null;
            view.InspectionGrid.Columns.Clear();
            inspectionHeaderCreation();
            DisableSortableColumns();
            view.InspectionPageHeader.Text = "INSPECTION PAGE";
        }

        /// <summary>
        /// This method calls the "smallInspectionPageClear()" and then clears some additional info on the inspection page.
        /// Best called on false opkeys.
        /// </summary>
        public void fullInspectionPageClear()
        {
            view.PartNumberLabel.Text = null;
            view.JobLabel.Text = null;
            view.OperationLabel.Text = null;
            smallInspectionPageClear();
        }

        /// <summary>
        /// This method binds nominal feature values to the graph focus combo box.
        /// </summary>
        /// <param name="featuresTable"></param>
        public void BindFocusComboBox(DataTable featuresTable)
        {
            view.ChartFocusComboBox.DisplayMember = "Nominal";
            view.ChartFocusComboBox.ValueMember = "Feature_Key";
            view.ChartFocusComboBox.DataSource = featuresTable;
            

            
        }

        /// <summary>
        /// This method binds the entered part count into the "inspected parts" list box.
        /// </summary>
        /// <param name="partsTable"></param>
        public void BindPartListBox(DataTable partsTable)
        {
            view.ListBoxBindingSource.DataSource = partsTable;
            view.PartsListBox.DataSource = view.ListBoxBindingSource;
            view.PartsListBox.DisplayMember = "PartList";
        }

        /// <summary>
        /// *TODO: Fill out this summary with an accurate description.
        /// </summary>
        public void AdapterUpdateInspection()
        {
            //Call EndEdit method before updating database
            view.InspectionBindingSource.EndEdit();
            model.AdapterUpdateInspection((DataTable)view.InspectionBindingSource.DataSource);
            BindFocusCharts();
            noInspectionStartClear();
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

        public void checkFilterOrEnter(object sender, KeyEventArgs e)
        {
            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                view.LotsizeTextBox.ReadOnly = false;

                ValidateValidOpKey();
            }
            else
            {
                filterTextBox(sender, e);
            }
        }



        private void ValidateValidOpKey()
        {
            bool isValidOpKey = SetOpKeyInfoInspection();

            if (isValidOpKey)
            {
                ValidateInspectionExistForOpKey();
            }
            else
            {
                InvalidOpKeyProcess();
            }

        }

        private void InvalidOpKeyProcess()
        {
            fullInspectionPageClear();
            MessageBox.Show(view.OpKeyTextBox.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
            view.OpKeyTextBox.Clear();
        }

        private bool ValidateInspectionExistForOpKey()
        {
            bool inspectionExists = model.GetInspectionExistsOnOpKey(view.OpKey);
            
            if (inspectionExists)
            {
                BeginInspectionDataGridViewInitialization();
            }
            else
            {
                CreateInspectionForOpKey();

                BeginInspectionDataGridViewInitialization();
            }

            return inspectionExists;
        }

        private void CreateInspectionForOpKey()
        {
            //Create the inspection in inspection table
            view.LotsizeTextBox.Clear();
            model.CreateInspectionInInspectionTable(view.OpKey);
            smallInspectionPageClear();
            //MessageBox.Show("Creating Inspection");            
        }

        private void BeginInspectionDataGridViewInitialization()
        {
            DataTable featureTable;
            DataTable featureList;
            //Check if there are features related on op and part numn
            featureTable = model.GetFeaturesOnOpKey(view.OpKey);

            featureList = model.GetFeatureList(view.OpKey);
            BindFocusComboBox(featureList);

            if (featureTable.Rows.Count > 0)
            {
                InitializeInspectionGridViewWithCorrespondingParts();
                view.InspectionHeaderText = view.PartsListBox.Text;
                BindAndConfigureDataGridView();
                BindFocusCharts();
            }
            else
            {
                //Message user to add features to this part num op num
                smallInspectionPageClear();
                //Message user to add features to this part num op num
                MessageBox.Show("Lead must add features to this Part and Operation number");

            }
        }

        private void BindAndConfigureDataGridView()
        {
            DataTable featureTable;
            int pieceID = view.PartsListBox.SelectedIndex + 1; //Due to 0 indexing. Used for naming parts in ListBox i.e. Part 1(1 is piece ID)
            featureTable = UpdateTable(pieceID);
            BindDataGridViewInspection(featureTable);
            IfInspectionCellEqualsZero_NoLock();
        }

        private void InitializeInspectionGridViewWithCorrespondingParts()
        {
            DataTable partList = model.GetPartsList(view.OpKey);

            //Check if there are parts in position table
            if (partList.Rows.Count > 0)
            {               
                DetermineIfNewFeaturesHaveBeenAddedToOpKey();
                BindNewlyCreatedPartListToListBox();
                
            }
            else if (view.LotsizeTextBox.Text != "")
            {
                SetUpDataAndListBox();
            }
        }

        private void DetermineIfNewFeaturesHaveBeenAddedToOpKey()
        {
            //Search for new parts       
            view.LotsizeTextBox.Text = model.GetLotSize(view.OpKey);
            view.LotsizeTextBox.ReadOnly = true;
            model.InsertPartsToPositionTable(view.OpKey, Int32.Parse(view.LotsizeTextBox.Text));
        }

        private void SetUpDataAndListBox()
        {         
            CreateDataInDataBase();
            BindNewlyCreatedPartListToListBox();
        }

        private void BindNewlyCreatedPartListToListBox()
        {
            //Get part list DataTable partList = model.GetPartsList(opkey);
            DataTable partList = model.GetPartsList(view.OpKey);

            //Bind the part list box BindListBox(partList);
            BindPartListBox(partList);
            
        }

        private void CreateDataInDataBase()
        {
            // Insert Lot Size to Inspection Table
            model.InsertLotSizeToInspectionTable(Int32.Parse(view.LotsizeTextBox.Text), view.OpKey);
            //Create the parts in the positions table
            model.InsertPartsToPositionTable(view.OpKey, Int32.Parse(view.LotsizeTextBox.Text));
        }


        /// <summary>
        /// This method checks to see if the value in the opKeyTextBox exists in the DB, if it does, it populates the inspection page
        /// with the part number, job number, and operation number of that opkey.
        /// </summary>
        /// <returns></returns>
        public bool SetOpKeyInfoInspection()
        {
            DataTable info = new DataTable();

            try
            {
                info = model.GetInfoFromOpKeyEntry(view.OpKey);
            }
            catch
            {
                //If OpKey is null
                return false;
            }

            if (info.Rows.Count > 0)
            {
                view.PartNumberLabel.Text = info.Rows[0]["Part_Number"].ToString();
                view.JobLabel.Text = info.Rows[0]["Job_Number"].ToString();
                view.OperationLabel.Text = info.Rows[0]["Operation_Number"].ToString();
                view.LotsizeTextBox.Focus();

                return true;
            }

            else
            {
                return false;
            }
        }

        public void ShowChartDetails(MouseEventArgs e)
        {

            HitTestResult result = view.InspectionChart.HitTest(e.X, e.Y);

            if (result.PointIndex > -1 && result.ChartArea != null)
            {
                view.ChartLabel1.Text = "Measured Dimension: " + result.Series.Points[result.PointIndex].YValues[0].ToString();
                view.ChartLabel2.Text = "Part Inspected: " + result.Series.Points[result.PointIndex].XValue.ToString();
            }
        }

    }
}
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
            view.InspectionGrid.Columns.Clear();

            view.InspectionGrid.DataSource = null;

            view.InspectionBindingSource = new BindingSource();

            view.InspectionBindingSource.DataSource = featuresTable;

            view.InspectionGrid.DataSource = view.InspectionBindingSource;

            HideInspectionColumns();

            SetInspectionReadOnlyColumns();

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
                int pieceID = listBox.SelectedIndex + 1; //Due to 0 indexing
                featureTable = UpdateTable(pieceID);
                BindDataGridViewInspection(featureTable);
            }

            ifInspectionCellEqualsZero_NoLock();
        }

        /// <summary>
        /// This method checks to each row in the inspection grid to see if they equal 0 or not. If a value != 0, then the row locks.
        /// </summary>
        private void ifInspectionCellEqualsZero_NoLock()
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
            //chart.ChartAreas[0].Area3DStyle.Enable3D = true;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.Series[0].MarkerStyle = MarkerStyle.Square;
            chart.Series[0].MarkerSize = 9;

            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
        }

        /// <summary>
        /// This method binds the opkey and feature to the graph area and makes them visible.
        /// </summary>
        public void BindFocusCharts()
        {
            try
            {
                DataTable table = model.GetChartData(view.OpKey, (int)view.ChartFocusComboBox.SelectedValue);
                view.InspectionChart.Visible = true;
                view.InspectionChart.DataSource = table;
                view.InspectionChart.DataBind();

                double max = view.InspectionChart.Series["UpperToleranceSeries"].Points[0].YValues[0];
                double min = view.InspectionChart.Series["LowerToleranceSeries"].Points[0].YValues[0];
                string title = view.InspectionChart.Series["NominalSeries"].Points[0].YValues[0].ToString();

                view.InspectionChart.Titles[0].Text = title;
                view.InspectionChart.ChartAreas[0].AxisY.Maximum = max + .0003;  //This should be a little above max tolerance
                view.InspectionChart.ChartAreas[0].AxisY.Minimum = min - .0003;    //A little below minimum tolerance
                //view.InspectionChart.ChartAreas[0].AxisY.Interval = .001; //interval scale should be based on --> To Be Determined
            }
            catch { }
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
        }

        /// <summary>
        /// This event handler supresses pressinng '0' when there are no characters in the sending textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void suppressZeroFirstChar(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;
            int lotChars = textbox.Text.Length;
            if (lotChars == 0)
            {
                if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        /*TODO: This is without at doubt our most bloated handler. I am not even sure how to start trimming the fat off it.
        It is also one of the offenders of redundant and overriding logic along with "SetOpKeyInfoInspection", "numOnly_KeyDown", 
        "suppressZeroFirstChar" and "filterTextBox".*/
        public void checkEnter_ValidateOpKey(KeyEventArgs e)
        {

            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                view.LotsizeTextBox.ReadOnly = false;
                DataTable featureTable;
                bool isValidOpKey = false;
                bool inspectionExists = false;
                DataTable partList = null;
                String text = view.OpKeyTextBox.Text;
                DataTable featureList = null;

                partList = model.GetPartsList(view.OpKey);
                isValidOpKey = SetOpKeyInfoInspection();

                if (isValidOpKey)
                {
                    inspectionExists = model.GetInspectionExistsOnOpKey(view.OpKey);

                    if (inspectionExists)
                    {
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(view.OpKey);

                        featureList = model.GetFeatureList(view.OpKey);
                        BindFocusComboBox(featureList);

                        if (featureTable.Rows.Count > 0)
                        {
                            
                            //Check if there are parts in position table
                            if (partList.Rows.Count > 0)
                            {
                                view.LotsizeTextBox.Text = model.GetLotSize(view.OpKey);
                                view.LotsizeTextBox.ReadOnly = true;
                                model.InsertPartsToPositionTable(view.OpKey, Int32.Parse(view.LotsizeTextBox.Text)); // Add any new features that were added by lead

                                //Get the parts if there are
                                BindPartListBox(partList);
                                
                            }
                            else if (view.LotsizeTextBox.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(view.LotsizeTextBox.Text), view.OpKey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(view.OpKey, Int32.Parse(view.LotsizeTextBox.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(view.OpKey);

                                //Bind the part list box BindListBox(partList);
                                BindPartListBox(partList);

                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                            smallInspectionPageClear();
                            MessageBox.Show("OpKey exists, enter in how many parts you have");

                        }
                    }
                    else
                    {
                        //Create the inspection in inspection table
                        view.LotsizeTextBox.Clear();
                        model.CreateInspectionInInspectionTable(view.OpKey);
                        MessageBox.Show("Lead must add features to this Part and Operation number");

                        //Run the logic inside the if loop above
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(view.OpKey);


                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                BindPartListBox(partList);
                                view.LotsizeTextBox.Text = model.GetLotSize(view.OpKey);
                                view.LotsizeTextBox.ReadOnly = true;
                            }
                            else if (view.LotsizeTextBox.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(view.LotsizeTextBox.Text), view.OpKey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(view.OpKey, Int32.Parse(view.LotsizeTextBox.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(view.OpKey);

                                //Bind the part list box BindListBox(partList);
                                BindPartListBox(partList);
                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                            MessageBox.Show("Lead must add features to this Part and Operation number");

                        }
                    }

                }
                else
                {
                    //Not valid opkey

                }

            }
        }

        /*TODO: Currently this contains logic that is strongly linked to "numOnly_KeyDown", "suppressZeroFirstChar", and 
        "checkEnterKeyPressedInspection", refactoring should be taking all of these methods and events into consideration as there is 
        definitely still some redundant/ovderiding logic among them.*/
        public bool SetOpKeyInfoInspection()
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(view.OpKey);

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
                fullInspectionPageClear();
                MessageBox.Show(view.OpKeyTextBox.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
                view.OpKeyTextBox.Clear();

                return false;
            }
        }

    }
}
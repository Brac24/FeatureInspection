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

        private void Inspection_SetSelectedCell()
        {
            if (view.InspectionGrid.RowCount != 0)
            {
                view.InspectionGrid.Rows[0].Cells["Measured Actual"].Selected = true;
            }

        }

        private void SetInspectionReadOnlyColumns()
        {
            view.InspectionGrid.Columns["Feature"].ReadOnly = true;
            view.InspectionGrid.Columns["Sketch Bubble"].ReadOnly = true;
        }

        private void HideInspectionColumns()
        {
            view.InspectionGrid.Columns["Inspection_Key_FK"].Visible = false;
            view.InspectionGrid.Columns["Feature_Key"].Visible = false;
            view.InspectionGrid.Columns["Position_Key"].Visible = false;
            view.InspectionGrid.Columns["Old Value"].Visible = false;
            view.InspectionGrid.Columns["Oldest Value"].Visible = false;
        }

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
            chart.Titles.Add("TEST");
            chart.Series["NominalSeries"].XValueMember = "Piece_ID";
            chart.Series["NominalSeries"].YValueMembers = "Measured_Value";
            chart.Series["NominalSeries"].ChartType = SeriesChartType.Line;
            chart.Titles[0].Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.Titles[0].ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].BackColor = System.Drawing.Color.DimGray;
            chart.Series[0].Color = System.Drawing.Color.MediumSeaGreen;
            chart.Series[0].BorderWidth = 3;
            chart.ChartAreas[0].Area3DStyle.Enable3D = false;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Gainsboro;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }


    }
}
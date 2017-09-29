using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

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

        public void suppressZeroFirstChar(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;
            int lotChars = textbox.Text.Length;
            if (lotChars == 0)
            {
                if (e.KeyCode == Keys.D0)
                {
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.NumPad0)
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        public DataTable UpdateTable(int pieceID)
        {
            DataTable featureTable = model.GetFeaturesOnPartIndex(pieceID, view.OpKey);
            return featureTable;
        }

        public void updateGridViewOnIndexChange(object sender)
        {
            //Use (listbox)sender.SelectedIndex
            var listBox = (ListBox)sender;
            DataTable featureTable;

            if (listBox.Text.Contains("Part"))
            {
                view.InspectionHeader = listBox.Text;
                int pieceID = listBox.SelectedIndex + 1; //Due to 0 indexing
                featureTable = UpdateTable(pieceID);
                view.BindDataGridViewInspection(featureTable);
            }

            for (int i = 0; i < view.inspectionGrid.RowCount; i++)
            {
                float test = float.Parse(view.inspectionGrid.Rows[i].Cells[5].Value.ToString());
                if (test != 0)
                {
                    view.inspectionGrid.Rows[i].ReadOnly = true;
                }
                else
                {
                    view.inspectionGrid.Rows[i].ReadOnly = false;
                }
            }
        }

        public void GotToNextPart()
        {
            //+1 to selectedindex because we need to check what index it is going in to first
            if (view.ListBoxCount > 0)
            {
                view.ListBoxIndex = (view.ListBoxIndex + 1 < view.ListBoxCount) ?
                view.ListBoxIndex += 1 : view.ListBoxIndex = 0;
            }
        }

        internal void nextPartButton_Click()
        {
            GotToNextPart();
        }

        public void DisableSortableColumns()
        {
            for (int j = 0; j < view.inspectionGrid.ColumnCount; j++)
            {
                view.inspectionGrid.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        internal void lockCellInspection(object sender, DataGridViewCellEventArgs e)
        {
            var table = (DataGridView)sender;

                float test = float.Parse(view.inspectionGrid.Rows[e.RowIndex].Cells[5].Value.ToString());
                if (test != 0)
                {
                    view.inspectionGrid.Rows[e.RowIndex].ReadOnly = true;
                }
                else
                {
                    view.inspectionGrid.Rows[e.RowIndex].ReadOnly = false;
                }

            //TODO: Remember to make the table row ReadOnly
        }

        internal void RedoDataGridViewRow(object sender, DataGridViewCellMouseEventArgs e)
        {
            var table = (DataGridView)sender;

            //float measured = (float)view.inspectionGrid.Rows[e.RowIndex].Cells["Measured Actual"].Value;
            //float old = (float)view.inspectionGrid.Rows[e.RowIndex].Cells["Old Value"].Value;
            //float oldest = (float)view.inspectionGrid.Rows[e.RowIndex].Cells["Oldest Value"].Value;

            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == table.Columns[table.ColumnCount - 1].Index)
                {
                    if (float.Parse(view.inspectionGrid.Rows[e.RowIndex].Cells["Oldest Value"].Value.ToString()) == 0)
                    {
                        view.inspectionGrid.Rows[e.RowIndex].Cells["Oldest Value"].Value = view.inspectionGrid.Rows[e.RowIndex].Cells["Old Value"].Value;
                        view.inspectionGrid.Rows[e.RowIndex].Cells["Old Value"].Value = view.inspectionGrid.Rows[e.RowIndex].Cells["Measured Actual"].Value;
                        
                        table.Rows[e.RowIndex].ReadOnly = false;
                    }
                    else
                    {
                        MessageBox.Show("Maximum allowed of Redo's reached");
                    }
                }
            }
        }

    }
}
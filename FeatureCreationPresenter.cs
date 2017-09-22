using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace Feature_Inspection
{
    public class FeatureCreationPresenter
    {
        private IFeatureCreationView view;
        private IFeaturesDataSource model;
        

        public FeatureCreationPresenter(IFeatureCreationView view, IFeaturesDataSource model)
        {
            this.view = view;
            this.model = model;
            Initialize();

        }

        private void Initialize()
        {


        }


        public void checkEnterKeyPressed(KeyEventArgs e)
        {
            
            SuppressKeyIfWhiteSpaceChar(e);
                       
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                ValidateTextBoxes();

                InitializeFeatureGridView();
            }
        }

        public void cancelChanges_Click()
        {
            var result = AskIfChangesWillBeUndone();

            if (result == "Yes")
            {
                DataTable partList = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeature(partList);
            }
        }

        public void saveButton_Click()
        {
            var result = AskIfChangesWillBeSaved();

            if (result == "Yes")
            {
                //Save Changes made in gridview back to the database
                AdapterUpdate();

                //Prompt user that changes have been saved
                Message_ChangesSaved();
            }

            else if (result == "No")
            {
                Message_ChangesNotSaved();
            }
        }

        internal void dataGridView1_CellEndEdit(DataGridViewCellEventArgs e)
        {
            SetSampleIDAndFeatureTypeHiddenColumns(e);
        }

        public void addFeature_Click()
        {
            if (view.BindingSource == null)
            {
                return;
            }

            AddFeatureRow((DataTable)(view.BindingSource.DataSource));
        }

        private static void Message_ChangesNotSaved()
        {
            const string message2 = "Any Changes to the table have not been updated to the database";
            const string caption2 = "Table Not Saved";
            var result2 = MessageBox.Show(message2, caption2);
        }

        private static void Message_ChangesSaved()
        {
            const string message1 = "All changes made to the table have been updated to the database";
            const string caption1 = "Table Saved";
            var result2 = MessageBox.Show(message1, caption1);
        }

        private void AdapterUpdate()
        {
            //Must call EndEdit Method before trying to update database
            view.BindingSource.EndEdit();
            view.SampleBindingSource.EndEdit();

            //Update database
            model.AdapterUpdate((DataTable)view.BindingSource.DataSource);
        }

        private string AskIfChangesWillBeSaved()
        {
            const string message0 = "Are you sure you want to save all changes made to this set of features? " +
                            "All changes will save to the database.";
            const string caption0 = "Save Changes";

            var result = view.CreateYesNoMessage(message0, caption0);
            return result.ToString();
        }


        private string AskIfChangesWillBeUndone()
        {
            const string message = "Are you sure you want to cancel all changes made to this set of features? " +
                            "Any changes to this table will be reverted.";
            const string caption = "Cancel Changes";
            var result = view.CreateYesNoMessage(message, caption);
            return result.ToString();
        }

        

        public void SuppressKeyIfWhiteSpaceChar(KeyEventArgs e)
        {
            char currentKey = (char)e.KeyCode;
            bool nonNumber = char.IsWhiteSpace(currentKey); //Deals with all white space characters which inlcude tab, return, etc.

            if (nonNumber)
                e.SuppressKeyPress = true;
        }

        internal void ValidateTextBoxes()
        {
            //Pressing enter key on part number text box
            if (view.PartTextBox.ContainsFocus)//partBoxFeature.ContainsFocus
            {
                CheckPartNumberExists(view.PartNumber);
            }
            //Pressing enter on op number text box
            else if (view.OpTextBox.ContainsFocus)
            {
                CheckOpNumberExists();
            }
        }

        private void InitializeFeatureGridView()
        {
            //As long as both textboxes are not empty
            if (view.PartNumber != "" && view.OperationNumber != "")
            {
                DataTable featureList = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeature(featureList);
                SetFeatureGridViewHeader();
            }
        }

        private void SetFeatureGridViewHeader()
        {
            if (view.FeatureCount > 0)
            {

                view.FeaturePageHeader = "PART " + view.LastRowFeaturePartNumberFK + " OP " + view.LastRowFeatureOperationNumberFK + " FEATURES";
            }
            else
            {
                view.FeaturePageHeader = "FEATURES PAGE";
            }
        }

        private void DataBindFeature(DataTable featureTable)
        {
            view.FeatureGridView.Columns.Clear();

            view.BindingSource = new BindingSource();

            view.FeatureGridView.DataSource = null;

            view.BindingSource.DataSource = featureTable;

            view.FeatureGridView.DataSource = view.BindingSource;

            ConfigureFeatureDataGridView(featureTable);
        }

        private void ConfigureFeatureDataGridView(DataTable featureTable)
        {
            int maxRows;

            HideFeatureColumns();

            SetHeaderTexts();

            maxRows = featureTable.Rows.Count;

            SetUpFeatureTypeColumnComboBox();

            SampleComboBind(); //Adds and binds the sample combo box column

            SetUpToolColumnComboBox();

            SetUpDeleteButtonColumn();

            DisableSortableColumns();
        }

        private void DisableSortableColumns()
        {
            for (int j = 0; j < view.FeatureGridView.ColumnCount; j++)
            {
                view.FeatureGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SetUpDeleteButtonColumn()
        {
            DataGridViewButtonColumn DeleteButtonColumn = new DataGridViewButtonColumn();
            {
                DeleteButtonColumn.FlatStyle = FlatStyle.Popup;
                DeleteButtonColumn.CellTemplate.Style.BackColor = Color.DarkRed;
                DeleteButtonColumn.CellTemplate.Style.SelectionBackColor = Color.DarkRed;
                DeleteButtonColumn.HeaderText = "Delete Feature";
                DeleteButtonColumn.Text = "Delete";
                view.FeatureGridView.Columns.Insert(view.FeatureGridView.Columns.Count, DeleteButtonColumn);
                DeleteButtonColumn.UseColumnTextForButtonValue = true;
            }
        }

        private void SetUpToolColumnComboBox()
        {
            DataGridViewComboBoxColumn ToolCategoryColumn = new DataGridViewComboBoxColumn();
            {
                ToolCategoryColumn.FlatStyle = FlatStyle.Flat;
                ToolCategoryColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
                ToolCategoryColumn.HeaderText = "Tool";
                view.FeatureGridView.Columns.Insert(view.FeatureGridView.Columns.Count, ToolCategoryColumn);
                ToolCategories(ToolCategoryColumn);
            }
        }

        private static void ToolCategories(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("0-1 Mic", "Height Stand");
        }

        private void SampleComboBind()
        {
            DataGridViewComboBoxColumn SamplingColumn = CreateSampleColumn();

            view.FeatureGridView.Columns.Insert(view.FeatureGridView.Columns.Count, SamplingColumn);

            view.SampleBindingSource.DataSource = model.GetSampleChoices(); //Set binding source to list of sample choices from database

            //Binding combo box to database table of sample choices
            SetSampleColumnProperties(SamplingColumn);

            SamplingColumn.DataSource = view.SampleBindingSource; //Set column data source to binding source

            //Initialize combo box value for each row in Sample and FeatureType column
            InitSampleAndFeatureTypeComboBoxColumnValue();
        }

        private void InitSampleAndFeatureTypeComboBoxColumnValue()
        {
            for (int i = 0; i < view.FeatureCount; i++)
            {
                view.FeatureGridView.Rows[i].Cells["Sample"].Value = view.FeatureGridView.Rows[i].Cells["SampleID"].Value;
                view.FeatureGridView.Rows[i].Cells["FeatureTypeColumn"].Value = view.FeatureGridView.Rows[i].Cells["FeatureType"].Value;
            }
        }

        private static void SetSampleColumnProperties(DataGridViewComboBoxColumn SamplingColumn)
        {
            SamplingColumn.DisplayMember = "SampleChoice";
            SamplingColumn.ValueMember = "SampleID";

            SamplingColumn.HeaderText = "Sample";
            SamplingColumn.Name = "Sample";
        }

        private static DataGridViewComboBoxColumn CreateSampleColumn()
        {
            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            SamplingColumn.FlatStyle = FlatStyle.Flat;
            SamplingColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
            return SamplingColumn;
        }

        private void HideFeatureColumns()
        {
            view.FeatureGridView.Columns["Feature_Key"].Visible = false;
            view.FeatureGridView.Columns["Part_Number_FK"].Visible = false;
            view.FeatureGridView.Columns["Operation_Number_FK"].Visible = false;
            view.FeatureGridView.Columns["Feature_Name"].Visible = false;
            view.FeatureGridView.Columns["Active"].Visible = false;
            view.FeatureGridView.Columns["Pieces"].Visible = false;
            view.FeatureGridView.Columns["FeatureType"].Visible = false;
            view.FeatureGridView.Columns["Places"].Visible = false;
            view.FeatureGridView.Columns["SampleID"].Visible = false;
        }

        private void SetHeaderTexts()
        {
            view.FeatureGridView.Columns["Sketch_Bubble"].HeaderText = "Sketch Bubble (Optional)";
            view.FeatureGridView.Columns["Plus_Tolerance"].HeaderText = "+";
            view.FeatureGridView.Columns["Minus_Tolerance"].HeaderText = "-";
        }

        private void SetUpFeatureTypeColumnComboBox()
        {
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            {
                FeatureDropColumn.FlatStyle = FlatStyle.Flat;
                FeatureDropColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
                view.FeatureGridView.Columns.Insert(0, FeatureDropColumn);
                FeatureDropChoices(FeatureDropColumn);
                FeatureDropColumn.HeaderText = "Feature Type (Optional)";
                FeatureDropColumn.Name = "FeatureTypeColumn";
                FeatureDropColumn.DropDownWidth = 160;
            }
        }

        //IP>Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange(" ", "Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "Depth", "Straightness", "Flatness", "Parallelism", "Perpendicularity", "Circular Runout", "Total Runout", "Position", "Concentricity");
        }

        public void CheckPartNumberExists(string partNumber)
        {

            if (partNumber == "")
            {
                MessageBox.Show("Please Enter a Part Number");
            }
            else if (model.PartNumberExists(partNumber)) //Check if part number entered exists
            {
                view.OpTextBox.Select();//opBoxFeature.Focus();
                
            }
            else
            {

                MessageBox.Show("Part Number does not exist");
                view.PartTextBox.Clear();
                
            }
        }

        private void CheckOpNumberExists()
        {
            if (view.OperationNumber == "")
            {
                MessageBox.Show("Please Enter an Operation Number");
            }
            else if (model.OpExists(view.OperationNumber, view.PartNumber))
            {
                MessageBox.Show("This is a valid Part and Operation Number");
                view.FeatureGridView.Focus();
            }
            else
            {
                MessageBox.Show("Op Number does not exist for this Part Number");
                view.OpTextBox.Clear();
            }
        }

        public void DeleteDataGridViewRow(object sender, DataGridViewCellMouseEventArgs e)
        {
            var table = (DataGridView)sender;

            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == table.Columns[table.ColumnCount - 1].Index)
                {
                    table.Rows.Remove(table.Rows[e.RowIndex]);
                }
            }
        }

        public void AddFeatureRow(DataTable data)
        {

            data = AddTableRow(data);

            SetPartAndOpNumInTable();
        }

        private void SetPartAndOpNumInTable()
        {
            if (view.FeatureCount > 0)
            {
                //Set the last row Part_Number_FK and Operation_Number_FK to the same value as in the first row
                view.LastRowFeaturePartNumberFK = view.PartNumber;
                view.LastRowFeatureOperationNumberFK = view.OperationNumber;
            }

        }



        private void SetSampleIDAndFeatureTypeHiddenColumns(DataGridViewCellEventArgs e)
        {
            if (view.FeatureGridView.Columns["Sample"] != null || view.FeatureGridView.Columns["FeatureTypeColumn"] != null)
            {

                if (view.FeatureGridView.Columns["Sample"].Index == e.ColumnIndex)
                {
                    view.FeatureGridView.Rows[e.RowIndex].Cells["SampleID"].Value = view.FeatureGridView.Rows[e.RowIndex].Cells["Sample"].Value;
                }
                else if (view.FeatureGridView.Columns["FeatureTypeColumn"].Index == e.ColumnIndex)
                {
                    view.FeatureGridView.Rows[e.RowIndex].Cells["FeatureType"].Value = view.FeatureGridView.Rows[e.RowIndex].Cells["FeatureTypeColumn"].Value;
                }

            }
        }

        private DataTable AddTableRow(DataTable t)
        {
            if (view.FeatureDataSource == null)
            {
                return t;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);

            return t;
        }


        public bool ViewExists()
        {
            if (view == null)
            {
                return false;
            }
            else
                return true;
        }

    }
}
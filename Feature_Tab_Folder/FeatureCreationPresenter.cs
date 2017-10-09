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

        /*NOTE: Even if these event handlers and methods have summaries, this does not necessarily mean they have been checked for 
        redundancies or if they should be refactored more. */

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

                InitializeFeatureGridView();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This event handler is currently unused.
        /// </summary>
        /// <param name="e"></param>
        public void leaveFocus(EventArgs e)
        {
            //ValidateFeatureTabTextBoxes();

            InitializeFeatureGridView();
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
        /// This event handler will delete a row on the sending grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteDataGridViewRowOnRowDeleteButtonWasClicked(object sender, DataGridViewCellMouseEventArgs e)
        {
            var featureDataGridView = (DataGridView)sender;

            if (e.RowIndex != -1)
            {
                if (e.ColumnIndex == featureDataGridView.Columns[featureDataGridView.ColumnCount - 1].Index)
                {
                    featureDataGridView.Rows.RemoveAt(e.RowIndex);
                    featureDataGridView.Refresh();
                }
            }
        }

        /// <summary>
        /// This event handler sets the  Sample and Feature type.
        /// </summary>
        /// <param name="e"></param>
        internal void SetSampleIDAndFeatureTypeHiddenColumns(DataGridViewCellEventArgs e)
        {  

            if (view.FeatureGridView.Columns["Sample"] != null && view.FeatureGridView.Columns["FeatureTypeColumn"] != null)
            {
                ChooseColumnToSet(e);
            }
        }

        private void ChooseColumnToSet(DataGridViewCellEventArgs e)
        {
            var sampleChosen = view.FeatureGridView.Columns["Sample"].Index.Equals(e.ColumnIndex);
            var featureTypeChosen = view.FeatureGridView.Columns["FeatureTypeColumn"].Index.Equals(e.ColumnIndex);

            if (sampleChosen)
            {
                SetSampleIDColumn(e.RowIndex);
            }
            else if (featureTypeChosen)
            {
                SetFeatureTypeColumn(e.RowIndex);
            }
        }

        private void SetFeatureTypeColumn(int rowIndex)
        {
            view.FeatureGridView.Rows[rowIndex].Cells["FeatureType"].Value = view.FeatureGridView.Rows[rowIndex].Cells["FeatureTypeColumn"].Value;
        }

        private void SetSampleIDColumn(int rowIndex)
        {
            view.FeatureGridView.Rows[rowIndex].Cells["SampleID"].Value = view.FeatureGridView.Rows[rowIndex].Cells["Sample"].Value;
        }

        /// <summary>
        /// This method calls the "AskIfChangesWillBeUndone()" and if user clicks "Yes", the gridview will revert back to its original state.
        /// </summary>
        public void cancelChanges_Click()
        {
            var result = AskIfChangesWillBeUndone();

            if (result == "Yes")
            {
                DataTable partList = model.GetFeaturesOnOpKey(view.PartStorage, view.OpStorage);
                DataBindFeaturesToFeatureDataGridView(partList);
            }
        }

        /// <summary>
        /// This method calls the "AskIfChangesWillBeSaved()" and if user clicks "Yes", the gridview will save changes to DB.
        /// </summary>
        public void saveButton_Click()
        {
            var result = AskIfChangesWillBeSaved();

            if (result == "Yes")
            {
                //Save Changes made in gridview back to the database
                AdapterUpdate();
                InitializeFeatureGridView();
            }

            else if (result == "No")
            {
                Message_ChangesNotSaved();
            }
        }

        /// <summary>
        /// This method checks to see if the feature grid has any rows. If it does, then it calls the AddFeatureRow(Datatable data).
        /// </summary>
        public void addFeature_Click()
        {
            if (view.BindingSource == null)
            {
                return;
            }

            AddFeatureRow((DataTable)(view.BindingSource.DataSource));
        }

        /// <summary>
        /// This method creates a message box that lets the user know changes have not been save.
        /// </summary>
        private static void Message_ChangesNotSaved()
        {
            const string message2 = "Any Changes to the table have not been updated to the database";
            const string caption2 = "Table Not Saved";
            var result2 = MessageBox.Show(message2, caption2);
        }

        /// <summary>
        /// This method creates a message box that lets the user know changes have beend saved.
        /// </summary>
        private static void Message_ChangesSaved()
        {
            const string message1 = "All changes made to the table have been updated to the database";
            const string caption1 = "Table Saved";
            var result2 = MessageBox.Show(message1, caption1);
        }

        /// <summary>
        /// TODO: create an accurate summary description.
        /// </summary>
        private void AdapterUpdate()
        {

            try
            {
                //Must call EndEdit Method before trying to update database
                view.BindingSource.EndEdit();
                view.SampleBindingSource.EndEdit();

                //Update database
                model.AdapterUpdate((DataTable)view.BindingSource.DataSource);

                //Prompt user that changes have been saved
                Message_ChangesSaved();
            }
            catch
            {
                MessageBox.Show("Error: There was a problem with saving to the database. Check your entries and try again or contact the application development team if the problem persists.");
            }
        }

        /// <summary>
        ///  This method creates a Yes/No messagebox that asks the user if they are sure they want to save changes to feature page.
        /// </summary>
        /// <returns></returns>
        private string AskIfChangesWillBeSaved()
        {
            const string message0 = "Are you sure you want to save all changes made to this set of features? " +
                            "All changes will save to the database.";
            const string caption0 = "Save Changes";

            var result = CreateYesNoMessage(message0, caption0);
            return result.ToString();
        }

        /// <summary>
        /// This method creates a Yes/No messagebox that asks the user if they are sure they want to undo changes to feature page.
        /// </summary>
        /// <returns></returns>
        private string AskIfChangesWillBeUndone()
        {
            const string message = "Are you sure you want to cancel all changes made to this set of features? " +
                            "Any changes to this table will be reverted.";
            const string caption = "Cancel Changes";
            var result = CreateYesNoMessage(message, caption);
            return result.ToString();
        }

        /// <summary>
        /// This method is a framework for initializing a message box with a yes/no button response.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public string CreateYesNoMessage(string message, string caption)
        {
            DialogResult result = MessageBox.Show(message, caption,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

            return result.ToString();
        }

        /// <summary>
        /// This method sees which textbox is in focus in the feature page, and validates that those values match the DB.
        /// </summary>
        //TODO: Always called with "InitializeFeatureGridView()", could they be combined or is there any redundancy?
        internal void ValidatePartAndOpNumberExistWhenEntered(object sender)
        {
            //Pressing enter key on part number text box
            if (sender.Equals(view.PartTextBox))//partBoxFeature.ContainsFocus
            {
                CheckPartNumberExists(view.PartTextBox.Text);
            }
            //Pressing enter on op number text box
            else if (sender.Equals(view.FeatureOpTextBox))
            {
                CheckOpNumberExists();
            }
        }

        /// <summary>
        /// If both textboxes in the feature page have values, this method will try to call other methods to draw the page.
        /// </summary>
        private void InitializeFeatureGridView()
        {
            //As long as both textboxes are not empty
            if (view.PartTextBox.Text != "" && view.FeatureOpTextBox.Text != "")
            {
                DataTable featureTable = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeaturesToFeatureDataGridView(featureTable);
                //SetFeatureDataGridViewHeader();
                StorePartOpNumbers();
            }
        }

        /// <summary>
        /// This method is used to store what part and op number are being viewd in the grid view, in case either value is deleted
        /// in the text box.
        /// </summary>
        private void StorePartOpNumbers()
        {
            view.PartStorage = view.PartTextBox.Text;
            view.OpStorage = view.OperationNumber;
        }

        /*
        /// <summary>
        /// This method sets the feature page header to match whatever part and op is currently being edited.
        /// </summary>
        private void SetFeatureDataGridViewHeader()
        {
            if (view.FeatureCount > 0)
            {

                view.FeaturePageHeaderText = "PART " + view.LastRowFeaturePartNumberFK + " OP " + view.LastRowFeatureOperationNumberFK + " FEATURES";
            }
            else
            {
                view.FeaturePageHeaderText = "FEATURES PAGE";
            }
        }*/

        /// <summary>
        /// TODO: create an accurate summary for this method.
        /// </summary>
        /// <param name="featureTable"></param>
        private void DataBindFeaturesToFeatureDataGridView(DataTable featureTable)
        {
            

            view.FeatureGridView.Columns.Clear();

            view.BindingSource = new BindingSource();

            view.FeatureGridView.DataSource = null;

            view.BindingSource.DataSource = featureTable;

            view.FeatureGridView.DataSource = view.BindingSource;

            ConfigureFeatureDataGridView(featureTable);
        }

        /// <summary>
        /// TODO: create an accurate summary for this method.
        /// </summary>
        /// <param name="featureTable"></param>
        private void ConfigureFeatureDataGridView(DataTable featureTable)
        {

            HideUnecessaryFeatureDataGridViewColumns();

            SetFeatureDataGridViewColumnHeaderTexts();

            SetUpFeatureTypeColumnComboBox();

            SampleComboBind(); //Adds and binds the sample combo box column

            SetUpToolColumnComboBox();

            SetUpDeleteButtonColumn();

            DisableSortableColumns();
        }

        /// <summary>
        /// This method makes all columns in a grid view not sortable.
        /// </summary>
        //TODO: exact same logic as in InspectionPresenter. Can we consolidate this?
        public void DisableSortableColumns()
        {
            for (int j = 0; j < view.FeatureGridView.ColumnCount; j++)
            {
                view.FeatureGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// This method is used to create a not DB column, used to delete rows.
        /// </summary>
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

        /// <summary>
        /// This method is used to create a ComboBoxColumn for inspection tools.
        /// </summary>
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

        /// <summary>
        /// This method creates the sample plan column.
        /// </summary>
        /// <returns></returns>
        private static DataGridViewComboBoxColumn CreateSampleColumn()
        {
            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            SamplingColumn.FlatStyle = FlatStyle.Flat;
            SamplingColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
            SamplingColumn.DisplayMember = "SampleChoice";
            SamplingColumn.ValueMember = "SampleID";
            SamplingColumn.HeaderText = "Sample";
            SamplingColumn.Name = "Sample";
            return SamplingColumn;
        }

        /// <summary>
        /// This method sets up the feature type column in feature grid.
        /// </summary>
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

        /// <summary>
        /// This is a temporary method used to create some temporary tools for inspection tools.
        /// </summary>
        /// <param name="comboboxColumn"></param>
        private static void ToolCategories(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("0-1 Mic", "Height Stand");
        }

        /// <summary>
        /// This method binds what sample plan is chosen for a feature.
        /// </summary>
        private void SampleComboBind()
        {
            DataGridViewComboBoxColumn SamplingColumn = CreateSampleColumn();

            view.FeatureGridView.Columns.Insert(view.FeatureGridView.Columns.Count, SamplingColumn);

            view.SampleBindingSource.DataSource = model.GetSampleChoices(); //Set binding source to list of sample choices from database

            SamplingColumn.DataSource = view.SampleBindingSource; //Set column data source to binding source

            //Initialize combo box value for each row in Sample and FeatureType column
            InitSampleAndFeatureTypeComboBoxColumnValue();
        }

        /// <summary>
        /// TODO: create an accurate summary for this method.
        /// </summary>
        /// <param name="featureTable"></param>
        private void InitSampleAndFeatureTypeComboBoxColumnValue()
        {
            for (int i = 0; i < view.FeatureCount; i++)
            {
                view.FeatureGridView.Rows[i].Cells["Sample"].Value = view.FeatureGridView.Rows[i].Cells["SampleID"].Value;
                view.FeatureGridView.Rows[i].Cells["FeatureTypeColumn"].Value = view.FeatureGridView.Rows[i].Cells["FeatureType"].Value;
            }
        }

        /// <summary>
        /// This method limits the specific grid columns that are visible to the user.
        /// </summary>
        private void HideUnecessaryFeatureDataGridViewColumns()
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
            view.FeatureGridView.Columns["Upper_Tolerance"].Visible = false;
            view.FeatureGridView.Columns["Lower_Tolerance"].Visible = false;
        }

        /// <summary>
        /// This method changes the names of a handful of columns in the feature grid.
        /// </summary>
        private void SetFeatureDataGridViewColumnHeaderTexts()
        {
            view.FeatureGridView.Columns["Sketch_Bubble"].HeaderText = "Sketch Bubble (Optional)";
            view.FeatureGridView.Columns["Plus_Tolerance"].HeaderText = "+";
            view.FeatureGridView.Columns["Minus_Tolerance"].HeaderText = "-";
        }

        //Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange(" ", "Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "Depth", "Straightness", "Flatness", "Parallelism", "Perpendicularity", "Circular Runout", "Total Runout", "Position", "Concentricity");
        }

        /// <summary>
        /// This method validates that the part number exists.
        /// </summary>
        /// <param name="partNumber"></param>
        public void CheckPartNumberExists(string partNumber)
        {

            if (partNumber == "")
            {
                DataTable featureTable = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeaturesToFeatureDataGridView(featureTable);
                MessageBox.Show("Please Enter a Part Number");
                view.PartNumber = null;
                view.OperationNumber = null;
                view.FeaturePageHeaderText = "FEATURES PAGE";
            }
            else if (model.PartNumberExists(partNumber)) //Check if part number entered exists
            {
                view.FeatureOpTextBox.Select();//opBoxFeature.Focus();

            }
            else
            {

                view.FeaturePageHeaderText = "FEATURES PAGE";
                MessageBox.Show("Part Number does not exist");
                view.PartNumber = null; ;
                view.OperationNumber = null;
                DataTable featureTable = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeaturesToFeatureDataGridView(featureTable);


            }
        }

        /// <summary>
        /// This method validates that the op number exists.
        /// </summary>
        private void CheckOpNumberExists()
        {
            if (view.OperationNumber == "")
            {

                view.FeaturePageHeaderText = "FEATURES PAGE";
                MessageBox.Show("Please Enter an Operation Number");
                view.FeatureOpTextBox.Clear();
                DataTable featureTable = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeaturesToFeatureDataGridView(featureTable);
            }
            else if (model.OpExists(view.OperationNumber, view.PartNumber))
            {
                MessageBox.Show("This is a valid Part and Operation Number");
                view.FeatureGridView.Focus();
                view.FeaturePageHeaderText = "PART " + view.PartNumber + " OP " + view.OperationNumber + " FEATURES";
            }
            else
            {
                MessageBox.Show("Op Number does not exist for this Part Number");
                view.FeaturePageHeaderText = "FEATURES PAGE";
                DataTable featureTable = model.GetFeaturesOnOpKey(view.PartNumber, view.OperationNumber);
                DataBindFeaturesToFeatureDataGridView(featureTable);
                view.FeatureOpTextBox.Clear();
            }
        }

        /// <summary>
        /// This method adds a row to the feature grid for next feature entry.
        /// </summary>
        /// <param name="data"></param>
        public void AddFeatureRow(DataTable data)
        {

            data = AddTableRow(data);

            SetPartAndOpNumInTable();
        }

        /// <summary>
        /// TODO: create an accurate summary for this method.
        /// </summary>
        /// <param name="featureTable"></param>
        private void SetPartAndOpNumInTable()
        {
            if (view.FeatureCount > 0)
            {
                //Set the last row Part_Number_FK and Operation_Number_FK to the same value as in the first row
                view.LastRowFeaturePartNumberFK = view.PartNumber;
                view.LastRowFeatureOperationNumberFK = view.OperationNumber;
            }

        }

        /// <summary>
        /// This method checks that there are rows in the grid view, and adds a row.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method checks that the view exists.
        /// </summary>
        /// <returns></returns>
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
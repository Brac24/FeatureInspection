using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public partial class FeatureCreationTableMock : Form, IFeatureCreationView, IInspectionView
    {
        public FeatureCreationPresenter presenter;
        private FeatureCreationModelMock model;
        private InspectionPresenter inspectionPresenter;

        public event EventHandler AddFeatureClicked;
        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> DoneClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;

        BindingSource bindingSource;
        BindingSource bindingSourceInspection = new BindingSource();
        BindingSource bindingSourceListBox = new BindingSource();
        BindingSource bindingSourceFocusCombo = new BindingSource();
        BindingSource sampleBindingSource = new BindingSource();


        public FeatureCreationTableMock()
        {
            InitializeComponent();
            lotSizeBoxInspection.KeyDown += checkEnterKeyPressedInspection;
            opKeyBoxInspection.KeyDown += numOnly_KeyDown;
            lotSizeBoxInspection.KeyDown += numOnly_KeyDown;
            opKeyBoxInspection.KeyDown += keyDownOpLot_Textbox;
            lotSizeBoxInspection.KeyDown += keyDownOpLot_Textbox;
            partsListBox.Text = null;

            for (int i = 0; i < inspectionEntryGridView.ColumnCount; i++)
            {
                inspectionEntryGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int j = 0; j < featureEditGridView.ColumnCount; j++)
            {
                featureEditGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

       


        /************************/
        /***** Properties *******/
        /************************/

        public string PartNumber
        {
            get { return partBoxFeature.Text; }
            set { partBoxFeature.Text = value; }
        }

        public string JobNumber
        {
            get { return jobLabelInspection.Text; }
            set { jobLabelInspection.Text = value; }
        }

        public string OperationNumber
        {
            get { return opBoxFeature.Text; }
        }

        public int PartsInspected
        {
            get { return Int32.Parse(partsInspectedLabel.Text); }
            set { partsInspectedLabel.Text = value.ToString(); }
        }

        public int OpKey
        {
            get { return Int32.Parse(opKeyBoxInspection.Text); }
        }

        public string InspectionHeader
        {
            set { inspectionPageHeader.Text = value.ToString(); }
        }

        public int ListBoxIndex
        {
            get { return partsListBox.SelectedIndex; }
            set { partsListBox.SelectedIndex = value; }
        }
        public int FeatureCount
        {
            get { return featureEditGridView.Rows.Count; }
        }

        public int ListBoxCount
        {
            get { return partsListBox.Items.Count; }
        }


        public object LastRowFeaturePartNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Part_Number_FK"].Value = value; }

        }

        public object LastRowFeatureOperationNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Operation_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Operation_Number_FK"].Value = value; }
        }

        public object FeatureDataSource
        {
            get { return featureEditGridView.DataSource; }
            set { featureEditGridView.DataSource = value; }
        }


        /************************/
        /******* Methods ********/
        /************************/

        public void ShowJobInformation(Job job)
        {
            throw new NotImplementedException();

        }

        public void ShowRelatedFeatures(IList<Feature> relatedFeaures)
        {
            throw new NotImplementedException();
        }

        private void filterTextBox(object sender, KeyEventArgs e)
        {
            int opChars = opKeyBoxInspection.Text.Length;
            int lotChars = lotSizeBoxInspection.Text.Length;

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
                case Keys.PageUp:
                case Keys.PageDown:
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

        #region Inspection Tab Methods
        // INSPECTION TAB METHODS

        public void BindDataGridViewInspection(DataTable featuresTable)
        {
            inspectionEntryGridView.Columns.Clear();
            inspectionEntryGridView.DataSource = null;
            bindingSourceInspection.DataSource = featuresTable;
            inspectionEntryGridView.DataSource = bindingSourceInspection;

            inspectionEntryGridView.Columns["Inspection_Key_FK"].Visible = false;
            inspectionEntryGridView.Columns["Feature_Key"].Visible = false;
            inspectionEntryGridView.Columns["Position_Key"].Visible = false;

            inspectionEntryGridView.Columns["Feature"].ReadOnly = true;

            DataGridViewTextBoxColumn BubbleColumn = new DataGridViewTextBoxColumn();
            {
                BubbleColumn.HeaderText = "Drawing Bubble";
                inspectionEntryGridView.Columns.Insert(0, BubbleColumn);
            }

            DataGridViewTextBoxColumn InspectorID = new DataGridViewTextBoxColumn();
            {
                InspectorID.HeaderText = "Inspector";
                inspectionEntryGridView.Columns.Insert(inspectionEntryGridView.ColumnCount, InspectorID);
            }

            if (inspectionEntryGridView.RowCount != 0)
            {
                inspectionEntryGridView.Rows[0].Cells["Measured Actual"].Selected = true;
            }

            for (int j = 0; j < inspectionEntryGridView.ColumnCount; j++)
            {
                inspectionEntryGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void BindListBox(DataTable partsTable)
        {
            partsListBox.DataSource = null;
            bindingSourceListBox.DataSource = partsTable;
            partsListBox.DataSource = bindingSourceListBox;
            partsListBox.DisplayMember = "PartList";
        }

        private void BindComboBox(DataTable featuresTable)
        {
            inspectionFocusCombo.DataSource = null;
            bindingSourceFocusCombo.DataSource = featuresTable;
            inspectionFocusCombo.DataSource = bindingSourceFocusCombo;
            inspectionFocusCombo.DisplayMember = "Nominal";
            inspectionFocusCombo.ValueMember = "Feature_Key";
        }

        private void AdapterUpdateInspection()
        {
            //Call EndEdit method before updating database
            bindingSourceInspection.EndEdit();

            model.AdapterUpdateInspection((DataTable)bindingSourceInspection.DataSource);
        }

        private bool SetOpKeyInfoInspection(int opkey)
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(opkey);

            if (info.Rows.Count > 0)
            {
                partNumberLabelInspection.Text = info.Rows[0]["Part_Number"].ToString();
                jobLabelInspection.Text = info.Rows[0]["Job_Number"].ToString();
                opLabelInspection.Text = info.Rows[0]["Operation_Number"].ToString();
                lotSizeBoxInspection.Focus();

                return true;
            }

            else
            {
                inspectionEntryGridView.Columns.Clear();
                partNumberLabelInspection.Text = null;
                jobLabelInspection.Text = null;
                opLabelInspection.Text = null;
                MessageBox.Show(opKeyBoxInspection.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
                opKeyBoxInspection.Clear();
                lotSizeBoxInspection.Clear();
                opKeyBoxInspection.Focus();

                return false;
            }
        }

        private void checkEnterKeyPressedInspection(object sender, KeyEventArgs e)
        {

        }

        #endregion

        #region Feature Tab Methods
        // FEATURE TAB METHODS

        //IP>Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange(" ", "Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "Depth", "Straightness", "Flatness", "Parallelism", "Perpendicularity", "Circular Runout", "Total Runout", "Position", "Concentricity");
        }

        private static void ToolCategories(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("0-1 Mic", "Height Stand");
        }

        private void DataBindFeature(DataTable featureTable)
        {

            int maxRows;
            featureEditGridView.Columns.Clear();

            bindingSource = new BindingSource();

            featureEditGridView.DataSource = null;
            bindingSource.DataSource = featureTable;
            featureEditGridView.DataSource = bindingSource;

            featureEditGridView.Columns["Feature_Key"].Visible = false;
            featureEditGridView.Columns["Part_Number_FK"].Visible = false;
            featureEditGridView.Columns["Operation_Number_FK"].Visible = false;
            featureEditGridView.Columns["Feature_Name"].Visible = false;
            featureEditGridView.Columns["Active"].Visible = false;
            featureEditGridView.Columns["Sketch_Bubble"].HeaderText = "Sketch Bubble (Optional)";

            featureEditGridView.Columns["Plus_Tolerance"].HeaderText = "+";
            featureEditGridView.Columns["Minus_Tolerance"].HeaderText = "-";
            featureEditGridView.Columns["Pieces"].Visible = false;
            featureEditGridView.Columns["FeatureType"].Visible = false;
            featureEditGridView.Columns["Places"].Visible = false;
            featureEditGridView.Columns["SampleID"].Visible = false;

            maxRows = featureTable.Rows.Count;

            /*
            //Creates extra columns in Feature Page
            DataGridViewTextBoxColumn BubbleColumn = new DataGridViewTextBoxColumn();
            {
                BubbleColumn.HeaderText = "Sketch Bubble (Optional)";
                featureEditGridView.Columns.Insert(0, BubbleColumn);
            }
            */

            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            {
                FeatureDropColumn.FlatStyle = FlatStyle.Flat;
                FeatureDropColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
                featureEditGridView.Columns.Insert(0, FeatureDropColumn);
                FeatureDropChoices(FeatureDropColumn);
                FeatureDropColumn.HeaderText = "Feature Type (Optional)";
                FeatureDropColumn.Name = "FeatureTypeColumn";
                FeatureDropColumn.DropDownWidth = 160;
            }

            SampleComboBind(); //Adds and binds the sample combo box column

            DataGridViewComboBoxColumn ToolCategoryColumn = new DataGridViewComboBoxColumn();
            {
                ToolCategoryColumn.FlatStyle = FlatStyle.Flat;
                ToolCategoryColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
                ToolCategoryColumn.HeaderText = "Tool";
                featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, ToolCategoryColumn);
                ToolCategories(ToolCategoryColumn);
            }

            DataGridViewButtonColumn DeleteButtonColumn = new DataGridViewButtonColumn();
            {
                DeleteButtonColumn.FlatStyle = FlatStyle.Popup;
                DeleteButtonColumn.CellTemplate.Style.BackColor = Color.DarkRed;
                DeleteButtonColumn.CellTemplate.Style.SelectionBackColor = Color.DarkRed;
                DeleteButtonColumn.HeaderText = "Delete Feature";
                DeleteButtonColumn.Text = "Delete";
                featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, DeleteButtonColumn);
                DeleteButtonColumn.UseColumnTextForButtonValue = true;
            }

            for (int j = 0; j < featureEditGridView.ColumnCount; j++)
            {
                featureEditGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SampleComboBind()
        {
            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            SamplingColumn.FlatStyle = FlatStyle.Flat;
            SamplingColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);

            featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, SamplingColumn);

            sampleBindingSource.DataSource = model.GetSampleChoices();

            //Binding combo box to database table of sample choices
            SamplingColumn.DisplayMember = "SampleChoice";
            SamplingColumn.ValueMember = "SampleID";

            SamplingColumn.HeaderText = "Sample";
            SamplingColumn.Name = "Sample";
            SamplingColumn.DataSource = sampleBindingSource;

            //setting initial selected value to hidden SampleID column value for the current row
            for (int i = 0; i < featureEditGridView.Rows.Count; i++)
            {
                featureEditGridView.Rows[i].Cells["Sample"].Value = featureEditGridView.Rows[i].Cells["SampleID"].Value;
                featureEditGridView.Rows[i].Cells["FeatureTypeColumn"].Value = featureEditGridView.Rows[i].Cells["FeatureType"].Value;
            }
        }

        private void AdapterUpdate()
        {
            //Must call EndEdit Method before trying to update database
            bindingSource.EndEdit();
            sampleBindingSource.EndEdit();

            //Update database
            model.AdapterUpdate((DataTable)bindingSource.DataSource);
        }



        private void SetOpKeyInfoFeature(int opkey)
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(opkey);
        }


        #endregion

        /************************/
        /***  Event Handlers ****/
        /************************/

        //FORM HANLDERS

        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            model = new FeatureCreationModelMock();
            presenter = new FeatureCreationPresenter(this, model); //Give a reference of the view and model to the presenter class
            inspectionPresenter = new InspectionPresenter(this, model);
        }

        //HANDLER FOR BOTH TABS

        private void numOnly_KeyDown(object sender, KeyEventArgs e)
        {
            filterTextBox(sender, e);

            if (sender == opKeyBoxInspection)
            {
                checkEnterKeyPressedInspection(sender, e);
            }
        }


        #region Inspection Handlers
        //INSPECTION ENTRY TAB HANDLERS

        private void keyDownOpLot_Textbox(object sender, KeyEventArgs e)
        {
            inspectionPresenter.suppressZeroFirstChar(sender, e);
        }

        private void nextPartButton_Click(object sender, EventArgs e)
        {
            inspectionPresenter.GotToNextPart();
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            inspectionPresenter.updateGridViewOnIndexChange(sender);
        }

        private void inspectionEntryGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AdapterUpdateInspection();
        }

        private void inspectionEntryGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        #endregion

        #region Feature Handlers

        // FEATURE HANDLERS

        /// <summary>
        /// This event handler is fired on part and operation number text boxes and checks 
        /// the enter key was pressed in each of these
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkEnterKeyPressedFeatures(object sender, KeyEventArgs e)
        {
            SuppressKeyIfNotANumber(e);

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                ValidateTextBoxes();

                InitializeFeatureGridView();
            }
        }

        private static void SuppressKeyIfNotANumber(KeyEventArgs e)
        {
            char currentKey = (char)e.KeyCode;
            bool nonNumber = char.IsWhiteSpace(currentKey);

            if (nonNumber)
                e.SuppressKeyPress = true;
        }

        private void ValidateTextBoxes()
        {
            //Pressing enter key on part number text box
            if (partBoxFeature.ContainsFocus)
            {
                CheckPartNumberExists();
            }
            //Pressing enter on op number text box
            else if (opBoxFeature.ContainsFocus)
            {
                CheckOpNumberExists();
            }
        }

        private void CheckOpNumberExists()
        {
            if(opBoxFeature.Text == "")
            {
                MessageBox.Show("Please Enter an Operation Number");
            }
            else if (model.OpExists(opBoxFeature.Text, partBoxFeature.Text))
            {
                featureEditGridView.Focus();
            }
            else
            {
                MessageBox.Show("Op Number does not exist for this Part Number");
                opBoxFeature.Clear();
            }
        }

        private void CheckPartNumberExists()
        {
            //TODO: ADD CHECK IF EMPTY. IF IT IS TELL USER TO ENTER A PART NUMBER

            if(partBoxFeature.Text == "")
            {
                MessageBox.Show("Please Enter a Part Number");
            }
            else if (model.PartNumberExists(PartNumber)) //Check if part number entered exists
            {
                opBoxFeature.Focus();
            }
            else
            {
                MessageBox.Show("Part Number does not exist");
                partBoxFeature.Clear();
            }
        }

        private void InitializeFeatureGridView()
        {
            //As long as both textboxes are not empty
            if (PartNumber != "" && OperationNumber != "")
            {
                DataTable featureList = model.GetFeaturesOnOpKey(PartNumber, OperationNumber);
                DataBindFeature(featureList);
                SetFeatureGridViewHeader();
            }
        }

        private void SetFeatureGridViewHeader()
        {
            if (FeatureCount > 0)
            {

                featurePageHeader.Text = "PART " + LastRowFeaturePartNumberFK + " OP " + LastRowFeatureOperationNumberFK + " FEATURES";
            }
            else
            {
                featurePageHeader.Text = "FEATURES PAGE";
            }
        }

        /// <summary>
        /// Used to set the type of sampling and the feature type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SetSampleIDAndFeatureTypeHiddenColumns(e);

            // AdapterUpdate((BindingSource)table.DataSource);

        }

        private void SetSampleIDAndFeatureTypeHiddenColumns(DataGridViewCellEventArgs e)
        {
            if (featureEditGridView.Columns["Sample"] != null || featureEditGridView.Columns["FeatureTypeColumn"] != null)
            {

                if (featureEditGridView.Columns["Sample"].Index == e.ColumnIndex)
                {
                    featureEditGridView.Rows[e.RowIndex].Cells["SampleID"].Value = featureEditGridView.Rows[e.RowIndex].Cells["Sample"].Value;
                }
                else if (featureEditGridView.Columns["FeatureTypeColumn"].Index == e.ColumnIndex)
                {
                    featureEditGridView.Rows[e.RowIndex].Cells["FeatureType"].Value = featureEditGridView.Rows[e.RowIndex].Cells["FeatureTypeColumn"].Value;
                }

            }
        }

        /// <summary>
        /// Will add an empty row to FeatureGridView and set invisible part number and op number columns
        /// so that the feature will properly get inserted to the database with the correct info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFeature_Click(object sender, EventArgs e)
        {
            if (bindingSource == null)
            {
                return;
            }

            presenter.AddFeatureRow((DataTable)(bindingSource.DataSource));
        }


        /// <summary>
        /// Deletes the row that the user clicks delete on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRowFeature_MouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            presenter.DeleteDataGridViewRow(sender, e);
        }



        /// <summary>
        /// Will simply rebind feature data grid view without updating the database.
        /// To have the old state the user started with
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelChanges_Click(object sender, EventArgs e)
        {
            DialogResult result = AskIfChangesWillBeUndone();

            if (result == DialogResult.Yes)
            {
                DataTable partList = model.GetFeaturesOnOpKey(PartNumber, OperationNumber);
                DataBindFeature(partList);
            }
        }

        private static DialogResult AskIfChangesWillBeUndone()
        {
            const string message = "Are you sure you want to cancel all changes made to this set of features? " +
                            "Any changes to this table will be reverted.";
            const string caption = "Cancel Changes";
            DialogResult result = CreateYesNoMessage(message, caption);
            return result;
        }

        private static DialogResult CreateYesNoMessage(string message, string caption)
        {
            return MessageBox.Show(message, caption,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);
        }

        /// <summary>
        /// Updates, deletes, or inserts any data needed to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult result = AskIfChangesWillBeSaved();

            if (result == DialogResult.Yes)
            {
                //Save Changes made in gridview back to the database
                AdapterUpdate();

                //Prompt user that changes have been saved
                Message_ChangesSaved();
            }

            else if (result == DialogResult.No)
            {
                Message_ChangesNotSaved();
            }

        }

        private static DialogResult AskIfChangesWillBeSaved()
        {
            const string message0 = "Are you sure you want to save all changes made to this set of features? " +
                            "All changes will save to the database.";
            const string caption0 = "Save Changes";

            var result = CreateYesNoMessage(message0, caption0);
            return result;
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

        /// <summary>
        /// Handle invalid data input to datagridview and alert the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featureEditGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        #endregion

        #region Report Handlers

        private void ReportSwitch(object sender, EventArgs e)
        {
            if (summaryChart.Visible == true)
            {
                summaryChart.Visible = false;
                SummaryList.Visible = true;
            }
            else
            {
                summaryChart.Visible = true;
                SummaryList.Visible = false;
            }
        }

        #endregion

    }
}

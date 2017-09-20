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

        public TextBox PartTextBox
        {
            get { return partBoxFeature; }
            set { }
        }

        public TextBox OpTextBox
        {
            get { return opBoxFeature; }
        }

        public DataGridView FeatureGridView { get { return featureEditGridView; } }

        public BindingSource BindingSource { get { return bindingSource; } set { bindingSource = value; } }

        public BindingSource SampleBindingSource { get { return sampleBindingSource; } set { sampleBindingSource = value; } }

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

        public string FeaturePageHeader { set { featurePageHeader.Text = value; } }

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

        #endregion

        #region Feature Tab Methods
        // FEATURE TAB METHODS

        public string CreateYesNoMessage(string message, string caption)
        {
            DialogResult result = MessageBox.Show(message, caption,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

            return result.ToString();
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

        private void checkEnterKeyPressedInspection(object sender, KeyEventArgs e)
        {

            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                lotSizeBoxInspection.ReadOnly = false;
                DataTable featureTable;
                bool isValidOpKey = false;
                bool inspectionExists = false;
                int opkey = 0;
                DataTable partList = null;
                String text = opKeyBoxInspection.Text;
                DataTable featureList = null;

                try
                {
                    opkey = Int32.Parse(text);
                    partList = model.GetPartsList(opkey);
                    isValidOpKey = SetOpKeyInfoInspection(opkey);
                }
                catch
                {
                    inspectionEntryGridView.DataSource = null;
                    partsListBox.DataSource = null;
                    lotSizeBoxInspection.Text = null;
                    partNumberLabelInspection.Text = null;
                    jobLabelInspection.Text = null;
                    opLabelInspection.Text = null;
                    inspectionEntryGridView.Columns.Clear();
                    inspectionPageHeader.Text = "INSPECTION PAGE";
                    MessageBox.Show("Please enter a valid Op Key", "Invalid OpKey");
                }

                if (isValidOpKey)
                {
                    inspectionExists = model.GetInspectionExistsOnOpKey(opkey);

                    if (inspectionExists)
                    {
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(opkey);

                        featureList = model.GetFeatureList(opkey);
                        BindComboBox(featureList);

                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                BindListBox(partList);
                                lotSizeBoxInspection.Text = model.GetLotSize(opkey);
                                lotSizeBoxInspection.ReadOnly = true;
                            }
                            else if (lotSizeBoxInspection.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(lotSizeBoxInspection.Text), opkey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(opkey, Int32.Parse(lotSizeBoxInspection.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(opkey);

                                //Bind the part list box BindListBox(partList);
                                BindListBox(partList);

                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                            inspectionEntryGridView.DataSource = null;
                            partsListBox.DataSource = null;
                            lotSizeBoxInspection.Text = null;
                            inspectionPageHeader.Text = "INSPECTION PAGE";
                            inspectionEntryGridView.Columns.Clear();

                            MessageBox.Show("Lead must add features to this Part and Operation number");

                        }
                    }
                    else
                    {
                        //Create the inspection in inspection table
                        lotSizeBoxInspection.Clear();
                        model.CreateInspectionInInspectionTable(opkey);
                        MessageBox.Show("Creating Inspection");

                        //Run the logic inside the if loop above
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(opkey);


                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                BindListBox(partList);
                                lotSizeBoxInspection.Text = model.GetLotSize(opkey);
                                lotSizeBoxInspection.ReadOnly = true;
                            }
                            else if (lotSizeBoxInspection.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(lotSizeBoxInspection.Text), opkey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(opkey, Int32.Parse(lotSizeBoxInspection.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(opkey);

                                //Bind the part list box BindListBox(partList);
                                BindListBox(partList);
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

        /// <summary>
        /// This event handler stops Inspection page textboxes from accepting 0's as their first character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyDownOpLot_Textbox(object sender, KeyEventArgs e)
        {
            inspectionPresenter.keyDownOpLot_TextBox(sender, e);       
        }

        /// <summary>
        /// This event handler will cycle through parts in the part list box when the user clicks on the 'next part' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextPartButton_Click(object sender, EventArgs e)
        {
            inspectionPresenter.GotToNextPart();
        }

        /// <summary>
        /// This will update the grid view to follow what part is highlighted in the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_ChangeWithPart(object sender, EventArgs e)
        {
            inspectionPresenter.updateGridViewOnIndexChange(sender);
        }

        /// <summary>
        /// Handles updating the DB when the Inspection grid view is done being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AdapterUpdateInspection();
        }

        /// <summary>
        /// Handles exception error on the inspection grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            presenter.checkEnterKeyPressed(e);
        }  

        /// <summary>
        /// Used to set the type of sampling and the feature type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            presenter.dataGridView1_CellEndEdit(e);
            

            // AdapterUpdate((BindingSource)table.DataSource);

        }

        /// <summary>
        /// Will add an empty row to FeatureGridView and set invisible part number and op number columns
        /// so that the feature will properly get inserted to the database with the correct info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFeature_Click(object sender, EventArgs e)
        {
            presenter.addFeature_Click();
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
            presenter.cancelChanges_Click();
        }

        /// <summary>
        /// Updates, deletes, or inserts any data needed to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            presenter.saveButton_Click();

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

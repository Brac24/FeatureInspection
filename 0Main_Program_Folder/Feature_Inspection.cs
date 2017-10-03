using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public partial class Feature_Inspection : Form, IFeatureCreationView, IInspectionView
    {
        public FeatureCreationPresenter presenter;
        private FeatureCreationModelMock model;
        private InspectionPresenter inspectionPresenter;

        BindingSource bindingSource;
        BindingSource bindingSourceInspection = new BindingSource(); //Look at all references of "bindingSourceInspection" It looks a little unnecessary.
        BindingSource bindingSourceListBox = new BindingSource();
        BindingSource bindingSourceFocusCombo = new BindingSource();
        BindingSource sampleBindingSource = new BindingSource();


        public Feature_Inspection()
        {
            InitializeComponent();
        }


        /************************/
        /***** Properties *******/
        /************************/


        #region Properties

        public TextBox PartTextBox { get { return partBoxFeature; } set { } }

        public TextBox FeatureOpTextBox { get { return opBoxFeature; } }

        public DataGridView FeatureGridView { get { return featureEditGridView; } }

        public DataGridView InspectionGrid { get { return inspectionEntryGridView; } }

        public BindingSource BindingSource { get { return bindingSource; } set { bindingSource = value; } }

        public BindingSource InspectionBindingSource { get { return bindingSourceInspection; } set { bindingSourceInspection = value; } }

        //redudant? couldn't we use ^^ "BindingSource" for anyplace we are calling "SampleBindingSource"
        public BindingSource SampleBindingSource { get { return sampleBindingSource; } set { sampleBindingSource = value; } }

        public BindingSource ListBoxBindingSource { get { return bindingSourceListBox; } set { bindingSourceListBox = value; } }

        public Chart InspectionChart { get { return inspectionChart; } set { inspectionChart = value; } }

        public ComboBox ChartFocusComboBox { get { return inspectionFocusCombo; } set { inspectionFocusCombo = value; } }

        public TextBox LotsizeTextBox { get { return lotSizeBoxInspection; } set { lotSizeBoxInspection = value; } }

        public TextBox OpKeyTextBox { get { return opKeyBoxInspection; } set { opKeyBoxInspection = value; } }

        public ListBox PartsListBox { get { return partsListBox; } set { partsListBox = value; } }

        public Label InspectionPageHeader { get { return inspectionPageHeader; } set { inspectionPageHeader = value; } }

        public Label PartNumberLabel { get { return partNumberLabelInspection; } set { partNumberLabelInspection = value; } }

        public Label JobLabel { get { return jobLabelInspection; } set { jobLabelInspection = value; } }

        public Label OperationLabel { get { return opLabelInspection; } set { opLabelInspection = value; } }

        public string PartStorage { get { return partStorageLabel.Text; } set { partStorageLabel.Text = value; } }

        public string OpStorage { get { return opStorageLabel.Text; } set { opStorageLabel.Text = value; } }

        public string PartNumber { get { return partBoxFeature.Text; } set { partBoxFeature.Text = value; } }

        public string InspectionHeaderText { set { inspectionPageHeader.Text = value.ToString(); } }

        public string OperationNumber { get { return opBoxFeature.Text; } }

        public string JobNumber { get { return jobLabelInspection.Text; } set { jobLabelInspection.Text = value; } }

        public string FeaturePageHeaderText { get { return featurePageHeader.Text; } set { featurePageHeader.Text = value; } }

        public int OpKey { get { return Int32.Parse(opKeyBoxInspection.Text); } }

        public int ListBoxIndex { get { return partsListBox.SelectedIndex; } set { partsListBox.SelectedIndex = value; } }

        public int FeatureCount { get { return featureEditGridView.Rows.Count; } }

        public int ListBoxCount { get { return partsListBox.Items.Count; } }

        public object FeatureDataSource { get { return featureEditGridView.DataSource; } set { featureEditGridView.DataSource = value; } }

        public object LastRowFeatureOperationNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Operation_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Operation_Number_FK"].Value = value; }
        }

        public object LastRowFeaturePartNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Part_Number_FK"].Value = value; }
        }

        #endregion


        /************************/
        /******* Methods ********/
        /************************/

        #region Inspection Tab Methods
        // INSPECTION TAB METHODS

        #endregion

        #region Feature Tab Methods
        // FEATURE TAB METHODS

        //TODO: Is there a plan for this method?
        public void ShowJobInformation(Job job)
        {
            throw new NotImplementedException();
        }

        //TODO: Is there a plan for this method?
        public void ShowRelatedFeatures(IList<Feature> relatedFeaures)
        {
            throw new NotImplementedException();
        }

        #endregion


        /************************/
        /***  Event Handlers ****/
        /************************/
        /*All Handlers with <summary> blocks have been refactored or have been deemed simple enough.
        Handlers that have TODOs on them are either non-functioning or are bloated and contain too much logic.
        
        I think that plenty of our handlers can also use clearer names, even though they have summaries.*/


        #region Form Handlers

        //FORM HANLDERS

        /// <summary>
        /// This event handler gives a reference of the view and model to the presenter class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            model = new FeatureCreationModelMock();
            presenter = new FeatureCreationPresenter(this, model);
            inspectionPresenter = new InspectionPresenter(this, model);

            inspectionPresenter.createGraphArea(inspectionChart);
        }

        #endregion

        #region Multiple Tab Handlers

        //HANDLER FOR BOTH TABS

        /*TODO: Figure out if this can be handled with "filterTextBox" or "suppressZeroFirstChar" or "checkEnterKeyPressedInspection"
        in a better way*/
        private void numOnly_KeyDown(object sender, KeyEventArgs e)
        {
            filterTextBox(sender, e);

            if (sender == opKeyBoxInspection)
            {
                checkEnterKeyPressedInspection(sender, e);
            }
        }

        #endregion

        #region Inspection Handlers

        //INSPECTION ENTRY TAB HANDLERS

        /*TODO: This is without at doubt our most bloated handler. I am not even sure how to start trimming the fat off it.
         It is also one of the offenders of redundant and overriding logic along with "SetOpKeyInfoInspection", "numOnly_KeyDown", 
         "suppressZeroFirstChar" and "filterTextBox".*/
        private void checkEnterKeyPressedInspection(object sender, KeyEventArgs e)
        {

            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                lotSizeBoxInspection.ReadOnly = false;
                DataTable featureTable;
                bool isValidOpKey = false;
                bool inspectionExists = false;
                DataTable partList = null;
                String text = opKeyBoxInspection.Text;
                DataTable featureList = null;

                partList = model.GetPartsList(OpKey);
                isValidOpKey = inspectionPresenter.SetOpKeyInfoInspection();

                if (isValidOpKey)
                {
                    inspectionExists = model.GetInspectionExistsOnOpKey(OpKey);

                    if (inspectionExists)
                    {
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(OpKey);

                        featureList = model.GetFeatureList(OpKey);
                        inspectionPresenter.BindFocusComboBox(featureList);

                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                inspectionPresenter.BindPartListBox(partList);
                                lotSizeBoxInspection.Text = model.GetLotSize(OpKey);
                                lotSizeBoxInspection.ReadOnly = true;
                            }
                            else if (lotSizeBoxInspection.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(lotSizeBoxInspection.Text), OpKey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(OpKey, Int32.Parse(lotSizeBoxInspection.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(OpKey);

                                //Bind the part list box BindListBox(partList);
                                inspectionPresenter.BindPartListBox(partList);

                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                            inspectionPresenter.smallInspectionPageClear();
                            MessageBox.Show("OpKey exists, enter in how many parts you have");

                        }
                    }
                    else
                    {
                        //Create the inspection in inspection table
                        lotSizeBoxInspection.Clear();
                        model.CreateInspectionInInspectionTable(OpKey);
                        MessageBox.Show("Lead must add features to this Part and Operation number");

                        //Run the logic inside the if loop above
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(OpKey);


                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                inspectionPresenter.BindPartListBox(partList);
                                lotSizeBoxInspection.Text = model.GetLotSize(OpKey);
                                lotSizeBoxInspection.ReadOnly = true;
                            }
                            else if (lotSizeBoxInspection.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table

                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(lotSizeBoxInspection.Text), OpKey);
                                //Create the parts in the positions table
                                model.InsertPartsToPositionTable(OpKey, Int32.Parse(lotSizeBoxInspection.Text));

                                //Get part list DataTable partList = model.GetPartsList(opkey);
                                partList = model.GetPartsList(OpKey);

                                //Bind the part list box BindListBox(partList);
                                inspectionPresenter.BindPartListBox(partList);
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

        /*TODO: one of our larger handlers that should be slimmed down. It is also one of the offenders of redundant and
         overriding logic along with "SetOpKeyInfoInspection", "numOnly_KeyDown", "suppressZeroFirstChar" and "checkEnterKeyPressedInspection"
         a specific example is that this event handler makes "suppressZeroFirstChar" only half functional.*/
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

        //TODO: Tried to make it so when you click out of OpKeyTextBox inspectionEntryGridView would update
        private void partBoxFeature_Leave(object sender, EventArgs e)
        {
            //presenter.leaveFocus(e);
        }

        /*TODO: Tried to lock the cells upon value changing instead of hitting enter. Otherwise its possible to lock yourself out
        of redos without ever changing the measured value.*/
        private void inspectionEntryGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //TODO: stuff
            //inspectionPresenter.lockCellInspection(sender, e);
        }

        /// <summary>
        /// This event handler stops Inspection page textboxes from accepting 0's as their first character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyDownOpLot_Textbox(object sender, KeyEventArgs e)
        {
            inspectionPresenter.suppressZeroFirstChar(sender, e);
            numOnly_KeyDown(sender, e);
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
        /// This will call for a method to update the inspection page when the index of the part changes.
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
            //TODO: Table should be binding everytime the Measured value has been edited
            inspectionPresenter.AdapterUpdateInspection();
            inspectionPresenter.BindFocusCharts();
            //inspectionPresenter.lockCellInspection(sender, e);
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

        /// <summary>
        /// Handles when the value selected in the inspection focus combo box changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionFocusCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            inspectionPresenter.BindFocusCharts();
        }

        /// <summary>
        /// Handles when databinding is complete on the inspection entry grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            inspectionPresenter.BindFocusCharts();
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
            presenter.SetSampleIDAndFeatureTypeHiddenColumns(e);
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
        /// Allows user to redo an inspection entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RedoRowInspection_MouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            inspectionPresenter.RedoDataGridViewRow(sender, e);
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

        // REPORT HANDLERS

        /// <summary>
        /// This handler switches the report view when clicking on the "switch button".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
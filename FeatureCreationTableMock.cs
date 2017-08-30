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
    public partial class FeatureCreationTableMock : Form, IFeatureCreationView
    {
        public FeatureCreationPresenter presenter;
        private FeatureCreationModelMock model;

        public event EventHandler AddFeatureClicked;
        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> DoneClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;


        BindingSource bindingSource;
        BindingSource bindingSourceInspection = new BindingSource();
        BindingSource bindingSourceListBox = new BindingSource();


        public FeatureCreationTableMock()
        {
            InitializeComponent();
            opKeyBoxInspection.KeyDown += numOnly_KeyDown;
            //partBoxFeature.KeyDown += numOnly_KeyDown;
            //opBoxFeature.KeyDown += numOnly_KeyDown;
            partBoxFeature.KeyDown += checkEnterKeyPressedFeatures;
            opBoxFeature.KeyDown += checkEnterKeyPressedFeatures;
        }


        /************************/
        /***** Properties *******/
        /************************/

        public string PartNumber
        {
            get { return partNumberLabelInspection.Text; }
            set { partNumberLabelInspection.Text = value; }
        }

        public string JobNumber
        {
            get { return jobLabelInspection.Text; }
            set { jobLabelInspection.Text = value; }
        }

        public string OperationNumber
        {
            get { return opLabelInspection.Text; }
            set { opLabelInspection.Text = value; }
        }

        public string Status
        {
            get { return statusLabelInspection.Text; }
            set { statusLabelInspection.Text = value; }
        }

        public int PartsInspected
        {
            get { return Int32.Parse(partsInspectedLabel.Text); }
            set { partsInspectedLabel.Text = value.ToString(); }
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

            //Block non-number characters
            char currentKey = (char)e.KeyCode;
            bool modifier = e.Control || e.Alt || e.Shift;
            bool nonNumber = char.IsLetter(currentKey) ||
                             char.IsSymbol(currentKey) ||
                             char.IsWhiteSpace(currentKey) ||
                             char.IsPunctuation(currentKey) ||
                             char.IsSeparator(currentKey) ||
                             char.IsUpper(currentKey);



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

        private void BindDataGridViewInspection(DataTable featuresTable)
        {
            inspectionEntryGridView.DataSource = null;
            bindingSourceInspection.DataSource = featuresTable;
            inspectionEntryGridView.DataSource = bindingSourceInspection;

            inspectionEntryGridView.Columns["Inspection_Key_FK"].Visible = false;
            inspectionEntryGridView.Columns["Feature_Key"].Visible = false;
            inspectionEntryGridView.Columns["Position_Key"].Visible = false;

            inspectionEntryGridView.Columns["Feature"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inspectionEntryGridView.Columns["Measured Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            inspectionEntryGridView.Columns["InspectionTool"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            
            inspectionEntryGridView.Columns["Feature"].ReadOnly = true;

            if (inspectionEntryGridView.RowCount != 0)
            {
                inspectionEntryGridView.Rows[0].Cells["Measured Actual"].Selected = true;
                inspectionEntryGridView.Rows[(inspectionEntryGridView.RowCount - 1)].ReadOnly = true;
            }
        }

        private void BindListBox(DataTable partsTable)
        {
            partsListBox.DataSource = null;
            bindingSourceListBox.DataSource = partsTable;
            partsListBox.DataSource = bindingSourceListBox;
            partsListBox.DisplayMember = "PartList";
            //partsListBox.ValueMember = "PartList";
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

                return true;
            }

            else
            {
                partNumberLabelInspection.Text = null;
                jobLabelInspection.Text = null;
                opLabelInspection.Text = null;
                MessageBox.Show(opKeyBoxInspection.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
                opKeyBoxInspection.Clear();

                return false;
            }
        }

        private void checkEnterKeyPressedInspection(object sender, KeyEventArgs e)

        {
            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                DataTable featureTable;
                bool isValidOpKey = false;
                bool inspectionExists = false;
                e.Handled = true;
                int opkey = Int32.Parse(opKeyBoxInspection.Text);
                DataTable partList = model.GetPartsList(opkey);
                isValidOpKey = SetOpKeyInfoInspection(opkey);

                if(isValidOpKey)
                {
                    inspectionExists = model.GetInspectionExistsOnOpKey(opkey);

                    if(inspectionExists)
                    {
                        //Check if there are features related on op and part numn
                        featureTable = model.GetFeaturesOnOpKey(opkey);

                        if (featureTable.Rows.Count > 0)
                        {
                            //Check if there are parts in position
                            if (partList.Rows.Count > 0)
                            {
                                //Get the parts if there are
                                BindListBox(partList);
                            }
                            else
                            {
                                //Create the parts in the positions table

                                //Get part list DataTable partList = model.GetPartsList(opkey);

                                //Bind the part list box BindListBox(partList);
                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                            MessageBox.Show("Lead must add features to this Part and Operation number");
                        }
                    }
                    else
                    {
                        //Create the inspection in inspection table
                        MessageBox.Show("Creating Inspection");

                        //Run the logic inside the if loop above
                    }

                }
                else
                {
                    //Not valid opkey
                    
                }
                
                partNumberLabelInspection.Focus();
            }
        }

        #endregion



        #region Feature Tab Methods
        // FEATURE TAB METHODS

        //IP>Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "GDT", "Depth");
        }

        private static void SampleChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("100%", "First Piece", "First & Last", "Sample Plan");
        }

        private static void ToolCategories(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("0-1 Mic", "Height Stand");
        }

        private void DataBindTest(DataTable featureTable)
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
            featureEditGridView.Columns["Plus_Tolerance"].HeaderText = "+";
            featureEditGridView.Columns["Minus_Tolerance"].HeaderText = "-";
            featureEditGridView.Columns["Pieces"].Visible = false;
            featureEditGridView.Columns["FeatureType"].Visible = false;
            featureEditGridView.Columns["Places"].Visible = false;

            maxRows = featureTable.Rows.Count;

            /*
            for (int i = 0; i < featureEditGridView.Rows.Count; i++)
            {
                featureEditGridView.Rows[i].ReadOnly = true;
            }

            //IP>Initializes and defines the edit button column.
            
            DataGridViewButtonColumn EditButtonColumn = new DataGridViewButtonColumn();
            EditButtonColumn.Name = "Edit Column";
            featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, EditButtonColumn);
            for (int i = 0; i < featureEditGridView.Rows.Count; i++)
            {
                featureEditGridView.Rows[i].Cells["Edit Column"].Value = "Edit";
            }
            */



            //IP>Initializes and defines the feature type column.
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            DataGridViewTextBoxColumn BubbleColumn = new DataGridViewTextBoxColumn();
            DataGridViewComboBoxColumn ToolCategoryColumn = new DataGridViewComboBoxColumn();
            FeatureDropColumn.HeaderText = "Feature Type";
            featureEditGridView.Columns.Insert(0, FeatureDropColumn);
            FeatureDropColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            FeatureDropChoices(FeatureDropColumn);
            BubbleColumn.HeaderText = "Sketch Bubble";
            featureEditGridView.Columns.Insert(0, BubbleColumn);
            featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, SamplingColumn);
            SamplingColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            SamplingColumn.HeaderText = "Sample";
            SampleChoices(SamplingColumn);
            featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, ToolCategoryColumn);
            ToolCategoryColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            ToolCategories(ToolCategoryColumn);
            ToolCategoryColumn.HeaderText = "Tool";


            for (int j = 0; j < featureEditGridView.ColumnCount; j++)
            {
                featureEditGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void AdapterUpdate()
        {
            //Must call EndEdit Method before trying to update database
            bindingSource.EndEdit();


            //Update database
            model.AdapterUpdate((DataTable)bindingSource.DataSource);
        }

        private void AddTableRow(DataTable t)
        {
            if (featureEditGridView.DataSource == null)
            {
                return;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);
        }

        private void SetOpKeyInfoFeature(int opkey)
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(opkey);

            if (info.Rows.Count <= 0)
            {
                MessageBox.Show(partBoxFeature.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
                partBoxFeature.Clear();
            }
        }

        private void checkEnterKeyPressedFeatures(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                string partNumber = partBoxFeature.Text;
                string operationNum = opBoxFeature.Text; ;

                if (partNumber != "" && operationNum != "")
                {
                    DataTable partList = model.GetFeaturesOnOpKey(partNumber, operationNum);
                    DataBindTest(partList);
                    if (partList.Rows.Count > 0)
                    {
                        featurePageHeader.Text = "PART " + featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value + " OP " + featureEditGridView.Rows[0].Cells["Operation_Number_FK"].Value + " FEATURES";
                    }
                    else
                    {
                        featurePageHeader.Text = "FEATURES PAGE";
                    }
                }
                else
                {
                    return;
                }

            }
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
        }


        //HANDLER FOR BOTH TABS

        private void numOnly_KeyDown(object sender, KeyEventArgs e)
        {
            filterTextBox(sender, e);

            if (sender == opKeyBoxInspection)
            {
                checkEnterKeyPressedInspection(sender, e);
            }
           /* else if (sender == partBoxFeature)
            {
                checkEnterKeyPressedFeatures(sender, e);
            }
            else if (sender == opBoxFeature)
            {
                checkEnterKeyPressedFeatures(sender, e);
            }*/

        }



        #region Inspection Handlers
        //INSPECTION ENTRY TAB HANDLERS

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Use (listbox)sender.SelectedIndex
            var listBox = (ListBox)sender;
            DataTable featureTable;

            if (listBox.Text.Contains("Part"))
            {
                int listBoxIndex = listBox.SelectedIndex;
                inspectionPageHeader.Text = listBox.Text;
                int pieceID = listBoxIndex + 1; //Due to 0 indexing
                featureTable = model.GetFeaturesOnPartIndex(pieceID, Int32.Parse(opKeyBoxInspection.Text));
                BindDataGridViewInspection(featureTable);
            }
        }

        private void nextPartButton_Click(object sender, EventArgs e)
        {
            //+1 to selectedindex because we need to check what index it is going in to first
            if (partsListBox.Items.Count > 0)
            {
                partsListBox.SelectedIndex = (partsListBox.SelectedIndex + 1 < partsListBox.Items.Count) ?
                partsListBox.SelectedIndex += 1 : partsListBox.SelectedIndex = 0;
            }
        }

        private void inspectionEntryGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == inspectionEntryGridView.Columns["Measured Actual"].Index)
            {
                inspectionEntryGridView.BeginEdit(true);
            }
        }

        #endregion

        #region Feature Handlers

        // FEATURE HANDLERS

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // AdapterUpdate((BindingSource)table.DataSource);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bindingSource == null)
            {
                return;
            }

            DataTable data = (DataTable)(bindingSource.DataSource);
            AddTableRow(data);

            //Set the last row Part_Number_FK and Operation_Number_FK to the same value as in the first row
            featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Part_Number_FK"].Value = featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value;
            featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Operation_Number_FK"].Value = featureEditGridView.Rows[0].Cells["Operation_Number_FK"].Value;

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            featureEditGridView.Rows[e.RowIndex].ReadOnly = false;
        }

        private void inspectionEntryGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AdapterUpdateInspection();
        }

        #endregion

    }

}

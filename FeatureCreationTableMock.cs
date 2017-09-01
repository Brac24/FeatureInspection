﻿using System;
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
            partBoxFeature.KeyDown += checkEnterKeyPressedFeatures;
            opBoxFeature.KeyDown += checkEnterKeyPressedFeatures;
            lotSizeBoxInspection.KeyDown += numOnly_KeyDown;
            featureEditGridView.CellMouseUp += DeleteRowFeature;
            lotSizeBoxInspection.KeyDown += checkEnterKeyPressedInspection;

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
                lotSizeBoxInspection.Clear();

                return false;
            }
        }

        private void checkEnterKeyPressedInspection(object sender, KeyEventArgs e)

        {
            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                DataTable featureTable;
                bool isValidOpKey = false;
                bool inspectionExists = false;
                
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
                            else if(lotSizeBoxInspection.Text != "")
                            {
                                //TODO: Need to get lot size inserted/updated to Inspection table
                                // Insert Lot Size to Inspection Table
                                model.InsertLotSizeToInspectionTable(Int32.Parse(lotSizeBoxInspection.Text), opkey);
                                //Create the parts in the positions table

                                //Get part list DataTable partList = model.GetPartsList(opkey);

                                //Bind the part list box BindListBox(partList);
                            }
                        }
                        else
                        {
                            //Message user to add features to this part num op num
                           // MessageBox.Show("Lead must add features to this Part and Operation number");

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

                lotSizeBoxInspection.Focus();
                
            }
        }

        #endregion


        #region Feature Tab Methods
        // FEATURE TAB METHODS

        //IP>Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange(" ", "Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "GDT", "Depth");
        }

        private static void SampleChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            //comboboxColumn.Items.AddRange("100%", "First Piece", "First & Last", "Sample Plan");
        }

        private static void ToolCategories(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("0-1 Mic", "Height Stand");
            
            /*
            string query = "SELECT Category FROM ATI_uniPoint_Live.dbo.PT_Equip WHERE Class = 'Inspection Tool' AND Category IS NOT NULL GROUP BY Category";
            comboboxColumn.Items.AddRange(query); */
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
            featureEditGridView.Columns["SampleID"].Visible = false;

            maxRows = featureTable.Rows.Count;


            //Creates extra columns in Feature Page
            DataGridViewTextBoxColumn BubbleColumn = new DataGridViewTextBoxColumn();
            {
                BubbleColumn.HeaderText = "Sketch Bubble (Optional)";
                featureEditGridView.Columns.Insert(0, BubbleColumn);
            }
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            {
                FeatureDropColumn.FlatStyle = FlatStyle.Flat;
                FeatureDropColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
                featureEditGridView.Columns.Insert(0, FeatureDropColumn);
                FeatureDropChoices(FeatureDropColumn);
                FeatureDropColumn.HeaderText = "Feature Type (Optional)";
            }

            SampleComboBind(); //Adds and binds the sample combo box column

            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            {
                SamplingColumn.ValueMember = "Sample";
                SamplingColumn.DisplayMember = "Sample";
                featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, SamplingColumn);
                SamplingColumn.DataSource = featureTable;
                SampleChoices(SamplingColumn);
                //SamplingColumn.DisplayMember = "Sample";
                //SamplingColumn.HeaderText = "Sample";
                SamplingColumn.FlatStyle = FlatStyle.Flat;
                SamplingColumn.CellTemplate.Style.BackColor = Color.FromArgb(50, 50, 50);
            }

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
                DeleteButtonColumn.FlatStyle = FlatStyle.Flat;
                DeleteButtonColumn.CellTemplate.Style.BackColor = Color.DarkRed;
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


        BindingSource sampleBindingSource = new BindingSource();
        private void SampleComboBind()
        {
            DataGridViewComboBoxColumn SamplingColumn = new DataGridViewComboBoxColumn();
            
           
            featureEditGridView.Columns.Insert(featureEditGridView.Columns.Count, SamplingColumn);

            sampleBindingSource.DataSource = model.GetSampleChoices();

            //Binding combo box to database table of sample choices
            SamplingColumn.DisplayMember = "SampleChoice";
            SamplingColumn.ValueMember = "SampleID";

            SamplingColumn.HeaderText = "Sample";
            SamplingColumn.Name = "Sample";
            SamplingColumn.DataSource = sampleBindingSource;
            
            
            //setting initial selected value to hidden SampleID column value for the current row
            for(int i=0; i<featureEditGridView.Rows.Count; i++)
            {
                featureEditGridView.Rows[i].Cells["Sample"].Value = featureEditGridView.Rows[i].Cells["SampleID"].Value;
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
        }

        private void checkEnterKeyPressedFeatures(object sender, KeyEventArgs e)
        {
            char currentKey = (char)e.KeyCode;
            bool nonNumber = char.IsWhiteSpace(currentKey);

            if (nonNumber)
                e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (partBoxFeature.ContainsFocus)
                {
                    opBoxFeature.Focus();
                }
                else if (opBoxFeature.ContainsFocus)
                {
                    featureEditGridView.Focus();
                }
                string partNumber = partBoxFeature.Text;
                string operationNum = opBoxFeature.Text;

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

        private void DeleteRowFeature(object sender, DataGridViewCellMouseEventArgs e)
        {
            var table = (DataGridView)sender;

            if (e.ColumnIndex != -1)
            {
                if (e.ColumnIndex == featureEditGridView.Columns[featureEditGridView.ColumnCount -1].Index)
                {
                    featureEditGridView.Rows.Remove(featureEditGridView.Rows[e.RowIndex]);
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

        private void inspectionEntryGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AdapterUpdateInspection();
        }

        #endregion

        #region Feature Handlers

        // FEATURE HANDLERS

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(featureEditGridView.Columns["Sample"] != null)
            {
                if (featureEditGridView.Columns["Sample"].Index == e.ColumnIndex)
                {
                    featureEditGridView.Rows[e.RowIndex].Cells["SampleID"].Value = featureEditGridView.Rows[e.RowIndex].Cells["Sample"].Value;
                }
            }
            
            // AdapterUpdate((BindingSource)table.DataSource);

        }

        private void addFeature_Click(object sender, EventArgs e)
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

        private void cancelChanges_Click(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to cancel all changes made to this set of features? " +
                "Any changes to this table will be reverted.";
            const string caption = "Cancel Changes";
            var result = MessageBox.Show(message, caption,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Rebind Database
                string partNumber = partBoxFeature.Text;
                string operationNum = opBoxFeature.Text;
                DataTable partList = model.GetFeaturesOnOpKey(partNumber, operationNum);
                DataBindTest(partList);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            const string message0 = "Are you sure you want to save all changes made to this set of features? " +
                "All changes will save to the database.";
            const string caption0 = "Save Changes";
            var result = MessageBox.Show(message0, caption0,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdapterUpdate();
                const string message1 = "All changes made to the table have been updated to the database";
                const string caption1 = "Table Saved";
                var result2 = MessageBox.Show(message1, caption1);
                //Send featureEditGridView table back to database.

            }

            else if (result == DialogResult.No)
            {
                const string message2 = "Any Changes to the table have not been updated to the database";
                const string caption2 = "Table Not Saved";
                var result2 = MessageBox.Show(message2, caption2);
            }

            
        }
        private void featureEditGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                featureEditGridView.Update();
            }
            catch
            {

            }

        }


        #endregion

    }

}

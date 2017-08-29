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

        private readonly string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";

        public FeatureCreationPresenter presenter;
        private FeatureCreationModelMock model;

        public event EventHandler AddFeatureClicked;
        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> DoneClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;

        public string PartNumber {
            get { return partNumberLabelInspection.Text; }
            set { partNumberLabelInspection.Text = value; }
        }

        public string JobNumber {
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

        public int LotSize
        {
            get { return Int32.Parse(lotLabelInspection.Text); }
            set { lotLabelInspection.Text = value.ToString(); }
        }

        public int PartsInspected
        {
            get { return Int32.Parse(partsInspectedLabel.Text); }
            set { partsInspectedLabel.Text = value.ToString(); }
        }

        public FeatureCreationTableMock()
        {
            InitializeComponent();
            featureEditGridView.CellMouseUp += CellMouseUp;
            //opKeyBoxFeature.KeyPress += OpKeyEnter;
            opKeyBoxFeature.KeyDown += txtType3_KeyDown;
            opKeyBoxInspection.KeyPress += checkEnterKeyPressedInspection;  
        }

        BindingSource bindingSourceListBox = new BindingSource();
        private void BindListBox(DataTable partsTable)
        {
            partsListBox.DataSource = null;
            bindingSourceListBox.DataSource = partsTable;
            partsListBox.DataSource = bindingSourceListBox;
            partsListBox.DisplayMember = "PartList";
            //partsListBox.ValueMember = "PartList";
        }

        BindingSource bindingSourceInspection = new BindingSource();
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
            
            if(inspectionEntryGridView.RowCount != 0)
            {
                inspectionEntryGridView.Rows[0].Cells["Measured Actual"].Selected = true;
            }
        }

        //IP>Checks to make sure click event only triggers on the Edit column And changes ReadOnly.
        private void CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var table = (DataGridView)sender;
            var button = (DataGridViewButtonCell)table.Rows[e.RowIndex].Cells["Edit Column"];

            if (e.ColumnIndex == featureEditGridView.Columns["Edit Column"].Index
                && (string)featureEditGridView.Rows[e.RowIndex].Cells["Edit Column"].Value == "Edit")
            {
                button.UseColumnTextForButtonValue = false;
                featureEditGridView.Rows[e.RowIndex].ReadOnly = false;
                featureEditGridView.Rows[e.RowIndex].Cells["Edit Column"].Value = "Done";
            }
            else if(e.ColumnIndex == featureEditGridView.Columns["Edit Column"].Index
                && (string)featureEditGridView.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                DoneClicked(featureEditGridView.Rows[e.RowIndex], EventArgs.Empty);
                featureEditGridView.Rows[e.RowIndex].ReadOnly = true;
                featureEditGridView.Rows[e.RowIndex].Cells["Edit Column"].Value = "Edit";
                AdapterUpdate();
            }
            else if(featureEditGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn
                    && (string)featureEditGridView.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                featureEditGridView.BeginEdit(true);
                ((ComboBox)featureEditGridView.EditingControl).DroppedDown = true;
            }
            else
            {
                featureEditGridView.BeginEdit(true);
            }
        }

        //IP>Test code to try combo box workability. Will be replaced with a .DataSource method.
        private static void FeatureDropChoices(DataGridViewComboBoxColumn comboboxColumn)
        {
            comboboxColumn.Items.AddRange("Diameter", "Fillet", "Chamfer", "Angle", "M.O.W.",
                "Surface Finish", "Linear", "Square", "GDT", "Depth");
        }

        public void ShowJobInformation(Job job)
        {
            throw new NotImplementedException();
        }

        public void ShowRelatedFeatures(IList<Feature> relatedFeaures)
        {
            throw new NotImplementedException();
        }

        BindingSource bindingSource;

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
            featureEditGridView.Columns["FeatureType"].Visible = false;
            
            maxRows = featureTable.Rows.Count;


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
            for (int j = 0; j <featureEditGridView.ColumnCount; j++)
            {
                featureEditGridView.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            
            //IP>Initializes and defines the feature type column.
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            FeatureDropColumn.HeaderText = "Feature Type";
            featureEditGridView.Columns.Insert(0, FeatureDropColumn);
            FeatureDropChoices(FeatureDropColumn);
            

        }

        private void AdapterUpdate()
        {
            //Must call EndEdit Method before trying to update database
            bindingSource.EndEdit();

            //Update database
            model.AdapterUpdate((DataTable)bindingSource.DataSource);
        }

        private void AdapterUpdateInspection()
        {
            //Call EndEdit method before updating database
            bindingSourceInspection.EndEdit();

            model.AdapterUpdateInspection((DataTable)bindingSourceInspection.DataSource);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            featureEditGridView.Rows[e.RowIndex].ReadOnly = false;
        }

        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            model = new FeatureCreationModelMock();
            presenter = new FeatureCreationPresenter(this, model); //Give a reference of the view and model to the presenter class
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

        /*
        //FOR FEATURES TAB!!!!!
        private void OpKeyEnter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ((char)13) || e.KeyChar == '\t')
            {
                e.Handled = true;
                //Might want to add validation for textbox text to be an integer
                DataTable featureTable = model.GetFeaturesOnOpKey(Int32.Parse(opKeyBoxFeature.Text));
                DataBindTest(featureTable);
                featurePageHeader.Text = featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value + " FEATURES";
            }
        }
        */

        private void txtType3_KeyDown(object sender, KeyEventArgs e)
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

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                int opkey;
                bool isValidOpKey = Int32.TryParse(opKeyBoxFeature.Text, out opkey);
                if (isValidOpKey)
                {
                    DataTable partList = model.GetFeaturesOnOpKey(opkey);
                    DataBindTest(partList);
                    featurePageHeader.Text = featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value + " FEATURES";
                }

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

        private void checkEnterKeyPressedInspection(object sender, KeyPressEventArgs e)
        {

            //Will work on an enter or tab key press
            if ((Keys)e.KeyChar == Keys.Enter || (Keys)e.KeyChar == Keys.Tab)
            {
                e.Handled = true;
                int opkey;
                bool isValidOpKey = Int32.TryParse(opKeyBoxInspection.Text, out opkey);
                if(isValidOpKey)
                {
                    DataTable partList = model.GetPartsList(opkey);
                    SetOpKeyInfo(opkey);
                    BindListBox(partList);
                    partNumberLabelInspection.Focus();
                }
                
            }

        }

        private void SetOpKeyInfo(int opkey)
        {
            DataTable info = new DataTable();
            
            info = model.GetInfoFromOpKeyEntry(opkey);

            if (info.Rows.Count > 0)
            {
                partNumberLabelInspection.Text = info.Rows[0]["Part_Number"].ToString();
                jobLabelInspection.Text = info.Rows[0]["Job_Number"].ToString();
                opLabelInspection.Text = info.Rows[0]["Operation_Number"].ToString();
            }
            else
            {
                partNumberLabelInspection.Text = null;
                jobLabelInspection.Text = null;
                opLabelInspection.Text = null;
                MessageBox.Show(opKeyBoxInspection.Text + " is and invalid please enter a valid Op Key", "Invalid OpKey");
                opKeyBoxInspection.Clear();
            }

        }

        private void AddTableRow(DataTable t)
        {
            if (featureEditGridView.DataSource == null)
            {
                return;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);
            featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Edit Column"].Value = "Done";
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // AdapterUpdate((BindingSource)table.DataSource);

        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Use (listbox)sender.SelectedIndex
            var listBox = (ListBox)sender;
            DataTable featureTable;

            if(listBox.Text.Contains("Part"))
            {
                int listBoxIndex = listBox.SelectedIndex;
                int pieceID = listBoxIndex + 1; //Due to 0 indexing
                featureTable = model.GetFeaturesOnPartIndex(pieceID, Int32.Parse(opKeyBoxInspection.Text));
                BindDataGridViewInspection(featureTable);
            }
            
        }

        private void nextPartButton_Click(object sender, EventArgs e)
        {
            //+1 to selectedindex because we need to check what index it is going in to first
            if(partsListBox.Items.Count > 0)
            {
                partsListBox.SelectedIndex = (partsListBox.SelectedIndex + 1 < partsListBox.Items.Count) ?
                partsListBox.SelectedIndex += 1 : partsListBox.SelectedIndex = 0;
            }

        }

        private void opKeyBoxFeature_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }
    }

}

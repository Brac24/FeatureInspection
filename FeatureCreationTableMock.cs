using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
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

        public FeatureCreationTableMock()
        {
            InitializeComponent();
            featureEditGridView1.CellMouseUp += CellMouseUp;
            opKeyBox1.KeyDown += new KeyEventHandler(OpKeyEnter);
        }

        //IP>Checks to make sure click event only triggers on the Edit column And changes ReadOnly.
        private void CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var table = (DataGridView)sender;
            var button = (DataGridViewButtonCell)table.Rows[e.RowIndex].Cells["Edit Column"];

            if (e.ColumnIndex == featureEditGridView1.Columns["Edit Column"].Index
                && (string)featureEditGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Edit")
            {
                button.UseColumnTextForButtonValue = false;
                featureEditGridView1.Rows[e.RowIndex].ReadOnly = false;
                featureEditGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Done";
            }
            else if (e.ColumnIndex == featureEditGridView1.Columns["Edit Column"].Index
                && (string)featureEditGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                DoneClicked(featureEditGridView1.Rows[e.RowIndex], EventArgs.Empty);
                featureEditGridView1.Rows[e.RowIndex].ReadOnly = true;
                featureEditGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Edit";
                AdapterUpdate();
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
            featureEditGridView1.Columns.Clear();

            bindingSource = new BindingSource();

            featureEditGridView1.DataSource = null;
            bindingSource.DataSource = featureTable;
            featureEditGridView1.DataSource = bindingSource;

            featureEditGridView1.Columns["Feature_Key"].Visible = false;
            featureEditGridView1.Columns["Part_Number_FK"].Visible = false;
            featureEditGridView1.Columns["Operation_Number_FK"].Visible = false;
            featureEditGridView1.Columns["Feature_Name"].Visible = false;
            featureEditGridView1.Columns["Active"].Visible = false;
            featureEditGridView1.Columns["Pieces"].Visible = false;
            featureEditGridView1.Columns["Plus_Tolerance"].HeaderText = "+";
            featureEditGridView1.Columns["Minus_Tolerance"].HeaderText = "-";
            maxRows = featureTable.Rows.Count;


            for (int i = 0; i < featureEditGridView1.Rows.Count; i++)
            {
                featureEditGridView1.Rows[i].ReadOnly = true;
            }

            //IP>Initializes and defines the edit button column.
            DataGridViewButtonColumn EditButtonColumn = new DataGridViewButtonColumn();
            EditButtonColumn.Name = "Edit Column";
            featureEditGridView1.Columns.Insert(featureEditGridView1.Columns.Count, EditButtonColumn);
            for (int i = 0; i < featureEditGridView1.Rows.Count; i++)
            {
                featureEditGridView1.Rows[i].Cells["Edit Column"].Value = "Edit";
            }

            //IP>Initializes and defines the feature type column.
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            FeatureDropColumn.HeaderText = "Feature Type";
            featureEditGridView1.Columns.Insert(0, FeatureDropColumn);
            FeatureDropChoices(FeatureDropColumn);
        }

        private void AdapterUpdate()
        {
            model.AdapterUpdate((DataTable)bindingSource.DataSource);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            featureEditGridView1.Rows[e.RowIndex].ReadOnly = false;
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
            featureEditGridView1.Rows[featureEditGridView1.Rows.Count - 1].Cells["Part_Number_FK"].Value = featureEditGridView1.Rows[0].Cells["Part_Number_FK"].Value;
            featureEditGridView1.Rows[featureEditGridView1.Rows.Count - 1].Cells["Operation_Number_FK"].Value = featureEditGridView1.Rows[0].Cells["Operation_Number_FK"].Value;

        }

        private void OpKeyEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && opKeyBox1.Text != "")
            {

                //Might want to add validation for textbox text to be an integer
                DataTable featureTable = model.GetFeaturesOnOpKey(Int32.Parse(opKeyBox1.Text));

                DataBindTest(featureTable);


            }

        }

        private void AddTableRow(DataTable t)
        {
            if (featureEditGridView1.DataSource == null)
            {
                return;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);
            featureEditGridView1.Rows[featureEditGridView1.Rows.Count - 1].Cells["Edit Column"].Value = "Done";
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // AdapterUpdate((BindingSource)table.DataSource);

        }
    }
}

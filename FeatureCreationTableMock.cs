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

        public event EventHandler AddFeatureClicked;
        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> DoneClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;

        public FeatureCreationTableMock()
        {
            InitializeComponent();
            dataGridView1.CellMouseUp += CellMouseUp;  
            textBox1.KeyDown += new KeyEventHandler(OpKeyEnter);
        }

        //IP>Checks to make sure click event only triggers on the Edit column And changes ReadOnly.
        private void CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var table = (DataGridView)sender;

            var button = (DataGridViewButtonCell)table.Rows[e.RowIndex].Cells["Edit Column"];

            if (e.ColumnIndex == dataGridView1.Columns["Edit Column"].Index
                && dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Edit")
            {
                EditClicked(sender, e);
                button.UseColumnTextForButtonValue = false;
                int edit = e.RowIndex;
                dataGridView1.Rows[edit].ReadOnly = false;
                dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Done";
            }
            else if (e.ColumnIndex == dataGridView1.Columns["Edit Column"].Index
                && dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                EditClicked(sender, e);
                int edit = e.RowIndex;
                dataGridView1.Rows[edit].ReadOnly = true;
                dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Edit";
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

        private void DataBind()
        {
            int maxRows;

                using (OdbcConnection conn = new OdbcConnection(connection_string))
                using (OdbcCommand com = conn.CreateCommand())
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
                {

                    string query = "SELECT Feature_Key, Nominal, Plus_Tolerance as '+', Minus_Tolerance as '-', Places FROM ATI_FeatureInspection.dbo.Features";

                    com.CommandText = query;
                    DataTable t = new DataTable();
                    adapter.Fill(t);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = t;
                    dataGridView1.Columns["Feature_Key"].Visible = false;
                    maxRows = t.Rows.Count;

            }
        }

        //IP> Data Bind to OP Key entered in textBox1
        private void NewDataBind()
        {
            int maxRows;
            dataGridView1.Columns.Clear();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT * FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key =  " + textBox1.Text + ") AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + textBox1.Text + ");";

                com.CommandText = query;
                DataTable t = new DataTable();
                adapter.Fill(t);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = t;
                dataGridView1.Columns["Feature_Key"].Visible = false;
                maxRows = t.Rows.Count;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].ReadOnly = true;
                }
            }

            //IP>Initializes and defines the edit button column.
            DataGridViewButtonColumn EditButtonColumn = new DataGridViewButtonColumn();
            EditButtonColumn.UseColumnTextForButtonValue = true;
            EditButtonColumn.Name = "Edit Column";
            EditButtonColumn.Text = "Edit";
            dataGridView1.Columns.Insert(dataGridView1.Columns.Count, EditButtonColumn);

            //IP>Initializes and defines the feature type column.
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            FeatureDropColumn.HeaderText = "Feature Type";
            dataGridView1.Columns.Insert(0, FeatureDropColumn);
            FeatureDropChoices(FeatureDropColumn);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ReadOnly = false;
        }

        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            presenter = new FeatureCreationPresenter(this, new FeatureCreationModelMock()); //Give a reference of the view and model to the presenter class
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable data = (DataTable)(dataGridView1.DataSource);
            AddTableRow(data);
        }

        private void OpKeyEnter (object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBox1.Text != "")
            {
                NewDataBind();

                //var table = (DataGridView)sender;
                //var button = (DataGridViewButtonCell)table.Rows[dataGridView1.Rows.Count - 1].Cells["Edit Column"];
                //button.UseColumnTextForButtonValue = false;
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Edit Column"].Value = "Done";
            }

        }

        private void AddTableRow(DataTable t)
        {
            if (dataGridView1.DataSource == null)
            {
                return;
            }
            DataRow newRow = t.NewRow();
            t.Rows.Add(newRow);
        }
    }
}

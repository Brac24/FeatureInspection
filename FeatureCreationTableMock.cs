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

        private FeatureCreationPresenter presenter;

        public FeatureCreationTableMock()
        {
            //Removed for testing first might return when testing is finished
            //presenter = new FeatureCreationPresenter(this, new FeatureCreationModelMock()); //Give a reference of the view and model to the presenter class
            InitializeComponent();
            DataBind();

            //IP>Initializes and defines the edit button column.
            DataGridViewButtonColumn EditButtonColumn = new DataGridViewButtonColumn();
            EditButtonColumn.Name = "Edit Column";
            dataGridView1.Columns.Insert(dataGridView1.Columns.Count, EditButtonColumn);
            dataGridView1.CellContentClick += editRow;

            //IP>Initializes and defines the feature type column.
            DataGridViewComboBoxColumn FeatureDropColumn = new DataGridViewComboBoxColumn();
            FeatureDropColumn.HeaderText = "Feature Type";
            dataGridView1.Columns.Insert(0, FeatureDropColumn);
            FeatureDropChoices(FeatureDropColumn);
        }
        
        //IP>Checks to make sure click event only triggers on the Edit column And changes ReadOnly.
        private void editRow(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Edit Column"].Index 
                && dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Edit"
                || dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == null) //Remove once column is populated with "Edit" values.
            {
                //EditClicked(sender, e);
                int edit = e.RowIndex;
                dataGridView1.Rows[edit].ReadOnly = false;
                dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Done";
            }
            else if (e.ColumnIndex == dataGridView1.Columns["Edit Column"].Index
                && dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                //EditClicked(sender, e);
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

        public event EventHandler AddFeatureClicked;
        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler EnterClicked;
        public event EventHandler LotInspectionReadyClicked;

        public void ShowJobInformation(Job job)
        {
            throw new NotImplementedException();
        }

        public void ShowRelatedFeatures(IList<Feature> relatedFeaures)
        {
            throw new NotImplementedException();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DataBind()
        {
            int maxRows;

            //try
            //{
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                using (OdbcCommand com = conn.CreateCommand())
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
                {

                    string query = "SELECT Nominal, Plus_Tolerance as '+', Minus_Tolerance as '-', Places FROM ATI_FeatureInspection.dbo.Features";

                    com.CommandText = query;
                    DataTable t = new DataTable();
                    adapter.Fill(t);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = t;
                    maxRows = t.Rows.Count;

                }
            //}
            //catch (Exception e)
            /*{
                MessageBox.Show("You're a foo", e.Message);
            }*/
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ReadOnly = true;
        }

        private void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {

        }
    }
}

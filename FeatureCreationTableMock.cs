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

        public FeatureCreationTableMock()
        {
            InitializeComponent();
            DataBind();
            DataGridViewButtonColumn FeatureButtonColumn = new DataGridViewButtonColumn();
            FeatureButtonColumn.UseColumnTextForButtonValue = true;
            FeatureButtonColumn.Text = "Edit";
            dataGridView1.Columns.Insert(dataGridView1.Columns.Count, FeatureButtonColumn);
            dataGridView1.CellClick += editRow;
        }

        private void editRow(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
        }

        public FeatureCreationPresenter Presenter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler AddFeatureClicked;
        public event EventHandler EditClicked;
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

                    string query = "SELECT Feature_Name as 'Feature Name', Nominal, Plus_Tolerance as '+', Minus_Tolerance as '-', Places FROM ATI_FeatureInspection.dbo.Features";

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
                MessageBox.Show("Youre a foo", e.Message);
            }*/
        }


        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView table = sender as DataGridView;

            
        }
    }
}

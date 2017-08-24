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
                && (string)dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Edit")
            {
                //EditClicked(sender, e);
                button.UseColumnTextForButtonValue = false;
                int edit = e.RowIndex;
                dataGridView1.Rows[edit].ReadOnly = false;
                dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Done";
            }
            else if (e.ColumnIndex == dataGridView1.Columns["Edit Column"].Index
                && (string)dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value == "Done")
            {
                DoneClicked(dataGridView1.Rows[e.RowIndex], EventArgs.Empty);
                int edit = e.RowIndex;
                dataGridView1.Rows[edit].ReadOnly = true;
                dataGridView1.Rows[e.RowIndex].Cells["Edit Column"].Value = "Edit";
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


        OdbcDataAdapter dataAdapter;
        DataTable table;
        BindingSource bindingSource;

        private void DataBindTest(DataTable featureTable)
        {

            int maxRows;
            dataGridView1.Columns.Clear();

            bindingSource = new BindingSource();

            dataGridView1.DataSource = null;
            bindingSource.DataSource = featureTable;
            dataGridView1.DataSource = bindingSource;

            dataGridView1.Columns["Feature_Key"].Visible = false;
            dataGridView1.Columns["Part_Number_FK"].Visible = false;
            dataGridView1.Columns["Operation_Number_FK"].Visible = false;
            maxRows = featureTable.Rows.Count;


            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].ReadOnly = true;
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


        private void AdapterUpdate()
        {
            DataTable dt = new DataTable();
            DataTable changedTable = new DataTable();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (dataAdapter = new OdbcDataAdapter(com))
            {
                string update = "UPDATE ATI_FeatureInspection.dbo.Features SET Nominal = ?, Plus_Tolerance = ?, Minus_Tolerance = ?, " +
                                "Feature_Name = ?, Places = ?, Active = ?, Pieces = ? " +
                                "WHERE Feature_Key = ?;";

                string insert = "INSERT INTO ATI_FeatureInspection.dbo.Features (Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Active, Pieces, Part_Number_FK, Operation_Number_FK)" +
                            "VALUES(?,?,?,?,?,?,?,?,?); ";

                string delete = "DELETE FROM ATI_FeatureInspection.dbo.Features WHERE Feature_Key = ?";

                /*****UPDATE COMMAND*****/

                dataAdapter.UpdateCommand = new OdbcCommand(update, conn);

                dataAdapter.UpdateCommand.Parameters.Add("@Nominal", OdbcType.Decimal, 3, "Nominal");
                dataAdapter.UpdateCommand.Parameters.Add("@Plus_Tolerance", OdbcType.Decimal, 3, "Plus_Tolerance");
                dataAdapter.UpdateCommand.Parameters.Add("@Minus_Tolerance", OdbcType.Decimal, 3, "Minus_Tolerance");
                dataAdapter.UpdateCommand.Parameters.Add("@Feature_Name", OdbcType.NVarChar, 50, "Feature_Name");
                dataAdapter.UpdateCommand.Parameters.Add("@Places", OdbcType.Int, 1, "Places");
                dataAdapter.UpdateCommand.Parameters.Add("@Active", OdbcType.NChar, 10, "Active");
                dataAdapter.UpdateCommand.Parameters.Add("@Pieces", OdbcType.Int, 1, "Pieces");
                dataAdapter.UpdateCommand.Parameters.Add("@Feature_Key", OdbcType.Int, 5, "Feature_Key");


                /****INSERT COMMAND*****/

                dataAdapter.InsertCommand = new OdbcCommand(insert, conn);

                dataAdapter.InsertCommand.Parameters.Add("@Nominal", OdbcType.Decimal, 3, "Nominal");
                dataAdapter.InsertCommand.Parameters.Add("@Plus_Tolerance", OdbcType.Decimal, 3, "Plus_Tolerance");
                dataAdapter.InsertCommand.Parameters.Add("@Minus_Tolerance", OdbcType.Decimal, 3, "Minus_Tolerance");
                dataAdapter.InsertCommand.Parameters.Add("@Feature_Name", OdbcType.NVarChar, 50, "Feature_Name");
                dataAdapter.InsertCommand.Parameters.Add("@Places", OdbcType.Int, 1, "Places");
                dataAdapter.InsertCommand.Parameters.Add("@Active", OdbcType.NChar, 10, "Active");
                dataAdapter.InsertCommand.Parameters.Add("@Pieces", OdbcType.Int, 1, "Pieces");
                dataAdapter.InsertCommand.Parameters.Add("@Part_Number_FK", OdbcType.NVarChar, 50, "Part_Number_FK");
                dataAdapter.InsertCommand.Parameters.Add("@Operation_Number_FK", OdbcType.NVarChar, 50, "Operation_NUmber_FK");

                /******DELETE COMMAND*****/

                dataAdapter.DeleteCommand = new OdbcCommand(delete, conn);

                dataAdapter.DeleteCommand.Parameters.Add("@Feature_Key", OdbcType.Int, 5, "Feature_Key");

                //End Command Initialization

                dt = (DataTable)bindingSource.DataSource;

                changedTable = dt.GetChanges();

                if (changedTable != null)
                {
                    int rowsInChangedTable = changedTable.Rows.Count;
                }

                dataAdapter.Update(dt);


            }
        }




        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ReadOnly = false;
        }

        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            model = new FeatureCreationModelMock();
            presenter = new FeatureCreationPresenter(this, model); //Give a reference of the view and model to the presenter class
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable data = (DataTable)(bindingSource.DataSource);
            AddTableRow(data);

            //Set the last row Part_Number_FK and Operation_Number_FK to the same value as in the first row
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Part_Number_FK"].Value = dataGridView1.Rows[0].Cells["Part_Number_FK"].Value;
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Operation_Number_FK"].Value = dataGridView1.Rows[0].Cells["Operation_Number_FK"].Value;

        }

        private void OpKeyEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBox1.Text != "")
            {
                //Might want to add validation for textbox text to be an integer
                DataTable featureTable = model.GetFeaturesOnOpKey(Int32.Parse(textBox1.Text));

                DataBindTest(featureTable);

                try
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Edit Column"].Value = "Done";
                }
                catch (Exception exception)
                {
                    Console.WriteLine("OpKey did not return any results");
                }
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

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // AdapterUpdate((BindingSource)table.DataSource);

        }
    }
}

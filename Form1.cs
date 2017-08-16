using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;

namespace Tester
{
    public partial class Form1 : Form
    {
        int counter = 1;
        string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();
                for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
                {
                    string query = "INSERT INTO [ATI_Workflow].[dbo].[Tester_deletelater]" +
                    "\t([Customer]\n" +
                    "\t,[PN]\n" +
                    "\t,[Job]\n" +
                    "\t,[OpKey])\n" +
                    "\tVALUES\n" +
                    "\t      ('" + dataGridView1.Rows[i].Cells[0].Value.ToString() + "'\n" +
                    "\t      ,'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "'\n" +
                    "\t      , '" + dataGridView1.Rows[i].Cells[2].Value.ToString() + "'\n" +
                    "\t      ,'" + dataGridView1.Rows[i].Cells[3].Value.ToString() + "')\n";
                    OdbcCommand conn_command = new OdbcCommand(query, conn);
                    int rows_affected = conn_command.ExecuteNonQuery();

                    counter++;
                }
            }
        }

        private void mainSubmit_button_Click(object sender, EventArgs e)
        {
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string query = "UPDATE [ATI_Workflow].[dbo].[Tester_deletelater]" +
                                "\t SET[Feature] = '"+Feature_TextBox.Text+"'\n" +
                                "\t   ,[NominalValue] = '"+Nominal_TextBox.Text+"'\n" +
                                "\t   ,[PlusTolerance] = '"+PlusTol_TextBox.Text+"'\n" +
                                "\t   ,[MinusTolerance] = '"+MinusTol_TextBox.Text+"'\n" +
                                "\t   ,[PlaceNumber] = '"+Place_TextBox.Text+" '\n" +
                                "\t WHERE OpKey = '"+OpKey_TextBox.Text+"'\n";

                OdbcCommand command = new OdbcCommand(query, conn);
                int rows_affected = command.ExecuteNonQuery();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public class FeatureCreationModelMock : IFeaturesDataSource
    {
        private readonly string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";

        public FeatureCreationModelMock()
        {

        }


        // FEATURE MODEL

        public DataTable AdapterUpdate(DataTable dt)
        {

            DataTable changedTable = new DataTable();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                string update = "UPDATE ATI_FeatureInspection.dbo.Features SET Nominal = ?, Plus_Tolerance = ?, Minus_Tolerance = ?, " +
                                "Feature_Name = ?, Places = ?, Active = ?, Pieces = ?, Sample = ? " +
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
                dataAdapter.UpdateCommand.Parameters.Add("@Sample", OdbcType.Decimal, 1, "Sample");
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



                changedTable = dt.GetChanges();

                int rowsInChangedTable;

                if (changedTable != null)
                {
                    rowsInChangedTable = changedTable.Rows.Count;

                }

                dataAdapter.Update(dt);

            }
            return changedTable;
        }

        public DataTable GetFeaturesOnOpKey(string partNumber, string operationNum)
        {
            DataTable t;
            OdbcDataAdapter dataAdapter;
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT * FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = '" + partNumber + "' AND Operation_Number_FK = '" + operationNum + "';";

                dataAdapter = adapter;


                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;

        }

        public DataTable GetFeaturesOnOpKey(int opKey)
        {
            DataTable t;
            OdbcDataAdapter dataAdapter;
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT * FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key =  " + opKey + ") AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opKey + ");";

                dataAdapter = adapter;


                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;

        }


        // INSPECTION MODEL

        public DataTable AdapterUpdateInspection(DataTable dt)
        {
            string update = "UPDATE ATI_FeatureInspection.dbo.Position SET Measured_Value = ? " +
                            "WHERE Position_Key = ?;";

            DataTable changedTable = new DataTable();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                dataAdapter.UpdateCommand = new OdbcCommand(update, conn);

                dataAdapter.UpdateCommand.Parameters.Add("@Measured_Value", OdbcType.Decimal, 3, "Measured Actual");
                dataAdapter.UpdateCommand.Parameters.Add("@Position_Key", OdbcType.Int, 3, "Position_Key");
                changedTable = dt.GetChanges();

                int rowsInChangedTable;

                if (changedTable != null)
                {
                    rowsInChangedTable = changedTable.Rows.Count;

                }

                dataAdapter.Update(dt);

            }
            return changedTable;


        }

        public DataTable GetPartsList(int opKey)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT  'Part ' + CAST(Piece_ID as varchar(10)) AS PartList FROM ATI_FeatureInspection.dbo.Position" +
                               " WHERE Inspection_Key_FK = (SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection" +
                               " WHERE Op_Key = " + opKey + ") GROUP BY Piece_ID ORDER BY Piece_ID ASC;";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;
        }

        public DataTable GetFeaturesOnPartIndex(int partIndex, int opKey)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT CAST(Features.Nominal AS varchar(50)) + ' +' + CAST(Features.Plus_Tolerance AS varchar(50)) + ' -' + CAST(Features.Minus_Tolerance AS varchar(50)) AS Feature, Position.Inspection_Key_FK, Features.Feature_Key,Position.Position_Key, Measured_Value AS 'Measured Actual', Position.InspectionTool FROM ATI_FeatureInspection.dbo.Position " +
                                " LEFT JOIN ATI_FeatureInspection.dbo.Features ON Position.Feature_Key = Features.Feature_Key" +
                                " WHERE Inspection_Key_FK = (SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection" +
                                " WHERE Op_Key = " + opKey + ") AND Piece_ID = " + partIndex + ";";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;
        }

        public DataTable GetInfoFromOpKeyEntry(int opkey)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                conn.Open();

                string query = "SELECT Job.Part_Number AS Part_Number, Job_Operation.Job AS Job_Number, Job_Operation.Operation_Service AS Operation_Number\n" +
                               "FROM PRODUCTION.dbo.Job\n" +
                               "INNER JOIN PRODUCTION.dbo.Job_Operation\n" +
                               "ON Job.Job = Job_Operation.Job\n" +
                               "WHERE Job_Operation.Job_Operation = '" + opkey + "';";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

                return t;

            }
        }

        public bool GetInspectionExistsOnOpKey(int opkey)
        {
            bool inspectionExists = false;
            string query = "SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection " +
                           "WHERE Op_Key = " + opkey;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();


                OdbcCommand com2 = new OdbcCommand(query, conn);
                OdbcDataReader reader2 = com2.ExecuteReader();

                //Check that query returned data
                if (reader2.Read())
                {
                    inspectionExists = true;
                }
            }

            return inspectionExists;
        }

    }
}

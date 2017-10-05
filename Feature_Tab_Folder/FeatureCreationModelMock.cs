using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
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
                                "Feature_Name = ?, Places = ?, Active = ?, Pieces = ?, SampleID = ?, Sketch_Bubble = ?, FeatureType = ? " +
                                "WHERE Feature_Key = ?;";


                string insert = "INSERT INTO ATI_FeatureInspection.dbo.Features (Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Active, Pieces, Part_Number_FK, Operation_Number_FK, SampleID, Sketch_Bubble, FeatureType)" +
                            " VALUES(?,?,?,?,?,?,?,?,?,?,?,?); ";

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
                dataAdapter.UpdateCommand.Parameters.Add("@SampleID", OdbcType.Int, 1, "SampleID");
                dataAdapter.UpdateCommand.Parameters.Add("@Sketch_Bubble", OdbcType.NVarChar, 50, "Sketch_Bubble");
                dataAdapter.UpdateCommand.Parameters.Add("@FeatureType", OdbcType.NVarChar, 50, "FeatureType");

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
                dataAdapter.InsertCommand.Parameters.Add("@Operation_Number_FK", OdbcType.NVarChar, 50, "Operation_Number_FK");
                dataAdapter.InsertCommand.Parameters.Add("@SampleID", OdbcType.Int, 1, "SampleID");
                dataAdapter.InsertCommand.Parameters.Add("@Sketch_Bubble", OdbcType.NVarChar, 50, "Sketch_Bubble");
                dataAdapter.InsertCommand.Parameters.Add("@FeatureType", OdbcType.NVarChar, 50, "FeatureType");

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

        public bool PartNumberExists(string partNumber)
        {
            string query = "SELECT Part_Number FROM PRODUCTION.dbo.Job " +
                           "WHERE Part_Number = '" + partNumber + "' GROUP BY Part_Number";

            bool partExists = false;

            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(query, connection);

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    //Means part number exists
                    partExists = true;
                }


            }

            return partExists;
        }

        public bool OpExists(string op, string partNumber)
        {
            bool opExists = false;

            string query = "SELECT Operation_Service, Part_Number FROM PRODUCTION.dbo.Job_Operation " +
                           "INNER JOIN PRODUCTION.dbo.Job ON Job_Operation.Job = Job.Job " +
                           "WHERE Operation_Service = '" + op + "' AND Part_Number = '" + partNumber + "' GROUP BY Operation_Service, Part_Number";

            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(query, connection);

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    opExists = true;
                }
            }

            return opExists;
        }

        public DataTable GetFeaturesOnOpKey(string partNumber, string operationNum)
        {

            DataTable t;
            DataTable sampleChoices = new DataTable();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT * FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = '" + partNumber + "' AND Operation_Number_FK = '" + operationNum + "';";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;

        }

        //TODO: Put opkey and feature key paramters inputs for this method
        //Feature key is located in the inspectionEntryGridView it is one of the hidden columns
        public DataTable GetChartData(int opKey, int featureKey)
        {
            DataTable features = new DataTable();
            string getFeatures = "SELECT Measured_Value, Piece_ID, Features.Upper_Tolerance, Features.Lower_Tolerance FROM ATI_FeatureInspection.dbo.Position LEFT JOIN ATI_FeatureInspection.dbo.Features ON Features.Feature_Key = Position.Feature_Key WHERE Inspection_Key_FK = (SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Op_Key = " + opKey + ") AND Position.Feature_Key = " + featureKey + "; ";

            using (OdbcConnection connection = new OdbcConnection(connection_string))
            using (OdbcCommand command = connection.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(command))
            {
                command.CommandText = getFeatures;

                dataAdapter.Fill(features);
            }

            return features;
        }

        public DataTable GetSampleChoices()
        {
            DataTable sampleChoices = new DataTable();
            string getSampleChoices = "SELECT * FROM ATI_FeatureInspection.dbo.SampleChoices;";

            using (OdbcConnection connection = new OdbcConnection(connection_string))
            using (OdbcCommand command = connection.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(command))
            {
                command.CommandText = getSampleChoices;

                dataAdapter.Fill(sampleChoices);

            }
            return sampleChoices;
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
        //TODO: Move to new model file.

        public DataTable AdapterUpdateInspection(DataTable dt)
        {
            string update = "UPDATE ATI_FeatureInspection.dbo.Position SET Measured_Value = ?, Old_Value = ?, Oldest_Value = ? " +
                            "WHERE Position_Key = ?;";

            DataTable changedTable = new DataTable();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                dataAdapter.UpdateCommand = new OdbcCommand(update, conn);

                dataAdapter.UpdateCommand.Parameters.Add("@Measured_Value", OdbcType.Decimal, 3, "Measured Actual");
                dataAdapter.UpdateCommand.Parameters.Add("@Old_Value", OdbcType.Decimal, 3, "Old Value");
                dataAdapter.UpdateCommand.Parameters.Add("@Oldest_Value", OdbcType.Decimal, 3, "Oldest Value");
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

        public DataTable GetFeatureList(int opKey)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {

                string query = "SELECT Nominal, Feature_Key FROM ATI_FeatureInspection.dbo.Features" +
                               " JOIN ATI_FeatureInspection.dbo.Operation ON Part_Number_FK = Part_Number AND Operation_Number_FK = Operation_Number" +
                               " WHERE Op_Key = " + opKey + ";";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }
            return t;
        }

        public DataTable GetFeatureKey(int opKey, int feature)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {

                string query = "SELECT Feature_Key FROM ATI_FeatureInspection.dbo.Features" +
                               " JOIN ATI_FeatureInspection.dbo.Operation ON Part_Number_FK = Part_Number AND Operation_Number_FK = Operation_Number" +
                               " WHERE Op_Key = " + opKey + " AND Nominal = " + feature + ";";

                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }
            return t;
        }

        public string GetLotSize(int opkey)
        {
            int lotSize = 0;
            string query = "SELECT Lot_Size FROM ATI_FeatureInspection.dbo.Inspection WHERE Op_Key = " + opkey + ";";

            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(query, connection);

                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        lotSize = reader.GetInt32(reader.GetOrdinal("Lot_Size"));
                    }
                    catch
                    {
                        MessageBox.Show("Could not get Lot Size from query");
                    }

                }

            }

            return lotSize.ToString();
        }

        public DataTable GetFeaturesOnPartIndex(int partIndex, int opKey)
        {
            DataTable t;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter dataAdapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT Features.Sketch_Bubble AS 'Sketch Bubble', CAST(Features.FeatureType AS varchar(50)) + ' ' + CAST(Features.Nominal AS varchar(50)) + ' +' + CAST(Features.Plus_Tolerance AS varchar(50)) + ' -' + CAST(Features.Minus_Tolerance AS varchar(50)) AS Feature, "
                    + "Position.Inspection_Key_FK, Features.Feature_Key, Position.Position_Key, Measured_Value AS 'Measured Actual', Position.Old_Value AS 'Old Value', Position.Oldest_Value AS 'Oldest Value', Position.InspectionTool FROM ATI_FeatureInspection.dbo.Position " +
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

        public void InsertLotSizeToInspectionTable(int lotSize, int opkey)
        {
            string insertQuery = "UPDATE ATI_FeatureInspection.dbo.Inspection SET Lot_Size =  " + lotSize + " WHERE Op_Key = " + opkey + ";";
            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(insertQuery, connection);

                command.ExecuteNonQuery();
            }
        }

      
        public void InsertPartsToPositionTable(int opkey, int lotSize)
        {
            string insert = "DECLARE @Count INT, @RowFeature INT,  @MaxParts INT, @TotalNewFeatures INT " +
            "DECLARE @InspectionKey INT, @FeatureKey INT" +
            " SET @InspectionKey = (SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Op_Key = " + opkey + "); " +
            " SET @TotalNewFeatures = (SELECT COUNT(DISTINCT Features.Feature_Key) FROM ATI_FeatureInspection.dbo.Features LEFT JOIN ATI_FeatureInspection.dbo.Position ON Features.Feature_Key = Position.Feature_Key WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + ") AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + ") AND Position.Feature_Key IS NULL);" +
            " SET @Count = 0; " +
            " SET @RowFeature = 0 " +
            " SET @MaxParts = " + lotSize +
            " WHILE(@RowFeature < @TotalNewFeatures) " +
            " BEGIN " +
            " SET @RowFeature = @RowFeature + 1 " +
            " SET @FeatureKey = (SELECT Feature_Key FROM (SELECT ROW_NUMBER() OVER(ORDER BY Features.Feature_Key ASC) AS RowNumber, Features.Feature_Key  FROM ATI_FeatureInspection.dbo.Features LEFT JOIN ATI_FeatureInspection.dbo.Position ON Features.Feature_Key = Position.Feature_Key WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + ") AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + ") AND Position.Feature_Key IS NULL ) AS foo  " +
            " WHERE RowNumber = @RowFeature) " +
            " WHILE(@Count < @MaxParts) " +
            " BEGIN " +
            " INSERT INTO ATI_FeatureInspection.dbo.Position(Inspection_Key_FK, Piece_ID, Feature_Key) " +
            " VALUES(@InspectionKey, @Count + 1, @FeatureKey) " +
            " SET @Count = @Count + 1 " +
            " END " +
            " SET @Count = 0 " +
            " END";



            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(insert, connection);

                command.ExecuteNonQuery();
            }
        }

        public void CreateInspectionInInspectionTable(int opkey)
        {

            string insert = "INSERT INTO ATI_FeatureInspection.dbo.Inspection(Op_Key)" +
                            "VALUES(" + opkey + ");";
            using (OdbcConnection connection = new OdbcConnection(connection_string))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(insert, connection);

                command.ExecuteNonQuery();
            }
        }
    }
}
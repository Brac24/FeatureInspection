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
using BrightIdeasSoftware;

namespace Feature_Inspection
{
    public partial class UserInputForm : Form
    {
        string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";
        string finishAddingButton = "Finish Adding";
        string newFeatureText;
        bool addingNewFeature = false;
        public static bool submitClicked = false;
        int currentRow;
        bool currentInspection;


        AlternateUI op = new AlternateUI();
       // List<FeatureInfo> fInfo = new List<FeatureInfo>();
        public static List<int> selectedFeatureRows = new List<int>();


        List<int> featureRowsChanged = new List<int>();

        public UserInputForm()
        {
            InitializeComponent();

        }
        private void UserInputForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
            saveFeaturesButton.Hide(); //Hide the save button

            newFeatureText = newFeatureButton.Text;

            OpService_TextBox.Text = op.getOpService();
            partNoTextBox.Text = op.getPartNo();

            currentInspection = op.getFeaturesExistInPosition(); //Sets whether there is a current inspection for this opkey

            //If there is another inspection that means there are features 
            //That relate to the opkey at hand we need to load those
            if (currentInspection)
            {
                loadCurrentInspectionFeatures();
            }
            else
                formLoadQuery();

            submitClicked = false;
            dataGridView1.AutoResizeColumns();
            disableFeaturesAlreadyBeingMeasured();
        }


        private void validateSelectedRows()
        {
            int maxRows = dataGridView1.RowCount;
            int maxCols = dataGridView1.ColumnCount - 1;
            int rowIndex = 0;
            int colIndex = 0;
            bool emptyCell = false;
            bool selected;
            int inheritedFeatures = 0;

            List<int> newIndexes = new List<int>();
            List<int> oldRows = new List<int>();
            List<int> selectedIndexes = new List<int>();

            bool featureNumEmpty = false;

            selectedFeatureRows.Clear(); //Clear anything in there before

            string message = "Please fill in all values before submitting the features";

            while (rowIndex < maxRows)
            {
                selected = (bool)dataGridView1.Rows[rowIndex].Cells[0].Value; //Check if current row was checked

                //If row has a check
                if (selected)
                {
                    //Checks to see if there is an empty column in the selected row
                    for (colIndex = 1; colIndex <= maxCols; colIndex++)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[colIndex].Value == null)
                        {
                            //If there is no feature number because new feature
                            if (colIndex == 7)
                            {
                                featureNumEmpty = true;
                            }
                            //Empty cell besides feature number. NOT GOOD!!
                            else
                            {
                                emptyCell = true;
                                colIndex = maxCols; //exit loop if empty cell in selected feature
                            }
                        }
                    }
                    if (featureNumEmpty && colIndex >= maxCols)
                    {
                        //Features with no Feature No. Column value means they are newly created features
                        //Therefore grab those indeces where the new features have been created
                        if (dataGridView1.Rows[rowIndex].Cells[maxCols].Value == null)
                        {
                            newIndexes.Add(rowIndex); //Any newly added features
                            selectedIndexes.Add(rowIndex);

                        }

                    }
                    else
                    {
                        selectedIndexes.Add(rowIndex); //Indexes of related features from other inspections being used for this new inspection    
                        oldRows.Add(rowIndex);
                    }

                }

                rowIndex++;
            }

            if (selectedIndexes.Count == 0)
            {
                MessageBox.Show("Please Select Features to add or Exit out");
                submitClicked = false;
            }
            //If there are no empty cells go ahead and add the new features to the 
            //Database as well as add all the selected rows to a List of DataGridViewRows
            else if (!emptyCell && colIndex >= maxCols)
            {
                foreach (int value in selectedIndexes)
                {
                    //If the row is not a newly created row then insert
                    if (!newIndexes.Contains(value))
                    {
                        insertFeaturesToDatabase(value, false); // Reinsert features that inherit from a previously created feature from another inspection. false --> Means the feature is an existing feature but now we want to use it again but for a different inspection
                    }
                    else
                        insertFeaturesToDatabase(value, true); //Will add new features to database. true --> Means this is a completely new feature so it is not a feature from any past inspections

                }


                //Update the form with the new features so the Feature No. will be present
                if (newIndexes.Count > 0 || selectedIndexes.Count > 0)
                {
                    updateFeaturesInGridView(newIndexes, oldRows);//Refresh table with the new features
                }

                foreach (int value in selectedIndexes)
                {
                    insertToPositionsTable(Int32.Parse(dataGridView1.Rows[value].Cells[5].Value.ToString()), Int32.Parse(dataGridView1.Rows[value].Cells[6].Value.ToString()), Int32.Parse(dataGridView1.Rows[value].Cells[7].Value.ToString()));
                }
            }
            else
            {
                MessageBox.Show(message);
                submitClicked = false;
            }


            newIndexes.Clear();
            selectedIndexes.Clear();


        }

        private List<int> listOfFeaturesThatExistForCurrentInspection()
        {
            List<int> currentFeatures = new List<int>();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string query = "SELECT DISTINCT Feature_Key FROM ATI_FeatureInspection.dbo.Position " +
                               "WHERE Inspection_Key_FK = " + op.getInspectionKey() + "; ";

                OdbcCommand comm = new OdbcCommand(query, conn);
                OdbcDataReader reader = comm.ExecuteReader();

                while (reader.Read()) 
                {
                    currentFeatures.Add(reader.GetInt32(reader.GetOrdinal("Feature_Key")));
                }
            }

            return currentFeatures;
        }

        /// <summary>
        /// This function should disable any features from being edited and reinserted to the database
        /// This should simply freeze those features which can't be edited anymore
        /// </summary>
        private void disableFeaturesAlreadyBeingMeasured()
        {
            List<int> currentFeaturesBeingMeasured = listOfFeaturesThatExistForCurrentInspection();

            foreach (int value in currentFeaturesBeingMeasured)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if ((int)dataGridView1.Rows[i].Cells[7].Value == value)
                    {
                        dataGridView1.Rows[i].ReadOnly = true; //Prevent features already being measured from being edited
                        dataGridView1.Rows[i].Cells[0].Value = false; //Uncheck these features to keep them from being added again to the same inspection
                    }
                }
            }
        }

        private void updateFeaturesInGridView(List<int> newRows, List<int> oldRows)
        {
            dataGridView1.Refresh();
            int col = 1;
            string query;
            int inheritedFeature;

            List<int> allRows = new List<int>();

            for(int i=0; i<dataGridView1.RowCount; i++)
            {
                allRows.Add(i);
            }

            foreach (int index in oldRows)
            {
                inheritedFeature = (int)dataGridView1.Rows[index].Cells[7].Value;

                query = "SELECT Feature_Key, Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces FROM ATI_FeatureInspection.dbo.Features\n" +
                    "WHERE Feature_Key = " + inheritedFeature + ";";

                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    OdbcCommand comm = new OdbcCommand(query, conn);
                    OdbcDataReader reader = comm.ExecuteReader();

                    if (reader.Read())
                    {
                        dataGridView1.Rows[index].Cells[col].Value = reader.GetString(reader.GetOrdinal("Feature_Name"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Nominal"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Places"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Pieces"));
                        col++;
                        dataGridView1.Rows[index].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Feature_Key"));
                        col = 1;
                        dataGridView1.Rows.Add(); //This will add an extra row after all rows are place in the table
                    }
                }
               // dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1); //Will remove the last row that was created
            }
            



            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();
                    int inspecKey = op.getInspectionKey();


                    query = "SELECT Feature_Key, Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces FROM ATI_FeatureInspection.dbo.Features\n" +
                        "WHERE Feature_Key NOT IN (" + string.Join(",", listOfFeaturesThatExistForCurrentInspection()) + ") AND Part_Number_FK = '" + op.getPartNo() + "' AND Operation_Number_Fk = '" + op.getOpService() + "';";

                    OdbcCommand com = new OdbcCommand(query, conn);
                    OdbcDataReader reader = com.ExecuteReader();

                    foreach (int row in newRows)
                    {
                        if (reader.Read())
                        {

                            dataGridView1.Rows[row].Cells[col].Value = reader.GetString(reader.GetOrdinal("Feature_Name"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Nominal"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Places"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Pieces"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Feature_Key"));
                            col = 1;
                            dataGridView1.Rows.Add(); //This will add an extra row after all rows are place in the table
                        }
                    }
                }
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1); //Will remove the last row that was created
            }

            catch
            {
                formLoadQuery();
                /*
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();
                    //STOP RIGHT HERE!!!
                    query = "SELECT Feature_Key, Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces FROM ATI_FeatureInspection.dbo.Features\n" +
                        " WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = (SELECT Op_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Inspection_Key = " + op.getInspectionKey() + ")) AND InheritedFromFeature IS NULL;";

                    OdbcCommand com = new OdbcCommand(query, conn);
                    OdbcDataReader reader = com.ExecuteReader();

                    foreach (int row in allRows)
                    {
                        if (reader.Read())
                        {

                            dataGridView1.Rows[row].Cells[col].Value = reader.GetString(reader.GetOrdinal("Feature_Name"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Nominal"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Places"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Pieces"));
                            col++;
                            dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Feature_Key"));
                            col = 1;
                            dataGridView1.Rows.Add(); //This will add an extra row after all rows are place in the table
                        }
                    }
                }
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1); //Will remove the last row that was created
                */
            }
            
   
            
        }

        private void reInsertOldFeaturesButNewInspection()
        {
            // string query = 
        }

        private void mainSubmit_button_Click(object sender, EventArgs e)
        {
            submitClicked = true;
            validateSelectedRows();
            if (submitClicked)
            {
                this.Close();
            }

        }

        /// <summary>
        /// Will check if an inspection exists other than the one created
        /// </summary>
        /// <returns></returns>
        private bool checkIfInspectionExists()
        {
            bool moreThanOneInspection = false;
            List<int> simOpKeys = getOpKeysWithSamePartAndOpNum(op.getOpKey());

            int inspectionCount = 0;

            foreach (int opkey in simOpKeys)
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();


                    string query = "SELECT Op_Key FROM ATI_FeatureInspection.dbo.Inspection " +
                                   "WHERE Op_Key = " + opkey + ";";

                    OdbcCommand com = new OdbcCommand(query, conn);
                    OdbcDataReader reader = com.ExecuteReader();

                    //Checking if OpKey exists in Operation table
                    if (reader.Read())
                    {
                        inspectionCount++;
                    }

                }

                //Lets us know if there are at least 2 opkeys with the same part num and operation number
                if (inspectionCount > 1)
                {
                    moreThanOneInspection = true;
                    break;
                }


            }

            return moreThanOneInspection;
        }

        /// <summary>
        /// This will query the operation table and returns all opkeys that contain the same part number and operation number
        /// as the specified opkey at hand
        /// </summary>
        /// <param name="opkey"></param>
        /// <returns></returns>
        private List<int> getOpKeysWithSamePartAndOpNum(int opkey)
        {
            List<int> similarOpKeys = new List<int>();

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string query = "SELECT Op_Key FROM ATI_FeatureInspection.dbo.Operation " +
                               "WHERE Part_Number = (SELECT DISTINCT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + ") AND Operation_Number = (SELECT DISTINCT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opkey + "); ";

                OdbcCommand com = new OdbcCommand(query, conn);
                OdbcDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    similarOpKeys.Add(reader.GetInt32(reader.GetOrdinal("Op_Key")));
                }
            }

            return similarOpKeys;
        }

        private List<int> getInspecKeysWithOpKeys(List<int> opkeys)
        {
            List<int> similarInspecKeys = new List<int>();

            string joinedOpKeys = string.Join(",", opkeys);

            string query = "SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection " +
                           "WHERE Op_Key IN (" + joinedOpKeys + ");";

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                OdbcCommand com = new OdbcCommand(query, conn);
                OdbcDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    similarInspecKeys.Add(reader.GetInt32(reader.GetOrdinal("Inspection_Key")));
                }
            }

            return similarInspecKeys;
        }

        //Function for what happens when user clicks to add a new feature
        //Remember for the SQL when user clicks this button do an INSERT not an UPDATE
        //As of now turned into a Save Feature Button
        private void newFeatureButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
            int lastRow = dataGridView1.RowCount - 1;
            //dataGridView1.Rows[lastRow].Cells[1].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRow].Cells[1];
            dataGridView1.BeginEdit(true);

            

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// If a cell value changed in the datagridview assume that a row is being changed or added by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Decimal value;
            int nominalCol = 2;
            int plusCol = 3;
            int minusCol = 4;
            int placesCol = 5;
            int piecesCol = 6;

            int countColCheck = 0;

            if (e.ColumnIndex == nominalCol || e.ColumnIndex == plusCol || e.ColumnIndex == minusCol
                || e.ColumnIndex == placesCol || e.ColumnIndex == piecesCol)
            {
                //Wait for form to draw the column names
                if (e.RowIndex >= 0)
                {
                    try
                    {
                        value = Decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        saveRowIndexOfFeatureThatChanged();
                    }
                    catch
                    {
                        MessageBox.Show("Please Enter a Numerical Value");
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Decimal.Parse(0.ToString());
                    }
                }

            }

        }

        /// <summary>
        /// Adds row indexes to a list row indexes whose features have changed
        /// </summary>
        private void saveRowIndexOfFeatureThatChanged()
        {
            int currentRow;

            if (dataGridView1.CurrentRow != null)
            {
                currentRow = dataGridView1.CurrentRow.Index;
                if (!featureRowsChanged.Contains(currentRow))
                {
                    featureRowsChanged.Add(currentRow);
                }
            }
            dataGridView1.RefreshEdit();

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            setDefaultRowValues(e);
        }

        /// <summary>
        /// Sets default values for Select, Places, and Pieces columns
        /// </summary>
        /// <param name="e"></param>
        private void setDefaultRowValues(DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells[0].Value = true; //Sets the check for each row added
            dataGridView1.Rows[e.RowIndex].Cells[5].Value = 1;    //Sets places value to 1 for each row added
            dataGridView1.Rows[e.RowIndex].Cells[6].Value = 1;    //Sets Pieces value to 1 for each row added
        }

        /// <summary>
        /// Checks the Datagridview Control in UserInputForm for an empty cell
        /// </summary>
        /// <returns></returns>
        private bool checkForEmptyCellInGrid()
        {
            int maxRows = dataGridView1.RowCount;
            int maxCols = dataGridView1.ColumnCount;
            int rowIndex;
            int colIndex;
            bool emptyCell = false;
            string message = "Please fill in all values before saving the features";

            for (rowIndex = 0; rowIndex < maxRows; rowIndex++)
            {
                for (colIndex = 1; colIndex < maxCols; colIndex++)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[colIndex].Value == null)
                    {
                        emptyCell = true;
                    }
                }
            }

            return emptyCell;
        }

        private void saveFeaturesButton_Click(object sender, EventArgs e)
        {
            bool emptyCell = checkForEmptyCellInGrid(); //Is there an empty cell when trying to save the features



            //User is done adding features
            //Now insert this new feature to database

            if (emptyCell)
            {

            }


            for (int i = 0; i < featureRowsChanged.Count; i++)
            {

                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();
                    string query;
                    if (op.getInspectionExists())
                    {
                        //Insert new Data from DataGridView Table to the Features table in ATI_Feature Inspectio Database
                        query = "INSERT INTO ATI_FeatureInspection.dbo.Features (Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places)\n" +
                                       "VALUES (" + dataGridView1.Rows[featureRowsChanged[i]].Cells[2].Value + "," + dataGridView1.Rows[featureRowsChanged[i]].Cells[3].Value + "," +
                                                  dataGridView1.Rows[featureRowsChanged[i]].Cells[4].Value + ",'" + dataGridView1.Rows[featureRowsChanged[i]].Cells[1].Value + "'," +
                                                  dataGridView1.Rows[featureRowsChanged[i]].Cells[5].Value + ");\n";
                    }
                    else
                    {
                        //Insert new Data from DataGridView Table to the Features table in ATI_Feature Inspectio Database
                        query = "INSERT INTO ATI_FeatureInspection.dbo.Features (Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places)\n" +
                                       "VALUES (" + dataGridView1.Rows[featureRowsChanged[i]].Cells[2].Value + "," + dataGridView1.Rows[featureRowsChanged[i]].Cells[3].Value + "," +
                                                  dataGridView1.Rows[featureRowsChanged[i]].Cells[4].Value + ",'" + dataGridView1.Rows[featureRowsChanged[i]].Cells[1].Value + "'," +
                                                  dataGridView1.Rows[featureRowsChanged[i]].Cells[5].Value + ");\n";
                    }

                    OdbcCommand connCommand = new OdbcCommand(query, conn);
                    connCommand.ExecuteNonQuery();
                }


            }

            featureRowsChanged.Clear();
        }

        private void insertToPositionsTable(int places, int pieces, int featurekey)
        {
            int place;
            int piece;
            for (int j = 0; j < pieces; j++)
            {
                piece = j + 1;

                for (int i = 0; i < places; i++)
                {
                    place = i + 1;

                    using (OdbcConnection conn = new OdbcConnection(connection_string))
                    {
                        conn.Open();

                        string query = "INSERT INTO ATI_FeatureInspection.dbo.Position (Place, Feature_Key, Piece_ID, Inspection_Key_FK)\n" + //What are we gonna do about features? When are they updated. How to not duplicate every time we go in to the add features form
                                       "VALUES (" + place + ", " + featurekey + "," + piece + ", " + op.getInspectionKey() + ");";


                        OdbcCommand connCommand = new OdbcCommand(query, conn);
                        connCommand.ExecuteNonQuery();
                    }
                }
            }

        }

        private void insertFeaturesToDatabase(int row, bool notPastFeature)
        {

            string query;

            //Adding new features
            if (notPastFeature)
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();


                    //Insert new Data from DataGridView Table to the Features table in ATI_Feature Inspectio Database
                    query = "INSERT INTO ATI_FeatureInspection.dbo.Features (Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces, Part_Number_FK, Operation_Number_FK)\n" +
                                   "VALUES (" + dataGridView1.Rows[row].Cells[2].Value + "," + dataGridView1.Rows[row].Cells[3].Value + "," +
                                              dataGridView1.Rows[row].Cells[4].Value + ",'" + dataGridView1.Rows[row].Cells[1].Value + "'," +
                                              dataGridView1.Rows[row].Cells[5].Value + "," + dataGridView1.Rows[row].Cells[6].Value + ",(SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + op.getOpKey() + "), (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = "+ op.getOpKey() + "));\n";

                    OdbcCommand connCommand = new OdbcCommand(query, conn);
                    connCommand.ExecuteNonQuery();
                }
            }
            
            //Updating old feature for new inspection
            //Should only occur when it is a feature that is in the database already
            else
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    query = "UPDATE ATI_FeatureInspection.dbo.Features " +
                                  "SET Nominal = " + dataGridView1.Rows[row].Cells[2].Value + ", Plus_Tolerance = " + dataGridView1.Rows[row].Cells[3].Value + ", Minus_Tolerance = " +
                                             dataGridView1.Rows[row].Cells[4].Value + ", Feature_Name = '" + dataGridView1.Rows[row].Cells[1].Value + "', Places = " +
                                             dataGridView1.Rows[row].Cells[5].Value + ", Pieces = " + dataGridView1.Rows[row].Cells[6].Value + " WHERE Feature_Key = " +
                                             dataGridView1.Rows[row].Cells[7].Value + ";\n";

                    OdbcCommand connCommand = new OdbcCommand(query, conn);
                    connCommand.ExecuteNonQuery();
                }
            }
            

        }

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            currentRow = e.RowIndex;
        }

        private void deleteSelectedButton_Click(object sender, EventArgs e)
        {
            int maxRows = dataGridView1.RowCount;
            int maxCols = dataGridView1.ColumnCount;

            int rowIndex = 0;
            int colIndex;
            bool selected;


            while (rowIndex < maxRows)
            {
                selected = (bool)dataGridView1.Rows[rowIndex].Cells[0].Value;
                if (selected)
                {
                    dataGridView1.Rows.RemoveAt(rowIndex);
                    maxRows = dataGridView1.RowCount;
                }
                else
                    rowIndex++;
            }
        }

        //So far this only deletes the selected features
        //Wanted to make this function to be able to find which
        //rows were selected so I would need to return a list of row indexes
        private int featuresSelected()
        {
            int maxRows = dataGridView1.RowCount;
            int maxCols = dataGridView1.ColumnCount;

            int rowIndex = 0;
            int colIndex;
            int rowsSelected = 0;
            bool selected;


            while (rowIndex < maxRows)
            {
                selected = (bool)dataGridView1.Rows[rowIndex].Cells[0].Value;
                if (selected)
                {
                    rowsSelected += rowsSelected;
                    dataGridView1.Rows.RemoveAt(rowIndex);
                    maxRows = dataGridView1.RowCount;
                }
                else
                    rowIndex++;
            }

            return rowsSelected;
        }

        private void loadCurrentInspectionFeatures()
        {
            int row = 0;
            int col = 1;

            string query;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                query = "SELECT Feature_Key, Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces FROM ATI_FeatureInspection.dbo.Features\n" +
                        " WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = (SELECT Op_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Inspection_Key = " + op.getInspectionKey() + "))  AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = (SELECT Op_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Inspection_Key = " + op.getInspectionKey() + "));";

                OdbcCommand comm = new OdbcCommand(query, conn);
                OdbcDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows[row].Cells[col].Value = reader.GetString(reader.GetOrdinal("Feature_Name"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Nominal"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Places"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Pieces"));
                    col++;
                    dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Feature_Key"));
                    col = 1;
                    row++;
                    dataGridView1.Rows.Add(); //This will add an extra row after all rows are place in the table
                }

                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1); //Will remove the last row that was created
            }
        }

        private void formLoadQuery()
        {

            List<int> opKeys = new List<int>();
            List<int> inspecKeys = new List<int>();
            int row = 0;
            int col = 1;

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add();

            opKeys = getOpKeysWithSamePartAndOpNum(op.getOpKey());
            inspecKeys = getInspecKeysWithOpKeys(opKeys);

            string query;
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

               //This query is for new inspections only
                    query = "SELECT DISTINCT Feature_Key, Nominal, Plus_Tolerance, Minus_Tolerance, Feature_Name, Places, Pieces FROM ATI_FeatureInspection.dbo.Features\n" +
                            "INNER JOIN ATI_FeatureInspection.dbo.Operation " +
                            "ON Part_Number_FK = '" + op.getPartNo() + "'  AND Operation_Number_FK = " + op.getOpService() + "; ";


                    OdbcCommand com = new OdbcCommand(query, conn);
                    OdbcDataReader reader = com.ExecuteReader();

                    while (reader.Read())
                    {

                        dataGridView1.Rows[row].Cells[col].Value = reader.GetString(reader.GetOrdinal("Feature_Name"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = (float) reader.GetDouble(reader.GetOrdinal("Nominal"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = (float) reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Places"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Pieces"));
                        col++;
                        dataGridView1.Rows[row].Cells[col].Value = reader.GetInt32(reader.GetOrdinal("Feature_Key"));
                        col = 1;
                        row++;
                        dataGridView1.Rows.Add(); //This will add an extra row after all rows are place in the table
                    }
                
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1); //Will remove the last row that was created

            }

        }

        public bool getSubmitClicked()
        {
            return submitClicked;
        }

        public List<int> getFeatureKeysSelected()
        {
            return selectedFeatureRows;
        }

        public int featureKeySelectedListSize()
        {
            return selectedFeatureRows.Count;
        }

        private void deselectButton_Click(object sender, EventArgs e)
        {
            for(int i=0; i<dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = false;
            }
        }
    }
}

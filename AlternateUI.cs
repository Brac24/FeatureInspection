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
using System.Speech.Synthesis;

/*ALL TABLES REFER TO THE ATI_FeatureInspection Database */

namespace Feature_Inspection
{
    public partial class AlternateUI : Form
    {


        private readonly string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";
        public static string OpKeyGlobal;
        public static string PartNumGlobal;
        public static string JobNumGlobal;
        public static string OpService;
        public static int InspectionKeyGlobal;
        public static bool FormClose;
        public static bool FeaturesExistInPosition = false;
        public bool FIRST_TIME_CREATING_GROUPS = true;
        messagePrompt messPrompt = new messagePrompt();

        SpeechSynthesizer synth = new SpeechSynthesizer();

        string[] inspectionValues = new string[7];  // This will contain info from one of the queried values. Is used as input to currentInspection ListViewItem
        bool newInspection = false;
        public static bool inspectionExists = false; //Does an inspection already exist for a specific opkey? Checks the Inpsection table

        List<DataGridViewRow> featureData = new List<DataGridViewRow>();
        List<bool> valueInTolerance = new List<bool>();

        string featureKey;

        float Nominal; 
        float PlusTol;
        float MinusTol;
        float MeasuredValue;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AlternateUI()
        {
            InitializeComponent();

        }

        
        /// <summary>
        /// This is the loader which happens after the constructor and loads data to form and sets control events to their event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlternateUI_Load(object sender, EventArgs e)
        {
            //Default values for the form in these text fields when first opening the program
            partNumberLabelValue.Text = null;
            jobNumberLabelValue.Text = null;
            opNumberLabelValue.Text = null;
            statusLabelValue.Text = null;


            // event handlers
            this.FormClosing += Close_AlternateUI;     
            textBox1.KeyPress += checkEnterKeyPressed;
            textBox1.Validating += Validating;
            textBox1.Validated += Validated;

            //synth.Speak("What up doggy, I'm bout to open this dank ass program, This shit fire, This program is Major Key, If you don't like it then cash me outside how bout dat. We out thew.");
            dataListView1.AllColumns[0].IsVisible = false;
            dataListView1.RebuildColumns();

        }

        //The bottom two properties are to facilitate with displaying decimal values.
        public static int valuesBeforeDecimal
        {
           
            get; set;
        }

        public static int valuesAfterDecimal
        {
            get; set;
        }


        private void Close_AlternateUI(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Queries inspection status of an inpsection and and puts the value on to the form
        /// </summary>
        /// <remarks>
        /// Modifies:
        /// Form Label Text: statusLabelValue.Text
        /// 
        /// Responsibilities:
        /// 1) Query inspection status for current inspection
        /// 2) Put the value on to the form in the text field
        /// </remarks>
        private void queryInspectionStatus()
        {
            string status = null;

            string query;

            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    query = "SELECT status FROM ATI_FeatureInspection.dbo.Inspection WHERE Inspection_Key = " + getInspectionKey() + ";";

                    OdbcCommand comm = new OdbcCommand(query, conn);
                    OdbcDataReader reader = comm.ExecuteReader();

                    if (reader.Read())
                    {
                        status = reader.GetString(reader.GetOrdinal("status"));
                    }
                }
            }
            catch
            {
                status = null;
            }

            statusLabelValue.Text = status;

        }

        //Returns whether an inspection exists for a specific OpKey
        public bool getInspectionExists()
        {
            return inspectionExists;
        }

        public bool getFeaturesExistInPosition()
        {
            return FeaturesExistInPosition;
        }

        public int getOpKey()
        {
            return Int32.Parse(OpKeyGlobal);
        }

        public string getOpService()
        {
            return OpService;
        }

        public string getPartNo()
        {
            return PartNumGlobal;
        }

        public int getInspectionKey()
        {
            return InspectionKeyGlobal;
        }

        private bool featuresExistInPositionTable()
        {
            string query;
            bool featuresInPositionTable = false;
            int countValues = 0;

            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                query = "SELECT Feature_Key, Position_Key, Place, Piece_ID FROM ATI_FeatureInspection.dbo.Position\n " +
                        "WHERE Feature_Key IN (SELECT Feature_Key FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = (SELECT Op_Key FROM ATI_FeatureInspection.dbo.Inspection WHERE Inspection_Key = " + InspectionKeyGlobal + ")));";

                OdbcCommand comm = new OdbcCommand(query, conn);
                OdbcDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    countValues++; //Count the amount values that need to be measured
                }

                //If there is at least one value to be measured
                if (countValues > 0)
                {
                    featuresInPositionTable = true;  //There are features in the table
                }
                else
                    FeaturesExistInPosition = false;
            }

            return featuresInPositionTable;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private bool checkOpKeyTextBoxEmpty(TextBox opkey)
        {
            bool empty = false;

            if (opkey.Text.Equals(String.Empty))
            {
                empty = true;
            }

            return empty;
        }

        /// <summary>
        /// Queries the part number, job number, operation service number for a particular opkey
        /// and pushes this information to the their corresponding label text in the form
        /// </summary>
        /// <param name="opkey"></param>
        /// <remarks>
        /// SQL Query: INNER JOIN between Job and Job_Operation table on Job numbers for a specific opkey or Job_Operation from PRODUCTION DB
        /// </remarks>
        private void getInfoFromOpKeyEntry(TextBox opkey)
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();


                    OpKeyGlobal = opkey.Text;


                    string query = "SELECT Job.Part_Number, Job_Operation.Job, Job_Operation.Operation_Service\n" +
                                   "FROM PRODUCTION.dbo.Job\n" +
                                   "INNER JOIN PRODUCTION.dbo.Job_Operation\n" +
                                   "ON Job.Job = Job_Operation.Job\n" +
                                   "WHERE Job_Operation.Job_Operation = '" + opkey.Text + "';";

                    OdbcCommand com = new OdbcCommand(query, conn);
                    OdbcDataReader reader = com.ExecuteReader();

                    if (reader.Read())
                    {
                        partNumberLabelValue.Text = reader.GetString(reader.GetOrdinal("Part_Number"));
                        jobNumberLabelValue.Text = reader.GetString(reader.GetOrdinal("Job"));
                        opNumberLabelValue.Text = reader.GetString(reader.GetOrdinal("Operation_Service"));
                        partNumberLabelValue.Visible = true;
                        jobNumberLabelValue.Visible = true;
                        opNumberLabelValue.Visible = true;

                        PartNumGlobal = partNumberLabelValue.Text;
                        JobNumGlobal = jobNumberLabelValue.Text;
                        OpService = opNumberLabelValue.Text;
                    }
                    else
                    {
                        partNumberLabelValue.Text = null;
                        jobNumberLabelValue.Text = null;
                        opNumberLabelValue.Text = null;
                        OpKeyGlobal = null;
                    }
                }
            }
            catch
            {
                //MessageBox.Show("Please insert a valid OpKey ");
            }

        }

        /// <summary>
        /// Queries an inpsectionKey based on the current opKey and sets 
        /// The InspectionKeyGlobal variable to the inspection key that was returned
        /// </summary>
        private void queryInspectionKeyFromInspectionTable()
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    string getInspectionKey = "SELECT Inspection_Key FROM ATI_FeatureInspection.dbo.Inspection\n" +
                                             "WHERE Op_Key=" + OpKeyGlobal + ";";

                    OdbcCommand conncommand2 = new OdbcCommand(getInspectionKey, conn);
                    OdbcDataReader reader1 = conncommand2.ExecuteReader();

                    while (reader1.Read())
                    {
                        InspectionKeyGlobal = reader1.GetInt32(reader1.GetOrdinal("Inspection_Key"));
                    }

                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// This event is triggered when the user presses the Enter key. It is the initial step needed 
        /// to begin inspecting.
        /// 1) Check if the the opkey exists in the Operation table. If it does not then create it
        /// 2) If the op did not exist then automatically create an inspection for it in the Inspection table.
        /// 3) Check if 
        /// 4) If it exists then check if it is complete else create a new inspection for it in inspection table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool opExists = false;

            //Check the enter key was pressed
            if (e.KeyChar == (char)13)
            {

                queryInspectionKeyFromInspectionTable();

            }

        }

        /// <summary>
        /// Checks if an inpsection exists for a particular opkey that was entered
        /// </summary>
        /// <returns>
        /// True: if it finds an inspection for the opkey
        /// False: If it does not or if the status is "Complete"
        /// </returns>
        /// <remarks>
        /// SQL Query: retrieve Op_Key, Inspection_Key, status from Inspection table
        /// </remarks>
        private bool opKeyExistsInInspection()
        {
            //Query checking if an inspection for an opkey exists in the Inspection table
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string status;
                string inspectExist = "SELECT Op_Key, Inspection_Key, status FROM ATI_FeatureInspection.dbo.Inspection\n" +
                                      "WHERE Op_Key = " + textBox1.Text;
                OdbcCommand command = new OdbcCommand(inspectExist, conn);
                OdbcDataReader reader = command.ExecuteReader();

                // If there is a value in the reader then there is inspection 
                if (reader.Read())
                {
                    inspectionExists = true;

                    status = reader.GetString(reader.GetOrdinal("status"));
                    if (status == "Complete")
                    {
                        inspectionExists = false; //If the old inspection is complete allow for the creation of a new one
                    }
                    else
                    {
                        InspectionKeyGlobal = reader.GetInt32(reader.GetOrdinal("Inspection_Key"));
                    }
                }
                else
                    inspectionExists = false;
            }

            return inspectionExists;
        }

        /// <summary>
        /// This will insert the current opKey, jobNumber, partNumber, opService to the operation table
        /// </summary>
        /// <remarks>
        /// Responsibilities:
        /// 1) Insert opkey, jobnum, partnum, opservice to operation table
        /// </remarks>
        private void insertOpKeyToOperation()
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    string insertOperation = "INSERT INTO ATI_FeatureInspection.dbo.Operation (Op_Key, Job_Number, Part_Number, Operation_Number)\n" +
                                             " VALUES (convert(int," + OpKeyGlobal + "),'" + JobNumGlobal + "','" + PartNumGlobal + "','" + OpService + "');";

                    OdbcCommand com3 = new OdbcCommand(insertOperation, conn);
                    com3.ExecuteNonQuery();
                }
            }
            catch
            {
                MessageBox.Show("Please Enter a valid opkey");
            }

        }

        /// <summary>
        /// Inserts opkey value to inspection table
        /// </summary>
        /// <remarks>
        /// Responsibilities:
        /// 1) Insert opKey value and hard coded "Incomplete" status value to inspection table
        /// </remarks>
        private void insertOpKeyToInspection()
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();
                    string insertInspection = "INSERT INTO ATI_FeatureInspection.dbo.Inspection (Op_Key,status)\n" +
                                            "VALUES (" + OpKeyGlobal + ",'Incomplete');";

                    OdbcCommand conncommand = new OdbcCommand(insertInspection, conn);
                    conncommand.ExecuteNonQuery();
                }
            }
            catch
            {

            }


        }

        /// <summary>
        /// Queries all columns from the Operation table based on a particular opKey
        /// And checks if the query returns any data
        /// </summary>
        /// <returns>
        /// True: If query returns data
        /// False: If it does not
        /// </returns>
        /// <remarks>
        /// Responsibilities:
        /// 1) Query Operation table based on OpKey
        /// 2) Check the query returns data
        /// 3) If it did then return true else false
        /// </remarks>
        private bool opKeyExistsInOperationTable()
        {
            bool opExists = false;

            try
            {
                // Will check if OpKey exists in the Operation table
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();

                    string query2 = "SELECT *\n" +
                                    "FROM ATI_FeatureInspection.dbo.Operation\n" +
                                    "WHERE Op_Key = '" + OpKeyGlobal + "';";

                    OdbcCommand com2 = new OdbcCommand(query2, conn);
                    OdbcDataReader reader2 = com2.ExecuteReader();

                    //Check that query returned data
                    if(reader2.Read())
                    {
                        opExists = true;
                    }
                }
            }
            catch
            {
                //TODO: Add catch condition action
            }

            return opExists;
        }

        /// <summary>
        /// NOT CALLING THIS FUNCTION AS OF NOW. NO USE FOR IT SO FAR
        /// Queries the dimensions and measurement for a given inspectionKey from FeatureInspection database and puts each in to individual variables Nominal, PlusTol, MinusTol, and MeasuredValue
        /// </summary>
        /// <remarks>
        /// SQL Query: INNER JOIN between Position and Features table based on Inspection_Key_FK in Position table.
        /// 
        /// Responsibilities:
        /// 1) Getting Dimensions and Measured value for an inspectionKey
        /// 2) Checking that Measureg Value is in tolerance
        /// 3) Adding true or false to a valueInTolerance table that lists if the value was in tolerance or not. (Only using this table to check if all the measured values were check for tolerance. PROBABLY CAN FIND A BETTER WAY AND NOT HAVE TO USE THIS TABLE)
        /// 
        /// Modifying:
        /// 1) Globals - float Nominal,PlusTol, MinusTol, MeasuredValue
        /// 2) FunctionScope - float maxTol, minTol
        /// 3) Lists - (bool)valueInTolerance
        /// </remarks>
        public void SetToleranceVariablesAndMeasuredVariable()
        {
            float maxTol;
            float minTol;
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string query = "SELECT  Features.Nominal, Features.Plus_Tolerance, Features.Minus_Tolerance, cast(Measured_Value as float) AS Measured_Value  FROM ATI_FeatureInspection.dbo.Position " +
                                       "INNER JOIN ATI_FeatureInspection.dbo.Features " +
                                       "ON Position.Feature_Key = Features.Feature_Key " +
                                       "WHERE Position.Inspection_Key_FK = " + getInspectionKey();

                OdbcCommand comm = new OdbcCommand(query, conn);
                OdbcDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    Nominal = (float)reader.GetDouble(reader.GetOrdinal("Nominal"));
                    PlusTol = (float)reader.GetDouble(reader.GetOrdinal("Plus_Tolerance"));
                    MinusTol = (float)reader.GetDouble(reader.GetOrdinal("Minus_Tolerance"));
                    MeasuredValue = reader.GetFloat(reader.GetOrdinal("Measured_Value"));

                    maxTol = Nominal + PlusTol;
                    minTol = Nominal - MinusTol;

                    //Checking if in tolerance
                    if (MeasuredValue > maxTol || MeasuredValue < minTol)
                    {
                        valueInTolerance.Add(false);
                    }
                    else
                        valueInTolerance.Add(true);

                }
            }

        }


        /// <summary>
        /// Binds queried data from SQL Server to DataListView Object.
        /// </summary>
        /// <remarks>
        /// SQL Query: INNER JOIN of Position and Features table ON the same FeatureKey with the current inspectionKey
        /// </remarks>
        private void bindData()
        {
            int maxRows;

            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                using (OdbcCommand com = conn.CreateCommand())
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
                {

                    string query = "SELECT   Position.Feature_Key, Piece_ID, Place, Features.Feature_Name, CAST(Features.Nominal AS varchar(10)) + ' +' + CAST(Features.Plus_Tolerance AS varchar(10)) + ' -' + CAST(Features.Minus_Tolerance AS varchar(10)) AS Dimensions, CAST(cast(Measured_Value as float) AS varchar(10)) AS 'Measured Value' FROM ATI_FeatureInspection.dbo.Position " +
                                   "INNER JOIN ATI_FeatureInspection.dbo.Features " +
                                   "ON Position.Feature_Key = Features.Feature_Key " +
                                   "WHERE Position.Inspection_Key_FK = " + getInspectionKey() + "ORDER BY Piece_ID, Place";

                    com.CommandText = query;
                    DataTable t = new DataTable();
                    adapter.Fill(t);
                    dataListView1.DataSource = null;
                    dataListView1.DataSource = t;
                    maxRows = t.Rows.Count;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Youre a foo", e.Message);
            }



        }

        /// <summary>
        /// Opens the user input form and reopens main form after closed
        /// </summary>
        private void openUserInputForm()
        {
            this.Hide(); //Hides the AlternateUI Form

            //Form userInput = new UserInputForm();
            UserInputForm userInput = new UserInputForm(); //Create UserInputForm object
            userInput.ShowDialog();                        //Invoke the ShowDialog to show the instance of the form

            //Checking if user clicked the Submit button on the UserInputForm 
            if (userInput.getSubmitClicked())
            {
                featureKey = "(" + String.Join(",", userInput.getFeatureKeysSelected()) + ")"; //Gets the selected features that user chose and puts in to one featureKey string

                FIRST_TIME_CREATING_GROUPS = true; //Needs to be reset here after adding a new feature to the inspection. This ensures it does not order by feature key

                bindData(); //Bind the current data for that inspection will include features just added from UserInputForm

                textBox1.Focus(); //Put focus on the opKey text box



            }
            userInput = null;
            Show();

        }

        private void openMessagePrompt(messagePrompt mes)
        {
            this.Hide();
            mes.ShowDialog();
            newInspection = mes.getNewInspectionValue();
            mes = null;
            Show();

        }

       

        /// <summary>
        /// Event handler that checks an opkey and opens UserInputForm
        /// </summary>
        /// <remarks>
        /// Responsibilities:
        /// 1) Getting InspectionKey for the opkey currently being used
        /// 2) Check that the opkey is a valid opkey
        /// 3) Open UserInputForm
        /// 4) Display error message to user if invalid opkey
        /// </remarks>
        private void addFeatureButton_Click(object sender, EventArgs e)
        {
            queryInspectionKeyFromInspectionTable(); //Let form know what the inspection key is before going in to user input form

            if (partNumberLabelValue.Text != "" && opNumberLabelValue.Text != "")
            {
                openUserInputForm();
            }

            else
            {
                string message = "Please Enter a valid OpKey";
                string caption = "Invalid OpKey";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, button);

                if (result == DialogResult.OK)
                {

                }
            }


        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // dataListView1.AllColumns[0].Width = 0; //Hide the FeatureKey Column

        }

        private void dataListView2_BeforeCreatingGroups(object sender, BrightIdeasSoftware.CreateGroupsEventArgs e)
        {
            //e.Canceled = true;

        }

        /// <summary>
        /// Event handler that controls how the dataListView will order the groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Responsibilities:
        /// 1) Control how the groups will be ordered
        /// 2) Disable all columns from being editable except for Measured Value column
        /// </remarks>
        private void dataListView1_BeforeCreatingGroups(object sender, BrightIdeasSoftware.CreateGroupsEventArgs e)
        {


            if (FIRST_TIME_CREATING_GROUPS)
            {
                dataListView1.AlwaysGroupByColumn = dataListView1.GetColumn(1);
                
            }
            else
            {
                dataListView1.AlwaysGroupByColumn = dataListView1.SelectedColumn;
            }


            dataListView1.AutoResizeColumns();

            dataListView1.Refresh();
            dataListView1.Update();
            dataListView1.AllColumns[4].CellEditUseWholeCell = true;
            //dataListView1.AllColumns[0].IsVisible = false;

            //dataListView1.AllColumns[1].IsVisible = true;


            dataListView1.AllColumns[1].IsEditable = false;
            dataListView1.AllColumns[2].IsEditable = false;
            dataListView1.AllColumns[3].IsEditable = false;
            dataListView1.AllColumns[4].IsEditable = false;
            //dataListView1.AllColumns[5].IsEditable = false;
            //dataListView1.AllColumns[6].IsEditable = false;


            //dataListView1.AllColumns[0].Width = 0;

            FIRST_TIME_CREATING_GROUPS = false;

        }

        

        /// <summary>
        /// Event handler that updates the Measured Value in the Position table 
        /// Updates the cell that is coming out of focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Event is triggered when cell comes out of focus
        /// </remarks>
        private void dataListView1_CellEditValidating(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            //Try updating the table with the new Measured Value input by the user
            try
            {
                using (OdbcConnection conn = new OdbcConnection(connection_string))
                {
                    conn.Open();
                    string query = "UPDATE ATI_FeatureInspection.dbo.Position SET Measured_Value= " + Decimal.Parse(e.NewValue.ToString())
                                   + " WHERE Feature_Key=" + dataListView1.AllColumns[0].GetValue(e.RowObject) + " AND Piece_ID = " + dataListView1.AllColumns[1].GetValue(e.RowObject) +
                                      "AND Place = " + dataListView1.AllColumns[2].GetValue(e.RowObject) + " AND Inspection_Key_FK = " + getInspectionKey() + ";";

                    OdbcCommand conncommand = new OdbcCommand(query, conn);
                    conncommand.ExecuteNonQuery();
                    e.NewValue = Decimal.Parse(e.NewValue.ToString());

                }
            }
            //In NAN tell user to input a number
            catch (FormatException err)
            {
                MessageBox.Show("Please insert a number ", err.Message);
                e.NewValue = e.Value;

                dataListView1.Refresh();
                dataListView1.RefreshSelectedObjects();
            }
        }

        /// <summary>
        /// Event triggered after CellEditValidating that checks once more if value in Measured value is valid
        /// If it is not it will revert to the last value the cell had
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Event triggered after CellEditValidating
        /// </remarks>
        private void dataListView1_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            //Will check if the value in the measured value column is a number or not
            try
            {
                e.NewValue = Decimal.Parse(e.NewValue.ToString());
            }
            catch
            {
                //If not use the last value that was being used
                e.NewValue = Decimal.Parse(e.Value.ToString());
            }


        }

        /// <summary>
        /// Event handler triggered by the clicking the Finish Inspectio button and updates the status of an
        /// inspection to "Complete"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void finishInspectionButton_Click(object sender, EventArgs e)
        {
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            {
                conn.Open();

                string query = "UPDATE ATI_FeatureInspection.dbo.Inspection SET status= 'Complete'" +
                               "WHERE Inspection_Key=" + InspectionKeyGlobal;

                OdbcCommand com = new OdbcCommand(query, conn);
                com.ExecuteNonQuery();
            }

            queryInspectionStatus(); //Change staus in form
        }

        private void createOkCancelOpKeyConfirmationForm()
        {
            Form OkCancelForm = new Form();
            Button okButton = new Button();
            Button cancelButton = new Button();

            okButton.Text = "OK";
            okButton.Location = new Point(10, 10);
            cancelButton.Text = "Cancel";

            cancelButton.Location = new Point(okButton.Left, okButton.Height + okButton.Top + 10);

            okButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
        }


        /// <summary>
        /// This event is triggered when the user presses the Enter key. It is the initial step needed 
        /// to begin inspecting.
        /// 1) Validate OpKey Entry
        /// 2) Once all validation has been done we can now check if this information exists in the operation table
        /// 3) If the opkey does not exist in the operation table create it in the Operation table and create an inspection for it
        /// 4) If it already exists in operation but not in the Inspection table create an inspection for it
        /// 5) Now the initial information has been set in the database now we wait for a user action such as adding features
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkEnterKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ((char)13) || e.KeyChar == '\t')
            {
                e.Handled = true;
                //This is used in the Before Creating Groups event
                FIRST_TIME_CREATING_GROUPS = true; //This is reset to true after trying to search for a different opkey. This is used to always sort by the piece number column the first time we open an inspection

                //If empty

                if (checkOpKeyTextBoxEmpty(textBox1))
                {
                    invalidOpKeyMessage();
                }
                else
                {

                    FeaturesExistInPosition = false;
                    getInfoFromOpKeyEntry(textBox1);

                    if (opNumberLabelValue.Text.Equals(String.Empty))
                    {
                        invalidOpKeyMessage();
                    }
                    else if (!opKeyExistsInOperationTable())
                    {
                        insertOpKeyToOperation();
                        insertOpKeyToInspection();
                    }
                    else if (!opKeyExistsInInspection())
                    {
                        
                        insertOpKeyToInspection();
                        queryInspectionKeyFromInspectionTable(); //Grab newly inserted inspectionKey
                        olvColumn2.IsVisible = true;
                        dataListView1.RebuildColumns();
                        bindData();
                        this.addFeatureButton.Focus();

                    }
                    else if (featuresExistInPositionTable())
                    {
                        olvColumn2.IsVisible = true;
                        dataListView1.RebuildColumns();
                        queryInspectionStatus();
                        FeaturesExistInPosition = true;
                        //This is to let the other form know that there is already an inspection begun for this opkey
                        bindData();
                        this.addFeatureButton.Focus();

                    }
                    else
                    {
                        olvColumn2.IsVisible = true;
                        dataListView1.RebuildColumns();
                        bindData();
                        this.addFeatureButton.Focus();
                    }
                    

                    queryInspectionKeyFromInspectionTable();
                    queryInspectionStatus();

                    if (statusLabelValue.Text.Equals("Complete  "))
                    {
                        //Removed for now because there may be more than one user on the same op key and if one of the users clicks "finish inspection" it will lock the other users out
                        //dataListView1.CellEditActivation = ObjectListView.CellEditActivateMode.None; //Will lock the user from inputting measurement data
                    }
                    else
                        dataListView1.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClickAlways;
                }
            }
            //Do not allow spaces in opkey textbox
            else if (e.KeyChar == ((char)32))
            {
                string message = "Spaces are not permitted";
                string caption = "No Spaces Please";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, button);
                e.KeyChar = ((char)0);
            }
        }

        /// <summary>
        /// Mainly a check to see if user is closing the form 
        /// or user clicked one of the form buttons without first pressing enter on the opkey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validating(object sender, CancelEventArgs e)
        {

            if (FormClose)
            {

            }
            else if ((sender as TextBox).Text.Equals(string.Empty))
            {
                //e.Cancel = true;

                if (this.addFeatureButton.Focused || this.finishInspectionButton.Focused)
                {
                    invalidOpKeyMessage();
                }
            }
            else if (textBox1.Text != OpKeyGlobal)
            {
                e.Cancel = true;
                string message = "Please press Enter in the OpKey text field";
                string caption = "OpKey has changed";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult result;

                //If user has clicked one of these 2 buttons then ask to press enter first
                //else it means they probably pressed the minimize or close button
                if (this.addFeatureButton.Focused || this.finishInspectionButton.Focused)
                {
                    result = MessageBox.Show(message, caption, button);
                }

            }
            //Leave this for proper function
            else if(validateValidOpKey(opNumberLabelValue))
            {

            }
            else
            {
                e.Cancel = true;
                string message = "Please Hit Enter in the OpKey Text Field before continuing";
                string caption = "Must Press Enter to Continue";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, button);
            }
        }
        
        private void Validated(object sender, EventArgs e)
        {
            dataListView1.AutoSizeColumns();
        }


        private bool validateValidOpKey(object sender)
        {
            Label infoLabelText = sender as Label;

            bool validOpKey = false;
            if (infoLabelText.Text != "")
            {
                validOpKey = true;

            }
            else
                opNumberLabelValue.Hide();


            return validOpKey;
        }

        private void invalidOpKeyMessage()
        {

            string message = "Please Enter a valid OpKey";
            string caption = "Invalid OpKey";
            MessageBoxButtons button = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, button);
        }
        

  
        private void dataListView1_AfterCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            dataListView1.AutoResizeColumns();
            //dataListView1.AllColumns[0].Width = 0;


        }

        private void dataListView1_CellEditFinished(object sender, CellEditEventArgs e)
        {

        }

        private void dataListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            //e.UseCellFormatEvents = true;
            //e.Item.BackColor = Color.Red;
        }

        List<double> values = new List<double>(); //List of values that check for out of tolerance

        /// <summary>
        /// This event handler checks the measured values and colors the values red if they are out of tolerance
        /// for its given row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataListView1_FormatCell(object sender, FormatCellEventArgs e)
        {
            double measuredValue;
            
            string measuredStringValue;
            List<string> substring = new List<string>();
            

            int index = e.DisplayIndex;

            //Column index 5 is Measured Values column
            if (e.ColumnIndex == 5)
            {
                measuredValue = double.Parse(e.CellValue.ToString()); //Get measured value to check for tolerance

                //If value is not in tolerance color value red
                if (!ValueInTolerance(values[0], values[1], values[2], measuredValue))
                {
                    e.SubItem.ForeColor = Color.Red;
                    values.Clear();
                }
                                
            }

            //Will parse the Range column and split each value to be able to check for tolerance for current row
            else if(e.ColumnIndex == 4)
            {
                string tolerance = e.CellValue.ToString();   //Turn range column cell to a string
                
                 substring = tolerance.Split(new char[] { '+', '-' },StringSplitOptions.RemoveEmptyEntries).ToList<string>(); //Split the string into 3 seperate string values

                values = substring.Select(double.Parse).ToList(); //Now convert the list to a list of floats. index 0: nominal, index 1: plus, index 2: minus

            }
            
        }

        /// <summary>
        /// Will check if a value is in tolerance
        /// </summary>
        /// <param name="nominal">Nominal value</param>
        /// <param name="plus">Plus tolerance</param>
        /// <param name="minus">Minus tolerance</param>
        /// <param name="measured">Measured value being tested</param>
        /// <returns>Returns if in tolerance</returns>
        public bool ValueInTolerance(double nominal, double plus, double minus, double measured)
        {
            double max = nominal + plus;

            double min = nominal - minus;

            //If greateer than the max value
            if (measured > max)
            {
                return false;
            }
            //If greater than the min value
            else if (measured < min)
            {
                return false;
            }
            else
                return true;
        }

    }
}

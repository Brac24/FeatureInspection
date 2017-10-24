using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public partial class Feature_Inspection : Form, IFeatureCreationView, IInspectionView, IReportView
    {
        public FeatureCreationPresenter presenter;
        private FeatureCreationModelMock model;
        private InspectionPresenter inspectionPresenter;
        private ReportPresenter reportPresenter;

        BindingSource bindingSource;
        BindingSource bindingSourceInspection = new BindingSource();
        BindingSource bindingSourceListBox = new BindingSource();
        BindingSource bindingSourceFocusCombo = new BindingSource();
        BindingSource sampleBindingSource = new BindingSource();


        public Feature_Inspection()
        {
            InitializeComponent();
            this.mainTabControl.TabPages.Remove(this.mainTabControl.TabPages["Report_Page"]);
        }


        /************************/
        /***** Properties *******/
        /************************/
        

        #region Properties

        public TextBox PartTextBox { get { return partBoxFeature; } set { } }

        public TextBox FeatureOpTextBox { get { return opBoxFeature; } }

        public DataGridView FeatureGridView { get { return featureEditGridView; } }

        public DataGridView InspectionGrid { get { return inspectionEntryGridView; } }

        public BindingSource BindingSource { get { return bindingSource; } set { bindingSource = value; } }

        public BindingSource InspectionBindingSource { get { return bindingSourceInspection; } set { bindingSourceInspection = value; } }

        public BindingSource SampleBindingSource { get { return sampleBindingSource; } set { sampleBindingSource = value; } }

        public BindingSource ListBoxBindingSource { get { return bindingSourceListBox; } set { bindingSourceListBox = value; } }

        public Chart InspectionChart { get { return inspectionChart; } set { inspectionChart = value; } }

        public Chart ReportChart { get { return report_Chart; } set { report_Chart = value; } }

        public ComboBox ChartFocusComboBox { get { return inspectionFocusCombo; } set { inspectionFocusCombo = value; } }

        public ComboBox ReportTypeComboBox { get { return reportTypeDropdown; } set { reportTypeDropdown = value; } }

        public ComboBox ReportFocusComboBox { get { return reportFocusComboBox; } set { reportFocusComboBox = value; } }

        public TextBox LotsizeTextBox { get { return lotSizeBoxInspection; } set { lotSizeBoxInspection = value; } }

        public TextBox OpKeyTextBox { get { return opKeyBoxInspection; } set { opKeyBoxInspection = value; } }

        public ListBox PartsListBox { get { return partsListBox; } set { partsListBox = value; } }

        public Label InspectionPageHeader { get { return inspectionPageHeader; } set { inspectionPageHeader = value; } }

        public Label ReportPageHeader { get { return reportPageHeader; } set { reportPageHeader = value; } }

        public Label PartNumberLabel { get { return partNumberLabelInspection; } set { partNumberLabelInspection = value; } }

        public Label JobLabel { get { return jobLabelInspection; } set { jobLabelInspection = value; } }

        public Label ChartLabel1 { get { return partNumberLabel; } set { partNumberLabel = value; } }

        public Label ChartLabel2 { get { return measuredLabel; } set { measuredLabel = value; } }

        public Label OperationLabel { get { return opLabelInspection; } set { opLabelInspection = value; } }

        public TextBox ReportTB1 { get { return reportTB1; } set { reportTB1 = value; } }

        public TextBox ReportTB2 { get { return reportTB2; } set { reportTB2 = value; } }

        public Label ReportLB1 { get { return reportLB1; } set { reportLB1 = value; } }

        public Label ReportLB2 { get { return reportLB2; } set { reportLB2 = value; } }

        public string PartStorage { get { return partStorageLabel.Text; } set { partStorageLabel.Text = value; } }

        public string OpStorage { get { return opStorageLabel.Text; } set { opStorageLabel.Text = value; } }

        public string PartNumber { get { return partBoxFeature.Text; } set { partBoxFeature.Text = value; } }

        public string InspectionHeaderText { set { inspectionPageHeader.Text = value.ToString(); } }

        public string OperationNumber { get { return opBoxFeature.Text; } set { opBoxFeature.Text = value; } }

        public string JobNumber { get { return jobLabelInspection.Text; } set { jobLabelInspection.Text = value; } }

        public string FeaturePageHeaderText { get { return featurePageHeader.Text; } set { featurePageHeader.Text = value; } }

        public int OpKey { get { return Int32.Parse(opKeyBoxInspection.Text); } }

        public int ListBoxIndex { get { return partsListBox.SelectedIndex; } set { partsListBox.SelectedIndex = value; } }

        public int FeatureCount { get { return featureEditGridView.Rows.Count; } }

        public int ListBoxCount { get { return partsListBox.Items.Count; } }

        public object FeatureDataSource { get { return featureEditGridView.DataSource; } set { featureEditGridView.DataSource = value; } }

        public object LastRowFeatureOperationNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Operation_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Operation_Number_FK"].Value = value; }
        }

        public object LastRowFeaturePartNumberFK
        {
            get { return featureEditGridView.Rows[0].Cells["Part_Number_FK"].Value; }
            set { featureEditGridView.Rows[featureEditGridView.Rows.Count - 1].Cells["Part_Number_FK"].Value = value; }
        }

        #endregion



        /************************/
        /***  Event Handlers ****/
        /************************/
        /*All Handlers with <summary> blocks have been refactored or have been deemed simple enough.
        Handlers that have TODOs on them are either non-functioning or are bloated and contain too much logic.*/


        #region Form Handlers

        //FORM HANLDERS

        /// <summary>
        /// This event handler gives a reference of the view and model to the presenter class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FeatureCreationTableMock_Load(object sender, EventArgs e)
        {
            model = new FeatureCreationModelMock();
            presenter = new FeatureCreationPresenter(this, model);
            inspectionPresenter = new InspectionPresenter(this, model);
            inspectionPresenter.BindFocusCharts();
            reportPresenter = new ReportPresenter(this, model);

            inspectionPresenter.CreateGraphArea(inspectionChart);
            reportPresenter.CreateGraphArea(ReportChart);
            
        }

        #endregion

        #region Inspection Handlers

        //INSPECTION ENTRY TAB HANDLERS    

        /// <summary>
        /// This method calls a locking method when a cell value is changed in the inspection grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                inspectionPresenter.IfInspectionCellEqualsZero_NoLock();
            }
            catch
            {

            }
        }

        /// <summary>
        /// This event handler triggers when a key is pressed down while the focus is on either textbox in the inspection page.
        /// This event calls methods that filter accepted characters and validates values entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numOnly_KeyDown(object sender, KeyEventArgs e)
        {
            inspectionPresenter.CheckEnter_ValidateOpKeyAndLotSize(sender, e);
        }

        /// <summary>
        /// This event handler will cycle through parts in the part list box when the user clicks on the 'next part' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextPartButton_Click(object sender, EventArgs e)
        {
            inspectionPresenter.GotToNextPart();

        }

        /// <summary>
        /// This will call for a method to update the inspection page when the index of the part changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_ChangeWithPart(object sender, EventArgs e)
        {
            inspectionPresenter.UpdateGridViewOnIndexChange(sender);
        }

        /// <summary>
        /// Handles updating the DB when the Inspection grid view is done being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            inspectionPresenter.AdapterUpdateInspection();
        }

        /// <summary>
        /// Handles exception error on the inspection grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        /// <summary>
        /// Handles when the value selected in the inspection focus combo box changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionFocusCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            inspectionPresenter.BindFocusCharts();
        }

        /// <summary>
        /// Handles when databinding is complete on the inspection entry grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inspectionEntryGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            inspectionPresenter.AdapterUpdateInspection();
        }

        private void inspectionChart_MouseMove(object sender, MouseEventArgs e)
        {
            inspectionPresenter.ShowChartDetails(e);
        }

        #endregion

        #region Feature Handlers

        // FEATURE HANDLERS

        /// <summary>
        /// This event handler is fired on part and operation number text boxes and checks 
        /// the enter key was pressed in each of these
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkEnterKeyPressedFeatures_KeyDown(object sender, KeyEventArgs e)
        {
            presenter.OnEnterKeyInitializeDataGridView(sender, e);
        }



        /// <summary>
        /// Used to set the type of sampling and the feature type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            presenter.SetSampleIDAndFeatureTypeHiddenColumns(e);

            //TODO: Clean up code
            //Bubble number column
            if(e.ColumnIndex == 2)
            {
                //make letters upper case
                featureEditGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = featureEditGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToUpper();
            }
        }

        /// <summary>
        /// Will add an empty row to FeatureGridView and set invisible part number and op number columns
        /// so that the feature will properly get inserted to the database with the correct info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFeature_Click(object sender, EventArgs e)
        {
            presenter.addFeature_Click();
        }

        /// <summary>
        /// Deletes the row that the user clicks delete on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRowFeature_MouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            presenter.DeleteDataGridViewRowOnRowDeleteButtonWasClicked(sender, e);
        }

        /// <summary>
        /// Allows user to redo an inspection entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RedoRowInspection_MouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            inspectionPresenter.RedoDataGridViewRow(sender, e);
        }

        /// <summary>
        /// Will simply rebind feature data grid view without updating the database.
        /// To have the old state the user started with
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelChanges_Click(object sender, EventArgs e)
        {
            presenter.cancelChanges_Click();
        }

        /// <summary>
        /// Updates, deletes, or inserts any data needed to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            presenter.saveButton_Click();

        }

        /// <summary>
        /// Handle invalid data input to datagridview and alert the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featureEditGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        #endregion

        #region Report Handlers

        // REPORT HANDLERS

        /// <summary>
        /// This handler switches the report view when clicking on the "switch button".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportSwitch(object sender, EventArgs e)
        {
            if (report_Chart.Visible == true)
            {
                report_Chart.Visible = false;
            }
            else
            {
                report_Chart.Visible = true;
            }
        }

        private void reportTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            reportPresenter.ReportScope_TextBoxVisibility();
        }

        /// <summary>
        /// This event handler triggers when a key is pressed down while the focus is on either textbox in the inspection page.
        /// This event calls methods that filter accepted characters and validates values entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportTextBox_Check_Filter(object sender, KeyEventArgs e)
        {
            reportPresenter.Check_ReportScope(sender, e);
        }

        /// <summary>
        /// This event handler is fired on part and operation number text boxes and checks 
        /// the enter key was pressed in each of these
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkEnterKeyPressedReport_KeyDown(object sender, KeyEventArgs e)
        {
            //Same method that is called in second case of "check_ReportScope"
            reportPresenter.OnEnterKeyInitializeDataGridView(sender, e);
        }

        /// <summary>
        /// Handles when the value selected in the inspection focus combo box changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportFocusCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            reportPresenter.DataBindReportFocus();
        }




        #endregion

    }
}
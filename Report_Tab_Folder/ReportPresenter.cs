using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace Feature_Inspection
{
    class ReportPresenter
    {
        private IReportView view;
        private IFeaturesDataSource model;

        public ReportPresenter(IReportView view, IFeaturesDataSource model)
        {
            this.view = view;
            this.model = model;
            Initialize();
        }

        private void Initialize()
        {

        }

        public void ReportScope_TextBoxVisibility()
        {
            if (view.ReportTypeComboBox.SelectedIndex == 0)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "OPKEY:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = false;
                view.ReportTB2.Visible = false;
            }
            if (view.ReportTypeComboBox.SelectedIndex == 1)
            {
                view.ReportLB1.Visible = true;
                view.ReportLB1.Text = "PART:";
                view.ReportTB1.Visible = true;
                view.ReportLB2.Visible = true;
                view.ReportTB2.Visible = true;
            }
        }

        public void check_ReportScope()
        {
            if (view.ReportTypeComboBox.SelectedIndex == 0)
            {
                //TODO: Run method for filtering & checking value of view.ReportTB1
            }
            if (view.ReportTypeComboBox.SelectedIndex == 1)
            {
                //TODO: Run method for filtering & checking value of view.ReportTB1 AND view.ReportTB2
            }
        }

        public bool SetOpKeyInfoInspection()
        {
            DataTable info = new DataTable();
            info = model.GetInfoFromOpKeyEntry(Int32.Parse(view.ReportTB1.Text));

            if (info.Rows.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public void checkEnter_ValidateOpKey(KeyEventArgs e)

        {
            //Will work on an enter or tab key press
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                ValidateValidOpKey();
            }
        }

        private void ValidateValidOpKey()
        {
            bool isValidOpKey = SetOpKeyInfoInspection();

            if (isValidOpKey)
            {
                ValidateReportExistForOpKey();
                MessageBox.Show("Valid OpKey");
            }
            else
            {
                InvalidOpKeyProcess();
            }

        }

        private bool ValidateReportExistForOpKey()
        {
            DataTable partList = null;
            bool inspectionExists = model.GetInspectionExistsOnOpKey(Int32.Parse(view.ReportTB1.Text));
            partList = model.GetPartsList(Int32.Parse(view.ReportTB1.Text));
            /*
            if (inspectionExists)
            {
                BeginInpsectionDataGridViewInitialization(partList);
            }
            else
            {
                CreateInspectionForOpKey();

                BeginInpsectionDataGridViewInitialization(partList);
            }
            */
            return inspectionExists;
        }

        private void InvalidOpKeyProcess()
        {
            MessageBox.Show(view.ReportTB1.Text + " is invalid please enter a valid Op Key", "Invalid OpKey");
            view.ReportTB1.Clear();
        }

        /// <summary>
        /// This method handles filtering non number characters out of the opKeyTextBox and lotSizeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void filterTextBox(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;
            int lotChars = textbox.Text.Length;
            //Block non-number characters
            char currentKey = (char)e.KeyCode;
            bool modifier = e.Control || e.Alt || e.Shift;
            bool nonNumber = char.IsLetter(currentKey) ||
                             char.IsSymbol(currentKey) ||
                             char.IsWhiteSpace(currentKey) ||
                             char.IsPunctuation(currentKey) ||
                             char.IsSeparator(currentKey) ||
                             char.IsUpper(currentKey);

            //Allow navigation keyboard arrows
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Delete:
                    e.SuppressKeyPress = false;
                    return;
                default:
                    break;
            }

            if (modifier || nonNumber || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Oemcomma)
                e.SuppressKeyPress = true;

            if (e.KeyCode >= (Keys)96 && e.KeyCode <= (Keys)105)
            {
                e.SuppressKeyPress = false;
            }

            if (lotChars == 0)
            {
                if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
                {
                    e.SuppressKeyPress = true;
                }
            }

            //Handle pasted Text
            if (e.Control && e.KeyCode == Keys.V)
            {
                //Preview paste data (removing non-number characters)
                string pasteText = Clipboard.GetText();
                string strippedText = "";
                for (int i = 0; i < pasteText.Length; i++)
                {
                    if (char.IsDigit(pasteText[i]))
                        strippedText += pasteText[i].ToString();
                }

                if (strippedText != pasteText)
                {
                    //There were non-numbers in the pasted text
                    e.SuppressKeyPress = true;
                }
                else
                    e.SuppressKeyPress = false;
            }
        }


    }
}

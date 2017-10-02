using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public partial class messagePrompt : Form
    {
        bool newInspection; 
        bool buttonClick = false;
        public messagePrompt()
        {
            InitializeComponent();
           
        }

        public messagePrompt(string leftButton, string rightButton)
        {
            newInspectionButton.Text = leftButton;
            previousInspectionButton.Text = rightButton;
        }

        private void newInspectionButton_Click(object sender, EventArgs e)
        {
            newInspection = true;
            this.Close();
        }

        private void previousInspectionButton_Click(object sender, EventArgs e)
        {
            newInspection = false;
            this.Close();
        }

        private void messagePrompt_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        public bool getNewInspectionValue()
        {
            return newInspection;
        }
    }
}

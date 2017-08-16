namespace Tester
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Customer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Job = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Submit = new System.Windows.Forms.Button();
            this.OpKey_TextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Nominal_TextBox = new System.Windows.Forms.TextBox();
            this.PlusTol_TextBox = new System.Windows.Forms.TextBox();
            this.MinusTol_TextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Feature_TextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Place_TextBox = new System.Windows.Forms.TextBox();
            this.mainSubmit_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Customer,
            this.PN,
            this.Job,
            this.OpKey});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(587, 255);
            this.dataGridView1.TabIndex = 0;
            // 
            // Customer
            // 
            this.Customer.HeaderText = "Customer";
            this.Customer.Name = "Customer";
            // 
            // PN
            // 
            this.PN.HeaderText = "PN";
            this.PN.Name = "PN";
            // 
            // Job
            // 
            this.Job.HeaderText = "Job";
            this.Job.Name = "Job";
            // 
            // OpKey
            // 
            this.OpKey.HeaderText = "OpKey";
            this.OpKey.Name = "OpKey";
            // 
            // Submit
            // 
            this.Submit.Location = new System.Drawing.Point(12, 273);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(75, 23);
            this.Submit.TabIndex = 1;
            this.Submit.Text = "Submit";
            this.Submit.UseVisualStyleBackColor = true;
            this.Submit.Click += new System.EventHandler(this.button1_Click);
            // 
            // OpKey_TextBox
            // 
            this.OpKey_TextBox.Location = new System.Drawing.Point(12, 341);
            this.OpKey_TextBox.Name = "OpKey_TextBox";
            this.OpKey_TextBox.Size = new System.Drawing.Size(121, 20);
            this.OpKey_TextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 325);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "OpKey";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 371);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Place #";
            // 
            // Nominal_TextBox
            // 
            this.Nominal_TextBox.Location = new System.Drawing.Point(156, 388);
            this.Nominal_TextBox.Name = "Nominal_TextBox";
            this.Nominal_TextBox.Size = new System.Drawing.Size(100, 20);
            this.Nominal_TextBox.TabIndex = 6;
            // 
            // PlusTol_TextBox
            // 
            this.PlusTol_TextBox.Location = new System.Drawing.Point(262, 388);
            this.PlusTol_TextBox.Name = "PlusTol_TextBox";
            this.PlusTol_TextBox.Size = new System.Drawing.Size(38, 20);
            this.PlusTol_TextBox.TabIndex = 7;
            // 
            // MinusTol_TextBox
            // 
            this.MinusTol_TextBox.Location = new System.Drawing.Point(306, 388);
            this.MinusTol_TextBox.Name = "MinusTol_TextBox";
            this.MinusTol_TextBox.Size = new System.Drawing.Size(38, 20);
            this.MinusTol_TextBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 371);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Nominal";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(276, 371);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "+";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(317, 367);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "-";
            // 
            // Feature_TextBox
            // 
            this.Feature_TextBox.Location = new System.Drawing.Point(156, 341);
            this.Feature_TextBox.Name = "Feature_TextBox";
            this.Feature_TextBox.Size = new System.Drawing.Size(121, 20);
            this.Feature_TextBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(153, 325);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Feature";
            // 
            // Place_TextBox
            // 
            this.Place_TextBox.Location = new System.Drawing.Point(12, 388);
            this.Place_TextBox.Name = "Place_TextBox";
            this.Place_TextBox.Size = new System.Drawing.Size(121, 20);
            this.Place_TextBox.TabIndex = 14;
            // 
            // mainSubmit_button
            // 
            this.mainSubmit_button.Location = new System.Drawing.Point(12, 424);
            this.mainSubmit_button.Name = "mainSubmit_button";
            this.mainSubmit_button.Size = new System.Drawing.Size(75, 23);
            this.mainSubmit_button.TabIndex = 15;
            this.mainSubmit_button.Text = "Submit";
            this.mainSubmit_button.UseVisualStyleBackColor = true;
            this.mainSubmit_button.Click += new System.EventHandler(this.mainSubmit_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 460);
            this.Controls.Add(this.mainSubmit_button);
            this.Controls.Add(this.Place_TextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Feature_TextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MinusTol_TextBox);
            this.Controls.Add(this.PlusTol_TextBox);
            this.Controls.Add(this.Nominal_TextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpKey_TextBox);
            this.Controls.Add(this.Submit);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Submit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer;
        private System.Windows.Forms.DataGridViewTextBoxColumn PN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Job;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpKey;
        private System.Windows.Forms.TextBox OpKey_TextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Nominal_TextBox;
        private System.Windows.Forms.TextBox PlusTol_TextBox;
        private System.Windows.Forms.TextBox MinusTol_TextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Feature_TextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Place_TextBox;
        private System.Windows.Forms.Button mainSubmit_button;
    }
}


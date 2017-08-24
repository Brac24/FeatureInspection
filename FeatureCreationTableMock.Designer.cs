namespace Feature_Inspection
{
    partial class FeatureCreationTableMock
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
            this.featureEditGridView1 = new System.Windows.Forms.DataGridView();
            this.addFeatureButton1 = new System.Windows.Forms.Button();
            this.opKeyBox1 = new System.Windows.Forms.TextBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.Inspection_Page = new System.Windows.Forms.TabPage();
            this.Feature_Page = new System.Windows.Forms.TabPage();
            this.inspectionEntryGridView1 = new System.Windows.Forms.DataGridView();
            this.opKeyBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.featureEditGridView1)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.Inspection_Page.SuspendLayout();
            this.Feature_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inspectionEntryGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // featureEditGridView1
            // 
            this.featureEditGridView1.AllowUserToAddRows = false;
            this.featureEditGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.featureEditGridView1.Location = new System.Drawing.Point(3, 32);
            this.featureEditGridView1.Name = "featureEditGridView1";
            this.featureEditGridView1.Size = new System.Drawing.Size(676, 325);
            this.featureEditGridView1.TabIndex = 0;
            this.featureEditGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.featureEditGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // addFeatureButton1
            // 
            this.addFeatureButton1.Location = new System.Drawing.Point(507, 363);
            this.addFeatureButton1.Name = "addFeatureButton1";
            this.addFeatureButton1.Size = new System.Drawing.Size(169, 23);
            this.addFeatureButton1.TabIndex = 2;
            this.addFeatureButton1.Text = "ADD FEATURE";
            this.addFeatureButton1.UseVisualStyleBackColor = true;
            this.addFeatureButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // opKeyBox1
            // 
            this.opKeyBox1.Location = new System.Drawing.Point(3, 6);
            this.opKeyBox1.Name = "opKeyBox1";
            this.opKeyBox1.Size = new System.Drawing.Size(100, 20);
            this.opKeyBox1.TabIndex = 3;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.Inspection_Page);
            this.mainTabControl.Controls.Add(this.Feature_Page);
            this.mainTabControl.Location = new System.Drawing.Point(0, 2);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(690, 421);
            this.mainTabControl.TabIndex = 4;
            // 
            // Inspection_Page
            // 
            this.Inspection_Page.Controls.Add(this.button1);
            this.Inspection_Page.Controls.Add(this.opKeyBox2);
            this.Inspection_Page.Controls.Add(this.inspectionEntryGridView1);
            this.Inspection_Page.Location = new System.Drawing.Point(4, 22);
            this.Inspection_Page.Name = "Inspection_Page";
            this.Inspection_Page.Padding = new System.Windows.Forms.Padding(3);
            this.Inspection_Page.Size = new System.Drawing.Size(682, 395);
            this.Inspection_Page.TabIndex = 0;
            this.Inspection_Page.Text = "Inspection";
            this.Inspection_Page.UseVisualStyleBackColor = true;
            // 
            // Feature_Page
            // 
            this.Feature_Page.Controls.Add(this.featureEditGridView1);
            this.Feature_Page.Controls.Add(this.addFeatureButton1);
            this.Feature_Page.Controls.Add(this.opKeyBox1);
            this.Feature_Page.Location = new System.Drawing.Point(4, 22);
            this.Feature_Page.Name = "Feature_Page";
            this.Feature_Page.Padding = new System.Windows.Forms.Padding(3);
            this.Feature_Page.Size = new System.Drawing.Size(682, 395);
            this.Feature_Page.TabIndex = 1;
            this.Feature_Page.Text = "Feature Add";
            this.Feature_Page.UseVisualStyleBackColor = true;
            // 
            // inspectionEntryGridView1
            // 
            this.inspectionEntryGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.inspectionEntryGridView1.Location = new System.Drawing.Point(3, 33);
            this.inspectionEntryGridView1.Name = "inspectionEntryGridView1";
            this.inspectionEntryGridView1.Size = new System.Drawing.Size(673, 328);
            this.inspectionEntryGridView1.TabIndex = 0;
            // 
            // opKeyBox2
            // 
            this.opKeyBox2.Location = new System.Drawing.Point(3, 7);
            this.opKeyBox2.Name = "opKeyBox2";
            this.opKeyBox2.Size = new System.Drawing.Size(100, 20);
            this.opKeyBox2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(500, 367);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "NEXT PART";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FeatureCreationTableMock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 424);
            this.Controls.Add(this.mainTabControl);
            this.Name = "FeatureCreationTableMock";
            this.Text = "FeatureCreationTableMock";
            this.Load += new System.EventHandler(this.FeatureCreationTableMock_Load);
            ((System.ComponentModel.ISupportInitialize)(this.featureEditGridView1)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.Inspection_Page.ResumeLayout(false);
            this.Inspection_Page.PerformLayout();
            this.Feature_Page.ResumeLayout(false);
            this.Feature_Page.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inspectionEntryGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView featureEditGridView1;
        private System.Windows.Forms.Button addFeatureButton1;
        private System.Windows.Forms.TextBox opKeyBox1;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage Inspection_Page;
        private System.Windows.Forms.TabPage Feature_Page;
        private System.Windows.Forms.TextBox opKeyBox2;
        private System.Windows.Forms.DataGridView inspectionEntryGridView1;
        private System.Windows.Forms.Button button1;
    }
}
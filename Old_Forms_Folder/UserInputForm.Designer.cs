namespace Feature_Inspection
{
    partial class UserInputForm
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
            this.components = new System.ComponentModel.Container();
            this.partNoLabel = new System.Windows.Forms.Label();
            this.mainSubmit_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OpService_TextBox = new System.Windows.Forms.TextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.partNoTextBox = new System.Windows.Forms.TextBox();
            this.newFeatureButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.selectDataGridColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.featureDataGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nominalDataGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.plusDataGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minusDataGridColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.placesDataGridColumn = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.Pieces = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.featureKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveFeaturesButton = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deleteSelectedButton = new System.Windows.Forms.Button();
            this.deselectButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // partNoLabel
            // 
            this.partNoLabel.AutoSize = true;
            this.partNoLabel.Location = new System.Drawing.Point(153, 19);
            this.partNoLabel.Name = "partNoLabel";
            this.partNoLabel.Size = new System.Drawing.Size(36, 13);
            this.partNoLabel.TabIndex = 47;
            this.partNoLabel.Text = "Part #";
            // 
            // mainSubmit_button
            // 
            this.mainSubmit_button.Location = new System.Drawing.Point(6, 322);
            this.mainSubmit_button.Name = "mainSubmit_button";
            this.mainSubmit_button.Size = new System.Drawing.Size(75, 23);
            this.mainSubmit_button.TabIndex = 45;
            this.mainSubmit_button.Text = "Submit";
            this.mainSubmit_button.UseVisualStyleBackColor = true;
            this.mainSubmit_button.Click += new System.EventHandler(this.mainSubmit_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "OpService";
            // 
            // OpService_TextBox
            // 
            this.OpService_TextBox.Location = new System.Drawing.Point(12, 35);
            this.OpService_TextBox.Name = "OpService_TextBox";
            this.OpService_TextBox.ReadOnly = true;
            this.OpService_TextBox.Size = new System.Drawing.Size(121, 20);
            this.OpService_TextBox.TabIndex = 33;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // partNoTextBox
            // 
            this.partNoTextBox.Location = new System.Drawing.Point(156, 35);
            this.partNoTextBox.Name = "partNoTextBox";
            this.partNoTextBox.ReadOnly = true;
            this.partNoTextBox.Size = new System.Drawing.Size(121, 20);
            this.partNoTextBox.TabIndex = 50;
            // 
            // newFeatureButton
            // 
            this.newFeatureButton.Location = new System.Drawing.Point(84, 254);
            this.newFeatureButton.Name = "newFeatureButton";
            this.newFeatureButton.Size = new System.Drawing.Size(85, 24);
            this.newFeatureButton.TabIndex = 51;
            this.newFeatureButton.Text = "New Feature";
            this.newFeatureButton.UseVisualStyleBackColor = true;
            this.newFeatureButton.Click += new System.EventHandler(this.newFeatureButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.selectDataGridColumn,
            this.featureDataGridColumn,
            this.nominalDataGridColumn,
            this.plusDataGridColumn,
            this.minusDataGridColumn,
            this.placesDataGridColumn,
            this.Pieces,
            this.featureKey});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 74);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(502, 173);
            this.dataGridView1.TabIndex = 54;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter_1);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // selectDataGridColumn
            // 
            this.selectDataGridColumn.HeaderText = "Select";
            this.selectDataGridColumn.Name = "selectDataGridColumn";
            this.selectDataGridColumn.Width = 50;
            // 
            // featureDataGridColumn
            // 
            this.featureDataGridColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.featureDataGridColumn.HeaderText = "Feature";
            this.featureDataGridColumn.Name = "featureDataGridColumn";
            // 
            // nominalDataGridColumn
            // 
            this.nominalDataGridColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nominalDataGridColumn.HeaderText = "Nominal";
            this.nominalDataGridColumn.Name = "nominalDataGridColumn";
            // 
            // plusDataGridColumn
            // 
            this.plusDataGridColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.plusDataGridColumn.HeaderText = "+";
            this.plusDataGridColumn.Name = "plusDataGridColumn";
            // 
            // minusDataGridColumn
            // 
            this.minusDataGridColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.minusDataGridColumn.HeaderText = "-";
            this.minusDataGridColumn.Name = "minusDataGridColumn";
            // 
            // placesDataGridColumn
            // 
            this.placesDataGridColumn.HeaderText = "Places";
            this.placesDataGridColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.placesDataGridColumn.Name = "placesDataGridColumn";
            this.placesDataGridColumn.Width = 50;
            // 
            // Pieces
            // 
            this.Pieces.HeaderText = "Pieces";
            this.Pieces.Name = "Pieces";
            this.Pieces.Width = 50;
            // 
            // featureKey
            // 
            this.featureKey.HeaderText = "Feature No.";
            this.featureKey.Name = "featureKey";
            this.featureKey.ReadOnly = true;
            this.featureKey.Visible = false;
            // 
            // saveFeaturesButton
            // 
            this.saveFeaturesButton.Location = new System.Drawing.Point(342, 254);
            this.saveFeaturesButton.Name = "saveFeaturesButton";
            this.saveFeaturesButton.Size = new System.Drawing.Size(86, 23);
            this.saveFeaturesButton.TabIndex = 72;
            this.saveFeaturesButton.Text = "Save Features";
            this.saveFeaturesButton.UseVisualStyleBackColor = true;
            this.saveFeaturesButton.Click += new System.EventHandler(this.saveFeaturesButton_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Feature";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Nominal";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 50;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "+";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 50;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "-";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 50;
            // 
            // deleteSelectedButton
            // 
            this.deleteSelectedButton.Location = new System.Drawing.Point(175, 254);
            this.deleteSelectedButton.Name = "deleteSelectedButton";
            this.deleteSelectedButton.Size = new System.Drawing.Size(102, 23);
            this.deleteSelectedButton.TabIndex = 73;
            this.deleteSelectedButton.Text = "Delete Selected";
            this.deleteSelectedButton.UseVisualStyleBackColor = true;
            this.deleteSelectedButton.Click += new System.EventHandler(this.deleteSelectedButton_Click);
            // 
            // deselectButton
            // 
            this.deselectButton.Location = new System.Drawing.Point(12, 255);
            this.deselectButton.Name = "deselectButton";
            this.deselectButton.Size = new System.Drawing.Size(57, 37);
            this.deselectButton.TabIndex = 74;
            this.deselectButton.Text = "Deselect All";
            this.deselectButton.UseVisualStyleBackColor = true;
            this.deselectButton.Click += new System.EventHandler(this.deselectButton_Click);
            // 
            // UserInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(526, 387);
            this.Controls.Add(this.deselectButton);
            this.Controls.Add(this.deleteSelectedButton);
            this.Controls.Add(this.saveFeaturesButton);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.newFeatureButton);
            this.Controls.Add(this.partNoTextBox);
            this.Controls.Add(this.partNoLabel);
            this.Controls.Add(this.mainSubmit_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpService_TextBox);
            this.Name = "UserInputForm";
            this.Text = "UserInputForm";
            this.Load += new System.EventHandler(this.UserInputForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label partNoLabel;
        private System.Windows.Forms.Button mainSubmit_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OpService_TextBox;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox partNoTextBox;
        private System.Windows.Forms.Button newFeatureButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button saveFeaturesButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Button deleteSelectedButton;
        private System.Windows.Forms.Button deselectButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn selectDataGridColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn featureDataGridColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nominalDataGridColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn plusDataGridColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn minusDataGridColumn;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn placesDataGridColumn;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn Pieces;
        private System.Windows.Forms.DataGridViewTextBoxColumn featureKey;
    }
}
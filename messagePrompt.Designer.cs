namespace Feature_Inspection
{
    partial class messagePrompt
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
            this.messageLabel = new System.Windows.Forms.Label();
            this.newInspectionButton = new System.Windows.Forms.Button();
            this.previousInspectionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLabel.Location = new System.Drawing.Point(8, 9);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(197, 20);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "What would you like to do?";
            // 
            // newInspectionButton
            // 
            this.newInspectionButton.Location = new System.Drawing.Point(12, 62);
            this.newInspectionButton.Name = "newInspectionButton";
            this.newInspectionButton.Size = new System.Drawing.Size(92, 23);
            this.newInspectionButton.TabIndex = 1;
            this.newInspectionButton.Text = "New Inspection";
            this.newInspectionButton.UseVisualStyleBackColor = true;
            this.newInspectionButton.Click += new System.EventHandler(this.newInspectionButton_Click);
            // 
            // previousInspectionButton
            // 
            this.previousInspectionButton.Location = new System.Drawing.Point(113, 62);
            this.previousInspectionButton.Name = "previousInspectionButton";
            this.previousInspectionButton.Size = new System.Drawing.Size(92, 23);
            this.previousInspectionButton.TabIndex = 2;
            this.previousInspectionButton.Text = "Open Existing";
            this.previousInspectionButton.UseVisualStyleBackColor = true;
            this.previousInspectionButton.Click += new System.EventHandler(this.previousInspectionButton_Click);
            // 
            // messagePrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 90);
            this.Controls.Add(this.previousInspectionButton);
            this.Controls.Add(this.newInspectionButton);
            this.Controls.Add(this.messageLabel);
            this.Name = "messagePrompt";
            this.Text = "messagePrompt";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.messagePrompt_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Button newInspectionButton;
        private System.Windows.Forms.Button previousInspectionButton;
    }
}
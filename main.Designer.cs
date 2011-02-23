namespace l2cAlt
{
    partial class main
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
            this.dlgConnect = new System.Windows.Forms.Button();
            this.dlgOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // dlgConnect
            // 
            this.dlgConnect.Location = new System.Drawing.Point(4, 12);
            this.dlgConnect.Name = "dlgConnect";
            this.dlgConnect.Size = new System.Drawing.Size(106, 28);
            this.dlgConnect.TabIndex = 1;
            this.dlgConnect.Text = "Connect";
            this.dlgConnect.UseVisualStyleBackColor = true;
            this.dlgConnect.Click += new System.EventHandler(this.dlgConnect_Click);
            // 
            // dlgOutput
            // 
            this.dlgOutput.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dlgOutput.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.dlgOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dlgOutput.Location = new System.Drawing.Point(4, 46);
            this.dlgOutput.Name = "dlgOutput";
            this.dlgOutput.ReadOnly = true;
            this.dlgOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.dlgOutput.Size = new System.Drawing.Size(872, 346);
            this.dlgOutput.TabIndex = 2;
            this.dlgOutput.Text = "L2 Client Alternative\n";
            this.dlgOutput.TextChanged += new System.EventHandler(this.dlgOutput_TextChanged);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 397);
            this.Controls.Add(this.dlgOutput);
            this.Controls.Add(this.dlgConnect);
            this.Name = "main";
            this.Text = "L2 Client Alternative";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.main_FormClosed);
            this.Load += new System.EventHandler(this.main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button dlgConnect;
        private System.Windows.Forms.RichTextBox dlgOutput;
    }
}


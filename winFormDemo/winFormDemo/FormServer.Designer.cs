namespace winFormServer
{
    partial class FormServer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ClientIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_RequestFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Client ip";
            // 
            // textBox_ClientIP
            // 
            this.textBox_ClientIP.Location = new System.Drawing.Point(40, 60);
            this.textBox_ClientIP.Name = "textBox_ClientIP";
            this.textBox_ClientIP.ReadOnly = true;
            this.textBox_ClientIP.Size = new System.Drawing.Size(222, 23);
            this.textBox_ClientIP.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(282, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "port";
           
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(282, 60);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.ReadOnly = true;
            this.textBox_Port.Size = new System.Drawing.Size(79, 23);
            this.textBox_Port.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Request file";
            
            // 
            // textBox_RequestFile
            // 
            this.textBox_RequestFile.Location = new System.Drawing.Point(40, 123);
            this.textBox_RequestFile.Name = "textBox_RequestFile";
            this.textBox_RequestFile.ReadOnly = true;
            this.textBox_RequestFile.Size = new System.Drawing.Size(321, 23);
            this.textBox_RequestFile.TabIndex = 2;
            // 
            // FormServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 252);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_RequestFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_ClientIP);
            this.Controls.Add(this.label1);
            this.Name = "FormServer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Form Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label1;
        private TextBox textBox_ClientIP;
        private Label label2;
        private TextBox textBox_Port;
        private Label label3;
        private TextBox textBox_RequestFile;
    }
}
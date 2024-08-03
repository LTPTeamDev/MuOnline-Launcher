namespace ConfigCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtRSS = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtGSPort = new System.Windows.Forms.TextBox();
            this.txtCSPort = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TimeZone = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtStartFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAdress = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeZone)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "RSS Link:";
            // 
            // txtRSS
            // 
            this.txtRSS.Location = new System.Drawing.Point(66, 38);
            this.txtRSS.Name = "txtRSS";
            this.txtRSS.Size = new System.Drawing.Size(252, 20);
            this.txtRSS.TabIndex = 2;
            this.txtRSS.Text = "http://ltp-team.ru/index.php?app=core&module=global&section=rss&type=forums&id=1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Server IP :";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(66, 63);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(252, 20);
            this.txtServerIP.TabIndex = 4;
            this.txtServerIP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "GS Port :";
            // 
            // txtGSPort
            // 
            this.txtGSPort.Location = new System.Drawing.Point(66, 88);
            this.txtGSPort.Name = "txtGSPort";
            this.txtGSPort.Size = new System.Drawing.Size(252, 20);
            this.txtGSPort.TabIndex = 6;
            this.txtGSPort.Text = "55901";
            // 
            // txtCSPort
            // 
            this.txtCSPort.Location = new System.Drawing.Point(66, 113);
            this.txtCSPort.Name = "txtCSPort";
            this.txtCSPort.Size = new System.Drawing.Size(252, 20);
            this.txtCSPort.TabIndex = 8;
            this.txtCSPort.Text = "44405";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtAdress);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.TimeZone);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtRSS);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtServerIP);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtGSPort);
            this.groupBox2.Controls.Add(this.txtStartFile);
            this.groupBox2.Controls.Add(this.txtCSPort);
            this.groupBox2.Location = new System.Drawing.Point(12, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(328, 285);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Start File:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "CS Port:";
            // 
            // TimeZone
            // 
            this.TimeZone.Location = new System.Drawing.Point(66, 163);
            this.TimeZone.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.TimeZone.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            -2147483648});
            this.TimeZone.Name = "TimeZone";
            this.TimeZone.Size = new System.Drawing.Size(252, 20);
            this.TimeZone.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 193);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(309, 33);
            this.button2.TabIndex = 16;
            this.button2.Text = "Load Config";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "TimeZone :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 232);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(309, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "Save Config";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtStartFile
            // 
            this.txtStartFile.Location = new System.Drawing.Point(66, 138);
            this.txtStartFile.Name = "txtStartFile";
            this.txtStartFile.Size = new System.Drawing.Size(252, 20);
            this.txtStartFile.TabIndex = 8;
            this.txtStartFile.Text = "Main.exe";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Update Adress:";
            // 
            // txtAdress
            // 
            this.txtAdress.Location = new System.Drawing.Point(92, 13);
            this.txtAdress.Name = "txtAdress";
            this.txtAdress.Size = new System.Drawing.Size(226, 20);
            this.txtAdress.TabIndex = 20;
            this.txtAdress.Text = "ADRESS";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 319);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Mu Launcher - Config Creator";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeZone)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRSS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtGSPort;
        private System.Windows.Forms.TextBox txtCSPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown TimeZone;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStartFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAdress;
    }
}


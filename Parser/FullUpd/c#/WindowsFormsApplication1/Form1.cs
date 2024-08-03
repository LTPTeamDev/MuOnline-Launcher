using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public class Form1 : Form
	{
		private static uint key = 99u;

		private IContainer components;

		private Button button1;

		private GroupBox groupBox1;

		private Button button2;

		private BackgroundWorker backgroundWorker1;

		private Label label1;

		private Label label2;

		private Button button3;

		private BackgroundWorker backgroundWorker2;

		private ListBox listBox1;

		private GroupBox groupBox2;

		private Button button4;

		private TextBox textBox1;

		public Form1()
		{
			this.InitializeComponent();
		}

		public void setTextLabel1(string s)
		{
			if (this.label1 != null)
			{
				this.label1.Text = s;
			}
		}

		public void setTextLabel2(string s)
		{
			if (this.label2 != null)
			{
				this.label2.Text = s;
			}
		}

		private void setTextLabelInvoke(string s, int type)
		{
			if (base.InvokeRequired)
			{
				switch (type)
				{
				case 1:
					base.Invoke(new Action<string>(this.setTextLabel1), new object[]
					{
						s
					});
					return;
				case 2:
					base.Invoke(new Action<string>(this.setTextLabel2), new object[]
					{
						s
					});
					return;
				default:
					return;
				}
			}
			else
			{
				switch (type)
				{
				case 1:
					this.setTextLabel1(s);
					return;
				case 2:
					this.setTextLabel2(s);
					return;
				default:
					return;
				}
			}
		}

		private bool My_CheckFileName(string fName)
		{
			for (int i = 0; i < this.listBox1.Items.Count; i++)
			{
				if (this.listBox1.Items[i].ToString() == fName)
				{
					return false;
				}
			}
			return true;
		}

		public static string XOR_EncryptDecrypt(string str)
		{
			char[] array = str.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)((uint)array[i] ^ Form1.key);
			}
			return new string(array);
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				string currentDirectory = Directory.GetCurrentDirectory();
				string text = "";
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < currentDirectory.Length; i++)
				{
					if (currentDirectory[i] == '\\')
					{
						num = i;
					}
				}
				if (num != 0)
				{
					text = currentDirectory.Substring(num + 1);
				}
				FileStream output = new FileStream("client.info", FileMode.Create);
				BinaryWriter binaryWriter = new BinaryWriter(output);
				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
				string[] array = files;
				for (int j = 0; j < array.Length; j++)
				{
					string text2 = array[j];
					if (num != 0)
					{
						text = text2.Substring(currentDirectory.Length + 1);
					}
					if (this.My_CheckFileName(text))
					{
						num2++;
					}
					this.setTextLabelInvoke(string.Concat(new object[]
					{
						"Файлов : ",
						num2,
						"  Папок : ",
						num3
					}), 1);
				}
				string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories);
				string[] array2 = directories;
				for (int k = 0; k < array2.Length; k++)
				{
					string path = array2[k];
					num3++;
					this.setTextLabelInvoke(string.Concat(new object[]
					{
						"Файлов : ",
						num2,
						"  Папок : ",
						num3
					}), 1);
					string[] files2 = Directory.GetFiles(path);
					string[] array3 = files2;
					for (int l = 0; l < array3.Length; l++)
					{
						string arg_160_0 = array3[l];
						num2++;
						this.setTextLabelInvoke(string.Concat(new object[]
						{
							"Файлов : ",
							num2,
							"  Папок : ",
							num3
						}), 1);
					}
				}
				binaryWriter.Write(Form1.XOR_EncryptDecrypt(Convert.ToString(num2)));
				string[] files3 = Directory.GetFiles(Directory.GetCurrentDirectory());
				string[] array4 = files3;
				for (int m = 0; m < array4.Length; m++)
				{
					string text3 = array4[m];
					if (num != 0)
					{
						text = text3.Substring(currentDirectory.Length + 1);
					}
					if (this.My_CheckFileName(text))
					{
						Crc32 crc = new Crc32();
						string text4 = string.Empty;
						byte[] buffer = File.ReadAllBytes(text3);
						byte[] array5 = crc.ComputeHash(buffer);
						for (int n = 0; n < array5.Length; n++)
						{
							byte b = array5[n];
							text4 += b.ToString("x2").ToUpper();
						}
						this.setTextLabelInvoke(string.Concat(new object[]
						{
							"Файлов : ",
							num2,
							"  Папок : ",
							num3
						}), 1);
						this.setTextLabelInvoke(text + " " + text4, 2);
						binaryWriter.Write(Form1.XOR_EncryptDecrypt(text));
						binaryWriter.Write(Form1.XOR_EncryptDecrypt(text4));
					}
				}
				string[] directories2 = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories);
				string[] array6 = directories2;
				for (int num4 = 0; num4 < array6.Length; num4++)
				{
					string text5 = array6[num4];
					if (num != 0)
					{
						text = text5.Substring(currentDirectory.Length + 1);
					}
					this.setTextLabelInvoke(string.Concat(new object[]
					{
						"Файлов : ",
						num2,
						"  Папок : ",
						num3
					}), 1);
					string[] files4 = Directory.GetFiles(text5);
					string[] array7 = files4;
					for (int num5 = 0; num5 < array7.Length; num5++)
					{
						string text6 = array7[num5];
						if (num != 0)
						{
							text = text6.Substring(currentDirectory.Length + 1);
						}
						if (this.My_CheckFileName(text))
						{
							Crc32 crc2 = new Crc32();
							string text7 = string.Empty;
							byte[] buffer2 = File.ReadAllBytes(text6);
							byte[] array8 = crc2.ComputeHash(buffer2);
							for (int num6 = 0; num6 < array8.Length; num6++)
							{
								byte b2 = array8[num6];
								text7 += b2.ToString("x2").ToUpper();
							}
							this.setTextLabelInvoke(string.Concat(new object[]
							{
								"Файлов : ",
								num2,
								"  Папок : ",
								num3
							}), 1);
							this.setTextLabelInvoke(text + " " + text7, 2);
							binaryWriter.Write(Form1.XOR_EncryptDecrypt(text));
							binaryWriter.Write(Form1.XOR_EncryptDecrypt(text7));
						}
					}
				}
				binaryWriter.Close();
				this.button1.Enabled = true;
				this.button2.Enabled = false;
			}
			catch (Exception ex)
			{
				this.button1.Enabled = true;
				this.button2.Enabled = false;
				MessageBox.Show(ex.Message);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!this.backgroundWorker1.IsBusy)
			{
				this.backgroundWorker1.RunWorkerAsync();
				this.button1.Enabled = false;
				this.button2.Enabled = true;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.button1.Enabled = true;
			this.button2.Enabled = false;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (!this.backgroundWorker2.IsBusy)
			{
				this.backgroundWorker2.RunWorkerAsync();
				this.button3.Enabled = false;
			}
		}

		private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
		{
			FileStream input = new FileStream("client.info", FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(input);
			Convert.ToInt32(Form1.XOR_EncryptDecrypt(binaryReader.ReadString()));
			while (binaryReader.PeekChar() > 0)
			{
				this.setTextLabelInvoke(Form1.XOR_EncryptDecrypt(binaryReader.ReadString()), 1);
				this.setTextLabelInvoke(Form1.XOR_EncryptDecrypt(binaryReader.ReadString()), 2);
			}
			binaryReader.Close();
			this.button3.Enabled = true;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (this.textBox1.Text != "")
			{
				this.listBox1.Items.Add(this.textBox1.Text);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button3 = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 50);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Результат";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Для запуска нажмите \'СТАРТ\'...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Файлов : 0  Папок : 0";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(232, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Стоп";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(125, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Проверить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "file_parser.exe",
            "client.info"});
            this.listBox1.Location = new System.Drawing.Point(6, 21);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(182, 108);
            this.listBox1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(295, 141);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Исключения";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(194, 47);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Добавить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(194, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(95, 20);
            this.textBox1.TabIndex = 5;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(319, 250);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Генератор листа обновлений";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
	}
}

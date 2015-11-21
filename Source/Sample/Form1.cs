using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using GenCode128;

namespace Sample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtInput;
      private System.Windows.Forms.Button cmdMakeBarcode;
      private System.Windows.Forms.PictureBox pictBarcode;
      private System.Windows.Forms.TextBox txtWeight;
      private System.Windows.Forms.Label label2;
      private System.Drawing.Printing.PrintDocument printDocument1;
      private System.Windows.Forms.Button cmdPrint;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.label1 = new System.Windows.Forms.Label();
         this.txtInput = new System.Windows.Forms.TextBox();
         this.cmdMakeBarcode = new System.Windows.Forms.Button();
         this.pictBarcode = new System.Windows.Forms.PictureBox();
         this.txtWeight = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.printDocument1 = new System.Drawing.Printing.PrintDocument();
         this.cmdPrint = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 12);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(84, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "Text to encode";
         // 
         // txtInput
         // 
         this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtInput.Location = new System.Drawing.Point(96, 8);
         this.txtInput.Name = "txtInput";
         this.txtInput.Size = new System.Drawing.Size(436, 20);
         this.txtInput.TabIndex = 1;
         this.txtInput.Text = "";
         // 
         // cmdMakeBarcode
         // 
         this.cmdMakeBarcode.Location = new System.Drawing.Point(192, 36);
         this.cmdMakeBarcode.Name = "cmdMakeBarcode";
         this.cmdMakeBarcode.Size = new System.Drawing.Size(92, 23);
         this.cmdMakeBarcode.TabIndex = 2;
         this.cmdMakeBarcode.Text = "Make barcode";
         this.cmdMakeBarcode.Click += new System.EventHandler(this.cmdMakeBarcode_Click);
         // 
         // pictBarcode
         // 
         this.pictBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pictBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.pictBarcode.Location = new System.Drawing.Point(8, 72);
         this.pictBarcode.Name = "pictBarcode";
         this.pictBarcode.Size = new System.Drawing.Size(528, 84);
         this.pictBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
         this.pictBarcode.TabIndex = 3;
         this.pictBarcode.TabStop = false;
         // 
         // txtWeight
         // 
         this.txtWeight.Location = new System.Drawing.Point(96, 36);
         this.txtWeight.Name = "txtWeight";
         this.txtWeight.Size = new System.Drawing.Size(44, 20);
         this.txtWeight.TabIndex = 5;
         this.txtWeight.Text = "2";
         this.txtWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 40);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(84, 16);
         this.label2.TabIndex = 4;
         this.label2.Text = "Bar weight";
         // 
         // printDocument1
         // 
         this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
         // 
         // cmdPrint
         // 
         this.cmdPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdPrint.Location = new System.Drawing.Point(460, 164);
         this.cmdPrint.Name = "cmdPrint";
         this.cmdPrint.TabIndex = 6;
         this.cmdPrint.Text = "Print";
         this.cmdPrint.Click += new System.EventHandler(this.cmdPrint_Click);
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(540, 194);
         this.Controls.Add(this.cmdPrint);
         this.Controls.Add(this.txtWeight);
         this.Controls.Add(this.txtInput);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.pictBarcode);
         this.Controls.Add(this.cmdMakeBarcode);
         this.Controls.Add(this.label1);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form1";
         this.Text = "Code128 Demo";
         this.ResumeLayout(false);

      }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

      private void cmdMakeBarcode_Click(object sender, System.EventArgs e)
      {
         try
         {
            Image myimg = Code128Rendering.MakeBarcodeImage(txtInput.Text, int.Parse(txtWeight.Text), true);
            pictBarcode.Image = myimg;
         }
         catch (Exception ex)
         {
            MessageBox.Show(this, ex.Message, this.Text);
         }
      }

      private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
      {
         using (Graphics g = e.Graphics)
         {
            using (Font fnt = new Font("Arial", 16))
            {
               string caption = string.Format("Code128 barcode weight={0}", txtWeight.Text);
               g.DrawString(caption, fnt, System.Drawing.Brushes.Black, 50, 50);
               caption = string.Format("message='{0}'", txtInput.Text);
               g.DrawString(caption, fnt, System.Drawing.Brushes.Black, 50, 75);
               g.DrawImage(pictBarcode.Image, 50, 110);
            }
         }
      }

      private void cmdPrint_Click(object sender, System.EventArgs e)
      {
         printDocument1.Print();
      }

	}
}

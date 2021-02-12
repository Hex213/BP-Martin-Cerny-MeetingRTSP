
namespace MeetingClientWPF.GUI.WinForm
{
    partial class FormMain
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.butConnect = new LibUIAcademy.XanderUI.XUIButton();
            this.butCreate = new LibUIAcademy.XanderUI.XUIButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.butConnect, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.butCreate, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(650, 384);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // butConnect
            // 
            this.butConnect.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.butConnect.ButtonImage = MeetingClientWPF.Properties.Resources.ConnectImg;
            this.butConnect.ButtonStyle = LibUIAcademy.XanderUI.XUIButton.Style.MaterialRounded;
            this.butConnect.ButtonText = "ButtonCon";
            this.butConnect.ClickBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.butConnect.ClickTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(130)))), ((int)(((byte)(140)))));
            this.butConnect.CornerRadius = 5;
            this.butConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.butConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.butConnect.Horizontal_Alignment = System.Drawing.StringAlignment.Center;
            this.butConnect.HoverBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.butConnect.HoverTextColor = System.Drawing.Color.Black;
            this.butConnect.ImagePosition = LibUIAcademy.XanderUI.XUIButton.imgPosition.Left;
            this.butConnect.Location = new System.Drawing.Point(35, 22);
            this.butConnect.Name = "butConnect";
            this.butConnect.Size = new System.Drawing.Size(579, 157);
            this.butConnect.TabIndex = 0;
            this.butConnect.TextColor = System.Drawing.Color.Black;
            this.butConnect.Vertical_Alignment = System.Drawing.StringAlignment.Center;
            this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
            // 
            // butCreate
            // 
            this.butCreate.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.butCreate.ButtonImage = MeetingClientWPF.Properties.Resources.CreateImg;
            this.butCreate.ButtonStyle = LibUIAcademy.XanderUI.XUIButton.Style.MaterialRounded;
            this.butCreate.ButtonText = "ButtonCreate";
            this.butCreate.ClickBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.butCreate.ClickTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(130)))), ((int)(((byte)(140)))));
            this.butCreate.CornerRadius = 5;
            this.butCreate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.butCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.butCreate.Horizontal_Alignment = System.Drawing.StringAlignment.Center;
            this.butCreate.HoverBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.butCreate.HoverTextColor = System.Drawing.Color.Black;
            this.butCreate.ImagePosition = LibUIAcademy.XanderUI.XUIButton.imgPosition.Left;
            this.butCreate.Location = new System.Drawing.Point(35, 204);
            this.butCreate.Name = "butCreate";
            this.butCreate.Size = new System.Drawing.Size(579, 157);
            this.butCreate.TabIndex = 1;
            this.butCreate.TextColor = System.Drawing.Color.Black;
            this.butCreate.Vertical_Alignment = System.Drawing.StringAlignment.Center;
            this.butCreate.Click += new System.EventHandler(this.butCreate_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(650, 384);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(650, 384);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LibUIAcademy.XanderUI.XUIButton butConnect;
        private LibUIAcademy.XanderUI.XUIButton butCreate;
    }
}
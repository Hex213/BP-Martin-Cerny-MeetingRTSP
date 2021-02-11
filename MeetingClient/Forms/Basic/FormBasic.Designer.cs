
using LibUIAcademy;
using LibUIAcademy.XanderUI;

namespace MeetingClient.Forms.Basic
{
    partial class FormBasic
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
            this.buttonConnect = new ButtonRounded();
            this.buttonCreate = new ButtonRounded();
            this.buttonExit = new ButtonRounded();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.Transparent;
            this.buttonConnect.BorderColor = System.Drawing.Color.Silver;
            this.buttonConnect.ButtonColor = System.Drawing.Color.Red;
            this.buttonConnect.FlatAppearance.BorderSize = 0;
            this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConnect.Location = new System.Drawing.Point(100, 50);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.OnHoverBorderColor = System.Drawing.Color.Gray;
            this.buttonConnect.OnHoverButtonColor = System.Drawing.Color.Yellow;
            this.buttonConnect.OnHoverTextColor = System.Drawing.Color.Gray;
            this.buttonConnect.Size = new System.Drawing.Size(500, 100);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "buttonConnect";
            this.buttonConnect.TextColor = System.Drawing.Color.White;
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // buttonCreate
            // 
            this.buttonCreate.BackColor = System.Drawing.Color.Transparent;
            this.buttonCreate.BorderColor = System.Drawing.Color.Silver;
            this.buttonCreate.ButtonColor = System.Drawing.Color.Red;
            this.buttonCreate.FlatAppearance.BorderSize = 0;
            this.buttonCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCreate.Location = new System.Drawing.Point(100, 200);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.OnHoverBorderColor = System.Drawing.Color.Gray;
            this.buttonCreate.OnHoverButtonColor = System.Drawing.Color.Yellow;
            this.buttonCreate.OnHoverTextColor = System.Drawing.Color.Gray;
            this.buttonCreate.Size = new System.Drawing.Size(500, 100);
            this.buttonCreate.TabIndex = 1;
            this.buttonCreate.Text = "buttonCreate";
            this.buttonCreate.TextColor = System.Drawing.Color.White;
            this.buttonCreate.UseVisualStyleBackColor = false;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackColor = System.Drawing.Color.Transparent;
            this.buttonExit.BorderColor = System.Drawing.Color.Silver;
            this.buttonExit.ButtonColor = System.Drawing.Color.Red;
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Location = new System.Drawing.Point(100, 350);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.OnHoverBorderColor = System.Drawing.Color.Gray;
            this.buttonExit.OnHoverButtonColor = System.Drawing.Color.Yellow;
            this.buttonExit.OnHoverTextColor = System.Drawing.Color.Gray;
            this.buttonExit.Size = new System.Drawing.Size(500, 100);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "buttonExit";
            this.buttonExit.TextColor = System.Drawing.Color.White;
            this.buttonExit.UseVisualStyleBackColor = false;
            // 
            // FormBasic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonConnect);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBasic";
            this.Text = "FormBasic";
            this.ResumeLayout(false);

        }

        #endregion

        private ButtonRounded buttonConnect;
        private ButtonRounded buttonCreate;
        private ButtonRounded buttonExit;
    }
}
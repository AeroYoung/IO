namespace ExpertLib.Dialogs
{
    partial class SplashScreen
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
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.remark = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(137, 135);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(53, 12);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "状态信息";
            // 
            // pnlStatus
            // 
            this.pnlStatus.Location = new System.Drawing.Point(17, 96);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(395, 27);
            this.pnlStatus.TabIndex = 2;
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.ForeColor = System.Drawing.Color.White;
            this.lblTimeRemaining.Location = new System.Drawing.Point(17, 135);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(53, 12);
            this.lblTimeRemaining.TabIndex = 3;
            this.lblTimeRemaining.Text = "时间剩余";
            // 
            // remark
            // 
            this.remark.AutoSize = true;
            this.remark.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.remark.ForeColor = System.Drawing.Color.White;
            this.remark.Location = new System.Drawing.Point(16, 66);
            this.remark.Name = "remark";
            this.remark.Size = new System.Drawing.Size(40, 16);
            this.remark.TabIndex = 5;
            this.remark.Text = "备注";
            this.remark.Visible = false;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("华文细黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.title.ForeColor = System.Drawing.Color.White;
            this.title.Location = new System.Drawing.Point(123, 18);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(66, 26);
            this.title.TabIndex = 4;
            this.title.Text = "TITLE";
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(424, 166);
            this.Controls.Add(this.remark);
            this.Controls.Add(this.title);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.lblStatus);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblTimeRemaining;
        private System.Windows.Forms.Label remark;
        private System.Windows.Forms.Label title;
    }
}
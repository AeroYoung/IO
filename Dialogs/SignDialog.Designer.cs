namespace ExpertLib.Dialogs
{
    partial class SignDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignDialog));
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.titleTextBox1 = new ExpertLib.Controls.TitleTextBox();
            this.titleCombox1 = new ExpertLib.Controls.TitleCombox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(74, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 29);
            this.button1.TabIndex = 2;
            this.button1.Text = "登录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.button1_KeyPress);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(214, 85);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(91, 20);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "记住密码";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // titleTextBox1
            // 
            this.titleTextBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleTextBox1.InputType = ExpertLib.Controls.TextInputType.NotControl;
            this.titleTextBox1.LabelWidth = 60;
            this.titleTextBox1.Location = new System.Drawing.Point(12, 46);
            this.titleTextBox1.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.titleTextBox1.Multiline = false;
            this.titleTextBox1.Name = "titleTextBox1";
            this.titleTextBox1.Padding = new System.Windows.Forms.Padding(2);
            this.titleTextBox1.PasswordChar = '*';
            this.titleTextBox1.PromptText = "默认密码 123456";
            this.titleTextBox1.RegexPattern = "";
            this.titleTextBox1.Size = new System.Drawing.Size(293, 28);
            this.titleTextBox1.TabIndex = 1;
            this.titleTextBox1.Title = "密码";
            // 
            // titleCombox1
            // 
            this.titleCombox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleCombox1.LabelWidth = 60;
            this.titleCombox1.Location = new System.Drawing.Point(12, 12);
            this.titleCombox1.Name = "titleCombox1";
            this.titleCombox1.Padding = new System.Windows.Forms.Padding(2);
            this.titleCombox1.Size = new System.Drawing.Size(293, 28);
            this.titleCombox1.TabIndex = 0;
            this.titleCombox1.Title = "账户";
            // 
            // FormSignIn
            // 
            this.ClientSize = new System.Drawing.Size(330, 115);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.titleTextBox1);
            this.Controls.Add(this.titleCombox1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSignIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.FormSignIn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ExpertLib.Controls.TitleTextBox titleTextBox1;
        private System.Windows.Forms.Button button1;
        private ExpertLib.Controls.TitleCombox titleCombox1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
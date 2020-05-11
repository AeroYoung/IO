namespace ExpertLib.Controls
{
    partial class TitleTextBox
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxEx1 = new ExpertLib.Controls.TextBoxEx();
            this.SuspendLayout();
            // 
            // textBoxEx1
            // 
            this.textBoxEx1.DecLength = 2;
            this.textBoxEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEx1.InputType = ExpertLib.Controls.TextInputType.NotControl;
            this.textBoxEx1.Location = new System.Drawing.Point(102, 2);
            this.textBoxEx1.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.textBoxEx1.MinValue = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.textBoxEx1.MyRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.textBoxEx1.Name = "textBoxEx1";
            this.textBoxEx1.OldText = null;
            this.textBoxEx1.PromptColor = System.Drawing.Color.Gray;
            this.textBoxEx1.PromptFont = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textBoxEx1.PromptText = "";
            this.textBoxEx1.RegexPattern = "";
            this.textBoxEx1.Size = new System.Drawing.Size(234, 23);
            this.textBoxEx1.TabIndex = 1;
            // 
            // TitleTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.Controls.Add(this.textBoxEx1);
            this.Name = "TitleTextBox";
            this.Size = new System.Drawing.Size(338, 28);
            this.Title = "文本";
            this.Controls.SetChildIndex(this.textBoxEx1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public TextBoxEx textBoxEx1;
    }
}

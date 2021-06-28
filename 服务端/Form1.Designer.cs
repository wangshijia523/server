namespace 服务端
{
    partial class Form1
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.button1 = new System.Windows.Forms.Button();
			this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
			this.log = new System.Windows.Forms.TextBox();
			this.listOnline = new System.Windows.Forms.ListBox();
			this.skinGroupBox2 = new CCWin.SkinControl.SkinGroupBox();
			this.monitorButton = new CCWin.SkinControl.SkinButton();
			this.button2 = new System.Windows.Forms.Button();
			this.skinGroupBox1.SuspendLayout();
			this.skinGroupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(197, 325);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// skinGroupBox1
			// 
			this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
			this.skinGroupBox1.BorderColor = System.Drawing.Color.Red;
			this.skinGroupBox1.Controls.Add(this.log);
			this.skinGroupBox1.ForeColor = System.Drawing.Color.Blue;
			this.skinGroupBox1.Location = new System.Drawing.Point(11, 42);
			this.skinGroupBox1.Name = "skinGroupBox1";
			this.skinGroupBox1.RectBackColor = System.Drawing.Color.White;
			this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
			this.skinGroupBox1.Size = new System.Drawing.Size(457, 228);
			this.skinGroupBox1.TabIndex = 3;
			this.skinGroupBox1.TabStop = false;
			this.skinGroupBox1.Text = "通讯记录";
			this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Red;
			this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.White;
			this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
			// 
			// log
			// 
			this.log.Location = new System.Drawing.Point(6, 20);
			this.log.Multiline = true;
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(438, 194);
			this.log.TabIndex = 0;
			// 
			// listOnline
			// 
			this.listOnline.FormattingEnabled = true;
			this.listOnline.ItemHeight = 12;
			this.listOnline.Location = new System.Drawing.Point(6, 20);
			this.listOnline.Name = "listOnline";
			this.listOnline.Size = new System.Drawing.Size(137, 196);
			this.listOnline.TabIndex = 0;
			// 
			// skinGroupBox2
			// 
			this.skinGroupBox2.BackColor = System.Drawing.Color.Transparent;
			this.skinGroupBox2.BorderColor = System.Drawing.Color.Red;
			this.skinGroupBox2.Controls.Add(this.listOnline);
			this.skinGroupBox2.ForeColor = System.Drawing.Color.Blue;
			this.skinGroupBox2.Location = new System.Drawing.Point(474, 42);
			this.skinGroupBox2.Name = "skinGroupBox2";
			this.skinGroupBox2.RectBackColor = System.Drawing.Color.White;
			this.skinGroupBox2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
			this.skinGroupBox2.Size = new System.Drawing.Size(155, 228);
			this.skinGroupBox2.TabIndex = 4;
			this.skinGroupBox2.TabStop = false;
			this.skinGroupBox2.Text = "分控在线列表";
			this.skinGroupBox2.TitleBorderColor = System.Drawing.Color.Red;
			this.skinGroupBox2.TitleRectBackColor = System.Drawing.Color.White;
			this.skinGroupBox2.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
			// 
			// monitorButton
			// 
			this.monitorButton.BackColor = System.Drawing.Color.Transparent;
			this.monitorButton.ControlState = CCWin.SkinClass.ControlState.Normal;
			this.monitorButton.DownBack = null;
			this.monitorButton.Location = new System.Drawing.Point(554, 276);
			this.monitorButton.MouseBack = null;
			this.monitorButton.Name = "monitorButton";
			this.monitorButton.NormlBack = null;
			this.monitorButton.Size = new System.Drawing.Size(75, 23);
			this.monitorButton.TabIndex = 5;
			this.monitorButton.Text = "监听任务";
			this.monitorButton.UseVisualStyleBackColor = false;
			this.monitorButton.Click += new System.EventHandler(this.monitorButton_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(326, 325);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(649, 315);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.monitorButton);
			this.Controls.Add(this.skinGroupBox2);
			this.Controls.Add(this.skinGroupBox1);
			this.Controls.Add(this.button1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "服务端";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.skinGroupBox1.ResumeLayout(false);
			this.skinGroupBox1.PerformLayout();
			this.skinGroupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox1;
        private System.Windows.Forms.ListBox listOnline;
        private System.Windows.Forms.TextBox log;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox2;
        private CCWin.SkinControl.SkinButton monitorButton;
        private System.Windows.Forms.Button button2;
    }
}


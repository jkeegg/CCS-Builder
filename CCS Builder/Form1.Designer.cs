namespace CCS_Builder
{
    partial class FormMainWindow
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainWindow));
            this.BuildButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.backgroundWorkerExec = new System.ComponentModel.BackgroundWorker();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.comboBoxProjectPath = new System.Windows.Forms.ComboBox();
            this.toolTipPath = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // BuildButton
            // 
            this.BuildButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BuildButton.Location = new System.Drawing.Point(137, 56);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(106, 23);
            this.BuildButton.TabIndex = 0;
            this.BuildButton.Text = "Build";
            this.toolTipPath.SetToolTip(this.BuildButton, "Build project(F5)");
            this.BuildButton.UseVisualStyleBackColor = true;
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ProjectPath";
            // 
            // SelectButton
            // 
            this.SelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectButton.Location = new System.Drawing.Point(12, 56);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(109, 23);
            this.SelectButton.TabIndex = 3;
            this.SelectButton.Text = "Select";
            this.toolTipPath.SetToolTip(this.SelectButton, "Select a project directory");
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(12, 85);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(231, 258);
            this.textBoxLog.TabIndex = 4;
            this.toolTipPath.SetToolTip(this.textBoxLog, "Double click to popup");
            this.textBoxLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textBoxLog_MouseDoubleClick);
            // 
            // backgroundWorkerExec
            // 
            this.backgroundWorkerExec.WorkerReportsProgress = true;
            this.backgroundWorkerExec.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExec_DoWork);
            this.backgroundWorkerExec.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerExec_ProgressChanged);
            this.backgroundWorkerExec.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerExec_RunWorkerCompleted);
            // 
            // buttonHelp
            // 
            this.buttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHelp.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHelp.Location = new System.Drawing.Point(168, 3);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(75, 23);
            this.buttonHelp.TabIndex = 5;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // comboBoxProjectPath
            // 
            this.comboBoxProjectPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxProjectPath.FormattingEnabled = true;
            this.comboBoxProjectPath.Location = new System.Drawing.Point(12, 30);
            this.comboBoxProjectPath.Name = "comboBoxProjectPath";
            this.comboBoxProjectPath.Size = new System.Drawing.Size(231, 20);
            this.comboBoxProjectPath.TabIndex = 6;
            this.comboBoxProjectPath.MouseHover += new System.EventHandler(this.comboBoxProjectPath_MouseHover);
            this.comboBoxProjectPath.MouseMove += new System.Windows.Forms.MouseEventHandler(this.comboBoxProjectPath_MouseMove);
            // 
            // FormMainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 355);
            this.Controls.Add(this.comboBoxProjectPath);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BuildButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FormMainWindow";
            this.Text = "CCS Builder";
            this.TopMost = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxProjectPath_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxProjectPath_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMainWindow_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BuildButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExec;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ComboBox comboBoxProjectPath;
        private System.Windows.Forms.ToolTip toolTipPath;
    }
}


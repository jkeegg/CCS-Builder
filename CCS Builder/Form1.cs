using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;



namespace CCS_Builder
{
    public partial class FormMainWindow : Form
    {
        FromDetail dlgDetail;
        public FormMainWindow()
        {
            InitializeComponent();
            dlgDetail = new FromDetail(this.textBoxLog);

            string[] currentPaths = Settings1.Default.workspace_path.Split('?');

            foreach (string a in currentPaths)
            {
                if (a != null && Directory.Exists(a))
                {
                    bool dup = false;
                    foreach (string b in this.comboBoxProjectPath.Items)
                    {
                        if (b == a)
                            dup = true;
                    }

                    if (!dup)
                        this.comboBoxProjectPath.Items.Add(a); 
                }
                    
            }
            comboBoxProjectPath.SelectedIndex = comboBoxProjectPath.Items.Count - 1;
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.comboBoxProjectPath.Text))
            {
                bool dup = false;
                foreach (string a in this.comboBoxProjectPath.Items)
                {
                    if (a == this.comboBoxProjectPath.Text)
                        dup = true;
                }

                if (!dup)
                {
                    this.comboBoxProjectPath.Items.Add(this.comboBoxProjectPath.Text);
                    Settings1.Default.workspace_path += "?";
                    Settings1.Default.workspace_path += this.comboBoxProjectPath.Text;
                    Settings1.Default.Save();
                }
            }
            string default_exe = Settings1.Default.eclipsec_path;
            if (!File.Exists(default_exe) || (Control.ModifierKeys == Keys.Shift))
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Multiselect = false;
                fileDialog.Filter = "可执行文件(*.exe)|*.exe";
                fileDialog.Title = "请选择eclipsec.exe所在路径";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    default_exe = fileDialog.FileName;
                    Settings1.Default.eclipsec_path = default_exe;
                    Settings1.Default.Save();
                }
                else
                    return;
            }
            bwArgs args = new bwArgs();
            args.ws_path = comboBoxProjectPath.Text;
            args.eclipsec_path = default_exe;
            BuildButton.Enabled = false;
            textBoxLog.Text = ">>>> Start call CCSv5 <<<<\r\n\r\n";
            backgroundWorkerExec.RunWorkerAsync(args);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderDialog = new FolderBrowserDialog();

            if (FolderDialog.ShowDialog() == DialogResult.OK)
            {
                this.comboBoxProjectPath.Text = FolderDialog.SelectedPath;
            }

            //OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.Multiselect = false;
            //fileDialog.Title = "请选择工作路径";
            //if (fileDialog.ShowDialog() == DialogResult.OK || fileDialog.ShowDialog() == DialogResult.Yes)
            //{
            //    this.textBoxProjectPath.Text = fileDialog.FileName;
            //}
           
        }

        private void textBoxProjectPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void textBoxProjectPath_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == "")  //判断文件类型，只接受txt文件
                {
                    comboBoxProjectPath.Text = file;
                    return;
                }
            }
            backgroundWorkerExec.ReportProgress(0, "Directory only!");
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            backgroundWorkerExec.ReportProgress(0, e.Data);
        }

        private void backgroundWorkerExec_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = (e.Argument as bwArgs).eclipsec_path;
                string tmp;
                string ws;
                string pj;
                string metadata;
                string path = (e.Argument as bwArgs).ws_path;
                if (path.LastIndexOf('\\') == -1)
                {
                    string msg;
                    msg = string.Format("Invalid Path: {0}", path);
                    backgroundWorkerExec.ReportProgress(0, msg);
                    //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ws = path.Substring(0, path.LastIndexOf('\\'));
                pj = path.Substring(path.LastIndexOf('\\'));
                pj = pj.Replace("\\", "");
                if (ws == "" || pj == "")
                {
                    string msg;
                    msg = string.Format("Invalid ProjectPath\r\nWorkspace: {0}\r\nProjectName: {1}\r\n", ws, pj);
                    backgroundWorkerExec.ReportProgress(0, msg);
                    //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                metadata = ws + "\\.metadata";
                if (!Directory.Exists(metadata))
                {
                    string msg;
                    msg = string.Format("Workspace: {0}\r\n isn't a valid CCSv5 workspace",ws);
                    backgroundWorkerExec.ReportProgress(0, msg);
                    //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                {
                    string msg;
                    msg = string.Format("Start with...\r\nWorkspace = {0}\r\nProject = {1}", ws, pj);
                    backgroundWorkerExec.ReportProgress(0, msg);
                }

                tmp = string.Format("-noSplash -data \"{0}\" -application com.ti.ccstudio.apps.projectBuild -ccs.autoOpen -ccs.configuration Release -ccs.buildType incremental -ccs.projects {1}", ws, pj);
                //backgroundWorkerExec.ReportProgress(0, string.Format("Compile command:\r\n{0}\r\n\r\n", tmp));
                p.StartInfo.Arguments = tmp;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                p.Start();
                p.BeginOutputReadLine();

                //p.StandardInput.WriteLine("ping 127.0.0.1 -n 3");

                //p.StandardInput.WriteLine("exit");

                //this.textBoxLog.Clear();
                //string tmpStr = p.StandardOutput.ReadLine();
                //while (tmpStr != null)
                //{
                //    this.textBoxLog.Text += tmpStr;
                //    //Application.DoEvents();
                //    tmpStr = p.StandardOutput.ReadLine();
                //}
                p.WaitForExit();
                p.Close();
            }
            catch (Win32Exception)
            {
                string msg = "";
                msg = string.Format("{0} 无法执行\r\n请选择正确的eclipsec.exe可执行文件\r\n", (e.Argument as bwArgs).eclipsec_path);
                backgroundWorkerExec.ReportProgress(0, msg);
                //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception  ex)
            { 
                string msg = "";
                msg = string.Format("执行失败\r\n{0}\r\n", ex);
                backgroundWorkerExec.ReportProgress(0, msg);
                //MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void backgroundWorkerExec_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBoxLog.AppendText("\r\n\r\n>>>> Finish call CCSv5 <<<<\r\n");
            BuildButton.Enabled = true;
        }

        private void backgroundWorkerExec_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                this.textBoxLog.AppendText(e.UserState as string);
                this.textBoxLog.AppendText("\r\n");
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("使用方法:\r\n1.选择或拖放CCSv5 Workspace目录下的工程目录到程序窗口\r\n2.点击Build编译工程\r\n3.程序默认扫描CCSv5安装路径,若找不到eclipsec.exe,点击Build会弹窗供选择,按住Shift点击Build,可强制选择\r\n4.双击日志窗口可弹出更大窗口显示日志\r\n\r\n当前配置:\r\neclipsec path = {0}\r\n\r\n\r\n\r\nCopyright © LiuChen. All Rights Reserved.", Settings1.Default.eclipsec_path), "帮助", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void textBoxLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dlgDetail.Show();
            dlgDetail.Activate();
        }

        private void comboBoxProjectPath_MouseMove(object sender, MouseEventArgs e)
        {
            //this.toolTipPath.SetToolTip(this.comboBoxProjectPath, this.comboBoxProjectPath.Text);
        }

        private void comboBoxProjectPath_MouseHover(object sender, EventArgs e)
        {
            this.toolTipPath.SetToolTip(this.comboBoxProjectPath, this.comboBoxProjectPath.Text);
        }

        private void FormMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                BuildButton.PerformClick();
            }
        }
    }
}


class bwArgs
{
    public string eclipsec_path;
    public string ws_path;
}
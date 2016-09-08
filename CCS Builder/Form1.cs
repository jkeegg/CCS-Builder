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
using System.Xml;

namespace CCS_Builder
{
    public partial class FormMainWindow : Form
    {
        DateTime startTime;

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
            args.build_configuration = comboBoxBuildConfiguration.Text;
            args.clean = false;
            if (Control.ModifierKeys == Keys.Control)
            {
                args.clean = true;
            }
            BuildButton.Enabled = false;
            startTime = DateTime.Now;
            textBoxLog.Text = ">>>> Start call CCSv5 <<<<\r\n\r\n";
            backgroundWorkerExec.RunWorkerAsync(args);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderDialog = new FolderBrowserDialog();

            if (FolderDialog.ShowDialog() == DialogResult.OK)
            {
                this.comboBoxProjectPath.Text = FolderDialog.SelectedPath;
                comboBoxProjectPath_SelectedIndexChanged(null, EventArgs.Empty);
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
                if (Directory.Exists(file))
                {
                    comboBoxProjectPath.Text = file;
                    comboBoxProjectPath_SelectedIndexChanged(null, EventArgs.Empty);
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

                string buildConfiguration = "";
                buildConfiguration = (e.Argument as bwArgs).build_configuration;
                if (buildConfiguration == "")
                {
                    buildConfiguration = "Release";
                }

                string buildMode = "incremental";
                if ((e.Argument as bwArgs).clean)
                {
                    buildMode = "clean";
                }

                {
                    string msg;
                    msg = string.Format("Start with...\r\nWorkspace = {0}\r\nProject = {1}\r\nConfiguration = {2}\r\nMode = {3}", ws, pj, buildConfiguration, buildMode);
                    backgroundWorkerExec.ReportProgress(0, msg);
                    //System.Threading.Thread.Sleep(1000);
                }

                tmp = string.Format("-noSplash -data \"{0}\" -application com.ti.ccstudio.apps.projectBuild -ccs.autoOpen -ccs.autoImport -ccs.configuration {1} -ccs.buildType {3} -ccs.projects {2}", ws, buildConfiguration, pj, buildMode);
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
            string timec = string.Format("\r\nTotal comsumed: {0}", (DateTime.Now - startTime).ToString(@"hh\:mm\:ss\.fff"));
            textBoxLog.AppendText(timec);
            BuildButton.Enabled = true;
            this.notifyIcon1.ShowBalloonTip(3000, "CCS Builder", "Operation completed", ToolTipIcon.Info);
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
            //this.notifyIcon1.ShowBalloonTip(1000, "当前时间：", DateTime.Now.ToLocalTime().ToString(), ToolTipIcon.Info);
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
        }

        private void comboBoxProjectPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string title = new DirectoryInfo(comboBoxProjectPath.Text).Name;
                this.Text = "CCS Builder - " + title;
                comboBoxBuildConfiguration.Items.Clear();
                comboBoxBuildConfiguration.Text = "";

                bool fDotcprojectFound = false;
                bool IsRemoteProject = false;
                // 搜索.cproject文件
                string dot_cproject_file_path = "";

//                 string[] dot_cproject_file_paths = Directory.GetFiles(comboBoxProjectPath.Text, ".cproject", SearchOption.TopDirectoryOnly);
//                 if (dot_cproject_file_paths.Count() == 0) 
//                     IsRemoteProject = true;
//                 else
//                 {
//                     dot_cproject_file_path = dot_cproject_file_paths[0];
//                     fDotcprojectFound = true;
//                 }

                if (File.Exists(comboBoxProjectPath.Text + @"\" + ".cproject"))
                {
                    dot_cproject_file_path = comboBoxProjectPath.Text + @"\" + ".cproject";
                    fDotcprojectFound = true;
                }
                else
                {
                    IsRemoteProject = true;
                }
                    

                if (IsRemoteProject)
                {
                    // 本地无.cproject文件，为远程Project，先通过本地.project找到.cproject可能存在的目录

//                     string[] dot_project_file_paths = Directory.GetFiles(comboBoxProjectPath.Text, ".project", SearchOption.TopDirectoryOnly);
//                     if (dot_project_file_paths.Count() == 0) return; // 也无.project文件，放弃
                    string dot_project_file_path = "";
                    if (File.Exists(comboBoxProjectPath.Text + @"\" + ".project"))
                    {
                        dot_project_file_path = comboBoxProjectPath.Text + @"\" + ".project";
                    }
                    else
                        return;

                   // string dot_project_file_path = dot_project_file_paths[0];

                    XmlDocument xml = new XmlDocument();
                    xml.Load(dot_project_file_path);
                    XmlNodeList nodes = xml.SelectNodes("projectDescription/variableList/variable/name");
                    foreach (XmlNode i in nodes)
                    {
                        if (i.InnerText == "copy_PARENT")
                        {
                            XmlNode value = i.ParentNode.SelectSingleNode("value");
                            string dot_cproject_dir_path = value.InnerText;
                            dot_cproject_dir_path = dot_cproject_dir_path.Replace("file:/", "");
                            if (Directory.Exists(dot_cproject_dir_path))
                            {
                                string[] files = System.IO.Directory.GetFiles(dot_cproject_dir_path, ".cproject", System.IO.SearchOption.AllDirectories);
                                if (files.Count() != 0)
                                {
                                    dot_cproject_file_path = files[0];
                                    fDotcprojectFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!fDotcprojectFound) return;

                XmlDocument xmlcproject = new XmlDocument();
                xmlcproject.Load(dot_cproject_file_path);
                XmlNodeList noodesk = xmlcproject.SelectNodes(@"cproject/storageModule/cconfiguration/storageModule");
                foreach (XmlNode k in noodesk)
                {
                    foreach (XmlAttribute attr in k.Attributes)
                    {
                        if (attr.Name == "moduleId" && attr.Value == "org.eclipse.cdt.core.settings")
                            foreach (XmlAttribute attr2 in k.Attributes)
                            {
                                if (attr2.Name == "name")
                                {
                                    //MessageBox.Show(attr2.Value);
                                    comboBoxBuildConfiguration.Items.Add(attr2.Value);
                                    comboBoxBuildConfiguration.SelectedIndex = comboBoxBuildConfiguration.Items.Count - 1;
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "";
                msg = string.Format("执行失败\r\n\r\n{0}\r\n", ex);
                backgroundWorkerExec.ReportProgress(0, msg);
            }
        }
    }
}


class bwArgs
{
    public string eclipsec_path;
    public string ws_path;
    public string build_configuration;
    public bool clean;
}
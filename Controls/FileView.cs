using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ExpertLib.Dialogs;

namespace ExpertLib.Controls
{
    public partial class FileView : UserControl
    {
        #region 属性

        private string _folder = Application.UserAppDataPath;

        [Category("自定义属性"), Description("文件夹地址")]
        public string Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                if (!_folder.EndsWith("\\"))
                    _folder += "\\";
                labelPath.Text = _folder;
                LoadView();
            }
        }

        [DefaultValue(true), Category("自定义属性"), Description("权限")]
        public bool Authority
        {
            set
            {
                if(DesignMode)
                    return;
                上传ToolStripMenuItem.Enabled = value;
                删除ToolStripMenuItem.Enabled = value;
            }
        }

        [DefaultValue(true), Category("自定义属性"), Description("Folder不存在时是否创建")]
        public bool CanCreateFolder { get; set; }

        #endregion

        #region 事件

        public delegate bool DelegateHandleFiles(string[] files);

        public DelegateHandleFiles BeforeUploadFiles;

        public DelegateHandleFiles BeforeDownloadFiles;

        #endregion

        public FileView()
        {
            InitializeComponent();
            if (DesignMode)
                return;

        }
        
        private void FileView_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            labelStatistic.Alignment = ToolStripItemAlignment.Right;
        }

        public void LoadView()
        {
            try
            {
                var dir = new DirectoryInfo(Folder);
                if (!dir.Exists)
                {
                    if (CanCreateFolder)
                        dir.Create();
                    else
                        return;
                }

                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    listView1.Items.Add(Path.GetFileName(file.Name));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("请联系管理员设置服务器共享文件夹属性", "提示");
                Log.e($"FileView {e}");
            }
        }

        public string[] SelectedFiles()
        {
            var files = new List<string>();

            var items = listView1.SelectedItems;
            foreach (ListViewItem item in items)
            {
                var name = item.SubItems[0].Text;
                files.Add($"{Folder}{name}");
            }

            return files.ToArray();
        }

        private void 上传ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "选择文件",
            };

            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            if (BeforeUploadFiles != null && !BeforeUploadFiles.Invoke(fileDialog.FileNames))
                return;
            
            try
            {
                SplashScreen.ShowSplashScreen("上传文件","");

                var dir = new DirectoryInfo(Folder);
                if (!dir.Exists)
                {
                    if (CanCreateFolder)
                    {
                        dir.Create();
                    }
                    else
                    {
                        MessageBox.Show($"文件夹不存在 {Folder}", "提示");
                        SplashScreen.CloseForm();
                        return;
                    }
                }

                foreach (var filePath in fileDialog.FileNames)
                {
                    var name = Path.GetFileName(filePath);
                    var target = $"{Folder}{name}";

                    if (File.Exists(target))
                    {
                        if (MessageBox.Show($"{name}已存在是否覆盖?", "提示", MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning) != DialogResult.OK)
                            continue;
                    }

                    File.Copy(filePath, target, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请联系管理员设置服务器共享文件夹属性", "提示");
                Log.e($"上传文件 {ex}");
            }
            finally
            {
                SplashScreen.CloseForm();
            }
            
            LoadView();
        }

        private void 下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var files = SelectedFiles();
            if (files.Length == 0)
            {
                MessageBox.Show("请选择文件", "提示");
                return;
            }

            if (BeforeDownloadFiles != null && !BeforeDownloadFiles.Invoke(files))
                return;

            foreach (var file in files)
            {
                var fileDialog = new SaveFileDialog
                {
                    Title = $"备份 {file}",
                    FileName = file,
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                };

                if (fileDialog.ShowDialog() != DialogResult.OK) continue;

                var targetPath = fileDialog.FileName;
                File.Copy(file, targetPath, true);
            }

            LoadView();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var files = SelectedFiles();
            if (files.Length == 0)
            {
                MessageBox.Show("请选择文件", "提示");
                return;
            }

            if (MessageBox.Show("删除后无法恢复,是否继续", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            try
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception e1)
            {
                Log.e($"删除ToolStripMenuItem_Click {e1}");
            }
            LoadView();
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Folder))
                System.Diagnostics.Process.Start("Explorer.exe", Folder);
            else
                MessageBox.Show($"{Folder}文件夹不存在", "提醒");
        }
    }
}

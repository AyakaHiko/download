using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace download
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Downloader.StartDownload += Downloader_StartDownload;
            Downloader.EndDownload += Downloader_EndDownload;
            Downloader.Error+= (error) => MessageBox.Show(error);
        }

        private void Downloader_EndDownload(string path, bool result)
        {
            Action end = () =>
             downloadPanel.Controls.RemoveByKey(path);
            if (InvokeRequired)
                Invoke(end);
            else
            {
                end();
            }

            var res = result ? "Download successfully!": "Download end!";
            MessageBox.Show(res);
        }

        private void Downloader_StartDownload(string path)
        {
            if (InvokeRequired)
                Invoke(new Action(() => _addDownload(path)));
            else
                _addDownload(path);
        }

        private async void downloadBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(addressBox.Text))
                return;
            await Downloader.DownloadAsync(addressBox.Text, _save());
        }

        private string _save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All | *.*";
            return saveFileDialog.ShowDialog() != DialogResult.OK ? null : saveFileDialog.FileName;
        }
        private void _addDownload(string path)
        {
            var panel = new FlowLayoutPanel();
            panel.AutoSize = true;
            panel.FlowDirection = FlowDirection.LeftToRight;
            Label pathLbl = new Label();
            pathLbl.Text = path;
            panel.Controls.Add(pathLbl);
            ProgressBar bar = new ProgressBar();
            bar.Style = ProgressBarStyle.Marquee;
            panel.Controls.Add(bar);
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Click += (sender, args) =>
            {
                Downloader.CancelDownload(path);
            };
            panel.Controls.Add(cancelButton);
            panel.Name = path;
            downloadPanel.Controls.Add(panel);
        }
    }
}

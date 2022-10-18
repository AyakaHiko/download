using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace download
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            _downloadAsync();
        }

        private Task _writeAsync(byte[] data, string path) => Task.Factory.StartNew(() => _write(data, path));
        private void _write(byte[] data, string path)
        {
            if (data == null)
                return;

            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        private Task _downloadAsync() => Task.Factory.StartNew(_download);
        private void _download()
        {

            try
            {
                if (string.IsNullOrEmpty(addressBox.Text))
                    return;
                WebClient webClient = new WebClient();
                var data = webClient.DownloadData(addressBox.Text);

                Func<string> save = () =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "All | *.*";
                    return saveFileDialog.ShowDialog() != DialogResult.OK ? null : saveFileDialog.FileName;
                };
                string path= InvokeRequired ? Invoke(save).ToString() : save();
                
                _write(data, path);
            }
            catch (Exception e)
            {

            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace download
{
    public static class Downloader
    {
        static Downloader()
        {
            EndDownload += (path, result) => _downloads.Remove(path);
        }

        public static event Action<string> Error;
        public static event Action<string> StartDownload;
        public static event Action<string, bool> EndDownload;
        private static Dictionary<string, CancellationTokenSource> _downloads = new Dictionary<string, CancellationTokenSource>();
        private static async void _write(byte[] data, string path, CancellationToken token)
        {
            if (data == null)
            {
                Error?.Invoke("Writing error");
                return;
            }
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                if (!token.IsCancellationRequested)
                    await stream.WriteAsync(data, 0, data.Length, token);
            }
            if (token.IsCancellationRequested)
            {
                Error?.Invoke("Cancelled");
                EndDownload?.Invoke(path, false);
                return;
            }
            EndDownload?.Invoke(path, true);
        }

        public static Task DownloadAsync(string source, string path) => Task.Factory.StartNew(() => Download(source, path));
        public static void Download(string source, string path)
        {
            try
            {
                WebClient webClient = new WebClient();
                var data = webClient.DownloadData(source);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                _downloads.Add(path, cancellationTokenSource);
                StartDownload?.Invoke(path);
                _write(data, path, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Error?.Invoke(e.Message);
            }
        }

        public static void CancelDownload(string path)
        {
            _downloads[path].Cancel();
        }
    }
}
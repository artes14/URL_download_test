using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Http;

namespace URL_download
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await DownloadAsync();

        }

        private async Task DownloadAsync()
        {
            using (var client = new WebClient())
            {
                var httpclient = new HttpClient();
                client.DownloadFileCompleted += (s, e) => label1.Text = "Download file completed.";
                client.DownloadProgressChanged += (s, e) => progressBar1.Value = e.ProgressPercentage;
                client.DownloadProgressChanged += (s, e) => label1.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                client.DownloadProgressChanged += (s, e) =>
                {
                    label2.Text = "%" + e.ProgressPercentage.ToString();
                    label2.Left = Math.Min(
                        (int)(progressBar1.Left + e.ProgressPercentage / 100f * progressBar1.Width),
                        progressBar1.Width - label2.Width
                    );
                }; 
                string foldername = "CV04";
                string filePath = Application.StartupPath + @"\\" + foldername + "\\fileList.txt";
                string uri = "http://192.168.0.10:5000/FileList";
                client.DownloadFile(uri, filePath);
                string[] filenames = File.ReadAllLines(filePath);
                string downURI = "http://192.168.0.10:5000/Download";
                if (filenames.Length > 0)
                {
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        client.Headers.Clear();
                        Dictionary<string, string> filedict = new Dictionary<string, string>();
                        filedict.Add("file", filenames[i]);

                        client.Headers.Add("file", filenames[i]);
                        await client.DownloadFileTaskAsync(downURI, Application.StartupPath + @"\" + foldername + @"\" + filenames[i]);
                        //HttpContent cont = new StringContent("{\"file\":" + filenames[i] + "}");
                        //var response = await httpclient.PostAsync(downURI, cont);
                        //var stream = await response.Content.ReadAsStreamAsync();
                        //var fs = new FileStream(Application.StartupPath + @"\" + foldername + @"\" + filenames[i], FileMode.OpenOrCreate);
                        //await stream.CopyToAsync(fs);
                    }
                }
            }

        }
    }
}

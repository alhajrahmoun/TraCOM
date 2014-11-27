using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace Receiver
{
    public partial class Form1 : Form
    {
        public delegate void updateUIDelegate(int current, int total, string status);
        public updateUIDelegate update;
        FileStream fs;
        string downloadedFile;
        string fileName;
        string fileExtension;
        string fileSize;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Hide();
            button3.Hide();
            label3.Hide();
            label4.Hide();
            label1.Text = "Not Connected";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(downloadedFile);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            this.update = new updateUIDelegate(updateUI);
            progressBar1.Hide();
            label1.Text = "Idle";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            fileName = serialPort1.ReadLine();
            fileExtension = serialPort1.ReadLine();
            fileSize = serialPort1.ReadLine();
            int size = Convert.ToInt32(fileSize);
            string initialPath = @"C:\";
            string fullPath = initialPath + fileName + fileExtension;
            downloadedFile = fullPath;
            fs = new FileStream(fullPath, FileMode.Create);
            byte[] b1 = new byte[size];
            string status = "Downloading: " + fileName;
            label3.Invoke((MethodInvoker)delegate { label3.Show(); });
            label4.Invoke((MethodInvoker)delegate { label4.Show(); });
            for (int i = 0; i < b1.Length; )
            {
                i += serialPort1.Read(b1, i, b1.Length - i);
                progressBar1.Invoke(this.update, new object[] { i, b1.Length, status });
            }
            label1.Invoke((MethodInvoker)delegate { label1.Text = "File Downloaded"; });
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Hide(); });
            button3.Invoke((MethodInvoker)delegate { button3.Show(); });
            fs.Write(b1, 0, b1.Length);
            fs.Close();
            serialPort1.Close();
        }

        public void updateUI(int current, int total, string status)
        {
            progressBar1.Show();
            progressBar1.Maximum = total;
            progressBar1.Value = current;
            progressBar1.Refresh();
            label1.Text = status;
            label4.Text = (current / 1024).ToString() + "/" + (total / 1024).ToString() + " KB";
        }
    }
}

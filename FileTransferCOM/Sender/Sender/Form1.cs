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

namespace Sender
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        OpenFileDialog openDialog;
        BinaryReader br;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            label1.Text = "Idle";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            else
            {
                openDialog = new OpenFileDialog();
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    label1.Text = "Uploading file..";
                    button5.Enabled = false;
                    bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Completed);
                    bw.RunWorkerAsync(openDialog);
                }
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            openDialog = (OpenFileDialog)e.Argument;
            FileInformation fileInfo = new FileInformation();
            string filePath = System.IO.Path.GetFullPath(openDialog.FileName);
            fileInfo.Bytes = File.ReadAllBytes(filePath);
            int size = fileInfo.Bytes.Length;
            fileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            fileInfo.Extension = System.IO.Path.GetExtension(filePath);
            FileStream fs = new FileStream(filePath, FileMode.Open);
            br = new BinaryReader(fs);
            serialPort1.Close();
            serialPort1.Open();
            serialPort1.DiscardInBuffer();
            serialPort1.WriteLine(fileInfo.FileName);
            serialPort1.WriteLine(fileInfo.Extension);
            serialPort1.WriteLine(size.ToString());

            byte[] b1 = br.ReadBytes((int)fs.Length);
            serialPort1.Write(b1, 0, b1.Length);

            br.Close();
            fs.Dispose();
            fs.Close();
            serialPort1.Close();
        }

        void bw_Completed(object sender, RunWorkerCompletedEventArgs e)
        {

            label1.Text = "File Uploaded ..";
            button5.Enabled = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Not Connected";
        }

    }
}

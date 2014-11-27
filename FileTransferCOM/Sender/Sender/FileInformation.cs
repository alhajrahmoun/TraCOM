using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sender
{
    class FileInformation
    {
        private string fileName;
        private string ext;
        private byte[] b;

        public string Extension
        {
            get { return ext; }
            set { ext = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public byte[] Bytes
        {
            get { return b; }
            set { b = value; }
        }
    }
}

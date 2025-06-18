using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class ConnectDialog : Form
    {
        public string ip;
        public int port;
        public string username;
        public ConnectDialog()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(ip, out IPAddress? wht) == true && !string.IsNullOrEmpty(txtUserName.Text)) 
            {
                ip = txtIp.Text;
                port = int.Parse(txtPort.Text);
                username = txtUserName.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Nije dobar ip ili nisi uneo ime");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClientForm
{
    public partial class FormConnect : Form
    {
        //static string userName;
        //private const string ip = "127.0.0.1";
        //private const int port = 25565;
        //static TcpClient client;
        //static NetworkStream stream;
        private static bool ValidateIPv4(string ipString)
        {
            if (ipString.Count(c => c == '.') != 3) return false;
            IPAddress address;
            return IPAddress.TryParse(ipString, out address);
        }
        public FormConnect()
        {
            InitializeComponent();
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(IPtextBox.Text) && !String.IsNullOrEmpty(PorttextBox.Text) && !String.IsNullOrEmpty(NickNameTextBox.Text) && ValidateIPv4(IPtextBox.Text))
            {
                //NickNameTextBox.Text;           

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPtextBox.Text), Convert.ToInt16(PorttextBox.Text));
                Form1 Parent = (Form1)this.Owner;
                Parent.GetFields(ipPoint, NickNameTextBox.Text);
                this.Close();
               
            }
        }
    }
}

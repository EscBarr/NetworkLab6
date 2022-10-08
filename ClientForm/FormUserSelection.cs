using Lab6Dependecies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClientForm
{
    public partial class FormUserSelection : Form
    {
        List<ClientInfo> Allclients;
        public FormUserSelection(List<ClientInfo> clients)
        {
            InitializeComponent();
            Allclients = clients;
           
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count >= 1 && !String.IsNullOrEmpty(textBox1.Text))
            {
                List<ClientInfo> Selected = new List<ClientInfo>();
                foreach (ListViewItem item in listView1.Items)
                {
                   Selected.Add(Allclients.First(S => S.ClientId.ToString() == item.SubItems[1].Text));
                }
                Form1 Parent = (Form1)this.Owner;
                Parent.CreateChat(textBox1.Text, Selected);
                this.Close();
            }
        }

        private void FormUserSelection_Load(object sender, EventArgs e)
        {
            this.listView1.Invoke((MethodInvoker)delegate {
                listView1.Items.Clear();
                // Running on the UI thread
                foreach (var item in Allclients)
                {
                    listView1.Items.Add(item.Name).SubItems.Add(item.ClientId.ToString());

                }
            });
        }
    }
}

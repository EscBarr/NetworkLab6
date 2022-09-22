using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClientForm
{
    [Serializable]
    public record ClientInfo
    {

        public Guid ClientId { get; set; }
        public string Name { get; set; }
    }

    public partial class Form1 : Form
    {
        static string userName;
        private const string ip = "127.0.0.1";
        private const int port = 25565;
        public IPEndPoint ipPoint { get; set; }
        static TcpClient client;
        static NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        public void GetFields(IPEndPoint IPPoint, string UserName)
        {
            ipPoint = IPPoint;
            userName = UserName;
            client = new TcpClient();
            Sendbutton.Enabled = true;
            try
            {
                client.Connect(ipPoint);
                stream = client.GetStream(); // �������� �����
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
                // ��������� ����� ����� ��� ��������� ������
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //����� ������
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ConnectToServer_Click(object sender, EventArgs e)
        {
            FormConnect newForm = new FormConnect();
            // passing this in ShowDialog will set the .Owner 
            // property of the child form
            newForm.TopMost = true;
            newForm.Show(this);
         
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox dynamictextbox = new TextBox();
            dynamictextbox.Dock = DockStyle.Fill;
            dynamictextbox.Multiline = true;
            dynamictextbox.ReadOnly = true;
            dynamictextbox.Name = "dynamictextbox_" + tabControl1.TabPages[0].Name;

            tabControl1.TabPages[0].Controls.Add(dynamictextbox);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
         
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TextBox dynamictextbox = new TextBox();
            dynamictextbox.Dock = DockStyle.Fill;
            dynamictextbox.Multiline = true;
            dynamictextbox.ReadOnly = true;
            dynamictextbox.Name = "dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name;
           
            tabControl1.TabPages[tabControl1.SelectedIndex].Controls.Add(dynamictextbox);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //textBoxForChat.Text = "(Enter some text)";
            //tabControl1.TabPages[tabControl1.SelectedIndex].Controls.Add(textBoxForChat);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //textBoxForChat.Text = "(Enter some text)";
            //tabControl1.TabPages[tabControl1.SelectedIndex].Controls.Add(textBoxForChat);
        }

        // �������� ���������
        //void SendMessage()
        //{
        //    Console.WriteLine("������� ���������: ");

        //    while (true)
        //    {
               
        //    }
        //}
        // ��������� ���������
        void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[4096]; // ����� ��� ���������� ������
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                    }
                    while (stream.DataAvailable);
                    data = TrimEnd(data);
                    try
                    {
                        var Test = ByteArrayToObject(data);
                        fillListView((List<ClientInfo>)Test);
                    }
                    catch (Exception ex)//����� ��������� � ���
                    {
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        string message = builder.ToString();
                        this.tabControl1.Invoke((MethodInvoker)delegate {
                            // Running on the UI thread
                            ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                            ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                        });
                    }

                }
                catch
                {
                    Console.WriteLine("����������� ��������!"); //���������� ���� ��������
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }

        void Disconnect()
        {
            if (stream != null)
                stream.Close();//���������� ������
            if (client != null)
                client.Close();//���������� �������
            Environment.Exit(0); //���������� ��������
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                byte[] data = Encoding.Unicode.GetBytes(textBox1.Text);
                stream.Write(data, 0, data.Length);
            }
            
        }

        private void fillListView(List<ClientInfo> clientInfos)
        {
            this.listView1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                foreach (var item in clientInfos)
                {
                    listView1.Items.Add(item.Name);
                }
            });
           
        }

        // Convert a byte array to an Object
        private List<ClientInfo> ByteArrayToObject(byte[] arrBytes)
        {

            List<ClientInfo>? obj = JsonSerializer.Deserialize<List<ClientInfo>>(arrBytes);

            return obj;
        }
    }
}
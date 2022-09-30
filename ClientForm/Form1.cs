using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System;
using Lab6Dependecies;

namespace ClientForm
{
    //[Serializable]
    //public record ClientInfo
    //{

    //    public Guid ClientId { get; set; }
    //    public string Name { get; set; }
    //}

    public partial class Form1 : Form
    {
        static string userName;
        public IPEndPoint ipPoint { get; set; }
        static TcpClient client;
        static NetworkStream stream;
        public Dictionary<int, List<ClientInfo>> AllChatsClients = new Dictionary<int, List<ClientInfo>>();
        public Dictionary<int, string> ChatsNames = new Dictionary<int, string>();//�������� ���� ����� � ����� ID ������� ����� ������
        public Dictionary<int, List<string>> ChatsHistory = new Dictionary<int, List<string>>();//�������� ������� �����

        public List<ClientInfo> MainChat = new List<ClientInfo>();
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
                byte[] data = Encoding.UTF8.GetBytes(message);
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

        public void InitiateChat(string chatName, List<ClientInfo> clientInfos)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox dynamictextbox = new TextBox();
            dynamictextbox.Dock = DockStyle.Fill;
            dynamictextbox.Multiline = true;
            dynamictextbox.ReadOnly = true;
            dynamictextbox.Name = "dynamictextbox_" + tabControl1.TabPages[0].Name;

            tabControl1.TabPages[0].Controls.Add(dynamictextbox);
            var Temp = new List<string>();
            ChatsHistory.Add(-1, Temp);
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

        private string GetMessage()//��������� ��������� ���������� � ��������� ���/������ � ����� ������� ��������� ���������
        {
            byte[] data = new byte[64]; // ����� ��� ���������� ������
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }

        private void HandleMessageType()
        {
            string message = GetMessage();//�������� JSON ���������
            var MessageHeader = MessageHandler.StringToObject<PacketInfo>(message);//����������� � ������
            switch (MessageHeader.Type)//��������� � ����������� �� ������������� ������������� ���������
            {
                case MessageTypes.Text:
                    HandleMessages(MessageHeader);
                    break;
                case MessageTypes.File:
                    HandleFile(MessageHeader);
                    break;
                case MessageTypes.ChatCreation:
                    HandleChatCreation();
                    break;
                case MessageTypes.UserList:
                    HandleUserList(MessageHeader);
                    break;
                case MessageTypes.P2PChat:
                    HandleP2PChatCreation();
                    break;
            }
        }

        private byte[] GetMessageWithSize(int size)//��������� ��������� ���������� � ��������� ���/������
        {
            byte[] data = new byte[size]; // ����� ��� ���������� ������
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
            }
            while (stream.DataAvailable);

            return data;
        }

        private void HandleMessages(PacketInfo messageHeader)
        {
            var message = GetMessage();
            ChatsHistory[-1].Add(message);
            this.tabControl1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
            });
        }

        private void HandleP2PChatCreation()
        {
            throw new NotImplementedException();
        }

        private void HandleUserList(PacketInfo messageHeader)
        {
            var data = GetMessageWithSize((int)messageHeader.Size);
            if (messageHeader.ChatID == -1)
            {
                MainChat = MessageHandler.ByteArrayToObject<List<ClientInfo>>(data);
                fillListView(MainChat);
            }
            else
            {

            }
            

        }

        private void HandleChatCreation()
        {
            throw new NotImplementedException();
        }

        private void HandleFile(PacketInfo messageHeader)
        {
            throw new NotImplementedException();
        }

        void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    HandleMessageType();
                }
                catch
                {
                    Console.WriteLine("����������� ��������!"); //���������� ���� ��������
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        //public static byte[] TrimEnd(byte[] array,int size)
        //{
        //    //int lastIndex = Array.FindLastIndex(array, b => b != 0);

        //    Array.Resize(ref array, size  + 1);

        //    return array;
        //}

        void Disconnect()
        {
            if (stream != null)
                stream.Close();//���������� ������
            if (client != null)
                client.Close();//���������� �������
            //Environment.Exit(0); //���������� ��������
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                byte[] data = Encoding.UTF8.GetBytes(textBox1.Text);
                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text, data.Length, -1);//�������������� ���������
                stream.Write(MessageHeader,0, MessageHeader.Length);
                Task.Delay(10);
                stream.Write(data, 0, data.Length);
                var message = String.Format("{0}: {1}", userName, textBox1.Text);
                this.tabControl1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                });
                textBox1.Text = "";
            }
            
        }

        private void fillListView(List<ClientInfo> clientInfos)
        {
            this.listView1.Invoke((MethodInvoker)delegate {
                listView1.Items.Clear();
                // Running on the UI thread
                foreach (var item in clientInfos)
                {
                    listView1.Items.Add(item.Name);
                }
            });
           
        }

        //private static bool TryParseJSON(byte[] arrBytes)
        //{
        //    try
        //    {
        //        JsonSerializer.Deserialize<List<ClientInfo>>(arrBytes);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public void ReceiveChatList(List<ClientInfo> clients)
        {
            
        }

        private void GroupChatMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ConnectToServer_Click(object sender, EventArgs e)
        {
            FormConnect newForm = new FormConnect();
            // passing this in ShowDialog will set the .Owner 
            // property of the child form
            newForm.TopMost = true;
            newForm.Show(this);
            CreateChatMenuItem.Enabled = true;
            DisconnectFromServer.Enabled = true;

        }
   

        private void DisconnectFromServer_Click(object sender, EventArgs e)
        {
            Disconnect();
            CreateChatMenuItem.Enabled = false;
            DisconnectFromServer.Enabled = false;
        }
    }
}
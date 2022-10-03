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
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Tulpep.NotificationWindow;

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
        public ConcurrentDictionary<int, List<ClientInfo>> AllChatsClients = new ConcurrentDictionary<int, List<ClientInfo>>();//хранение всех списков пользователей чата
        public ConcurrentDictionary<int, string> ChatsNames = new ConcurrentDictionary<int, string>();//хранение имен чатов а также ID которые выдал сервер
        public ConcurrentDictionary<int, List<string>> ChatsHistory = new ConcurrentDictionary<int, List<string>>();//хранение истории сообщений чатов по ID

        //public List<ClientInfo> MainChat = new List<ClientInfo>();
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
                stream = client.GetStream(); // получаем поток
                string message = userName;
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
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

            ChatsHistory.TryAdd(0, new List<string>());
            AllChatsClients.TryAdd(0, new List<ClientInfo>());
            ChatsNames.TryAdd(0, "Главный");

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

        private string GetMessage()//Получение первичной информации о сообщении тип/размер а также обычных текстовых сообщений
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
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
            string message = GetMessage();//Получаем JSON заголовок
            var MessageHeader = MessageHandler.StringToObject<PacketInfo>(message);//Сериализуем в объект
            switch (MessageHeader.Type)//Обработка в зависимости от отправленного пользователем сообщения
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

        private byte[] GetMessageWithSize(int size)//Получение первичной информации о сообщении тип/размер
        {
            byte[] data = new byte[size]; // буфер для получаемых данных
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
            ChatsHistory[messageHeader.ChatID].Add(message);
            PrintMessageOrNotify(messageHeader.ChatID,message);
        }

        private void PrintMessageOrNotify(int ChatID,string message)
        {
            if(tabControl1.TabPages[tabControl1.SelectedIndex].Name == "tabPage"+ChatID.ToString())
            {
                this.tabControl1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                  ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                  ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                    
                });
            }
            else//Вывести уведомление
            {
               
                PopupNotifier popup = new PopupNotifier();
                popup.Delay = 500;
                popup.TitleText = "Сообщение из чата" + ChatsNames[ChatID];
                popup.ContentText = message;
                popup.Popup();// show 
            }
           
        }

        private void PrintAllMessages(int ChatID)
        {
            this.tabControl1.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                foreach (var item in ChatsHistory[ChatID])
                {
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(item);
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                }

            });

        }

        private void HandleP2PChatCreation()
        {
            throw new NotImplementedException();
        }

        private void HandleUserList(PacketInfo messageHeader)
        {
            var data = GetMessageWithSize((int)messageHeader.Size);
            if (messageHeader.ChatID == 0)
            {
                AllChatsClients[messageHeader.ChatID] = MessageHandler.ByteArrayToObject<List<ClientInfo>>(data);
                fillListView(AllChatsClients[0]);
            }
            else
            {
                AllChatsClients[messageHeader.ChatID] = MessageHandler.ByteArrayToObject<List<ClientInfo>>(data);
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
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
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
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            //Environment.Exit(0); //завершение процесса
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                byte[] data = Encoding.UTF8.GetBytes(textBox1.Text);
                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text, data.Length, 0);//подготавливаем заголовок
                stream.Write(MessageHeader,0, MessageHeader.Length);
                Task.Delay(10);
                stream.Write(data, 0, data.Length);
                var message = String.Format("{0}: {1}", userName, textBox1.Text);                   
                ReadOnlySpan<Char> PageName = tabControl1.TabPages[tabControl1.SelectedIndex].Name;//в конце названия страницы всегда ID чата 
                var IdChat = PageName.Slice(7);//TabPage...
                
                ChatsHistory[int.Parse(IdChat)].Add(message);//Сохраняем наше сообщение,чтобы чат можно было восстановить при переключении вкладок
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

        public void ReceiveChatList(List<ClientInfo> clients)
        {
            byte[] data = MessageHandler.ObjectToByteArray(clients);
            var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.ChatCreation, data.Length, 0);//подготавливаем заголовок
            //stream.Write(MessageHeader, 0, MessageHeader.Length);
            //Task.Delay(10);
            //stream.Write(data, 0, data.Length);
        }

        private void GroupChatMenuItem_Click(object sender, EventArgs e)
        {
            FormUserSelection newForm = new FormUserSelection(AllChatsClients[0]);
            newForm.TopMost = true;
            newForm.Show(this);
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
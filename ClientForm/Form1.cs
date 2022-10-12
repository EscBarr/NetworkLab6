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
using Microsoft.VisualBasic.ApplicationServices;

namespace ClientForm
{
    /// <summary>
    /// TODO: Работа с отправкой файлов
    /// </summary>

    public partial class Form1 : Form
    {
        private static string userName;
        public IPEndPoint? ipPoint { get; set; }
        private static TcpClient client;
        private static NetworkStream stream;
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
            SendFilebutton.Enabled = true;
            try
            {
                client.Connect(ipPoint);
                stream = client.GetStream(); // получаем поток
                string message = userName;
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] NameSize = MessageHandler.GetHeaderSize(data.Length);
                stream.Write(NameSize, 0, NameSize.Length);//Размер Строки
                stream.Write(data, 0, data.Length);//Сама строка
                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

            var IdChat = GetCurrentChat();
            fillListView(AllChatsClients[IdChat]);//Заполняем список пользователей канала
            PrintAllMessages(IdChat);//Выводим все полученные сообщения
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

        private int GetPacketSize()
        {
            byte[] data = new byte[4]; // получаем целое число с размером последуюшего заголовка
            stream.Read(data, 0, data.Length);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// 1 Получение размера заголовка
        /// 2 Получение заголовка
        /// 3 Получения сообщения/Файла/Списка пользователей
        /// </summary>

        private void HandleMessageType()
        {
            int PacketSize = GetPacketSize();//Получаем размер заголовка
            var MessageHeaderBytes = GetMessageWithSize(PacketSize);//Получаем JSON заголовок
            var MessageHeader = MessageHandler.ByteArrayToObject<PacketInfo>(MessageHeaderBytes);//Сериализуем в объект
            switch (MessageHeader.Type)//Обработка в зависимости от отправленного пользователем сообщения
            {
                case MessageTypes.Text:
                    HandleMessages(MessageHeader);
                    break;

                case MessageTypes.File:
                    HandleFile(MessageHeader);
                    break;

                case MessageTypes.ChatCreation:
                    HandleChatCreation(MessageHeader);
                    break;

                case MessageTypes.UserList:
                    HandleUserList(MessageHeader);
                    break;
            }
        }

        private string GetStringWithSize(int size)//Получение текста
        {
            byte[] data = new byte[size]; // буфер для получаемых данных

            int readPos = 0;
            while (readPos < size)
            {
                var actuallyRead = stream.Read(data, readPos, size - readPos);
                if (actuallyRead == 0)//Ошибка в размере сообщения
                    throw new EndOfStreamException("Ошибка в размере сообщения со строкой");
                readPos += actuallyRead;
            }

            return Encoding.UTF8.GetString(data);
        }

        private byte[] GetMessageWithSize(int size)//Получение первичной информации о сообщении тип/размер
        {
            byte[] data = new byte[size]; // буфер для получаемых данных

            int readPos = 0;
            while (readPos < size)
            {
                var actuallyRead = stream.Read(data, readPos, size - readPos);
                if (actuallyRead == 0)//Ошибка в размере сообщения
                    throw new EndOfStreamException("Ошибка в размере сообщения с заголовком");
                readPos += actuallyRead;
            }

            return data;
        }

        private void HandleMessages(PacketInfo messageHeader)
        {
            var message = GetStringWithSize(messageHeader.Size);
            ChatsHistory[messageHeader.ChatID].Add(message);
            PrintMessageOrNotify(messageHeader.ChatID, message);
        }

        private void PrintMessageOrNotify(int ChatID, string message)
        {
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                var IdChat = GetCurrentChat();//TabPage...
                if (IdChat == ChatID)
                {
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                }
                else//Вывести уведомление
                {
                    PopupNotifier popup = new PopupNotifier();
                    popup.Delay = 1000;
                    popup.TitleText = "Сообщение из чата" + ChatsNames[ChatID];
                    popup.ContentText = message;
                    popup.Popup();// show
                }
            });
        }

        private void PrintAllMessages(int ChatID)
        {
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).Clear();
                foreach (var item in ChatsHistory[ChatID])
                {
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(item);
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                }
            });
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

        private void HandleChatCreation(PacketInfo messageHeader)
        {
            var data = GetMessageWithSize(messageHeader.Size);
            var chatinf = MessageHandler.ByteArrayToObject<ChatInfo>(data);
            ChatsNames.TryAdd(messageHeader.ChatID, chatinf.ChatName);
            AllChatsClients.TryAdd(messageHeader.ChatID, chatinf.CurChatUsers);
            ChatsHistory.TryAdd(messageHeader.ChatID, new List<string>());
            TabPage tp = new TabPage(chatinf.ChatName);
            tp.Name = "tabPage" + messageHeader.ChatID.ToString();
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                tabControl1.TabPages.Add(tp);
            });
        }

        private void HandleFile(PacketInfo messageHeader)
        {
            var data = GetMessageWithSize(messageHeader.Size);
            var FileNameSize = GetPacketSize();
            var FileName = GetStringWithSize(FileNameSize);
            var VS = File.Create(Path.Combine(Environment.CurrentDirectory, "Downloads", FileName));
            VS.Write(data);
            VS.Close();
        }

        private void ReceiveMessage()
        {
            while (client.Connected)
            {
                try
                {
                    HandleMessageType();
                }
                catch
                {
                    MessageBox.Show("Подключение прервано!");
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

        private void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                var IdChat = GetCurrentChat();
                var message = String.Format("{0}: {1}", userName, textBox1.Text);
                byte[] data = Encoding.UTF8.GetBytes(message);
                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text, data.Length, IdChat);//подготавливаем заголовок
                byte[] HeaderSize = MessageHandler.GetHeaderSize(MessageHeader.Length);
                stream.Write(HeaderSize, 0, HeaderSize.Length);
                stream.Write(MessageHeader, 0, MessageHeader.Length);
                stream.Write(data, 0, data.Length);

                ChatsHistory[IdChat].Add(message);//Сохраняем наше сообщение,чтобы чат можно было восстановить при переключении вкладок
                this.tabControl1.Invoke((MethodInvoker)delegate
                {
                    // Running on the UI thread
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                    ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                });
                textBox1.Text = "";
            }
        }

        private void fillListView(List<ClientInfo> clientInfos)
        {
            this.listView1.Invoke((MethodInvoker)delegate
            {
                listView1.Items.Clear();
                // Running on the UI thread
                foreach (var item in clientInfos)
                {
                    listView1.Items.Add(item.Name);
                }
            });
        }

        public void CreateChat(string ChatName, List<ClientInfo> clients)
        {
            var Chatinf = new ChatInfo();
            Chatinf.ChatName = ChatName;
            Chatinf.ChatID = -1;
            Chatinf.CurChatUsers = clients;
            byte[] data = MessageHandler.ObjectToByteArray(Chatinf);
            var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.ChatCreation, data.Length, -1);//подготавливаем заголовок
            byte[] HeaderSize = MessageHandler.GetHeaderSize(MessageHeader.Length);
            stream.Write(HeaderSize, 0, HeaderSize.Length);
            stream.Write(MessageHeader, 0, MessageHeader.Length);
            stream.Write(data, 0, data.Length);//отправляем информацию о новом чате
        }

        private void ConnectToServer_Click(object sender, EventArgs e)
        {
            FormConnect newForm = new FormConnect();
            //newForm.TopMost = true;
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //enter key is down

                this.Sendbutton_Click(sender, e);

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void CreateChatMenuItem_Click(object sender, EventArgs e)
        {
            FormUserSelection newForm = new FormUserSelection(AllChatsClients[0]);
            newForm.Show(this);
        }

        private int GetCurrentChat()
        {
            ReadOnlySpan<Char> PageName = tabControl1.TabPages[tabControl1.SelectedIndex].Name;//в конце названия страницы всегда ID чата
            var IdChat = PageName.Slice(7);//TabPage...
            return int.Parse(IdChat);
        }

        private void SendFilebutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                byte[] data = File.ReadAllBytes(file);

                ///
                /// TODO: Выводить имя файла как сообщение (Транслировать его всем пользователям)
                ///

                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.File, data.Length, GetCurrentChat());//подготавливаем заголовок
                byte[] HeaderSize = MessageHandler.GetHeaderSize(MessageHeader.Length);
                stream.Write(HeaderSize, 0, HeaderSize.Length);
                stream.Write(MessageHeader, 0, MessageHeader.Length);
                stream.Write(data, 0, data.Length);//отправляем информацию о новом чате

                ///Часть отправки
                string FileName = Path.GetFileName(file);
                byte[] dataFileName = Encoding.UTF8.GetBytes(FileName);
                byte[] SizeOfFileName = MessageHandler.GetHeaderSize(dataFileName.Length);

                /// TODO: КРИВО ОТПРАВЛЯЕТСЯ название файла
                stream.Write(SizeOfFileName, 0, SizeOfFileName.Length);//Размер строки
                stream.Write(dataFileName, 0, dataFileName.Length);//Строчка

                SendFileMessage(FileName);
            }
        }

        private void SendFileMessage(string FileName)
        {
            var message = String.Format("{0}: {1}", userName, FileName);
            byte[] data = Encoding.UTF8.GetBytes(message);
            var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text, data.Length, GetCurrentChat());//подготавливаем заголовок
            byte[] HeaderSize = MessageHandler.GetHeaderSize(MessageHeader.Length);
            stream.Write(HeaderSize, 0, HeaderSize.Length);
            stream.Write(MessageHeader, 0, MessageHeader.Length);
            stream.Write(data, 0, data.Length);

            ChatsHistory[GetCurrentChat()].Add(message);//Сохраняем наше сообщение,чтобы чат можно было восстановить при переключении вкладок
            this.tabControl1.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
            });
        }
    }
}
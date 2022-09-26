using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System;

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
                stream = client.GetStream(); // получаем поток
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
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

        // отправка сообщений
        //void SendMessage()
        //{
        //    Console.WriteLine("Введите сообщение: ");

        //    while (true)
        //    {
               
        //    }
        //}
        // получение сообщений
        void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[8192]; // буфер для получаемых данных
               
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                    }
                    while (stream.DataAvailable);
                    data = TrimEnd(data,bytes);
                    if(TryParseJSON(data))
                    {
                        var Test = ByteArrayToObject(data);
                        fillListView(Test);
                    }
                    else//вывод сообщения в чат
                    {
                        
                        string message = Encoding.Unicode.GetString(data);
                        this.tabControl1.Invoke((MethodInvoker)delegate {
                            // Running on the UI thread
                            ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(Environment.NewLine);
                            ((TextBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls["dynamictextbox_" + tabControl1.TabPages[tabControl1.SelectedIndex].Name]).AppendText(message);
                        });
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public static byte[] TrimEnd(byte[] array,int size)
        {
            //int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, size  + 1);

            return array;
        }

        void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox1.Text))
            {
                byte[] data = Encoding.Unicode.GetBytes(textBox1.Text);
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

        // Convert a byte array to an Object
        private T ByteArrayToObject<T>(byte[] arrBytes)
        {
            return JsonSerializer.Deserialize<T>(arrBytes);
        }

       
        private static bool TryParseJSON(byte[] arrBytes)
        {
            try
            {
                JsonSerializer.Deserialize<List<ClientInfo>>(arrBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
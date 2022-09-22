using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLab6
{
    public class Client
    {
        public Guid ClientId { get; init; }
        public string Name { get; set; }
        protected internal NetworkStream Stream { get; private set; }

        TcpClient client;

        ServerTcp server; // для вызова методов сервера, создания чатов
        public Client(TcpClient tcpClient, ServerTcp serverObj)
        {
            ClientId = Guid.NewGuid();
            client = tcpClient;
            server = serverObj;
            serverObj.AddConnection(this);
        }

        public void CreateChat()
        {

        }

        public void CreateP2PChat()
        {

        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                Name = message;

                message = "Сервер: " + Name + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.ClientId);
                server.BroadcastUsers();
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", Name, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.ClientId);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", Name);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.ClientId);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.ClientId);
                Close();
            }
        }

        // Подготовка файла к отправке
        //private byte[] GetFile()
        //{
        //    return NotImplementedException();
        //}

        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}

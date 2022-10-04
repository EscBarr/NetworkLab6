using Lab6Dependecies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetworkLab6
{
    public class Client
    {
        public Guid ClientId { get; set; }
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
                //client.ReceiveTimeout = 10;
                //client.SendTimeout = 10;
                // получаем имя пользователя
                string message = GetMessage();
                Name = message;

                message = "Сервер: " + Name + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text,null,0);//подготавливаем заголовок
                server.BroadcastMessageHeader(MessageHeader, ClientId, 0);
                //Task.Delay(10).Wait();
                server.BroadcastMessage(message, ClientId,0);//Оповещение для пользователей чата
                var ListUsers = server.ConvertClientList(server.ChatUsers);//Уменьшаем размер списка пользователей
                //Task.Delay(10).Wait();
                server.BroadcastUsers(ListUsers,0);//0 так мы только установили соединение и пользователю нужен основной список
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        HandleMessageType();
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", Name);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, ClientId,0);
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

        private string GetMessage()//Получение первичной информации о сообщении тип/размер а также обычных текстовых сообщений
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            

            return builder.ToString();
        }

        private byte[] GetMessageWithSize(int size)//Получение первичной информации о сообщении тип/размер
        {
            byte[] data = new byte[size]; // буфер для получаемых данных
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
            }
            while (Stream.DataAvailable);

            return data;
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
                    HandleChatCreation(MessageHeader);
                    break;
                case MessageTypes.UserListForChat:
                    HandleUserList(MessageHeader);
                    break;
                case MessageTypes.P2PChat:
                    HandleP2PChatCreation();
                    break;
            }
        }


        private void HandleMessages(PacketInfo packetInfo)
        {
            string message = GetMessage();
            message = String.Format("{0}: {1}", Name, message);
            Console.WriteLine(message);
            server.BroadcastMessageHeader(MessageHandler.ObjectToByteArray(packetInfo), ClientId, packetInfo.ChatID);
            //Task.Delay(10).Wait();
            server.BroadcastMessage(message, this.ClientId,packetInfo.ChatID);
         
        }
        private void HandleFile(PacketInfo packetInfo)
        {

        }

        private void HandleUserList(PacketInfo packetInfo)
        {

        }

        private void HandleChatCreation(PacketInfo packetInfo)
        {
            var Data = GetMessageWithSize((int)packetInfo.Size);//Получаем иформацию о чате
            var ChatInf = MessageHandler.ByteArrayToObject<ChatInfo>(Data);//Конвертация в информацию о чате
            ChatInf.ChatID = server.AllChats.Count+1;//+1 так как 0 по умолчанию используется для общего чата
            var ConvertedChatInfo = server.BackwardConvertClientList(ChatInf.CurChatUsers);//Получаем список клиентов с их потоками
            server.AllChats.TryAdd(ChatInf.ChatID,ConvertedChatInfo);//Добавляем информацию о чате для сервера
            packetInfo.ChatID = ChatInf.ChatID;
            server.BroadcastToAllUsers(MessageHandler.ObjectToByteArray(packetInfo), ChatInf.ChatID);//отправляем заголовок с выданным для чата ID
            //Task.Delay(10);
            server.BroadcastToAllUsers(Data, ChatInf.ChatID);//отсылаем информацию о чате всем кто был отмечен в списке


        }
        private void HandleP2PChatCreation()
        {

        }
        // закрытие подключения
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }


    }
}

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

        private TcpClient client;

        private ServerTcp server; // для вызова методов сервера, создания чатов

        public Client(TcpClient tcpClient, ServerTcp serverObj)
        {
            ClientId = Guid.NewGuid();
            client = tcpClient;
            server = serverObj;
            serverObj.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                /*client.ReceiveTimeout = 10;*/ //ВОЗМОЖНО НУЖНО ДЛЯ ОТПРАВКИ/ПРИНЯТИЯ ФАЙЛОВ
                //client.SendTimeout = 100;
                //client.NoDelay = true;
                //TODO: Стоит переписать первичное получение имени
                // получаем имя пользователя 
                int PacketSize = GetPacketSize();//Получаем размер строки
                string message = GetStringWithSize(PacketSize);
                Name = message;

                message = "Сервер: " + Name + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                var MessageByte = Encoding.UTF8.GetBytes(message);
                var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.Text, MessageByte.Length, 0);//подготавливаем заголовок
                var HeaderSize = MessageHandler.GetHeaderSize(MessageHeader.Length);
                //server.BroadcastByteArray(HeaderSize, ClientId, 0);
                //server.BroadcastByteArray(MessageHeader, ClientId, 0);
                //server.BroadcastByteArray(MessageByte, ClientId, 0);//Оповещение для пользователей чата
                server.BroadcastToAllUsers(HeaderSize, 0);
                server.BroadcastToAllUsers(MessageHeader, 0);
                server.BroadcastToAllUsers(MessageByte, 0);
                var ListUsers = server.ConvertClientList(server.ChatUsers);//Преобразуем список пользователей
                server.BroadcastUsers(ListUsers, 0);//0 так мы только установили соединение и пользователю нужен основной список
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
                        server.BroadcastMessage(message, ClientId, 0);
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

        //private string GetMessage()//Получение первичной информации о сообщении тип/размер а также обычных текстовых сообщений
        //{
        //    byte[] data = new byte[64]; // буфер для получаемых данных
        //    StringBuilder builder = new StringBuilder();
        //    int bytes = 0;
        //    do
        //    {
        //        bytes = Stream.Read(data, 0, data.Length);
        //        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
        //    }
        //    while (Stream.DataAvailable);

        //    return builder.ToString();
        //}

        private int GetPacketSize()
        {
            byte[] data = new byte[4]; // получаем целое число с размером последуюшего заголовка

            Stream.Read(data, 0, data.Length);

            return BitConverter.ToInt32(data, 0);
        }

        private byte[] GetMessageWithSize(int size)//Получение первичной информации о сообщении тип/размер
        {
            byte[] data = new byte[size]; // буфер для получаемых данных
            
            Stream.Read(data, 0, data.Length);

            return data;
        }

        private string GetStringWithSize(int size)//Получение текста
        {
            byte[] data = new byte[size]; // буфер для получаемых данных

            Stream.Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data);
        }

        private void HandleMessageType()
        {
            int PacketSize = GetPacketSize();//Получаем размер заголовка
            if(PacketSize > 0)
            {
                var MessageHeaderBytes = GetMessageWithSize(PacketSize);//Получаем JSON заголовок
                var MessageHeader = MessageHandler.ByteArrayToObject<PacketInfo>(MessageHeaderBytes);//Сериализуем в объект                                                  //Сериализуем в объект

                switch (MessageHeader.Type)//Обработка в зависимости от отправленного пользователем сообщения
                {
                    case MessageTypes.Text:
                        HandleMessages(MessageHeader, MessageHeaderBytes);
                        break;

                    case MessageTypes.File:
                        HandleFile(MessageHeader);
                        break;

                    case MessageTypes.ChatCreation:
                        HandleChatCreation(MessageHeader, MessageHeaderBytes);
                        break;

                    case MessageTypes.UserListForChat://Изменение списка клиентов, когда кто-то отключился
                        HandleUserList(MessageHeader, MessageHeaderBytes);
                        break;

                    case MessageTypes.P2PChat:
                        HandleP2PChatCreation();
                        break;
                }
            }
            
        }

        private void HandleMessages(PacketInfo packetInfo, byte[] Header)
        {
            string message = GetStringWithSize(packetInfo.Size);//Получаем строчку
            message = String.Format("{0} {1}: {2}",packetInfo.ChatID,  Name, message);
            Console.WriteLine(message);
            //var Header = MessageHandler.ObjectToByteArray(packetInfo);
            byte[] HeaderSize = MessageHandler.GetHeaderSize (Header.Length);
            server.BroadcastByteArray(HeaderSize, ClientId, packetInfo.ChatID);
            server.BroadcastByteArray(MessageHandler.ObjectToByteArray(packetInfo), ClientId, packetInfo.ChatID);
            //Task.Delay(10).Wait();
            server.BroadcastMessage(message, this.ClientId, packetInfo.ChatID);
        }

        private void HandleFile(PacketInfo packetInfo)
        {
        }

        private void HandleUserList(PacketInfo packetInfo, byte[] Header)
        {
        }

        private void HandleChatCreation(PacketInfo packetInfo, byte[] Header)
        {
            var Data = GetMessageWithSize(packetInfo.Size);//Получаем иформацию о чате
            var ChatInf = MessageHandler.ByteArrayToObject<ChatInfo>(Data);//Конвертация в информацию о чате
            ChatInf.ChatID = server.AllChats.Count + 1;//+1 так как 0 по умолчанию используется для общего чата
            var ConvertedChatInfo = server.BackwardConvertClientList(ChatInf.CurChatUsers);//Получаем список клиентов с их потоками
            server.AllChats.TryAdd(ChatInf.ChatID, ConvertedChatInfo);//Добавляем информацию о чате для сервера


            packetInfo.ChatID = ChatInf.ChatID;//меняем на ID, выданный сервером

            byte[] ChangedHeader = MessageHandler.ObjectToByteArray(packetInfo);//Сериализуем измененный заголовок
            byte[] HeaderSize = MessageHandler.GetHeaderSize(ChangedHeader.Length);
            server.BroadcastToAllUsers(HeaderSize, ChatInf.ChatID);
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
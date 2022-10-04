using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lab6Dependecies;
using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace NetworkLab6
{
    public class ServerTcp
    {
        public int Port = 25565;
        public string IP = "192.168.1.38";
        static TcpListener listener;//для общего чата
        public ConcurrentDictionary<int, List<Client>> AllChats = new ConcurrentDictionary<int, List<Client>>();//Информация по каждому чату
        public List<Client> ChatUsers = new List<Client>();//для общего чата
       
        public void Initiate()
        {

            try
            { 
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            // Прослушивание входящих соединений
            listener = new TcpListener(ipPoint);
            listener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Client client = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(client.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Shutdown();
            }
        }

        public void InitiateChat(List<Client> SelectedUsers)
        {

        }

        public void AddConnection(Client clientObj)
        {
            ChatUsers.Add(clientObj);
            
        }

        public void RemoveConnection(Guid id)
        {
            // получаем по id закрытое подключение
            Client client = ChatUsers.FirstOrDefault(c => c.ClientId == id);
            // и удаляем его из списка подключений
            if (client != null)
                ChatUsers.Remove(client);
            var ListUsers = ConvertClientList(ChatUsers);
            BroadcastUsers(ListUsers,0);
        }
        // отключение всех клиентов
        public void Shutdown()
        {
            listener.Stop(); //остановка сервера

            for (int i = 0; i < ChatUsers.Count; i++)
            {
                ChatUsers[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }

        public void BroadcastMessageHeader(byte[] data, Guid id, int ChatId)
        {
            if (ChatId == 0)
            {
                
                for (int i = 0; i < ChatUsers.Count; i++)
                {
                    if (ChatUsers[i].ClientId != id) // если id клиента не равно id отправляющего
                    {
                        ChatUsers[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                }
            }
            else
            {
                var Users = AllChats[ChatId];
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].ClientId != id) // если id клиента не равно id отправляющего
                    {
                        Users[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                }
            }
        }

        // трансляция сообщения подключенным клиентам
        public void BroadcastMessage(string message, Guid id, int ChatId)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            if (ChatId==0)
            {
                for (int i = 0; i < ChatUsers.Count; i++)
                {
                    if (ChatUsers[i].ClientId != id) // если id клиента не равно id отправляющего
                    {
                        ChatUsers[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                }
            }
            else
            {
                var Users = AllChats[ChatId];
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].ClientId != id) // если id клиента не равно id отправляющего
                    {
                        Users[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                }
            }
           
        }

        // трансляция списка подключенных пользователей
        public void BroadcastUsers(List<ClientInfo> ListUsers, int ChatId)
        {
            var data = MessageHandler.ObjectToByteArray(ListUsers);//получаем размер сообщения
            var MessageHeader = MessageHandler.PrepareMessageHeader(MessageTypes.UserList, data.Length,ChatId);//подготавливаем заголовок
            var test = MessageHandler.ByteArrayToObject<PacketInfo>(MessageHeader);
            BroadcastToAllUsers(MessageHeader, ChatId);
            Task.Delay(10);
            BroadcastToAllUsers(data,ChatId);
           
        }

        public void BroadcastToAllUsers(byte[] data, int ChatId)//В основном для рассылки списка пользователей
        {
            if (ChatId == 0)
            {
               
                for (int i = 0; i < ChatUsers.Count; i++)
                {
                   ChatUsers[i].Stream.Write(data, 0, data.Length); //передача данных 
                }
            }
            else
            {
                var Users = AllChats[ChatId];

                for (int i = 0; i < Users.Count; i++)
                {
                  Users[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        public List<ClientInfo> ConvertClientList(List<Client> AllClients)
        {
            List<ClientInfo> CovertClients = new List<ClientInfo>();
            
            foreach (var client in AllClients)
            {
                var ClientInfo = new ClientInfo();
                ClientInfo.ClientId = client.ClientId;
                ClientInfo.Name = client.Name;
                CovertClients.Add(ClientInfo);
            }
            return CovertClients;
        }

        public List<Client> BackwardConvertClientList(List<ClientInfo> AllClients)
        {
            List<Client> CovertClients = ChatUsers.Where( P => AllClients.Any(p2=> p2.ClientId == P.ClientId)).ToList();
            
            return CovertClients;
        }

    }
}



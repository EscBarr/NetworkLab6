using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetworkLab6
{
    [Serializable]
    public record ClientInfo
    {
       
        public Guid ClientId { get; set; }
        public string Name { get; set; }
    }

    public record ChatInfo
    {
        public Guid ChatId { get; init; }
        public string ChatName { get; set; }

        public TcpListener listener;//у каждого чата свой слушатель???

        public List<Client> CurChatUsers = new List<Client>();//у каждого чата свой список пользователей
    }


    public class ServerTcp
    {
        static int Port = 25565;
        static string IP = "192.168.1.38";
        static TcpListener listener;//для общего чата
        static Dictionary<int,ChatInfo> AllChats = new Dictionary<int, ChatInfo>();
        public static List<Client> ChatUsers = new List<Client>();

       
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

        public void InitiateChat(int Port, List<Client> SelectedUsers)//ПОРТ ДОЛЖЕН ВЫБРАТЬ САМ СЕРВЕР? ИЛИ СОЗДАВАТЬ НОВЫЙ СЛУШАТЕЛЬ У ПОЛЬЗОВАТЕЛЯ С НОВЫМ ПОТОКОМ
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
            BroadcastUsers();
        }
        // отключение всех клиентов
        public void Shutdown()
        {
            listener.Stop(); //остановка сервера
            foreach (var listeners in AllChats)
            {
                listeners.Value.listener.Stop();
            }

            for (int i = 0; i < ChatUsers.Count; i++)
            {
                ChatUsers[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }

        // трансляция сообщения подключенным клиентам
        public void BroadcastMessage(string message, Guid id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < ChatUsers.Count; i++)
            {
                //if (ChatUsers[i].ClientId != id) // если id клиента не равно id отправляющего
                //{
                    ChatUsers[i].Stream.Write(data, 0, data.Length); //передача данных
                //}
            }
        }

        // трансляция списка подключенных пользователей?????????????????????
        public void BroadcastUsers()
        {
            var ListUsers = ConvertClientList(ChatUsers);
            var data = ObjectToByteArray(ListUsers);
            //byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < ChatUsers.Count; i++)
            {
                //if (ChatUsers[i].ClientId != id) // если id клиента не равно id отправляющего
                //{
                ChatUsers[i].Stream.Write(data, 0, data.Length); //передача данных
                //}
            }
        }

        List<ClientInfo> ConvertClientList(List<Client> AllClients)
        {
            List<ClientInfo> CovertClients = new List<ClientInfo>();
            var ClientInfo = new ClientInfo();
            foreach (var client in AllClients)
            {
                ClientInfo.ClientId = client.ClientId;
                ClientInfo.Name = client.Name;
                CovertClients.Add(ClientInfo);
            }
            return CovertClients;
        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            return JsonSerializer.SerializeToUtf8Bytes(obj);
          
        }

    }
}



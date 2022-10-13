using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab6Dependecies
{
    [Serializable]
    public enum MessageTypes : ushort
    {
        Text,
        UserList,
        File,
        ChatCreation,
        UserID
    }

    [Serializable]
    public class PacketInfo
    {
        public MessageTypes Type { get; set; }
        public int Size { get; set; }
        public int ChatID { get; set; }
    }

    [Serializable]
    public record ClientInfo
    {
        public Guid ClientId { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public record ChatInfo
    {
        public int ChatID { get; set; }
        public string ChatName { get; set; }

        public List<ClientInfo> CurChatUsers { get; set; }//у каждого чата свой список пользователей
    }

    public static class MessageHandler
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public static byte[] PrepareMessageHeader(MessageTypes Type, int Size, int ChatId)
        {
            var Packet = new PacketInfo
            {
                Type = Type,
                Size = Size,
                ChatID = ChatId
            };

            return ObjectToByteArray(Packet);
        }

        public static byte[] GetHeaderSize(int Size)
        {
            //IPAddress.HostToNetworkOrder(Size)
            return BitConverter.GetBytes(Size);
        }

        // Convert a byte array to an Object
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            var bytesAsString = Encoding.UTF8.GetString(arrBytes);
            return JsonSerializer.Deserialize<T>(bytesAsString);
        }

        //// Convert a String to an Object
        //public static T StringToObject<T>(string obj)
        //{
        //    return JsonSerializer.Deserialize<T>(obj);
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CNetworking
{
    public interface IMsgTransceiver
    { 
        Task<CMessage> Proccess(CMessage msg);
    }
    public class CMessage
    {
        public string Type;
        public object Payload;
        public CMessage(string Type, object Payload)
        {
            this.Type = Type;
            this.Payload = Payload;
        }
    }
    public class NetworkUtils
    {
        public static byte[] Serialize<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static async Task SendObjectAsync<T>(T obj, NetworkStream stream)
        {
            byte[] payload = Serialize(obj);
            int length = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(length);

            if (stream != null)
            {
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

                await stream.WriteAsync(payload, 0, payload.Length);
            }
        }

        public static async Task<T> ReceiveObjectAsync<T>(NetworkStream stream)
        {
            byte[] lengthPrefix = new byte[4];
            int read = await stream.ReadAsync(lengthPrefix, 0, 4);
            if (read < 4) throw new Exception("Failed to read message length");

            int length = BitConverter.ToInt32(lengthPrefix, 0);
            byte[] buffer = new byte[length];

            int bytesRead = 0;
            while (bytesRead < length)
            {
                int chunkSize = await stream.ReadAsync(buffer, bytesRead, length - bytesRead);
                if (chunkSize == 0) throw new Exception("Disconnected while reading message");
                bytesRead += chunkSize;
            }

            string jsonString = Encoding.UTF8.GetString(buffer);
            T? result = JsonSerializer.Deserialize<T>(jsonString);

            if (result == null) throw new Exception("Result string is null");
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
namespace ClientApp
{
    public class NetworkClient
    {
        private TcpClient? client;
        private NetworkStream? stream;

        public async Task ConnectAsync(string serverIp, int port)
        {
            
            client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(serverIp), port);
            stream = client.GetStream();
        }

        public static byte[] Serialize<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public async Task SendObjectAsync<T>(T obj)
        {
            byte[] payload = Serialize(obj);
            int length = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(length);

            if(stream != null)
            {
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

                await stream.WriteAsync(payload, 0, payload.Length);
            }
        }

        public async Task<T> ReceiveObjectAsync<T>(NetworkStream stream)
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
            T? result =  JsonSerializer.Deserialize<T>(jsonString);

            if (result == null) throw new Exception("result string is null");
            return result;
        }

        public void Dispose()
        {
            stream?.Close();
            client?.Close();
        }

    }
}

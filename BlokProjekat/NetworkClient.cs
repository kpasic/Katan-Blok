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

        public void Dispose()
        {
            stream?.Close();
            client?.Close();
        }

    }
}

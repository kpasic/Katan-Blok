using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using Catan;
namespace ClientApp
{
    public class NetworkClient
    {
        private TcpClient? client;
        private NetworkStream? stream;
        public Func<Task<Move>> getMovecallback;
        public Action setBoardcallback;
        public async Task<CMessage> ConnectAsync(string serverIp, int port, string Name)
        {
            
            client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(serverIp), port);
            stream = client.GetStream();

            
            await NetworkUtils.SendObjectAsync(new CMessage(msgType.Connect, Name), stream);

            CMessage message = await NetworkUtils.ReceiveObjectAsync<CMessage>(stream);

            return message;
        }

        public async Task StartListening()
        {
            if (client != null && stream != null)
            {
                while (true)
                {
                    CMessage msg = await NetworkUtils.ReceiveObjectAsync<CMessage>(stream);
                    CMessage response;
                    switch (msg.Type)
                    {
                        case msgType.GetMove:
                            Move move = await getMovecallback();
                            response = new CMessage(msgType.Move, move);
                            break;
                        case msgType.GameState:
                            setBoardcallback();
                            break;

                    }
                }
            }
        }

        public void Dispose()
        {
            stream?.Close();
            client?.Close();
        }



    }
}

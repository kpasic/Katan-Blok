using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Diagnostics;

namespace CNetworking
{
    public class NetworkClient
    {
        private TcpClient? client;
        private NetworkStream? stream;
        IMsgTransceiver msgTransceiver;

        public NetworkClient(IMsgTransceiver msgTransceiver)
        {
            this.msgTransceiver = msgTransceiver;
        }

        public async Task<CMessage> ConnectAsync(string serverIp, int port, string Name)
        {
            
            client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(serverIp), port);
            stream = client.GetStream();

            
            await NetworkUtils.SendObjectAsync(new CMessage("Connect", Name), stream);

            CMessage message = await NetworkUtils.ReceiveObjectAsync<CMessage>(stream);

            return message;
        }

        public async Task StartListening(CMessage startMsg)
        {
            if (client != null && stream != null)
            {
                await NetworkUtils.SendObjectAsync(startMsg, stream);

                while (true)
                {
                    CMessage msg = await NetworkUtils.ReceiveObjectAsync<CMessage>(stream);
                    Debug.WriteLine("RECEIVED MSG !!");
                    await NetworkUtils.SendObjectAsync(msgTransceiver.Proccess(msg), stream);
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

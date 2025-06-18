using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Catan;
namespace Server
{
    internal class NetworkPlayer : IPlayer
    {
        public TcpClient client;
        public NetworkStream stream;
        private Dictionary<Resources, int> resources;

        public string Name { get;}
        public int Id { get;}
        Dictionary<Resources, int> IPlayer.resources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ResourcesCount => throw new NotImplementedException();

        public NetworkPlayer(TcpClient client, NetworkStream stream, string name, int id)
        {
            this.client = client;
            this.stream = stream;
            Name = name;
            Id = id;
        }

        public async Task<Move> GetMove()
        {
            await NetworkUtils.SendObjectAsync(new CMessage("GetMove", null), stream);
            return await NetworkUtils.ReceiveObjectAsync<Move>(stream);
        }

        public async Task<CMessage> Proccess(CMessage msg)
        {
            switch (msg.Type)
            {
                case "GetMove":
                    Move move = await GetMove();
                    return new CMessage("Move", move);
                case "BoardState":

                    

                default:
                    throw new Exception($"Unrecognized CMessage type: {msg.Type}");
            }
        }

        public void Send(CMessage msg) {

        }
        

    }
}

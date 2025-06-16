using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Catan;
namespace Server
{
    internal class NetworkPlayer : Player
    {
        private TcpClient client;
        private NetworkStream stream;
        private Dictionary<Resources, int> resources;

        public string Name { get;}
        public int Id { get;}
        Dictionary<Resources, int> Player.resources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            return await NetworkUtils.ReceiveObjectAsync<Move>(stream);
        }

    }
}

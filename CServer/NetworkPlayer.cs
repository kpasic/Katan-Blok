using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Catan;
using CNetworking;
namespace CServer
{
    public class NetworkPlayer : IPlayer
    {
        public TcpClient client;
        public NetworkStream stream;
        private Dictionary<Resources, int> resources;
        public Dictionary<Resources, int> TradingCurse { get; set; }
        public string Name { get;}
        public int Id { get;}

        public bool myTurn { get; set; }

        Dictionary<Resources, int> IPlayer.resources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ResourcesCount
        {
            get
            {
                int count = 0;
                foreach (int item in resources.Values)
                {
                    count += item;
                }
                return count;
            }
        }

        public NetworkPlayer(TcpClient client, NetworkStream stream, string name, int id)
        {
            this.client = client;
            this.stream = stream;
            Name = name;
            Id = id;

            resources = new Dictionary<Resources, int>();
        }

        public async Task<Move> GetMove()
        {
            await NetworkUtils.SendObjectAsync(new CMessage("Play", null), stream);
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

        public void ChangePoints(int x)
        {
            throw new NotImplementedException();
        }

        public int Points { get; set; }


        public void ChangeCurse(Resources resource, int x)
        {
            resources[resource] = x;
        }
       
        public void RemoveResources(Dictionary<Resources, int> resource)
        {
            foreach (Resources x in resource.Keys)
            {
                resources[x] -= resource[x];
            }
        }

        public void GiveResources(Dictionary<Resources, int> resource)
        {
            foreach (Resources x in resource.Keys)
            {
                resources[x] += resource[x];
            }
        }

        public Resources Rob()
        {
            Random rng = new Random();
            int index = rng.Next(ResourcesCount);
            int rs = 0;
            Resources[] list = (Resources[])Enum.GetValues(typeof(Resources));
            while (index > 0)
            {
                index -= resources[list[rs++]];
            }
            return list[rs];
        }

        public void Give(Resources resource)
        {
            if (resources.ContainsKey(resource)) resources[resource]++;
            else resources[resource] = 1;

        }

    }
}

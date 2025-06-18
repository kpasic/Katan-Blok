using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using Catan;
using Server;

namespace RServer{ 
    public class Program
    {
        const int maxPlayers = 2;

        public static async void Main(string[] args)
        {
            Console.WriteLine("Server startup");
            int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server listening");

            List<IPlayer> players = new List<IPlayer>();
            while(players.Count < maxPlayers)
            {
                Console.WriteLine("Waiting for connection...");
                TcpClient client = await listener.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Player Connected");


                string name = await NetworkUtils.ReceiveObjectAsync<string>(stream);

                NetworkPlayer newPlayer = new NetworkPlayer(client, stream, name, players.Count);
                players.Add(newPlayer);
            }
            



            Game game = new Game(players);

            foreach (IPlayer player in players)
            {
                if (player is NetworkPlayer)
                {
                    NetworkPlayer nPlayer = (NetworkPlayer)player;
                    await nPlayer.SendBoardState(game);
                }
            }

            await game.Update();


            //promeniti ovo eventualno..

            Console.WriteLine("Kraj");
            listener.Stop();
        }
    }
}
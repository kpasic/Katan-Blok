using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using Catan;
using CNetworking;
namespace CServer{ 
    public class Program
    {
        const int maxPlayers = 2;
        private static NetworkPlayer currentPlayer;
        private static bool playerRolled;

        private static Game myGame;
        private static List<IPlayer> players;
        private static CMessage stored;
        private static int storedLifespan;
        public static async Task Start(NetworkPlayer player)
        {
            while (true)
            {
                CMessage msg = await NetworkUtils.ReceiveObjectAsync<CMessage>(player.stream);
                CMessage response = Proccess(msg, player);
                if(storedLifespan <= 0)
                {
                    stored = null;
                    storedLifespan = players.Count - 1;
                }


                NetworkUtils.SendObjectAsync(response, player.stream);

                if (response.Type == "EndGame") break;
            }
        }
        
        public static CMessage Proccess(CMessage msg, NetworkPlayer sender)
        {
            CMessage response;
            switch (msg.Type)
            {
                case "Ok":
                    while (stored == null) Task.Delay(100);
                    storedLifespan--;
                    return stored;
                case "Begin":
                    response = new CMessage("null", null);
                    if (sender == currentPlayer) response.Type = "Play";
                    else response.Type = "Wait";
                    return response;
                case "RequestBoard":
                    response = new CMessage("BoardState", myGame.board);
                    return response;
                case "RequestRoll":
                    (int a, int b) = myGame.board.Roll();
                    myGame.board.DistributeResources(a + b);
                    return new CMessage("Roll", (a, b));
                case "RequestDiscard":
                    DiscardMove discardmove = (DiscardMove)msg.Payload;
                    throw new Exception("kys");
                case "Move":
                    Move move = (Move)msg.Payload;
                    (GameState state, IPlayer kys2) = myGame.Update(move);
                    currentPlayer = (NetworkPlayer)kys2;
                    stored = new CMessage("Move", move);
                    if (currentPlayer == sender) response = new CMessage("Play", null);
                    else if (state == GameState.Active) response = new CMessage("Wait", null);
                    else response = new CMessage("EndGame", null);
                    return response;                 
                default:
                    throw new Exception($"Unrecognized client message: {msg.Type}");
            }

        }


        public static async Task Main(string[] args)
        {
            Console.WriteLine("Server startup");
            int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server listening");

            players = new List<IPlayer>();
            while(players.Count < maxPlayers)
            {
                Console.WriteLine("Waiting for connection...");
                TcpClient client = await listener.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Player Connected");


                CMessage name = await NetworkUtils.ReceiveObjectAsync<CMessage>(stream);
                if (name.Type != "Connect") throw new Exception("ne radi connect");
                try
                {
                    throw new Exception($"test {name.Payload}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                NetworkPlayer newPlayer = new NetworkPlayer(client, stream, (string)name.Payload, players.Count);
                players.Add(newPlayer);

                //Handshake
                await NetworkUtils.SendObjectAsync(new CMessage("Handshake", newPlayer.Id), stream);
            }
            Console.WriteLine("begin game");



            myGame = new Game(players);
            
            for (int i = 0; i < players.Count; i++)
            {
                if(i != players.Count - 1) Start((NetworkPlayer)players[i]);
                else await Start((NetworkPlayer)players[i]);
            }
            


            //promeniti ovo eventualno..

            Console.WriteLine("Kraj");
            Console.ReadLine();

            listener.Stop();
            Console.ReadLine();
           
        }
    }
}
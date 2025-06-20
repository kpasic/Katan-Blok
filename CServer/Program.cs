using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using Catan;
using CNetworking;
namespace CServer{ 
    public class Program
    {
        static int maxPlayers;
        private static NetworkPlayer currentPlayer;
        private static bool playerRolled;

        private static Game myGame;
        private static List<IPlayer> players;
        private static CMessage stored;
        private static int storedLifespan;
        private static HashSet<IPlayer> uninformedPlayers;
        public static async Task Start(NetworkPlayer player)
        {
            while (true)
            {
                CMessage msg = await NetworkUtils.ReceiveObjectAsync<CMessage>(player.stream);
                Console.WriteLine($"poruka {msg.Type} plT {msg.PayloadType}");
                CMessage response = await Proccess(msg, player);
                if(storedLifespan <= 0)
                {
                    stored = null;
                    storedLifespan = players.Count;
                }

                if (response.Type == "BoardState") { 
                    Console.WriteLine("saljem board");
                    Console.WriteLine($"info {response.PayloadType} pl {response.Payload != null} rec {player.stream != null}");
                }


                await NetworkUtils.SendObjectAsync(response, player.stream);
                if (response.Type == "BoardState") Console.WriteLine("poslao sam board");
                if (response.Type == "EndGame") break;
            }
        }
        
        public async static Task<CMessage> Proccess(CMessage msg, NetworkPlayer sender)
        {
            CMessage response;
            switch (msg.Type)
            {
                
                case "Ok":
                    while (stored == null) await Task.Delay(1000);
                    if (uninformedPlayers.Contains(sender))
                    {
                        storedLifespan--;
                        uninformedPlayers.Remove(sender);
                        return stored;
                    }
                    if (currentPlayer == sender) response = new CMessage("Play", null);
                    else { 
                        response = new CMessage("Wait", null); 
                        await Task.Delay(1000);
                    }
                    return response;
                case "Begin":
                    response = new CMessage("null", null);
                    if (sender == currentPlayer) response.Type = "Play";
                    else response.Type = "Wait";
                    return response;
                case "RequestBoard":
                    Console.WriteLine("dobio sam request");
                    await Task.Delay(1000);
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
                    NetworkPlayer prevPlayer = currentPlayer;
                    currentPlayer = (NetworkPlayer)kys2;
                    int senderId = sender.Id;
                    stored = new CMessage("Move", (move, senderId));
                    foreach(NetworkPlayer pp in players)
                    {
                        if (pp != prevPlayer) uninformedPlayers.Add(pp);
                    }
                    if (myGame.GetGameState() == GameState.Active) response = new CMessage("Move", (move, sender.Id));
                    else response = new CMessage("EndGame", null);
                    return response;
                default:
                    //throw new Exception($"Unrecognized client message: {msg.Type}");
                    break;
                }
            Task.Delay(5000);
            return new CMessage("Ok", null);
        }


        public static async Task Main(string[] args)
        {
            Console.WriteLine("Server startup");
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Enter number of players:");
            maxPlayers = int.Parse(Console.ReadLine());

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
            currentPlayer = (NetworkPlayer)players[myGame.currentPlayerIndex];
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

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            Console.WriteLine("\n===================================");
            Console.WriteLine("A GLOBAL UNHANDLED EXCEPTION OCCURRED!");
            Console.WriteLine($"Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            Console.WriteLine("===================================");

            // Indicate whether the CLR is terminating. If false, the app might continue (rare for critical errors).
            if (args.IsTerminating)
            {
                Console.WriteLine("\nApplication is terminating due to this unhandled exception.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(); // Keep the console open
        }
    }
}
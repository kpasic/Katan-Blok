using Catan;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Server
{
    public class Program
    {
        public static void Main()
        {
           /* Console.WriteLine("test");
            Board board = new Board(4);
            Move move = new FirstMove();
            //IPlayer player = new StupidPlayer("cigan", 1);
            //board.GenerateBoard();
            for(int i=0;i<19; i++)
            {
                Console.Write("{0} " , board.board[i]);
            }
            for (int i = 0; i < 19; i++)
            {
                Console.Write("{0} ", board.numbers[i]);
            }
            Console.WriteLine();
            for (int i= 0; i < 19; i++)
            {
                for(int j=0; j<6; j++)
                {
                    Console.Write("{0} ", board.housePositions[i,j]);
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Console.Write("{0} ", board.roadsPositions[i, j]);
                }
                Console.WriteLine();
            }
            List<int> availableMoves = board.LegalHouseMovesBegining(player);
            move.Execute(board, player);
            availableMoves = board.LegalHouseMovesBegining(player);
            foreach(int n in availableMoves)Console.Write("{0} ",n);
            Console.WriteLine();
            availableMoves = board.LegalRoadsMoves(player);
            foreach (int n in availableMoves) Console.Write("{0} ", n);
            Console.WriteLine();
            int[] niz = new int[13];
            for(int i = 0;i<100; i++)
            {
                (int, int) a = board.Roll();
                niz[a.Item1+a.Item2]++;
            }
            foreach(int i in niz)Console.WriteLine(i);*/
        }
    }
}
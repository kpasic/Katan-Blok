using Catan;
using System.Runtime.InteropServices;

namespace Server
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("test");
            Board board = new Board();
            board.GenerateBoard();
            for(int i=0;i<19; i++)
            {
                Console.Write("{0} " , board.board[i]);
            }
            for (int i = 0; i < 19; i++)
            {
                Console.Write("{0} ", board.numbers[i]);
            }
        }
    }
}
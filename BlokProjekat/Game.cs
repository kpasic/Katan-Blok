using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{
    public class Game
    {
        private List<Player> players;
        public Board board;

        public Game(Board board)
        {
            this.board = board; 
        }

    }
}

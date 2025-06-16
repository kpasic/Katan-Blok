using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{
    public interface Player
    {
        public string Name { get; set; }

        public abstract Task<Move> GetMove();
    }

    public class HumanPlayer : Player
    {

    }
}

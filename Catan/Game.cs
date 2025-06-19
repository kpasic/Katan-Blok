using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan
{

    public enum GameState
    {
        Active,
        Over
    }
    public class Game
    {
        private List<IPlayer> players;
        public int currentPlayerIndex { get; private set; }
        public Board board { get; private set; }
        public int discardLimit {get; private set;}
        public int victoryPoints { get; private set; }


        public Game(List<IPlayer> players)
        {
            this.players = players;
            Board board = new Board(players.Count);
        }

        public (int, int) Roll()
        {
            (int a, int b) = board.Roll();
            board.DistributeResources(a + b);
            return (a, b);
        }

        public (GameState, IPlayer) Update(Move move)
        {
            if(move is EndMove)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }

            IPlayer curPlayer = players[currentPlayerIndex];
            return (GetGameState(),  curPlayer);
        }

        public GameState GetGameState()
        {
            return GameState.Active;
        }

    }
}

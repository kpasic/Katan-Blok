using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{

    public enum GameState
    {
        Active,
        Over
    }
    public class Game
    {
        private List<Player> players;
        public int currentPlayerIndex { get; private set; }
        public Board board { get; private set; }

        public Game(Board board)
        {
            this.board = board;
            currentPlayerIndex = 0;
        }

        public Game(List<Player> players)
        {
            this.players = players;
            currentPlayerIndex = 0;
        }

        public async Task Update()
        {
            while(GetGameState() == GameState.Active)
            {
                Player curPlayer = players[currentPlayerIndex];
                Move move = await curPlayer.GetMove();
                if (move != null)
                {
                    move.Execute(board);
                }
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }
        }

        private GameState GetGameState()
        {
            return GameState.Active;
        }

    }
}

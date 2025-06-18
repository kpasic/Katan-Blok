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

        public async Task Update()
        {
            while(GetGameState() == GameState.Active)
            {
                int diceNumber, dice1, dice2;
                    (dice1, dice2) = board.Roll();
                diceNumber = dice1+dice2;   

                if(diceNumber == 7)
                {
                    foreach (IPlayer player in players) {
                        if(player.ResourcesCount > discardLimit)
                        {
                            //await player.Discard();
                        }
                    }
                }

                IPlayer curPlayer = players[currentPlayerIndex];
                Move move = await curPlayer.GetMove();
                if (move != null)
                {
                    move.Execute(board, curPlayer);
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

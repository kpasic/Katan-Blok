using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{
    public interface Player
    {
        public string Name { get; }

        public  Task<Move> GetMove();
    }

    public class HumanPlayer : Player
    {
        private TaskCompletionSource<Move> ?ui;
        public event Action<HumanPlayer> OnMoveRequested;
        public string Name { get; }
        public HumanPlayer(string name)
        {
            Name = name;
        }

        
        public Task<Move> GetMove()
        {
            ui = new TaskCompletionSource<Move>();

            OnMoveRequested?.Invoke(this);

            return ui.Task;
        }

        public void SubmitMove(Move move)
        {
            if (ui != null && !ui.Task.IsCompleted)
            {
                ui.SetResult(move);
            }
        }
    }
}

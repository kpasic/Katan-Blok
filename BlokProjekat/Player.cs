using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{

    public enum Resources
    {
        Wheat,
        Wool,
        Wood,
        Stone,
        Brick,
    }
    public interface Player
    {
        public string Name { get; }
        public  Dictionary<Resources, int> resources { get; set; }

        public int ResourcesCount { get; }
        public  Task<Move> GetMove();

        public Task<int[]> Discard();

        
    }

    public class HumanPlayer : Player
    {
        private TaskCompletionSource<Move> ?ui;
        private TaskCompletionSource<int[]>? ds;

        public event Action<HumanPlayer> OnMoveRequested;
        public event Action<HumanPlayer> OnDiscardRequested;
        public string Name { get; }
        public Dictionary<Resources, int> resources { get; set; }
        public HumanPlayer(string name)
        {
            Name = name;
            resources = new Dictionary<Resources, int>();
        }

        public int ResourcesCount
        {
            get
            {
                int count = 0;
                foreach (int item in resources.Values)
                {
                    count += item;
                }
                return count;
            }
        }
        
        public Task<Move> GetMove()
        {
            ui = new TaskCompletionSource<Move>();

            OnMoveRequested?.Invoke(this);

            return ui.Task;
        }

        public Task<int[]> Discard()
        {
            ds = new TaskCompletionSource<int[]>(5);

            OnDiscardRequested?.Invoke(this);

            return ds.Task;

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

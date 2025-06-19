using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan
{
    public class HumanPlayer : IPlayer
    {
        private TaskCompletionSource<Move>? ui;
        private TaskCompletionSource<int[]>? ds;
        private TaskCompletionSource rl;

        public event Action<HumanPlayer> OnMoveRequested;
        public event Action<HumanPlayer> OnDiscardRequested;
        public string Name { get; }
        public int Id { get; set; }
        public int Points { get; set; }

        public bool myTurn {  get; set; }
        public Dictionary<Resources, int> resources { get; set; }
        public HumanPlayer(string name, int id)
        {
            Name = name;
            resources = new Dictionary<Resources, int>();
            Id = id;
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
        public void ChangePoints(int x)
        {
            Points += x;
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

        public bool CanBuildHouse()
        {
            return resources[Resources.Wood] > 0 && resources[Resources.Wool] > 0 && resources[Resources.Wheat] > 0 && resources[Resources.Brick] > 0;
        }

        public bool CanBuildRoad()
        {
            return resources[Resources.Wood] > 0 && resources[Resources.Brick] > 0;
        }
    }
}

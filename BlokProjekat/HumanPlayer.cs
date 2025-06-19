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
        public Dictionary<Resources, int> TradingCurse { get; set; }
        public HumanPlayer(string name, int id)
        {
            Name = name;
            resources = new Dictionary<Resources, int>();
            Id = id;
        }

        

        
        public void ChangePoints(int x)
        {
            Points += x;
        }

        #region ResourceFunctions

        public void ChangeCurse(Resources resource, int x)
        {
            resources[resource] = x;
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
        public void RemoveResources(Dictionary<Resources, int> resource)
        {
            foreach(Resources x in resource.Keys)
            {
                resources[x] -= resource[x];
            }
        }

        public void GiveResources(Dictionary<Resources, int> resource)
        {
            foreach (Resources x in resource.Keys)
            {
                resources[x] += resource[x];
            }
        }

        public Resources Rob()
        {
            Random rng = new Random();
            int index = rng.Next(ResourcesCount);
            int rs = 0;
            Resources[] list = (Resources[])Enum.GetValues(typeof(Resources));
            while (index > 0)
            {
                index -= resources[list[rs++]];
            }
            return list[rs];
        }

        public void Give(Resources resource)
        {
            if (resources.ContainsKey(resource)) resources[resource]++;
            else resources[resource] = 1;
            
        }

        #endregion
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
        #region Checkifpossible

        public bool CanBuildHouse()
        {
            return resources[Resources.Wood] > 0 && resources[Resources.Wool] > 0 && resources[Resources.Wheat] > 0 && resources[Resources.Brick] > 0;
        }

        public bool CanBuildRoad()
        {
            return resources[Resources.Wood] > 0 && resources[Resources.Brick] > 0;
        }

        public bool CanUpgradeHouse()
        {
            return resources[Resources.Stone] > 2 && resources[Resources.Wheat] > 1;
        }

        public bool CanBuyDevelopment()
        {
            return resources[Resources.Stone] > 0 && resources[Resources.Wheat] > 0 && resources[Resources.Wool] >0;
        }
        #endregion
    }
}

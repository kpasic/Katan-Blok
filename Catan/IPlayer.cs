using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan
{

    public enum Resources
    {
        Wheat,
        Wool,
        Wood,
        Stone,
        Brick,
    }
    public interface IPlayer
    {
        public string Name { get; }
        public int Id { get; }
        public int Points { get; set; }
        public  Dictionary<Resources, int> resources { get; set; }

        public Dictionary<Resources, int> TradingCurse { get; set; }

        public void RemoveResources(Dictionary<Resources, int> takenResources);

        public void ChangeCurse(Resources res, int x);

        public Resources Rob();

        public void Give(Resources resource);

        public void ChangePoints(int x);

        public int ResourcesCount { get; }
        public  Task<Move> GetMove();
        
    }

    
}

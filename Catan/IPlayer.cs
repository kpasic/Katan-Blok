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

        public bool myTurn { get; set; }
        public int Id { get; }
        public int Points { get; set; }
        public  Dictionary<Resources, int> resources { get; set; }

        public Resources Rob()
        {
            Random rng = new Random();
            int index = rng.Next(ResourcesCount);
            int rs = 0;
            Resources[] list = (Resources[])Enum.GetValues(typeof(Resources));
            while(index > 0)
            {
                index -= resources[list[rs++]];
            }
            return list[rs];
        }

        public void Give(Resources resource)
        {
            if(resources.ContainsKey(resource))resources[resource]++;
            else resources[resource] = 1;
        }

        public void ChangePoints(int x);

        public int ResourcesCount { get; }
        public  Task<Move> GetMove();
        
    }

    
}

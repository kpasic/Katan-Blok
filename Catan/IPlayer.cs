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
        public  Dictionary<Resources, int> resources { get; set; }

        public int ResourcesCount { get; }
        public  Task<Move> GetMove();
        
    }

    
}

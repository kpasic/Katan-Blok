using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan
{
    public class StupidPlayer : IPlayer
    {
    
        public string Name { get; }
        public int Id { get; }
        public int Points { get; set; }
        public Dictionary<Resources, int> resources { get; set; }
        public StupidPlayer(string name, int id)
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
           throw new NotImplementedException();
        }



      
        
    }
}

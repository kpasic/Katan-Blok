using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{
    public enum Space
    {
        Empty,
        Road,
        House,
        City
    }

    public enum Tile
    {
        Empty,
        Wheat,
        Wool,
        Wood,
        Stone,
        Brick,
    }

    public abstract class Move
    {
        public abstract void Execute(Board board);
    }

    public class Place : Move
    {
        int targetSpaceId;
        Space targetSpace;
        public override void Execute(Board board)
        {
            
        }
    }
    

    public class Board
    {


        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokProjekat
{

    #region Polja
    public enum Space
    {
        Empty,
        Road,
        House,
        City
    }

    public enum Tile
    {
        Empty, // 0
        Wheat, // 1
        Wool, // 2
        Wood, // 3
        Stone, // 4
        Brick, // 5
    }

    #endregion

    #region Classes

    public class MultiSet<T>
    {
        private Dictionary<T, int> setic;

        public MultiSet()
        {
            setic = new Dictionary<T, int>();
        }

        public MultiSet(IEnumerable<T> items) : this()
        {
            Add(items);
        }

        public bool Contains(T item)
        {
            return setic.ContainsKey(item);
        }

        public void Add(T item)
        {
            if (setic.ContainsKey(item))
                setic[item]++;
            else
                setic[item] = 1;
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }      
    }

        public struct Graph
    {
        int n;
        List<int>[] graph;
        MultiSet<int> allNumbers;
        int[] numberpermutation;
        public Graph(int n)
        {
            this.n = n;
            graph = new List<int>[n];
            numberpermutation = new int[n];
        }

        public Graph(int n,int[] numbers,int position7)
        {
            this.n = n;
            graph = new List<int>[n];
            numberpermutation = new int[n];
            allNumbers.Add(numbers);
            numberpermutation[position7] = 7;
        }

        public void AddEdge(int a, int b)
        {
            graph[a].Add(b);
            graph[b].Add(a);
        }

        void dfs()
        {

        }
    }

    #endregion

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
        int n = 19;
        Tile[] board = {Tile.Empty,
            Tile.Wheat, Tile.Wheat, Tile.Wheat, Tile.Wheat,
            Tile.Wool, Tile.Wool, Tile.Wool, Tile.Wool,
            Tile.Wood, Tile.Wood, Tile.Wood, Tile.Wood,
            Tile.Stone,Tile.Stone, Tile.Stone,
            Tile.Brick,Tile.Brick,Tile.Brick};
        int[] numbers = {2,3,3,4,4,5,5,6,6,7,8,8,9,9,10,10,11,11,12};
        Graph boardGraph;

        public void GenerateBoard()
        {
            GenerateTiles();
            boardGraph = new Graph(n, numbers,FindEmpty());
            GenerateNumbers();
        }

        private int FindEmpty()
        {
            for (int i = 0; i < n; i++) if (board[i] == Tile.Empty) return i;
            return -1;
        }

        private void AddAllEdges()
        {
            boardGraph.AddEdge(0,1);
            boardGraph.AddEdge(1,2);
            boardGraph.AddEdge(0,3);
            boardGraph.AddEdge(0,4);
            boardGraph.AddEdge(1,4);
            boardGraph.AddEdge(1,5);
            boardGraph.AddEdge(2,5);
            boardGraph.AddEdge(2,6);
            boardGraph.AddEdge(3,4);
            boardGraph.AddEdge(3,7);
            boardGraph.AddEdge(3,8);
            boardGraph.AddEdge(4,5);
            boardGraph.AddEdge(4,8);
            boardGraph.AddEdge(4,9);
            boardGraph.AddEdge(5,6);
            boardGraph.AddEdge(5,9);
            boardGraph.AddEdge(5,10);
            boardGraph.AddEdge(6,10);
            boardGraph.AddEdge(6,11);
            boardGraph.AddEdge(7,8);
            boardGraph.AddEdge(7,12);
            boardGraph.AddEdge(8,9);
            boardGraph.AddEdge(8,12);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
            boardGraph.AddEdge(8,13);
        }

        private void GenerateTiles()
        {
            int n = board.Length;
            Random rnd = new Random();
            while(n>1)
            {
                int k = rnd.Next(n--);
                Tile tile = board[n];
                board[n] = board[k];
                board[k] = tile;
            }
        }

        private void GenerateNumbers()
        {

        }
        
    }
}

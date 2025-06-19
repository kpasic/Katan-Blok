using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Catan
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

    #region Structure

    public class MultiSet<T>:IEnumerable<T>
    {
        private Dictionary<T, int> setic;

        public int Count()
        {
            return setic.Count; 
        }

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

        public void Remove(T item)
        {
            if (!setic.ContainsKey(item))
                throw new ArgumentException();
            if (--setic[item] == 0)
                setic.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in setic)
                for (int i = 0; i < kvp.Value; i++)
                    yield return kvp.Key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }



    }

    public struct Graph
    {
        int n;
        bool nasaoKombinaciju = false;
        int []susedi = {1, 2, 6, 7, 3,4,5,8,9,10,11,15,16,12,13,14,17,18,18};
        List<int>[] graph;
        MultiSet<int> allNumbers;
        public int[] numberpermutation;
        bool[] visited;
        public Graph(int n)
        {
            this.n = n;
            graph = new List<int>[n];
            numberpermutation = new int[n];
            visited = new bool[n];
        }

        public Graph(int n, int[] numbers, int position7)
        {
            this.n = n;
            graph = new List<int>[n];
            for(int i=0; i<n; i++) graph[i] = new List<int>();
            allNumbers = new MultiSet<int>();
            numberpermutation = new int[n];
            allNumbers.Add(numbers);
            numberpermutation[position7] = 7;
            visited = new bool[n];
        }

        public void AddEdge(int a, int b)
        {
            graph[a].Add(b);
            graph[b].Add(a);
        }

        void dfs(int nod, int cnt)
        {
            visited[nod] = true;
            if (numberpermutation[nod] == 7)
            {
                if (cnt == n)
                {
                    nasaoKombinaciju = true;
                    return;
                }
                dfs(susedi[nod], cnt + 1);
                if (nasaoKombinaciju) return;
            }
            else
            {
                int trcnt = 0;
                for(int i=2; i<=12; i++)if (allNumbers.Contains(i)) trcnt++;
                int[] trpermutation = new int[trcnt];
                trcnt = 0;
                for (int i = 2; i <= 12; i++) if (allNumbers.Contains(i)) trpermutation[trcnt++]= i;
                GenerateRandomPermutation(trpermutation);
                foreach(int nm in trpermutation)
                {
                    if (!allNumbers.Contains(nm)) continue;
                    allNumbers.Remove(nm);
                    numberpermutation[nod] = nm;
                    bool isValid = true;
                    foreach (int x in graph[nod])
                    {
                        if (numberpermutation[x] == nm || checkValid68(nm, numberpermutation[x]))
                        {
                            isValid = false; break;
                        }
                    }
                    if (isValid)
                    {
                        if (cnt == n)
                        {
                            nasaoKombinaciju = true;
                            return;
                        }
                        dfs(susedi[nod],cnt + 1);
                        if (nasaoKombinaciju) return;

                    }
                    allNumbers.Add(nm);
                    numberpermutation[nod] = 0;
                }
            }
            

        }


        private bool checkValid68(int a, int b)
        {
            return (Math.Min(a, b) == 6 && Math.Max(a, b) == 8);
        }


        public int[] GenerateNumberPermutation()
        {
            allNumbers.Remove(7);
            Random rnd = new Random();
            dfs(0, 1);
            return numberpermutation;
        }

        private void GenerateRandomPermutation(int[] array)
        {
            int n = array.Length;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = rnd.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
    }

    public struct PlaceGraph
    {
        int n;
        List<Edge>[] graph;
        public PlaceGraph(int n)
        {
            this.n = n;
            graph = new List<Edge>[n];
            for (int i = 0; i < n; i++) graph[i] = new List<Edge>();
        }

        public void AddEgde(int a, int b, Edge edge)
        {
            graph[a].Add(edge);
            graph[b].Add(edge);
        }

        public HashSet<int> AdjecentNodes(int node)
        {
            HashSet<int> adj = new HashSet<int> ();
            foreach(Edge n in graph[node])
            {
                adj.Add(n.nod1.nodeIndex);
                adj.Add(n.nod2.nodeIndex);
            }
            return adj;
        }


        public HashSet<int> AdjecentEdges(int node)
        {
            HashSet<int> adj = new HashSet<int>();
            foreach (Edge n in graph[node]) adj.Add(n.edgeId);
            return adj;
        }
    }

    #endregion


    #region Moves
    public abstract class Move
    {
        public abstract void Execute(Board board, IPlayer player);
    }

    public class HouseMove : Move
    {
        int nodeId;
        Space nodeSpace;
        public override void Execute(Board board, IPlayer player)
        {
            board.PlaceHouse(nodeId, player, nodeSpace);
        }
    }

    public class FirstMove : Move
    {
        int nodeId;
        Space nodeSpace;
        public override void Execute(Board board, IPlayer player)
        {
            board.PlaceHouse(nodeId, player, nodeSpace);
        }
    }

    public class RoadMove : Move
    {
        int roadId;
        public override void Execute(Board board, IPlayer player)
        {
            board.PlaceRoad(roadId, player, Space.Road);
        }
    }

    public class TradeMove : Move
    {
        public override void Execute(Board board, IPlayer player)
        {
            throw new NotImplementedException();
        }
    }

    public class RobberMove : Move
    {
        int tileId;
        IPlayer RobberPlayer;
        public override void Execute(Board board, IPlayer player)
        {
            board.MoveRobber(tileId, player, RobberPlayer);
        }
    }

    public class UpgradeMove : Move
    {
        int nodeId;
        public override void Execute(Board board, IPlayer player)
        {
            board.UpgradeHouse(nodeId, player);
        }
    }




    #endregion


    public class Node
    {
        public Node(int nmPlayers, int index) {
            Owner = null;
            state = true;
            nodeIndex = index;
            Type = Space.Empty;
            isAvailable = new HashSet<int>();
            for(int i=0; i< nmPlayers; i++)isAvailable.Add(i);
        }

        public bool state;
        private HashSet<int> isAvailable;
        public int nodeIndex;

        public bool CanPlace(int playerId)
        {
            return isAvailable.Contains(playerId);
        }

        public IPlayer? Owner { get; private set; }
        public Space Type { get; private set; }

        public void SetOwner(IPlayer? player)
        {
            Owner = player;
            isAvailable.Clear();
        }

        public void SetState(bool state)
        {
            this.state = state;
        }

        public void Add(int x)
        {
            isAvailable.Add(x);
        }

        public void Remove(int x)
        {
            isAvailable.Remove(x);
        }
        public void SetType(Space type) { Type = type; }
    }

    public class Edge
    {
        public int edgeId;
        public Node nod1;
        public Node nod2;

        public Edge(Node n1, Node n2, int id)
        {
            edgeId = id;
            Owner = null;
            nod1 = n1;
            nod2 = n2;
            Type = Space.Empty;
            isAvailable = new HashSet<int>();
        }
        public bool CanPlace(int playerId)
        {
            return isAvailable.Contains(playerId);
        }

        public void Add(int x)
        {
            isAvailable.Add(x);
        }

        private HashSet<int> isAvailable;
        public IPlayer? Owner { get; private set; }
        public Space Type { get; private set; }

        
        public void SetOwner(IPlayer player) 
        {
            Owner = player;
            isAvailable.Clear();
            
        }
        public void SetType(Space type) { Type = type; }
    }
    public class Board
    {
        #region Properties
        int robberPosition;
        int n = 19;
        int cntNodes = 54;
        int cntRoads = 72;
        public Tile[] board;
        public int[] numbers;
        Graph boardGraph;
        PlaceGraph placeGraph;
        Dictionary<int, Resources> allPorts;
        public int[,] housePositions;
        public int[,] roadsPositions;
        public Node[] allNodes;
        public Edge[] allRoads;
        (int, int)[] portNodes = { (0, 1), (3, 4), (14, 15), (26, 37), (45, 46), (50, 51), (47, 48), (28, 38), (7, 17) };
        string[] portStrings = { "wood", "brick", "stone", "wheat", "sheep", "31", "31", "31", "31" };
        int nmPlayers;
        #endregion

        #region BoardConstructor
        public Board() {
            nmPlayers = 0;
            n = 19;
            board = new Tile[]{
                Tile.Empty,
            Tile.Wheat, Tile.Wheat, Tile.Wheat, Tile.Wheat,
            Tile.Wool, Tile.Wool, Tile.Wool, Tile.Wool,
            Tile.Wood, Tile.Wood, Tile.Wood, Tile.Wood,
            Tile.Stone,Tile.Stone, Tile.Stone,
            Tile.Brick,Tile.Brick,Tile.Brick};
            numbers = new int[] { 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            robberPosition = FindEmpty();
            GenerateBoard();
            housePositions = new int[n,6];
            roadsPositions = new int[n,6];
            allNodes = new Node[cntNodes];
            for(int i=0; i<cntNodes; i++) allNodes[i] = new Node(nmPlayers, i);
            allRoads = new Edge[cntRoads];
            placeGraph = new PlaceGraph(cntNodes);
            MakeNodeGraph();
            GenerateHousePositions();
            GenerateRoadsPositions();
            GeneratePort();
            
        }

        public Board(int nmPlayers)
        {
            this.nmPlayers = nmPlayers;
            n = 19;
            board = new Tile[]{
                Tile.Empty,
            Tile.Wheat, Tile.Wheat, Tile.Wheat, Tile.Wheat,
            Tile.Wool, Tile.Wool, Tile.Wool, Tile.Wool,
            Tile.Wood, Tile.Wood, Tile.Wood, Tile.Wood,
            Tile.Stone,Tile.Stone, Tile.Stone,
            Tile.Brick,Tile.Brick,Tile.Brick};
            robberPosition = FindEmpty();
            numbers = new int[] { 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            GenerateBoard();
            housePositions = new int[n, 6];
            roadsPositions = new int[n, 6];
            allNodes = new Node[cntNodes];
            for (int i = 0; i < cntNodes; i++) allNodes[i] = new Node(nmPlayers,i);
            allRoads = new Edge[cntRoads];
            placeGraph = new PlaceGraph(cntNodes);
            MakeNodeGraph();
            GenerateHousePositions();
            GenerateRoadsPositions();
            GeneratePort();
        }

        #endregion

        #region LegalMoves
        public List<int> LegalHouseMovesBegining(IPlayer player)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < cntNodes; i++) if (allNodes[i].CanPlace(player.Id)) result.Add(allNodes[i].nodeIndex);
            return result;
        }

        public List<int> LegalUpgradeMoves(IPlayer player)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < cntNodes; i++) if (allNodes[i].Owner == player) result.Add(allNodes[i].nodeIndex);
            return result;
        }

        public List<int> LegalRoadsMoves(IPlayer player)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < cntRoads; i++) if (allRoads[i].CanPlace(player.Id))result.Add(i);
            return result;
        }

        public List<int> LegalAdjecentRoadMoves(IPlayer player, int nodeId)
        {
            List<int> result = new List<int>();
            foreach(int n in placeGraph.AdjecentEdges(nodeId)) if (allRoads[n].CanPlace(player.Id)) result.Add(n);
            return result;
        }

        
        public List<int> LegalRobberMoves()
        {
            List<int> results = new List<int>();
            for (int i = 0; i < n; i++)
            {
                bool valid = true;
                for (int j = 0; j < 6; i++)
                {
                    Node node = allNodes[housePositions[i, j]];
                    if (node.Owner == null) continue;
                    if (node.Owner.Points == 2)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) results.Add(i);
            }
            return results;
        }
        
        #endregion

        #region BoardFunctions

        private void AddRow(int a, int b, int x)
        {
            for (int i = a; i <= b; i++) if (i != b) boardGraph.AddEdge(i, i + 1);
            if (x == 0) return;
            for (int i = a; i <= b; i++)
            {
                boardGraph.AddEdge(i, i + x);
                boardGraph.AddEdge(i, i + x + 1);
            }
        }

        private void AddAllEdges()
        {
            AddRow(0, 2, 3);
            AddRow(3, 6, 4);
            AddRow(7, 11, 0);
            AddRow(12, 15, -5);
            AddRow(16, 18, -4);
        }

        public void AddRoads(int node, int playerId)
        {
            foreach(int e in placeGraph.AdjecentEdges(node))
            {
                if (allRoads[e].Owner == null) allRoads[e].Add(playerId);
            }
        }
        private void MakeNodeGraph()
        {
            HorizontalEdges(0, 5, 0);
            HorizontalEdges(10, 17, 7);
            HorizontalEdges(23, 32, 16);
            HorizontalEdges(39, 48, 27);
            HorizontalEdges(54, 61, 38);
            HorizontalEdges(66, 71, 47);
            VerticalEdges(6, 9, 0, 8);
            VerticalEdges(18, 22, 7, 10);
            VerticalEdges(33, 38, 16, 11);
            VerticalEdges(49, 53, 28, 10);
            VerticalEdges(62, 65, 39, 8);
        }

        private void VerticalEdges(int a, int b, int start, int off)
        {
            for (int i = a; i <= b; i++)
            {
                allRoads[i] = new Edge(allNodes[start], allNodes[start + off], i);
                placeGraph.AddEgde(start, start+ off, allRoads[i]);
                start += 2;
            }
        }

        private void HorizontalEdges(int a, int b, int start)
        {
            for(int i=a; i<=b; i++)
            {
                allRoads[i] = new Edge(allNodes[start], allNodes[start+1], i);
                placeGraph.AddEgde(start,start+1,allRoads[i]);
                start++;
            }
        }

        public (int, int) Roll()
        {
            Random dice = new Random();
            int nm1 = dice.Next(1, 7);
            int nm2 = dice.Next(1, 7);
            DistributeResources(nm1 + nm2);
            return (nm1, nm2);
            //throw new NotImplementedException();
        }

        public void DistributeResources(int nm)
        {
            for (int i = 0; i < n; i++)
            {
                if (numbers[i] == nm)
                {
                    if (robberPosition == i) continue;
                    for (int j = 0; j < 6; j++)
                    {
                        Node node = allNodes[housePositions[i, j]];
                        if (node.Owner != null) GiveResource(node.Owner, FindResources(board[i]), (node.Type == Space.House) ? 1 : 2);
                    }
                }
            }
        }

        Resources FindResources(Tile tile)
        {
            int index = Array.IndexOf(Enum.GetValues(typeof(Tile)), tile);
            Resources value = (Resources)Enum.GetValues(typeof(Resources)).GetValue(index);
            return value;

        }

        private void GiveResource(IPlayer player, Resources resource, int nm)
        {
            for(int i = 0; i < nm; i++) player.Give(resource);
        }

        private int FindEmpty()
        {
            for (int i = 0; i < n; i++) if (board[i] == Tile.Empty) return i;
            return -1;
        }

        #endregion

        #region BoardMoves
        public void PlaceHouse(int nodeId, IPlayer player, Space nodeSpace)
        {
            HashSet<int> adj = placeGraph.AdjecentNodes(nodeId);
            AddRoads(nodeId, player.Id);
            for (int i = 0; i < 9; i++)
            {
                if (nodeId == portNodes[i].Item1 || nodeId == portNodes[i].Item2)
                {
                    string s = portStrings[i];
                    switch (s)
                    {
                        case ("wood"):
                            player.ChangeCurse(Resources.Wood, 2);
                            break;
                        case ("sheep"):
                            player.ChangeCurse(Resources.Wood, 2);
                            break;
                        case ("brick"):
                            player.ChangeCurse(Resources.Brick, 2);
                            break;
                        case ("stone"):
                            player.ChangeCurse(Resources.Brick, 2);
                            break;
                        case ("wheat"):
                            player.ChangeCurse(Resources.Wheat, 2);
                            break;
                        case ("31"):
                            player.ChangeCurse(Resources.Wood, 3);
                            player.ChangeCurse(Resources.Wood, 3);
                            player.ChangeCurse(Resources.Brick, 3);
                            player.ChangeCurse(Resources.Brick, 3);
                            player.ChangeCurse(Resources.Wheat, 3);
                            break;
                    }
                }
            }
            foreach(int i in adj)
            {
                allNodes[i].SetOwner(null);
                allNodes[i].SetState(false);
            }
            allNodes[nodeId].SetType(nodeSpace);
            allNodes[nodeId].SetOwner(player);
            allNodes[nodeId].SetState(false);
            player.ChangePoints(1);
        }


        public void PlaceRoad(int roadId, IPlayer player, Space space)
        {
            HashSet<int> adj1 = placeGraph.AdjecentNodes(allRoads[roadId].nod1.nodeIndex);
            HashSet<int> adj2 = placeGraph.AdjecentNodes(allRoads[roadId].nod2.nodeIndex);
            foreach(int i in adj1)if (allNodes[i].state) allNodes[i].Add(player.Id);
            foreach (int i in adj2) if (allNodes[i].state) allNodes[i].Add(player.Id);
            allRoads[roadId].SetType(space);
            allRoads[roadId].SetOwner(player);
        }

        public void MoveRobber(int tileId, IPlayer player, IPlayer robing)
        {
            robberPosition = tileId;
            Resources resources = robing.Rob();
            player.Give(resources);

        }

        public void UpgradeHouse(int nodeId,  IPlayer player)
        {
            allNodes[nodeId].SetType(Space.City);
            player.ChangePoints(1);
        }

        #endregion

        #region BoardGeneration
        private void GenerateBoard()
        {
            GeneratePermutation(board);
            boardGraph = new Graph(n, numbers, robberPosition);
            AddAllEdges();
            GenerateNumbers();
            numbers = boardGraph.numberpermutation;
        }

        private void GeneratePort()
        {
            GeneratePermutation(portNodes);
            GeneratePermutation(portStrings);
        }

        private void GeneratePermutation<T>(T[] ts)
        {
            int n = ts.Length;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = rnd.Next(n--);
                (ts[n], ts[k]) = (ts[k], ts[n]);
            }
        }
        
        /*
        private void GenerateTiles()
        {
            int n = board.Length;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = rnd.Next(n--);
                (board[n], board[k]) = (board[k], board[n]);
            }
        }
        */

        private void GenerateNumbers()
        {
            numbers = boardGraph.GenerateNumberPermutation();
        }
        private void GenerateRoadsPositions()
        {
            GenerateRoadsPositionsRow(0, 2,0, 6, 11);
            GenerateRoadsPositionsRow(3, 6,10, 18, 14);
            GenerateRoadsPositionsRow(7, 11,23, 33, 16);
            GenerateRoadsPositionsRow(12, 15,40, 49, 14);
            GenerateRoadsPositionsRow(16, 18,55, 61, 11);
        }

        private void GenerateRoadsPositionsRow(int a, int b, int first, int mid, int off)
        {
            for (int i = a; i <= b; i++)
            {
                int poz = 0;
                roadsPositions[i, poz++] = first;
                roadsPositions[i, poz++] = first++ + off;
                roadsPositions[i, poz++] = mid;
                roadsPositions[i, poz++] = ++mid;
                roadsPositions[i, poz++] = first;
                roadsPositions[i, poz++] = first++ + off;
            }
        }


        private void GenerateHousePositions()
        {
            GenerateHousePositionsRow(0, 2, 0, 8);
            GenerateHousePositionsRow(3, 6, 7, 10);
            GenerateHousePositionsRow(7, 11, 16, 11);
            GenerateHousePositionsRow(12, 15, 28, 10);
            GenerateHousePositionsRow(16, 18, 39, 8);
        }

        private void GenerateHousePositionsRow(int a, int b, int first, int off)
        {
            for (int i = a; i <= b; i++)
            {
                int poz = 0;
                housePositions[i, poz++] = first;
                housePositions[i, poz++] = first++ + off;
                housePositions[i, poz++] = first ;
                housePositions[i, poz++] = first++ + off;
                housePositions[i, poz++] = first ;
                housePositions[i, poz++] = first + off;
            }
        }

        
        #endregion


        public void Clone(Board other)
        {
            this.robberPosition = other.robberPosition;
            this.n = other.n;
            this.cntNodes = other.cntNodes;
            this.cntRoads = other.cntRoads;
            this.board = other.board;
            this.numbers = other.numbers;
            this.boardGraph = other.boardGraph;
            this.placeGraph = other.placeGraph;
            this.housePositions = other.housePositions;
            this.roadsPositions = other.roadsPositions;
            this.allNodes = other.allNodes;
            this.allRoads = other.allRoads;
            this.nmPlayers = other.nmPlayers;
        }
        
    }
}

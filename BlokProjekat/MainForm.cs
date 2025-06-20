using System.Net.Security;
using System.Diagnostics;
using Catan;
using CNetworking;
namespace ClientApp
{
    public partial class MainForm : Form
    {


        NetworkClient client;
        MsgTransceiver transceiver;
        HumanPlayer myPlayer;
        Board myBoard;

        Panel gamePanel;
        float gameScale = 0.8f;
        float gameLeftMargin = 0.1f;
        float gameTopMargin = 0.1f;

        float gameScrollSpeed = 0.0005f;

        static float interactRadius = 5f;



        Dictionary<Tile, SolidBrush> colors;
        Dictionary<int, SolidBrush> playerColors;
        Dictionary<int, Point> HousePoints;
        Dictionary<int, Point> RoadPoints;
        

        bool placingHouse, placingRoad, upgradingHouse;


        public MainForm()
        {
            InitializeComponent();

            // List<Player> list = new List<Player>();
            //curGame = new Game(list);

            myBoard = new Board();

            gamePanel = new Panel();
            gamePanel.BackColor = Color.Blue;
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.TabStop = true;
            //gamePanel.SetStyle(ControlStyles.Selectable, true);

            //Debug.WriteLine("Main form init");
            gamePanel.MouseEnter += (object sender, EventArgs e) => gamePanel.Focus();
            gamePanel.MouseWheel += (object sender, MouseEventArgs e) =>
            {
                
                gameScale += gameScrollSpeed * e.Delta;
                gameScale = Math.Clamp(gameScale, 0.1f, 0.9f);
                gamePanel.Invalidate();
            };
            
            gamePanel.Paint += DrawGame;
            Resize += (object sender, EventArgs e) => gamePanel.Invalidate();
            Resize += (object sender, EventArgs e) => Invalidate();

            mainGrid.Controls.Add(gamePanel, 0, 0);


            // Colors
            colors = new Dictionary<Tile, SolidBrush>
            {
                { Tile.Empty, new SolidBrush(Color.LightGoldenrodYellow) },
                { Tile.Wheat, new SolidBrush(Color.Yellow) },
                { Tile.Wood, new SolidBrush(Color.DarkGreen) },
                { Tile.Wool, new SolidBrush(Color.LimeGreen) },
                { Tile.Brick, new SolidBrush(Color.OrangeRed) },
                { Tile.Stone, new SolidBrush(Color.DarkSlateGray) }
            };

           
            //Points
            HousePoints = new Dictionary<int, Point>();
            RoadPoints = new Dictionary<int, Point>();

            //Controls
            btnBuildHouse.Enabled = false;
            btnBuildRoad.Enabled = false;
            placingHouse = false;
            placingRoad = false;
            upgradingHouse = false;


            //Networking & Game
            myPlayer = new HumanPlayer("nis", 0);
            transceiver = new MsgTransceiver(myBoard, myPlayer, this);
            client = new NetworkClient(transceiver);

            myPlayer.OnMoveRequested += OnMoveRequested;
            myPlayer.OnDiscardRequested += OnDiscardRequested;
        }

        private void OnDiscardRequested(HumanPlayer obj)
        {
            throw new NotImplementedException();
        }

        private float SqrDistance(Point a, Point b)
        {
            return ((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private int? CollidePoints(Point p, Dictionary<int, Point> dict)
        {
            foreach (int ind in dict.Keys)
            {
                if (SqrDistance(p, dict[ind]) <= interactRadius * interactRadius)
                {
                    return ind;
                }
            }
            return null;
        }


        private async Task<Point> MouseClickAsync(Control control)
        {
            var tcs = new TaskCompletionSource<Point>();

            MouseEventHandler handler = null;
            handler = (s, e) =>
            {
                // Unsubscribe after click
                control.MouseClick -= handler;
                tcs.SetResult(e.Location);
            };

            control.MouseClick += handler;
            return await tcs.Task;
        }

        private async void OnMoveRequested(HumanPlayer player)
        {
            btnBuildHouse.Enabled = true;
            btnBuildRoad.Enabled = true;
            //PlaceMove place = new PlaceMove();


            while (true)
            {
                Point p = await MouseClickAsync(gamePanel);
                Debug.WriteLine("click");
                if (placingHouse)
                {
                    int? index = CollidePoints(p, HousePoints);
                    if (index != null)
                    {
                        HouseMove move = new HouseMove();
                        move.nodeId = (int)index;
                        move.nodeSpace = Space.House;
                        myPlayer.SubmitMove(move);
                        return;
                    }
                }
                else if (placingRoad)
                {

                }
                else if (upgradingHouse)
                {

                }
            }
            //player.SubmitMove(place);
            //ui sranja.
        }


        private async void btnConnectServer_Click(object sender, EventArgs e)
        {
            using (ConnectDialog dg = new ConnectDialog())
            {
                if (dg.ShowDialog() == DialogResult.OK)
                {
                    CMessage msg = await client.ConnectAsync(dg.ip, dg.port, dg.username);
                    if (msg.Type == "Handshake")
                    {
                        myPlayer.Id = (int)msg.Payload;
                        Debug.WriteLine($"MY ID {myPlayer.Id}");

                        btnConnectServer.Enabled = false;
                        CMessage startMsg = new CMessage("RequestBoard", null);
                        client.StartListening(startMsg);
                    }
                    else
                    {
                        throw new Exception("Bad client handshake");
                    }
                }
                else
                {
                    //throw new Exception("ijao");
                }
            }

        }

        #region drawing
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            btnBuildHouse.Enabled = myPlayer.CanBuildHouse() && myPlayer.myTurn;
            btnBuildRoad.Enabled = myPlayer.CanBuildRoad() && myPlayer.myTurn;
            //Debug.Write($"jednakost referenca je {ReferenceEquals(myBoard, transceiver.board)}");
        }


        static Point[] GetHexagonPoints(Point topLeft, float sideLength, bool pointy)
        {
            Point[] points = new Point[6];

            double r = sideLength / 2f;
            double centerX = topLeft.X + sideLength / 2.0;
            double centerY = topLeft.Y + sideLength / 2.0;

            for (int i = 0; i < 6; i++)
            {
                double angle_deg = 60 * i;
                if (pointy) angle_deg -= 30;
                double angle_rad = Math.PI / 180 * angle_deg;
                float px = (float)(centerX + r * Math.Cos(angle_rad));
                float py = (float)(centerY + r * Math.Sin(angle_rad));
                points[i] = new Point((int)px, (int)py);
            }

            return points;
        }

        static Point Lerp(Point A, Point B, float t)
        {
            Point C = new Point();
            C.X = A.X + (int)((B.X - A.X) * t);
            C.Y = A.Y + (int)((B.Y - A.Y) * t);
            return C;
        }

        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            float Size = gamePanel.Height * gameScale;
            float topMargin = gamePanel.Height * gameTopMargin;
            float leftMargin = gamePanel.Width * gameLeftMargin;




            SolidBrush p = new SolidBrush(Color.CadetBlue);
            g.FillPolygon(p, GetHexagonPoints(new Point((int)(leftMargin), (int)(topMargin)), Size, false));
            //g.DrawRectangle(new Pen(Color.Red), (int)leftMargin, (int)topMargin, (int)Size, (int)Size);
            float cellScale = 1 / 5.5f;
            float cellSize = Size * cellScale;


            float MidLeftOffset = leftMargin + cellSize / 4f; //+ Size*MathF.Sqrt(3)/16f;
            float sTopOffset = topMargin + Size / 2.6f + (cellSize - cellSize * MathF.Sqrt(3) / 8f) / 8f;

            //Debug.WriteLine($"kys {sTopOffset + 2.5f * (cellSize - cellSize * MathF.Sqrt(3) / 8f) - topMargin - Size/2f}");

            int[] rowOffsets = { 3, 7, 12, 16, 19 };
            int[] lOffsets = { 2, 1, 0, 1, 2 };
            int currentRow = 0;
            for (int i = 0; i < 19; i++)
            {
                float sLeftOffset = MidLeftOffset;
                if (i == rowOffsets[currentRow])
                {
                    //Debug.WriteLine("secerlema " + currentRow);
                    currentRow++;
                }
                sLeftOffset += lOffsets[currentRow] * cellSize / 2f;
                int currentColumn = i;
                if (currentRow != 0) currentColumn -= rowOffsets[currentRow - 1];

                Point[] hexPoints = GetHexagonPoints(new Point((int)(sLeftOffset + cellSize * currentColumn), (int)(sTopOffset + (cellSize - cellSize * MathF.Sqrt(3) / 8f) * (currentRow - 2))), cellSize, true);

                //Debug.Write($"TILE {i}");
                

                HousePoints[myBoard.housePositions[i, 0]] = hexPoints[4];
                HousePoints[myBoard.housePositions[i, 1]] = hexPoints[3];
                HousePoints[myBoard.housePositions[i, 2]] = hexPoints[5];
                HousePoints[myBoard.housePositions[i, 3]] = hexPoints[2];
                HousePoints[myBoard.housePositions[i, 4]] = hexPoints[0];
                HousePoints[myBoard.housePositions[i, 5]] = hexPoints[1];

                RoadPoints[myBoard.housePositions[i, 0]] = Lerp(hexPoints[4], hexPoints[5], 0.5f);
                RoadPoints[myBoard.housePositions[i, 1]] = Lerp(hexPoints[5], hexPoints[0], 0.5f);
                RoadPoints[myBoard.housePositions[i, 2]] = Lerp(hexPoints[3], hexPoints[4], 0.5f);
                RoadPoints[myBoard.housePositions[i, 3]] = Lerp(hexPoints[0], hexPoints[1], 0.5f);
                RoadPoints[myBoard.housePositions[i, 4]] = Lerp(hexPoints[2], hexPoints[3], 0.5f);
                RoadPoints[myBoard.housePositions[i, 5]] = Lerp(hexPoints[1], hexPoints[2], 0.5f);


                

                float radius = cellSize / 5f;
                //g.DrawEllipse(new Pen(Color.Red), hexPoints[0].X - radius, hexPoints[0].Y - radius, radius * 2, radius * 2);
                //g.DrawEllipse(new Pen(Color.Green), hexPoints[1].X - radius, hexPoints[1].Y - radius, radius * 2, radius * 2);


                g.FillPolygon(colors[myBoard.board[i]], hexPoints);
                //g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)(sLeftOffset + cellSize * currentColumn), (int)(sTopOffset + (cellSize - cellSize*MathF.Sqrt(3)/8f) * (currentRow-2)), (int)cellSize, (int)cellSize));
            }



            foreach (int ind in HousePoints.Keys)
            {
                float radius = cellSize / 5f;
                if(placingHouse) g.DrawEllipse(new Pen(Color.Blue), HousePoints[ind].X - radius, HousePoints[ind].Y - radius, radius*2, radius*2);
            }

            foreach (Point po in RoadPoints.Values)
            {
                float radius = cellSize / 5f;
                //g.DrawEllipse(new Pen(Color.Red), po.X - radius, po.Y - radius, radius * 2, radius*2);
            }
            for(int i = 0; i < myBoard.allNodes.Count(); i++)
            {
                if (myBoard.allNodes[i].Owner != null)
                {
                    Point pn = new Point(HousePoints[i].X, HousePoints[i].Y);
                    int radius = 5;
                    pn.X -= radius;
                    pn.Y -= radius;
                    g.FillEllipse(playerColors[myBoard.allNodes[i].Owner.Id], new Rectangle(pn.X, pn.Y, radius, radius));
                }
            }
        }
        #endregion

        private void btnBuildHouse_Click(object sender, EventArgs e)
        {
            placingHouse = !(placingHouse);
            gamePanel.Invalidate();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Process.Start("..\\..\\..\\..\\CServer\\bin\\Debug\\net8.0\\CServer.exe");
        }

        private void testTimer_Tick(object sender, EventArgs e)
        {
           //
        }
    }
}

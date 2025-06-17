using System.Net.Security;
using System.Diagnostics;
using Catan;
namespace ClientApp
{
    public partial class MainForm : Form
    {

        private Game curGame;
        NetworkClient client;
        Panel gamePanel;
        float gameScale = 0.8f;
        float gameLeftMargin = 0.1f;
        float gameTopMargin = 0.1f;

        float gameScrollSpeed = 0.0005f;

        Board tempBoard = new Board();

        Dictionary<Tile, SolidBrush> colors;
        
        public MainForm()
        {
            InitializeComponent();

           // List<Player> list = new List<Player>();
            //curGame = new Game(list);
            
            tempBoard = new Board();

            gamePanel = new Panel();
            gamePanel.BackColor = Color.Blue;
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.TabStop = true;
            //gamePanel.SetStyle(ControlStyles.Selectable, true);

            Debug.WriteLine("Main form init");
            gamePanel.MouseEnter += (object sender, EventArgs e) => gamePanel.Focus();
            gamePanel.MouseWheel += (object sender, MouseEventArgs e) =>
            {
                Debug.WriteLine($"detect scroll: {e.Delta} ");
                gameScale += gameScrollSpeed * e.Delta;
                gameScale = Math.Clamp(gameScale, 0.1f, 0.9f);
                gamePanel.Invalidate();
            };
            //events
            gamePanel.Paint += DrawGame;
            Resize += (object sender, EventArgs e) => gamePanel.Invalidate();

            mainGrid.Controls.Add(gamePanel, 0, 0);


            // Colors
            colors = new Dictionary<Tile, SolidBrush>();
            colors.Add(Tile.Empty, new SolidBrush(Color.LightGoldenrodYellow));
            colors.Add(Tile.Wheat, new SolidBrush(Color.Yellow));
            colors.Add(Tile.Wood, new SolidBrush(Color.DarkGreen));
            colors.Add(Tile.Wool, new SolidBrush(Color.LimeGreen));
            colors.Add(Tile.Brick, new SolidBrush (Color.OrangeRed));
            colors.Add(Tile.Stone, new SolidBrush(Color.DarkSlateGray));

        }

        private void OnMoveRequested(HumanPlayer player)
        {
            //curGame.board.GetLegalMoves();
            PlaceMove place = new PlaceMove();

            player.SubmitMove(place);
            //ui sranja.
        }

        private void OnDiscardRequested(HumanPlayer player)
        {
            //mora da dozvoli submit samo ako je broj discardovanih ok.

        }

        private async void StartGame()
        {
            await curGame.Update();
        }

        private async void btnConnectServer_Click(object sender, EventArgs e)
        {
            using (ConnectDialog dg = new ConnectDialog())
            {
                if (dg.ShowDialog() == DialogResult.OK)
                {
                    await client.ConnectAsync(dg.ip, dg.port);
                }
                else
                {
                    throw new Exception("ijao");
                }
            }

        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int[] widths = mainGrid.GetColumnWidths();
            int[] heights = mainGrid.GetRowHeights();
            int gameWidth = widths[0];
            int gameHeight = heights[0];

            
        }

        public Point[] InscribeHexagonDEPRECATED(Point topLeft, int size, bool RT)
        {
            Point[] points = new Point[6];
            Point topLeftT = new Point();
            topLeftT.X = topLeft.X;
            topLeftT.Y = topLeft.Y;

            if(RT) (topLeftT.X, topLeftT.Y) = (topLeftT.X, topLeftT.Y);

            points[0].X = topLeftT.X;
            points[0].Y = topLeftT.Y + size / 2;

            points[3].X = topLeftT.X + size;
            points[3].Y = topLeft.Y + size / 2;

            int length = (int)(size / Math.Sqrt(3f));

            points[1].X = topLeftT.X + (size - length) / 2;
            points[1].Y = topLeftT.Y + size;

            points[2].X = topLeftT.X + (size - length) / 2 + length;
            points[2].Y = topLeftT.Y + size;

            points[4].X = topLeftT.X + (size - length) / 2 + length;
            points[4].Y = topLeftT.Y;

            points[5].X = topLeftT.X + (size - length) / 2;
            points[5].Y = topLeftT.Y;

            if (RT)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    (points[i].X, points[i].Y) = (points[i].Y, points[i].X);
                }
            }
            return points;
        }

        Point[] GetHexagonPoints(Point topLeft, float sideLength, bool pointy)
        {
            Point[] points = new Point[6];

            double r = sideLength / Math.Sqrt(3);  
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

        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            float Size = gamePanel.Height * gameScale;
            float topMargin = gamePanel.Height * gameTopMargin;
            float leftMargin = gamePanel.Width * gameLeftMargin;

            

            
            SolidBrush p = new SolidBrush(Color.CadetBlue);
            g.FillPolygon(p, GetHexagonPoints(new Point((int)(gamePanel.Left + leftMargin), (int)(gamePanel.Top + topMargin)), Size, false));

            float cellSize = Size / 5.5f;
            float sLeftOffset = gamePanel.Left + leftMargin + Size / 5f;
            float sTopOffset = gamePanel.Top + topMargin + Size / 20f;



            int[] rowOffsets = { 3, 7, 12, 16, 19};
            
            int currentRow = 0;
            for(int i = 0; i < 19; i++)
            {
                if (i == rowOffsets[currentRow]) {
                    Debug.WriteLine("secerlema " + currentRow);
                    if (currentRow < 2) sLeftOffset -= cellSize / 2f;
                    else sLeftOffset += cellSize / 2f;
                    currentRow++;
                }
                int currentColumn = i;
                if (currentRow != 0) currentColumn -= rowOffsets[currentRow - 1];
                g.FillPolygon(colors[tempBoard.board[i]], GetHexagonPoints(new Point((int)(sLeftOffset + cellSize * currentColumn), (int)(sTopOffset + cellSize * currentRow)), cellSize, true));
                g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)(sLeftOffset + cellSize * currentColumn), (int)(sTopOffset + cellSize * currentRow), (int)cellSize, (int)cellSize));
            }

        }
    }
}

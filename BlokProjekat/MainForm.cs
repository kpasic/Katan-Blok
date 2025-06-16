using System.Net.Security;
using Catan;
namespace ClientApp
{
    public partial class MainForm : Form
    {

        private Game curGame;
        NetworkClient client;
        public MainForm()
        {
            InitializeComponent();
            HumanPlayer test = new HumanPlayer("test", 0);
            test.OnMoveRequested += OnMoveRequested;
            test.OnDiscardRequested += OnDiscardRequested;
            

        }

        private void OnMoveRequested(HumanPlayer player)
        {
            //curGame.board.GetLegalMoves();
            Place place = new Place();

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
    }
}

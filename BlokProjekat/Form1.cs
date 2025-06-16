using System.Net.Security;

namespace BlokProjekat
{
    public partial class Form1 : Form
    {

        private Game curGame;
        public Form1()
        {
            InitializeComponent();
            HumanPlayer test = new HumanPlayer("test");
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

        private void btnConnectServer_Click(object sender, EventArgs e)
        {

        }
    }
}

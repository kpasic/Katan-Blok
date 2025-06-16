namespace BlokProjekat
{
    public partial class Form1 : Form
    {

        private Game curGame;
        public List<Player> PlayerList;
        public Form1()
        {
            InitializeComponent();
            HumanPlayer test = new HumanPlayer("test");
            test.OnMoveRequested += OnMoveRequested;
            PlayerList = new List<Player> {test };
            curGame = new Game(PlayerList);

        }

        private void OnMoveRequested(HumanPlayer player)
        {
            //curGame.board.GetLegalMoves();
            Place place = new Place();

            player.SubmitMove(place);
            //ui sranja.
        }

        private async void StartGame()
        {
            await curGame.Update();
        }
    }
}

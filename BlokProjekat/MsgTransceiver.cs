using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Catan;
using CNetworking;
namespace ClientApp
{
    public class MsgTransceiver : IMsgTransceiver
    {
        public Board board { get; private set; }
        private HumanPlayer player;
        private Control control;
        private Dictionary<int, IPlayer> playerIds;
        //imaginarni chat receiver...
        public MsgTransceiver(Board board, HumanPlayer player, Control Control)
        {
            this.board = board;
            this.player = player;
            control = Control;
        }

        public async Task<CMessage> Proccess(CMessage msg)
        {
            CMessage response = new CMessage("null", null);
            Debug.WriteLine($"STIGAO MY MSG TYPE {msg.Type}");
            if (msg == null) throw new Exception("zasto null :(");
            control.Invalidate();
            switch (msg.Type)
            {
                case "Play":
                    player.myTurn = true;
                    Move reqMove = await player.GetMove();
                    return new CMessage("Move", reqMove);
                case "Move":
                    (Move move, int id) = ((Move, int))msg.Payload;
                    HumanPlayer fakePlayer = new HumanPlayer("ne znam", id);
                    move.Execute(board, fakePlayer);
                    response.Type = "Ok";
                    response.Payload = null;
                    return response;
                case "BoardState":
                    Board newBoard = (Board)msg.Payload;
                    board.Clone(newBoard);
                    response = new CMessage("Begin", null);
                    return response;
                case "Chat":
                    string message = (string)msg.Payload;
                    //imaginarni chat
                    break;
                case "Wait":
                    player.myTurn = false;
                    response.Type = "Ok";
                    response.Payload = null;
                    return response; 
                default:
                    throw new Exception($"Unrecognized message type: {msg.Type}");
            }
            throw new Exception("???");
        }

        

    }
}

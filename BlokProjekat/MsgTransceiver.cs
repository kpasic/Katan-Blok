using System;
using System.Collections.Generic;
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
        private Board board;
        private HumanPlayer player;
        //imaginarni chat receiver...
        public MsgTransceiver(Board board, HumanPlayer player)
        {
            this.board = board;
            this.player = player;
        }

        public async Task<CMessage> Proccess(CMessage msg)
        {
            if (msg == null) throw new Exception("zasto null :(");
            switch (msg.Type)
            {
                case "Play":
                    player.myTurn = true;
                    Move reqMove = await player.GetMove();
                    return new CMessage("Move", reqMove);
                case "Move":
                    (Move move, IPlayer movingPlayer) = ((Move, IPlayer))msg.Payload;
                    move.Execute(board, movingPlayer);
                    break;
                case "BoardState":
                    Board newBoard = (Board)msg.Payload;
                    board.Clone(newBoard);
                    CMessage response = new CMessage("Ok", null);
                    return response;
               
                case "Chat":
                    string message = (string)msg.Payload;
                    //imaginarni chat
                    break;
                default:
                    throw new Exception($"Unrecognized message type: {msg.Type}");
            }
            throw new Exception("???");
        }

        

    }
}

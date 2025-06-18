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
                case "GetMove":
                    Move move = await player.GetMove();
                    return new CMessage("Move", move);
                case "BoardState":
                    Board newBoard = msg.Payload as Board;

                case default:
                    throw new Exception($"Unrecognized message type: {msg.Type}");
            }
        }

    }
}

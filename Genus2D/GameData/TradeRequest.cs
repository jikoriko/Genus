using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class TradeRequest
    {

        public class TradeOffer
        {
            public int PlayerID;
            public int Gold;
            public List<Tuple<int, int>> Items;
            public bool Accepted;

            public TradeOffer(int playerID)
            {
                PlayerID = playerID;
                Gold = 0;
                Items = new List<Tuple<int, int>>();
                Accepted = false;
            }

        }
        

        public TradeRequest(int player1, int player2)
        {
            
        }

    }
}

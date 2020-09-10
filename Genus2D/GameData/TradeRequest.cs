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
            public static int MaxItems = 25;

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

            public int AddItem(int itemID, int count)
            {
                if (count < 1 || itemID < 0) return 0;

                ItemData data = ItemData.GetItemData(itemID);
                if (data == null) return 0;
                int max = data.GetMaxStack();
                int added = 0;
                int amountToAdd = count;

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Item1 == itemID)
                    {
                        if (Items[i].Item2 < max)
                        {
                            int amountCanAdd = max - Items[i].Item2;
                            if (amountToAdd <= amountCanAdd)
                            {
                                Items[i] = new Tuple<int, int>(itemID, Items[i].Item2 + amountToAdd);
                                added += amountToAdd;
                                amountToAdd = 0;
                            }
                            else
                            {
                                Items[i] = new Tuple<int, int>(itemID, Items[i].Item2 + amountCanAdd);
                                added += amountCanAdd;
                                amountToAdd -= amountCanAdd;
                            }
                        }
                    }

                    if (amountToAdd < 1)
                        break;
                }

                while (amountToAdd > 0)
                {
                    if (Items.Count < MaxItems)
                    {
                        if (amountToAdd <= max)
                        {
                            Items.Add(new Tuple<int, int>(itemID, amountToAdd));
                            added += amountToAdd;
                            break;
                        }
                        else
                        {
                            Items.Add(new Tuple<int, int>(itemID, max));
                            added += max;
                            amountToAdd -= max;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return added;
            }

            public bool RemoveItem(int index)
            {
                if (index >= 0 && index < Items.Count)
                {
                    Items.RemoveAt(index);
                    return true;
                }

                return false;
            }

        }

        public TradeOffer TradeOffer1;
        public TradeOffer TradeOffer2;

        public TradeRequest(int player1, int player2)
        {
            TradeOffer1 = new TradeOffer(player1);
            TradeOffer2 = new TradeOffer(player2);
        }

        public bool Accepted()
        {
            return TradeOffer1.Accepted && TradeOffer2.Accepted;
        }

    }
}

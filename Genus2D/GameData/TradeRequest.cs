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
            public static int MaxItems = 18;

            public int PlayerID;
            public int Gold;
            private List<Tuple<int, int>> _items;
            public bool Accepted;
            public int FreeSlots;

            public TradeOffer(int playerID)
            {
                PlayerID = playerID;
                Gold = 0;
                _items = new List<Tuple<int, int>>();
                Accepted = false;
                FreeSlots = 0;
            }

            public int NumItems()
            {
                return _items.Count;
            }

            public Tuple<int, int> GetItem(int index)
            {
                if (index >= 0 && index < _items.Count)
                {
                    return _items[index];
                }
                return null;
            }

            public int AddItem(int itemID, int count)
            {
                if (count < 1 || itemID < 0) return 0;

                ItemData data = ItemData.GetItemData(itemID);
                if (data == null) return 0;
                if (data.GetItemType() == ItemData.ItemType.Quest) return 0;

                int max = data.GetMaxStack();
                int added = 0;
                int amountToAdd = count;

                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Item1 == itemID)
                    {
                        if (_items[i].Item2 < max)
                        {
                            int amountCanAdd = max - _items[i].Item2;
                            if (amountToAdd <= amountCanAdd)
                            {
                                _items[i] = new Tuple<int, int>(itemID, _items[i].Item2 + amountToAdd);
                                added += amountToAdd;
                                amountToAdd = 0;
                            }
                            else
                            {
                                _items[i] = new Tuple<int, int>(itemID, _items[i].Item2 + amountCanAdd);
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
                    if (_items.Count < MaxItems)
                    {
                        if (amountToAdd <= max)
                        {
                            _items.Add(new Tuple<int, int>(itemID, amountToAdd));
                            added += amountToAdd;
                            break;
                        }
                        else
                        {
                            _items.Add(new Tuple<int, int>(itemID, max));
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

            public int RemoveItem(int index, int count)
            {
                int removed = 0;

                if (index >= 0 && index < _items.Count)
                {
                    if (count > 0)
                    {
                        if (count < _items[index].Item2)
                        {
                            removed = count;
                            _items[index] = new Tuple<int, int>(_items[index].Item1, _items[index].Item2 - count);
                        }
                        else
                        {
                            removed = _items[index].Item2;
                            _items.RemoveAt(index);
                        }
                    }
                }

                return removed;
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

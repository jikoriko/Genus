using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class PlayerData
    {

        [Serializable]
        public struct CombatStats
        {
            public int Strength;
            public int Defence;
            public int Accuracy;
            public int Evasion;
        }

        public int Level;
        public int Health;
        public int Mana;
        public int Stamina;

        private List<Tuple<int, int>> _inventory;
        private int[] _equipment;

        public PlayerData()
        {
            Level = 1;
            Health = 100;
            Mana = 100;
            Stamina = 100;

            _inventory = new List<Tuple<int, int>>();
            _equipment = new int[(int)EquipmentSlot.Hands + 1];
        }

        public int AddInventoryItem(int id, int count)
        {
            if (count < 1 || id < 0) return 0;

            ItemData data = ItemData.GetItemData(id);
            if (data == null) return 0;
            int max = data.MaxStack;
            int added = 0;

            if (max > 1)
            {
                int amountToAdd = count;

                for (int i = 0; i < _inventory.Count; i++)
                {
                    if (_inventory[i].Item1 == id)
                    {
                        if (_inventory[i].Item2 < max)
                        {
                            int amountCanAdd = max - _inventory[i].Item2;
                            if (amountToAdd <= amountCanAdd)
                            {
                                _inventory[i] = new Tuple<int, int>(id, _inventory[i].Item2 + amountToAdd);
                                added += amountToAdd;
                                amountToAdd = 0;
                            }
                            else
                            {
                                _inventory[i] = new Tuple<int, int>(id, _inventory[i].Item2 + amountCanAdd);
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
                    if (_inventory.Count < 30) //hardcoded max inv count
                    {
                        if (amountToAdd <= max)
                        {
                            _inventory.Add(new Tuple<int, int>(id, amountToAdd));
                            added += amountToAdd;
                            break;
                        }
                        else
                        {
                            _inventory.Add(new Tuple<int, int>(id, max));
                            added += max;
                            amountToAdd -= max;
                        }
                    }
                    else
                    {
                        break;
                    }
                }


            }

            return added;
        }

        public void EquipItem(EquipmentSlot slot, int id)
        {
            _equipment[(int)slot] = id + 1;
        }

        public int GetEquipedItem(EquipmentSlot slot)
        {
            return _equipment[(int)slot] - 1;
        }

    }
}

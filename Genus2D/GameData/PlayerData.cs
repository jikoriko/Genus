using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class PlayerData
    {

        public int Level;
        public int MaxHealth, Health;
        public int MaxStamina, Stamina;

        private static int InventorySize = 30; // hardcoded inventory size

        private List<Tuple<int, int>> _inventory; //item id, item count
        private int[] _equipment;

        public PlayerData()
        {
            Level = 1;
            MaxHealth = Health = 100;
            MaxStamina = Stamina = 100;

            _inventory = new List<Tuple<int, int>>();
            _equipment = new int[(int)EquipmentSlot.Ring + 1];
        }

        public int AddInventoryItem(int itemID, int count)
        {
            if (count < 1 || itemID < 0) return 0;

            ItemData data = ItemData.GetItemData(itemID);
            if (data == null) return 0;
            int max = data.GetMaxStack();
            int added = 0;

            if (max > 1)
            {
                int amountToAdd = count;

                for (int i = 0; i < _inventory.Count; i++)
                {
                    if (_inventory[i].Item1 == itemID)
                    {
                        if (_inventory[i].Item2 < max)
                        {
                            int amountCanAdd = max - _inventory[i].Item2;
                            if (amountToAdd <= amountCanAdd)
                            {
                                _inventory[i] = new Tuple<int, int>(itemID, _inventory[i].Item2 + amountToAdd);
                                added += amountToAdd;
                                amountToAdd = 0;
                            }
                            else
                            {
                                _inventory[i] = new Tuple<int, int>(itemID, _inventory[i].Item2 + amountCanAdd);
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
                    if (_inventory.Count < InventorySize)
                    {
                        if (amountToAdd <= max)
                        {
                            _inventory.Add(new Tuple<int, int>(itemID, amountToAdd));
                            added += amountToAdd;
                            break;
                        }
                        else
                        {
                            _inventory.Add(new Tuple<int, int>(itemID, max));
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

        public bool EquipItem(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < _inventory.Count)
            {
                ItemData data = ItemData.GetItemData(_inventory[itemIndex].Item1);
                if (data.Equipable())
                {
                    EquipmentSlot slot = (EquipmentSlot)data.GetItemStat("EquipmentSlot").Item2;
                    if (GetEquipedItemID(slot) == -1)
                    {
                        EquipItem(slot, _inventory[itemIndex].Item1);
                        _inventory.RemoveAt(itemIndex);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UnequipItem(EquipmentSlot slot)
        {
            if (GetEquipedItemID(slot) != -1)
            {
                if (AddInventoryItem(GetEquipedItemID(slot), 1) == 0)
                {
                    EquipItem(slot, -1);
                }
            }
            return false;
        }

        private void EquipItem(EquipmentSlot slot, int itemID)
        {
            _equipment[(int)slot] = itemID + 1;
        }

        public int GetEquipedItemID(EquipmentSlot slot)
        {
            return _equipment[(int)slot] - 1;
        }

        public byte[] GetBytes(bool isLocalPlayer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(Level), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MaxHealth), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Health), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MaxStamina), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Stamina), 0, sizeof(int));

                if (isLocalPlayer)
                {
                    stream.Write(BitConverter.GetBytes(_inventory.Count), 0, sizeof(int));
                    for (int i = 0; i < _inventory.Count; i++)
                    {
                        stream.Write(BitConverter.GetBytes(_inventory[i].Item1), 0, sizeof(int));
                        stream.Write(BitConverter.GetBytes(_inventory[i].Item2), 0, sizeof(int));
                    }
                }
                else
                {
                    stream.Write(BitConverter.GetBytes(0), 0, sizeof(int));
                }

                stream.Write(BitConverter.GetBytes(_equipment.Length), 0, sizeof(int));
                for (int i = 0; i < _equipment.Length; i++)
                {
                    stream.Write(BitConverter.GetBytes(_equipment[i]), 0, sizeof(int));
                }

                return stream.ToArray();
            }
        }

        public static PlayerData FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                PlayerData data = new PlayerData();

                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int level = BitConverter.ToInt32(tempBytes, 0);
                data.Level = level;

                stream.Read(tempBytes, 0, sizeof(int));
                int maxHP = BitConverter.ToInt32(tempBytes, 0);
                data.MaxHealth = maxHP;

                stream.Read(tempBytes, 0, sizeof(int));
                int hp = BitConverter.ToInt32(tempBytes, 0);
                data.Health = hp;

                stream.Read(tempBytes, 0, sizeof(int));
                int maxStamina = BitConverter.ToInt32(tempBytes, 0);
                data.MaxStamina = maxStamina;

                stream.Read(tempBytes, 0, sizeof(int));
                int stamina = BitConverter.ToInt32(tempBytes, 0);
                data.Stamina = stamina;

                stream.Read(tempBytes, 0, sizeof(int));
                int iventorySize = BitConverter.ToInt32(tempBytes, 0);

                if (InventorySize > 0)
                {
                    List<Tuple<int, int>> inventory = new List<Tuple<int, int>>();
                    for (int i = 0; i < InventorySize; i++)
                    {
                        stream.Read(tempBytes, 0, sizeof(int));
                        int itemId = BitConverter.ToInt32(tempBytes, 0);

                        stream.Read(tempBytes, 0, sizeof(int));
                        int stack = BitConverter.ToInt32(tempBytes, 0);
                        inventory.Add(new Tuple<int, int>(itemId, stack));
                    }
                    data._inventory = inventory;
                }

                stream.Read(tempBytes, 0, sizeof(int));
                int equipmentSize = BitConverter.ToInt32(tempBytes, 0);
                int[] equipment = new int[equipmentSize];
                for (int i = 0; i < equipmentSize; i++)
                {
                    stream.Read(tempBytes, 0, sizeof(int));
                    int equipmentID = BitConverter.ToInt32(tempBytes, 0);
                    equipment[i] = equipmentID;
                }
                data._equipment = equipment;

                return data;
            }
        }

    }
}

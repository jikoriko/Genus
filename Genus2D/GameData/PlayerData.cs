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
        public int HP, MP, Stamina;
        public int InvestmentPoints;

        private int _classID;
        public CombatStats BaseStats { get; private set; }
        public CombatStats InvestedStats { get; private set; }

        private static int InventorySize = 30; // hardcoded inventory size
        private List<Tuple<int, int>> _inventory; //item id, item count
        private int[] _equipment;

        public PlayerData()
        {
            Level = 0;
            HP = 0;
            MP = 0;
            Stamina = 0;
            SetClassID(-1);
            InvestedStats = new CombatStats();
            _inventory = new List<Tuple<int, int>>();
            _equipment = new int[(int)EquipmentSlot.Ring + 1];
        }

        public void SetClassID(int id)
        {
            _classID = id;
            if (id != -1)
            {
                Genus2D.GameData.ClassData data = Genus2D.GameData.ClassData.GetClass(id);
                BaseStats = data.BaseStats;
            }
            else
            {
                BaseStats = new CombatStats();
                BaseStats.Vitality = 1;
                BaseStats.Inteligence = 1;
                BaseStats.Strength = 1;
                BaseStats.Agility = 1;
                BaseStats.MeleeDefence = 1;
                BaseStats.RangeDefence = 1;
                BaseStats.MagicDefence = 1;
            }
        }

        public int GetClassID()
        {
            return _classID;
        }

        public CombatStats GetCombinedCombatStats()
        {
            CombatStats combined = new CombatStats();

            combined.Vitality = BaseStats.Vitality + InvestedStats.Vitality;
            combined.Inteligence = BaseStats.Inteligence + InvestedStats.Inteligence;
            combined.Strength = BaseStats.Strength + InvestedStats.Strength;
            combined.Agility = BaseStats.Agility + InvestedStats.Agility;
            combined.MeleeDefence = BaseStats.MeleeDefence + InvestedStats.MeleeDefence;
            combined.RangeDefence = BaseStats.RangeDefence + InvestedStats.RangeDefence;
            combined.MagicDefence = BaseStats.MagicDefence + InvestedStats.MagicDefence;

            for (int i = 0; i < _equipment.Length; i++)
            {
                if (_equipment[i] != 0)
                {
                    ItemData data = ItemData.GetItemData(_equipment[i] - 1);
                    combined.Vitality += (int)data.GetItemStat("VitalityBonus");
                    combined.Inteligence += (int)data.GetItemStat("InteligenceBonus");
                    combined.Strength += (int)data.GetItemStat("StrengthBonus");
                    combined.Agility += (int)data.GetItemStat("AgilityBonus");
                    combined.MeleeDefence += (int)data.GetItemStat("MeleeDefenceBonus");
                    combined.RangeDefence += (int)data.GetItemStat("RangeDefenceBonus");
                    combined.MagicDefence += (int)data.GetItemStat("MagicDefenceBonus");
                }
            }

            return combined;
        }

        public int GetMaxHP()
        {
            CombatStats combined = GetCombinedCombatStats();
            return combined.Vitality * 10;
        }

        public int GetMapMP()
        {
            CombatStats combined = GetCombinedCombatStats();
            return combined.Inteligence * 10;
        }

        public int GetMaxStamina()
        {
            CombatStats combined = GetCombinedCombatStats();
            return combined.Agility * 10;
        }

        public int AddInventoryItem(int itemID, int count)
        {
            if (count < 1 || itemID < 0) return 0;

            ItemData data = ItemData.GetItemData(itemID);
            if (data == null) return 0;
            int max = data.GetMaxStack();
            int added = 0;
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

            return added;
        }

        public void RemoveInventoryItem(int itemID, int count)
        {
            if (count < 1 || itemID < 0) return;
            ItemData data = ItemData.GetItemData(itemID);
            if (data == null) return;

            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].Item1 == itemID)
                {
                    if (_inventory[i].Item2 >= count)
                    {
                        int remainder = _inventory[i].Item2 - count;
                        if (remainder == 0)
                            _inventory.RemoveAt(i);
                        else
                            _inventory[i] = new Tuple<int, int>(itemID, remainder);
                        return;
                    }
                    else
                    {
                        count -= _inventory[i].Item2;
                        _inventory.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public Tuple<int, int> GetInventoryItem(int index)
        {
            if (index >= 0 && index < _inventory.Count)
            {
                return _inventory[index];
            }
            return null;
        }

        public bool EquipItem(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < _inventory.Count)
            {
                ItemData data = ItemData.GetItemData(_inventory[itemIndex].Item1);
                if (data.Equipable())
                {
                    EquipmentSlot slot = (EquipmentSlot)data.GetItemStat("EquipmentSlot");
                    int prev = GetEquipedItemID(slot);
                    EquipItem(slot, _inventory[itemIndex].Item1);
                    if (prev != -1)
                    {
                        _inventory[itemIndex] = new Tuple<int, int>(prev, 1);
                    }
                    else
                    {
                        _inventory.RemoveAt(itemIndex);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool UnequipItem(EquipmentSlot slot)
        {
            if (GetEquipedItemID(slot) != -1)
            {
                if (AddInventoryItem(GetEquipedItemID(slot), 1) == 1)
                {
                    EquipItem(slot, -1);
                    return true;
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

        public bool ItemEquipped(int itemID)
        {
            for (int i = 0; i < _equipment.Length; i++)
            {
                if (_equipment[i] - 1 == itemID)
                    return true;
            }
            return false;
        }

        public bool ItemInInventory(int itemID, int amount)
        {
            int count = 0;
            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i].Item1 == itemID)
                {
                    count += _inventory[i].Item2;
                    if (count >= amount)
                        return true;
                }
            }
            return false;
        }

        public byte[] GetBytes(bool isLocalPlayer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(Level), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(HP), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MP), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Stamina), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_classID), 0, sizeof(int));

                stream.Write(BitConverter.GetBytes(BaseStats.Vitality), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.Inteligence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.Strength), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.Agility), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.MeleeDefence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.RangeDefence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(BaseStats.MagicDefence), 0, sizeof(int));

                stream.Write(BitConverter.GetBytes(InvestedStats.Vitality), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.Inteligence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.Strength), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.Agility), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.MeleeDefence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.RangeDefence), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(InvestedStats.MagicDefence), 0, sizeof(int));

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
                int hp = BitConverter.ToInt32(tempBytes, 0);
                data.HP = hp;

                stream.Read(tempBytes, 0, sizeof(int));
                int mp = BitConverter.ToInt32(tempBytes, 0);
                data.MP = mp;

                stream.Read(tempBytes, 0, sizeof(int));
                int stamina = BitConverter.ToInt32(tempBytes, 0);
                data.Stamina = stamina;

                stream.Read(tempBytes, 0, sizeof(int));
                int classID = BitConverter.ToInt32(tempBytes, 0);
                data.SetClassID(classID);

                stream.Read(tempBytes, 0, sizeof(int));
                int vitality = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.Vitality = vitality;

                stream.Read(tempBytes, 0, sizeof(int));
                int inteligence = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.Inteligence = inteligence;

                stream.Read(tempBytes, 0, sizeof(int));
                int strength = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.Strength = strength;

                stream.Read(tempBytes, 0, sizeof(int));
                int agility = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.Agility = agility;

                stream.Read(tempBytes, 0, sizeof(int));
                int meleeDefence = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.MeleeDefence = meleeDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                int rangeDefence = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.RangeDefence = rangeDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                int magicDefence = BitConverter.ToInt32(tempBytes, 0);
                data.BaseStats.MagicDefence = magicDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                vitality = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.Vitality = vitality;

                stream.Read(tempBytes, 0, sizeof(int));
                inteligence = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.Inteligence = inteligence;

                stream.Read(tempBytes, 0, sizeof(int));
                strength = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.Strength = strength;

                stream.Read(tempBytes, 0, sizeof(int));
                agility = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.Agility = agility;

                stream.Read(tempBytes, 0, sizeof(int));
                meleeDefence = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.MeleeDefence = meleeDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                rangeDefence = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.RangeDefence = rangeDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                magicDefence = BitConverter.ToInt32(tempBytes, 0);
                data.InvestedStats.MagicDefence = magicDefence;

                stream.Read(tempBytes, 0, sizeof(int));
                int inventorySize = BitConverter.ToInt32(tempBytes, 0);

                if (inventorySize > 0)
                {
                    List<Tuple<int, int>> inventory = new List<Tuple<int, int>>();
                    for (int i = 0; i < inventorySize; i++)
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

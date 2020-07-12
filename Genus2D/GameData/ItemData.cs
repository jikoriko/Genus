using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class ItemData
    {

        [Serializable]
        public enum ItemType
        {
            Tool,
            Consumable,
            Material,
            Equipment,
            Ammo,
            Quest
        }

        public string Name;
        public string IconSheetImage;
        public int IconID;
        private ItemType _itemType;
        private int _maxStack;
        private Dictionary<string, object> _itemStats;

        public ItemData(string name)
        {
            Name = name;
            IconSheetImage = "";
            IconID = 0;
            _itemType = ItemType.Tool;
            _maxStack = 1;
            _itemStats = new Dictionary<string, object>();
            PopulateItemStats();
        }

        public ItemType GetItemType()
        {
            return _itemType;
        }

        public void SetItemType(ItemType type)
        {
            if (_itemType != type)
            {
                _itemType = type;
                if (_itemType != ItemType.Consumable || _itemType != ItemType.Material || _itemType != ItemType.Ammo)
                    _maxStack = 1;

                PopulateItemStats();
            }

        }

        private void PopulateItemStats()
        {
            _itemStats.Clear();
            switch(_itemType)
            {
                case ItemType.Tool:
                    _itemStats.Add("ToolType", ToolType.Axe);
                    break;
                case ItemType.Consumable:
                    _itemStats.Add("HpHeal", 0);
                    _itemStats.Add("MpHeal", 0);
                    _itemStats.Add("StaminaHeal", 0);
                    break;
                case ItemType.Material:
                    _itemStats.Add("MaterialID", 0);
                    break;
                case ItemType.Equipment:
                    _itemStats.Add("EquipmentSlot", EquipmentSlot.Weapon);
                    _itemStats.Add("AttackStyle", AttackStyle.None);
                    _itemStats.Add("VitalityBonus", 0);
                    _itemStats.Add("InteligenceBonus", 0);
                    _itemStats.Add("StrengthBonus", 0);
                    _itemStats.Add("AgilityBonus", 0);
                    _itemStats.Add("MeleeDefenceBonus", 0);
                    _itemStats.Add("RangeDefenceBonus", 0);
                    _itemStats.Add("MagicDefenceBonus", 0);
                    _itemStats.Add("ProjectileID", -1);
                    _itemStats.Add("MP", 0);
                    break;
                case ItemType.Ammo:
                    _itemStats.Add("StrengthBonus", 0);
                    _itemStats.Add("ProjectileID", -1);
                    break;
            }
        }

        public bool Equipable()
        {
            return _itemType == ItemType.Equipment;
        }

        public int GetMaxStack()
        {
            return _maxStack;
        }

        public void SetMaxStack(int max)
        {
            if (_itemType == ItemType.Consumable || _itemType == ItemType.Material || _itemType == ItemType.Ammo)
                _maxStack = max;
        }

        public object GetItemStat(string name)
        {
            if (_itemStats.ContainsKey(name))
                return _itemStats[name];
            return null;
        }

        public void SetItemStat(string name, object value)
        {
            if (_itemStats.ContainsKey(name))
                _itemStats[name] = value;
        }



        //static

        private static List<ItemData> _itemData = LoadItemData();
        private static List<ItemData> LoadItemData()
        {
            List<ItemData> data = null;

            if (File.Exists("Data/ItemData.data"))
            {
                FileStream stream = File.Open("Data/ItemData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<ItemData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<ItemData>();
            }

            return data;
        }

        public static void ReloadData()
        {
            _itemData = LoadItemData();
        }

        public static void SaveItemData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/ItemData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _itemData);
            stream.Close();
        }

        public static void AddItemData(ItemData data)
        {
            _itemData.Add(data);
        }

        public static void RemoveItemData(int index)
        {
            if (index >= 0 && index < _itemData.Count)
            {
                _itemData.RemoveAt(index);
            }
        }

        public static ItemData GetItemData(int index)
        {
            if (index >= 0 && index < _itemData.Count)
            {
                return _itemData[index];
            }
            return null;
        }

        public static int GetItemDataCount()
        {
            return _itemData.Count;
        }

        public static List<string> GetItemNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _itemData.Count; i++)
            {
                names.Add(_itemData[i].Name);
            }

            return names;
        }

    }
}

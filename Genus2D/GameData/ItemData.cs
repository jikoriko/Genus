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
            Equipment
        }

        public string Name;
        public string IconSheetImage;
        public int IconID;
        private ItemType _itemType;
        private int _maxStack;
        private Dictionary<string, Tuple<ItemStatType, object>> _itemStats;

        public ItemData(string name)
        {
            Name = name;
            IconSheetImage = "";
            IconID = 0;
            _itemType = ItemType.Tool;
            _maxStack = 1;
            _itemStats = new Dictionary<string, Tuple<ItemStatType, object>>();
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
                if (_itemType != ItemType.Consumable || _itemType != ItemType.Material)
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
                    _itemStats.Add("ToolType", Tuple.Create(ItemStatType.ToolType, (object)ToolType.Axe));
                    break;
                case ItemType.Consumable:
                    _itemStats.Add("HP", Tuple.Create(ItemStatType.Integer, (object)0));
                    _itemStats.Add("Stamina", Tuple.Create(ItemStatType.Integer, (object)0));
                    break;
                case ItemType.Material:
                    _itemStats.Add("MaterialID", Tuple.Create(ItemStatType.Integer, (object)0));
                    break;
                case ItemType.Equipment:
                    _itemStats.Add("EquipmentSlot", Tuple.Create(ItemStatType.Integer, (object)EquipmentSlot.Weapon));
                    _itemStats.Add("AttackStrength", Tuple.Create(ItemStatType.Integer, (object)0));
                    _itemStats.Add("Defence", Tuple.Create(ItemStatType.Integer, (object)0));
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
            if (_itemType == ItemType.Consumable || _itemType == ItemType.Material)
                _maxStack = max;
        }

        public Tuple<ItemStatType, object> GetItemStat(string name)
        {
            if (_itemStats.ContainsKey(name))
                return _itemStats[name];
            return null;
        }

        public void SetItemStat(string name, object value)
        {
            if (_itemStats.ContainsKey(name))
                _itemStats[name] = Tuple.Create(_itemStats[name].Item1, value);
        }



        //static

        private static List<ItemData> _itemData = LoadItemData();

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

    }
}

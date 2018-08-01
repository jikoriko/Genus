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
            Weapon,
            Armour,
            Accessory,
            Quest
        }

        private ItemType _itemType;
        public string Name;
        public int MaxStack;
        private Dictionary<string, int> _itemStats;

        public ItemData(ItemType type)
        {
            _itemType = type;
            Name = "";
            MaxStack = 1;
            _itemStats = new Dictionary<string, int>();
        }

        public int GetItemStat(string name)
        {
            return _itemStats[name];
        }

        public void SetItemStat(string name, int value)
        {
            _itemStats[name] = value;
        }

        public void RemoveItemStat(int index)
        {
            if (index < _itemStats.Count && index >= 0)
            {
                //_itemStats.RemoveAt(index);
            }
        }

        public ItemType GetItemType()
        {
            return _itemType;
        }

        public void SetItemType(ItemType type)
        {
            _itemType = type;
        }

        public bool Equipable()
        {
            return _itemType == ItemType.Weapon || _itemType == ItemType.Armour || _itemType == ItemType.Accessory;
        }



        //static

        private static List<ItemData> _itemData = LoadItemData();

        public static void AddItemData(ItemData data)
        {
            _itemData.Add(data);
        }

        public static void RemoveItemData(int index)
        {
            if (index < _itemData.Count && _itemData.Count > 0)
            {
                _itemData.RemoveAt(index);
            }
        }

        public static ItemData GetItemData(int index)
        {
            if (index < _itemData.Count && _itemData.Count > 0)
            {
                return _itemData[index];
            }
            return null;
        }

        public static int GetItemDataCount()
        {
            return _itemData.Count;
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

        private static void SaveItemData()
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

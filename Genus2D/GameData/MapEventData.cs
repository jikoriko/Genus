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
    public class MapEventData
    {
        // Static MapEventData
        private static List<MapEventData> _mapEventsData = LoadMapEventsData();
        private static List<MapEventData> LoadMapEventsData()
        {
            List<MapEventData> mapEventsData;

            if (File.Exists("Data/MapEventsData.data"))
            {
                FileStream stream = File.Open("Data/MapEventsData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                mapEventsData = (List<MapEventData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                mapEventsData = new List<MapEventData>();
            }

            return mapEventsData;
        }

        public static void SaveMapEventsData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/MapEventsData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _mapEventsData);
            stream.Close();
        }

        public static int MapEventsDataCount()
        {
            return _mapEventsData.Count;
        }

        public static MapEventData GetMapEventData(int index)
        {
            if (index >= 0 && index < _mapEventsData.Count)
            {
                return _mapEventsData[index];
            }
            return null;
        }

        public static void AddMapEventData(string name)
        {
            _mapEventsData.Add(new MapEventData(name));
            SaveMapEventsData();
        }

        public static void AddMapEventData(MapEventData data)
        {
            if (data != null)
            {
                _mapEventsData.Add(data);
                SaveMapEventsData();
            }
        }

        public static void RemoveMapEventData(int index)
        {
            if (index >= 0 && index < _mapEventsData.Count)
            {
                _mapEventsData.RemoveAt(index);
                SaveMapEventsData();
            }
        }

        public static List<String> GetMapEventsDataNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _mapEventsData.Count; i++)
            {
                names.Add(_mapEventsData[i].Name);
            }

            return names;
        }


        // MapEventData varialbles and functions

        public enum TriggerType
        {
            Action,
            PlayerTouch
        }

        public string Name;
        private TriggerType _triggerType;
        private bool _passable;
        public List<EventCommand> EventCommands { get; private set; }
        private int _spriteID;

        public MapEventData(string name)
        {
            Name = name;
            _triggerType = TriggerType.Action;
            _passable = false;
            EventCommands = new List<EventCommand>();
            _spriteID = -1;
        }

        public void AddEventCommand(EventCommand.CommandType type)
        {
            EventCommands.Add(new EventCommand(type));
        }

        public void RemoveEventCommand(int index)
        {
            if (index >= 0 && index < EventCommands.Count)
                EventCommands.RemoveAt(index);
        }

        public List<string> GetEventCommandStrings()
        {
            List<string> strings = new List<string>();

            for (int i = 0; i < EventCommands.Count; i++)
            {
                strings.Add(EventCommands[i].ToString());
            }

            return strings;
        }

        public TriggerType GetTriggerType()
        {
            return _triggerType;
        }

        public void SetTriggerType(TriggerType triggerType)
        {
            _triggerType = triggerType;
        }

        public bool Passable()
        {
            return _passable;
        }

        public void SetPassable(bool passable)
        {
            _passable = passable;
        }

        public override string ToString()
        {
            return "Event: " + Name;
        }

        public int GetSpriteID()
        {
            return _spriteID;
        }

        public void SetSpriteID(int id)
        {
            _spriteID = id;
        }

    }
}

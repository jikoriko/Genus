using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Genus2D.GameData
{

    [Serializable]
    public class MapEvent
    {
        public int EventID;
        public int MapX;
        public int MapY;

        public MapEvent(int id, int x, int y)
        {
            EventID = id;
            MapX = x;
            MapY = y;
        }

        public static int SizeOfBytes()
        {
            return sizeof(int) * 3;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Console.WriteLine("sending map event: " + MapX + "," + MapY);
                stream.Write(BitConverter.GetBytes(EventID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                return stream.ToArray();
            }
        }

        public static MapEvent FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int eventID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                Console.WriteLine("recived map event: " + mapX + "," + mapY);

                MapEvent mapEvent = new MapEvent(eventID, mapX, mapY);
                return mapEvent;
            }
        }

        public MapEventData GetMapEventData()
        {
            return MapEventData.GetMapEventData(EventID);
        }
    }

    [Serializable]
    public class MessageOption
    {
        public string Option;
        public int OptionEventID;

        public MessageOption()
        {
            Option = "";
            OptionEventID = -1;
        }
    }

    [Serializable]
    public class EventCommand
    {

        public enum CommandType
        {
            EventWaitTimer,
            MapTransfer,
            MovePlayer,
            ShowMessage,
            ShowOptions
        }

        public CommandType Type { get; private set; }
        private Dictionary<string, object> _parameters;

        public EventCommand(CommandType type)
        {
            Type = type;
            _parameters = new Dictionary<string, object>();

            switch (Type)
            {
                case CommandType.EventWaitTimer:
                    _parameters.Add("Time", 0.0f);
                    break;
                case CommandType.MapTransfer:
                    _parameters.Add("MapID", 0);
                    _parameters.Add("MapX", 0);
                    _parameters.Add("MapY", 0);
                    break;
                case CommandType.MovePlayer:
                    _parameters.Add("Direction", Direction.Down);
                    break;
                case CommandType.ShowMessage:
                    _parameters.Add("Message", "");
                    break;
                case CommandType.ShowOptions:
                    _parameters.Add("Message", "");
                    _parameters.Add("Options", new List<MessageOption>());
                    break;
            }

        }

        public int NumParameters()
        {
            return _parameters.Count;
        }

        public string GetParameterName(int index)
        {
            if (index >= 0 && index < _parameters.Count)
                return _parameters.ElementAt(index).Key;
            return null;
        }

        public object GetParameter(int index)
        {
            if (index >= 0 && index < _parameters.Count)
                return _parameters.ElementAt(index).Value;
            return null;
        }

        public object GetParameter(string name)
        {
            if (_parameters.ContainsKey(name))
                return _parameters[name];
            return null;
        }

        public void SetParameter(string name, object value)
        {
            if (_parameters.ContainsKey(name))
                _parameters[name] = value;
        }

        public override string ToString()
        {
            string s = "Command: " + Type.ToString();
            return s;
        }
    }

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
        private int _spriteID = -1; 

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

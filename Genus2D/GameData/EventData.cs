﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class EventData
    {
        // Static MapEventData
        private static List<EventData> _eventsData = LoadEventsData();
        private static List<EventData> LoadEventsData()
        {
            List<EventData> eventsData;

            if (File.Exists("Data/EventsData.data"))
            {
                FileStream stream = File.Open("Data/EventsData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                eventsData = (List<EventData>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                eventsData = new List<EventData>();
            }

            return eventsData;
        }

        public static void ReloadData()
        {
            _eventsData = LoadEventsData();
        }

        public static void SaveEventsData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/EventsData.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _eventsData);
            stream.Close();
        }

        public static int EventsDataCount()
        {
            return _eventsData.Count;
        }

        public static EventData GetEventData(int index)
        {
            if (index >= 0 && index < _eventsData.Count)
            {
                return _eventsData[index];
            }
            return null;
        }

        public static void AddEventData(string name)
        {
            _eventsData.Add(new EventData(name));
        }

        public static void AddEventData(EventData data)
        {
            if (data != null)
            {
                _eventsData.Add(data);
            }
        }

        public static void RemoveEventData(int index)
        {
            if (index >= 0 && index < _eventsData.Count)
            {
                _eventsData.RemoveAt(index);
            }
        }

        public static List<String> GetEventsDataNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _eventsData.Count; i++)
            {
                names.Add(_eventsData[i].Name);
            }

            return names;
        }


        // EventData varialbles and functions

        public string Name;
        public List<EventCommand> EventCommands { get; private set; }

        public EventData(string name)
        {
            Name = name;
            EventCommands = new List<EventCommand>();
        }

        public void AddEventCommand(EventCommand.CommandType type)
        {
            EventCommands.Add(new EventCommand(type));
        }

        public void AddEventCommand(EventCommand command)
        {
            EventCommands.Add(command);
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


        public override string ToString()
        {
            return "Event: " + Name;
        }

    }
}

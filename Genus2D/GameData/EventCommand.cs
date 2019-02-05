using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class EventCommand
    {

        public enum CommandType
        {
            WaitTimer,
            TeleportPlayer,
            MovePlayer,
            ChangePlayerDirection,
            TeleportMapEvent,
            MoveMapEvent,
            ChangeMapEventDirection,
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
                case CommandType.WaitTimer:
                    _parameters.Add("Time", 0.0f);
                    break;
                case CommandType.TeleportPlayer:
                    _parameters.Add("MapID", 0);
                    _parameters.Add("MapX", 0);
                    _parameters.Add("MapY", 0);
                    break;
                case CommandType.MovePlayer:
                    _parameters.Add("Direction", Direction.Down);
                    break;
                case CommandType.ChangePlayerDirection:
                    _parameters.Add("Direction", Direction.Down);
                    break;
                case CommandType.TeleportMapEvent:
                    _parameters.Add("MapID", -1);
                    _parameters.Add("EventID", -1);
                    _parameters.Add("MapX", 0);
                    _parameters.Add("MapY", 0);
                    break;
                case CommandType.MoveMapEvent:
                    _parameters.Add("MapID", -1);
                    _parameters.Add("EventID", -1);
                    _parameters.Add("Direction", Direction.Down);
                    break;
                case CommandType.ChangeMapEventDirection:
                    _parameters.Add("MapID", -1);
                    _parameters.Add("EventID", -1);
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

        public void SetParameter(int index, object value)
        {
            if (index >= 0 && index < _parameters.Count)
            {
                string name = _parameters.ElementAt(index).Key;
                _parameters[name] = value;
            }
        }

        public override string ToString()
        {
            string s = Type.ToString();
            return s;
        }
    }
}

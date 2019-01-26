using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.Networking
{
    public class ClientCommand
    {
        public enum CommandType
        {
            ShowMessage,
            CloseMessage,
            ShowOptions,
            UpdateOptions,
            UpdateMapEvent,
            ChangeMapEventDirection
        }

        private CommandType _commandType;
        private Dictionary<string, string> _parameters;

        public ClientCommand(CommandType type)
        {
            _commandType = type;
            _parameters = new Dictionary<string, string>();

            switch (type)
            {
                case CommandType.ShowMessage:
                    _parameters.Add("Message", "");
                    break;
                case CommandType.CloseMessage:

                    break;
                case CommandType.ShowOptions:
                    _parameters.Add("Message", "");
                    _parameters.Add("Options", "");
                    _parameters.Add("SelectedOption", "");
                    break;
                case CommandType.UpdateOptions:
                    _parameters.Add("SelectedOption", "");
                    break;
                case CommandType.UpdateMapEvent:
                    _parameters.Add("EventID", "");
                    _parameters.Add("MapX", "");
                    _parameters.Add("MapY", "");
                    _parameters.Add("RealX", "");
                    _parameters.Add("RealY", "");
                    _parameters.Add("Direction", "");
                    break;
                case CommandType.ChangeMapEventDirection:
                    _parameters.Add("EventID", "");
                    _parameters.Add("Direction", "");
                    break;


            }
        }

        public CommandType GetCommandType()
        {
            return _commandType;
        }

        public string GetParameter(string name)
        {
            if (_parameters.ContainsKey(name))
                return _parameters[name];
            return null;
        }

        public void SetParameter(string name, string value)
        {
            if (_parameters.ContainsKey(name))
                _parameters[name] = value;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes((int)_commandType), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_parameters.Count), 0, sizeof(int));
                for (int i = 0; i < _parameters.Count; i++)
                {
                    string parameterName = _parameters.ElementAt(i).Key;
                    string parameter = _parameters.ElementAt(i).Value;

                    byte[] bytes = Encoding.UTF8.GetBytes(parameterName);
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                    stream.Write(bytes, 0, bytes.Length);

                    bytes = Encoding.UTF8.GetBytes(parameter);
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                    stream.Write(bytes, 0, bytes.Length);
                }
                return stream.ToArray();
            }
        }

        public static ClientCommand FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                CommandType type = (CommandType)BitConverter.ToInt32(tempBytes, 0);

                ClientCommand command = new ClientCommand(type);

                stream.Read(tempBytes, 0, sizeof(int));
                int parametersCount = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < parametersCount; i++)
                {
                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    int size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
                    string parameterName = new string(Encoding.UTF8.GetChars(tempBytes));

                    tempBytes = new byte[sizeof(int)];
                    stream.Read(tempBytes, 0, sizeof(int));
                    size = BitConverter.ToInt32(tempBytes, 0);

                    tempBytes = new byte[size];
                    stream.Read(tempBytes, 0, size);
                    string parameter = new string(Encoding.UTF8.GetChars(tempBytes));

                    command.SetParameter(parameterName, parameter);
                }

                return command;
            }
        }
    }
}

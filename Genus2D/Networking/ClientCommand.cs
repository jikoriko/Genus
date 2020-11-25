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
            CloseMessage,
            SelectOption,
            MovePlayer,
            ToggleRunning,
            ActionTrigger,
            SelectItem,
            RemoveEquipment,
            RemoveAmmo,
            PickupItem,
            DropItem,
            AttackPlayer,
            AttackEnemy,
            CloseShop,
            BuyShopItem,
            SellShopItem,
            TradeRequest,
            CancelTrade,
            AcceptTrade,
            AddTradeItem,
            RemoveTradeItem,
            CloseBank,
            AddBankItem,
            RemoveBankItem

        }

        private CommandType _commandType;
        private class Parameter
        {
            public byte TypeCode; //0 = text, 1 = int, 2 = float, 3 = bool
            public object Value;

            public Parameter(byte typeCode, object value)
            {
                TypeCode = typeCode;
                Value = value;
            }
        }
        private Dictionary<string, Parameter> _parameters;

        public ClientCommand(CommandType type)
        {
            _commandType = type;
            _parameters = new Dictionary<string, Parameter>();

            switch (type)
            {
                case CommandType.CloseMessage:

                    break;
                case CommandType.SelectOption:
                    _parameters.Add("Option", new Parameter(1, 0));
                    break;
                case CommandType.MovePlayer:
                    _parameters.Add("Direction", new Parameter(1, 0));
                    break;
                case CommandType.ToggleRunning:
                    _parameters.Add("Running", new Parameter(3, false));
                    break;
                case CommandType.ActionTrigger:

                    break;
                case CommandType.SelectItem:
                    _parameters.Add("ItemIndex", new Parameter(1, 0));
                    break;
                case CommandType.RemoveEquipment:
                    _parameters.Add("EquipmentIndex", new Parameter(1, 0));
                    break;
                case CommandType.PickupItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Signature", new Parameter(1, -1));
                    break;
                case CommandType.DropItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.AttackPlayer:
                    _parameters.Add("PlayerID", new Parameter(1, -1));
                    break;
                case CommandType.AttackEnemy:
                    _parameters.Add("EnemyID", new Parameter(1, -1));
                    break;
                case CommandType.CloseShop:

                    break;
                case CommandType.BuyShopItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.SellShopItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.TradeRequest:
                    _parameters.Add("PlayerID", new Parameter(1, -1));
                    break;
                case CommandType.CancelTrade:

                    break;
                case CommandType.AcceptTrade:

                    break;
                case CommandType.AddTradeItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.RemoveTradeItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.CloseBank:

                    break;
                case CommandType.AddBankItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;
                case CommandType.RemoveBankItem:
                    _parameters.Add("ItemIndex", new Parameter(1, -1));
                    _parameters.Add("Count", new Parameter(1, -1));
                    break;

            }
        }

        public CommandType GetCommandType()
        {
            return _commandType;
        }

        public object GetParameter(string name)
        {
            if (_parameters.ContainsKey(name))
                return _parameters[name].Value;
            return null;
        }

        public void SetParameter(int index, object value)
        {
            if (index > -1 && index < _parameters.Count)
            {
                string key = _parameters.ElementAt(index).Key;
                _parameters[key].Value = value;
            }
        }

        public void SetParameter(string name, object value)
        {
            if (_parameters.ContainsKey(name))
                _parameters[name].Value = value;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.WriteByte((byte)_commandType);
                stream.Write(BitConverter.GetBytes(_parameters.Count), 0, sizeof(int));
                for (int i = 0; i < _parameters.Count; i++)
                {
                    Parameter parameter = _parameters.ElementAt(i).Value;
                    stream.WriteByte(parameter.TypeCode);

                    if (parameter.TypeCode == 0)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes((string)parameter.Value);
                        stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    else if (parameter.TypeCode == 1)
                        stream.Write(BitConverter.GetBytes((int)parameter.Value), 0, sizeof(int));
                    else if (parameter.TypeCode == 2)
                        stream.Write(BitConverter.GetBytes((float)parameter.Value), 0, sizeof(float));
                    else if (parameter.TypeCode == 3)
                        stream.Write(BitConverter.GetBytes((bool)parameter.Value), 0, sizeof(bool));
                }
                return stream.ToArray();
            }
        }

        public static ClientCommand FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                CommandType type = (CommandType)stream.ReadByte();

                ClientCommand command = new ClientCommand(type);

                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int parametersCount = BitConverter.ToInt32(tempBytes, 0);

                for (int i = 0; i < parametersCount; i++)
                {
                    byte typeCode = (byte)stream.ReadByte();
                    object value = null;
                    if (typeCode == 0)
                    {
                        tempBytes = new byte[sizeof(int)];
                        stream.Read(tempBytes, 0, sizeof(int));
                        int size = BitConverter.ToInt32(tempBytes, 0);

                        tempBytes = new byte[size];
                        stream.Read(tempBytes, 0, size);
                        value = new string(Encoding.UTF8.GetChars(tempBytes));
                    }
                    else if (typeCode == 1)
                    {
                        tempBytes = new byte[sizeof(int)];
                        stream.Read(tempBytes, 0, sizeof(int));
                        value = BitConverter.ToInt32(tempBytes, 0);
                    }
                    else if (typeCode == 2)
                    {
                        tempBytes = new byte[sizeof(float)];
                        stream.Read(tempBytes, 0, sizeof(float));
                        value = BitConverter.ToSingle(tempBytes, 0);
                    }
                    else if (typeCode == 3)
                    {
                        tempBytes = new byte[sizeof(bool)];
                        stream.Read(tempBytes, 0, sizeof(bool));
                        value = BitConverter.ToBoolean(tempBytes, 0);
                    }

                    command.SetParameter(i, value);
                }

                return command;
            }
        }
    }
}

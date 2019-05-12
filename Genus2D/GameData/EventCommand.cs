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
            ShowMessage,
            ShowOptions,
            ChangeSystemVariable,
            ConditionalBranchStart,
            ConditionalBranchElse,
            ConditionalBranchEnd,
            AddInventoryItem,
            RemoveInventoryItem,
            TeleportPlayer,
            MovePlayer,
            ChangePlayerDirection,
            ChangePlayerSprite,
            ChangeMapEvent,
            WaitForMovementCompletion,
            AddGold,
            RemoveGold,
            SpawnEnemy
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
                case CommandType.ShowMessage:
                    _parameters.Add("Message", "");
                    break;
                case CommandType.ShowOptions:
                    _parameters.Add("Message", "");
                    _parameters.Add("Options", new List<string>());
                    break;
                case CommandType.ChangeSystemVariable:
                    _parameters.Add("VariableID", -1);
                    _parameters.Add("VariableType", 0);
                    _parameters.Add("VariableValue", 0);
                    _parameters.Add("RandomInt", false);
                    _parameters.Add("RandomFloat", false);
                    _parameters.Add("RandomMin", 0);
                    _parameters.Add("RandomMax", 0);
                    break;
                case CommandType.ConditionalBranchStart:
                    _parameters.Add("ConditionalBranchType", ConditionalBranchType.PlayerPosition);
                    _parameters.Add("PlayerMapID", -1);
                    _parameters.Add("PlayerMapX", 0);
                    _parameters.Add("PlayerMapY", 0);
                    _parameters.Add("MapEventMapID", -1);
                    _parameters.Add("MapEventID", -1);
                    _parameters.Add("MapEventMapX", 0);
                    _parameters.Add("MapEventMapY", 0);
                    _parameters.Add("EquippedItemID", -1);
                    _parameters.Add("InventoryItemID", -1);
                    _parameters.Add("InventoryItemAmount", 1);
                    _parameters.Add("VariableID", -1);
                    _parameters.Add("VariableType", VariableType.Integer);
                    _parameters.Add("VariableIntegerValue", 0);
                    _parameters.Add("VariableFloatValue", 0.0f);
                    _parameters.Add("VariableBoolValue", true);
                    _parameters.Add("VariableTextValue", "");
                    _parameters.Add("ValueCondition", ConditionValueCheck.Equal);
                    _parameters.Add("TextCondition", ConditionalTextCheck.Equal);
                    _parameters.Add("QuestStatus", QuestStatus.NotStarted);
                    _parameters.Add("SelectedOption", 0);
                    _parameters.Add("TerrainTag", 0);
                    _parameters.Add("PlayerDirection", FacingDirection.Down);
                    _parameters.Add("Gold", 0);
                    break;
                case CommandType.AddInventoryItem:
                    _parameters.Add("ItemID", -1);
                    _parameters.Add("ItemAmount", 1);
                    break;
                case CommandType.RemoveInventoryItem:
                    _parameters.Add("ItemID", -1);
                    _parameters.Add("ItemAmount", 1);
                    break;
                case CommandType.TeleportPlayer:
                    _parameters.Add("MapID", 0);
                    _parameters.Add("MapX", 0);
                    _parameters.Add("MapY", 0);
                    break;
                case CommandType.MovePlayer:
                    _parameters.Add("Direction", MovementDirection.Down);
                    break;
                case CommandType.ChangePlayerDirection:
                    _parameters.Add("Direction", FacingDirection.Down);
                    break;
                case CommandType.ChangePlayerSprite:
                    _parameters.Add("SpriteID", -1);
                    break;
                case CommandType.ChangeMapEvent:
                    _parameters.Add("MapID", -1);
                    _parameters.Add("EventID", -1);
                    _parameters.Add("Property", ChangeMapEventProperty.Teleport);
                    _parameters.Add("MapX", 0);
                    _parameters.Add("MapY", 0);
                    _parameters.Add("MovementDirection", MovementDirection.Down);
                    _parameters.Add("FacingDirection", FacingDirection.Down);
                    _parameters.Add("SpriteID", -1);
                    _parameters.Add("RenderPriority", RenderPriority.BelowPlayer);
                    _parameters.Add("MovementSpeed", MovementSpeed.Normal);
                    _parameters.Add("MovementFrequency", MovementFrequency.Normal);
                    _parameters.Add("Passable", false);
                    _parameters.Add("RandomMovement", false);
                    _parameters.Add("Enabled", true);
                    break;
                case CommandType.AddGold:
                    _parameters.Add("Gold", 0);
                    break;
                case CommandType.RemoveGold:
                    _parameters.Add("Gold", 0);
                    break;
                case CommandType.SpawnEnemy:
                    _parameters.Add("EnemyID", -1);
                    _parameters.Add("Count", 1);
                    _parameters.Add("RespawnTime", 0.0f);
                    _parameters.Add("SpawnRadius", 0);
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

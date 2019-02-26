using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgServer
{
    public class EventInterpreter
    {
        private List<TriggeringEvent> _triggeringEvents;

        public EventInterpreter()
        {
            _triggeringEvents = new List<TriggeringEvent>();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _triggeringEvents.Count; i++)
            {
                TriggerEvent(_triggeringEvents[i], deltaTime);
                if (_triggeringEvents[i].Complete)
                {
                    _triggeringEvents.RemoveAt(i);
                    i--;
                }
            }
        }

        private void TriggerEvent(TriggeringEvent triggeringEvent, float deltaTime)
        {
            GameClient client = triggeringEvent.GetGameClient();
            EventData eventData = triggeringEvent.GetEventData();

            if (triggeringEvent.MessageShowing)
            {
                if (!client.MessageShowing || !client.Connected())
                {
                    triggeringEvent.MessageShowing = false;
                    client.MovementDisabled = false;
                }

                return;
            }
            else if (triggeringEvent.OptionsShowing)
            {
                if (!client.MessageShowing || !client.Connected())
                {
                    triggeringEvent.OptionsShowing = false;
                    client.MovementDisabled = false;
                    triggeringEvent.SelectedOption = client.SelectedOption;
                    client.SelectedOption = -1;
                }

                return;
            }

            if (triggeringEvent.WaitTimer > 0)
            {
                triggeringEvent.WaitTimer -= deltaTime;
                return;
            }

            if (triggeringEvent.CommandID < eventData.EventCommands.Count - 1)
            {
                triggeringEvent.CommandID++;
                EventCommand eventCommand = eventData.EventCommands[triggeringEvent.CommandID];

                ServerCommand serverCommand;
                int mapID;
                int eventID;
                int mapX;
                int mapY;
                FacingDirection facingDirection;
                MovementDirection movementDirection;
                int itemID;
                int itemAmount;
                int variableID;
                VariableType variableType;
                object variableValue;

                switch (eventCommand.Type)
                {
                    case EventCommand.CommandType.WaitTimer:

                        float timer = (float)eventCommand.GetParameter("Time");
                        triggeringEvent.WaitTimer = timer;

                        break;
                    case EventCommand.CommandType.TeleportPlayer:

                        if (client == null || !client.Connected()) break;

                        mapID = (int)eventCommand.GetParameter("MapID");
                        mapX = (int)eventCommand.GetParameter("MapX");
                        mapY = (int)eventCommand.GetParameter("MapY");
                        client.SetMapID(mapID);
                        client.SetMapPosition(mapX, mapY);

                        break;
                    case EventCommand.CommandType.MovePlayer:

                        if (client == null || !client.Connected()) break;

                        if (client.Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        movementDirection = (MovementDirection)eventCommand.GetParameter("Direction");
                        client.Move(movementDirection);

                        break;
                    case EventCommand.CommandType.ChangePlayerDirection:

                        if (client == null || !client.Connected()) break;

                        facingDirection = (FacingDirection)eventCommand.GetParameter("Direction");
                        client.ChangeDirection(facingDirection);

                        break;
                    case EventCommand.CommandType.TeleportMapEvent:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        mapX = (int)eventCommand.GetParameter("MapX");
                        mapY = (int)eventCommand.GetParameter("MapY");
                        if (!Server.Instance.GetMapInstance(mapID).TeleportMapEvent(eventID, mapX, mapY))
                            triggeringEvent.CommandID--;

                        break;
                    case EventCommand.CommandType.MoveMapEvent:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        movementDirection = (MovementDirection)eventCommand.GetParameter("Direction");
                        if (!Server.Instance.GetMapInstance(mapID).MoveMapEvent(eventID, movementDirection))
                            triggeringEvent.CommandID--;

                        break;
                    case EventCommand.CommandType.ChangeMapEventDirection:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        facingDirection = (FacingDirection)eventCommand.GetParameter("Direction");
                        Server.Instance.GetMapInstance(mapID).ChangeMapEventDirection(eventID, facingDirection);

                        break;
                    case EventCommand.CommandType.ShowMessage:

                        if (client == null || !client.Connected()) break;

                        serverCommand = new ServerCommand(ServerCommand.CommandType.ShowMessage);
                        serverCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                        client.AddServerCommand(serverCommand);
                        triggeringEvent.MessageShowing = true;
                        client.MovementDisabled = true;
                        client.MessageShowing = true;

                        break;
                    case EventCommand.CommandType.ShowOptions:

                        if (client == null || !client.Connected()) break;

                        List<string> options = (List<string>)eventCommand.GetParameter("Options");
                        client.SelectedOption = 0;
                        int OptionsCount = options.Count;

                        string optionStrings = "";
                        for (int i = 0; i < options.Count; i++)
                        {
                            optionStrings += options[i];
                            if (i < options.Count - 1)
                                optionStrings += ",";
                        }

                        serverCommand = new ServerCommand(ServerCommand.CommandType.ShowOptions);
                        serverCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                        serverCommand.SetParameter("Options", optionStrings);

                        client.AddServerCommand(serverCommand);
                        triggeringEvent.OptionsShowing = true;
                        client.MovementDisabled = true;
                        client.MessageShowing = true;

                        break;

                    case EventCommand.CommandType.ChangeSystemVariable:

                        variableID = (int)eventCommand.GetParameter("VariableID");
                        variableType = (VariableType)eventCommand.GetParameter("VariableType");
                        variableValue = eventCommand.GetParameter("VariableValue");

                        SystemVariable.GetSystemVariable(variableID).SetVariableType(variableType);
                        SystemVariable.GetSystemVariable(variableID).SetValue(variableValue);

                        break;

                    case EventCommand.CommandType.ConditionalBranchStart:

                        ConditionalBranchType type = (ConditionalBranchType)eventCommand.GetParameter("ConditionalBranchType");

                        bool conditionMet = false;

                        switch (type)
                        {
                            case ConditionalBranchType.PlayerPosition:
                                if (client == null) break;

                                mapID = (int)eventCommand.GetParameter("PlayerMapID");
                                if (mapID == -1) break;
                                mapX = (int)eventCommand.GetParameter("PlayerMapX");
                                mapY = (int)eventCommand.GetParameter("PlayerMapY");

                                if (client.GetPacket().MapID == mapID && client.GetPacket().PositionX == mapX &&
                                        client.GetPacket().PositionY == mapY)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.MapEventPosition:
                                mapID = (int)eventCommand.GetParameter("MapEventMapID");
                                eventID = (int)eventCommand.GetParameter("MapEventID");
                                if (mapID == -1 || eventID == -1) break;
                                mapX = (int)eventCommand.GetParameter("MapEventMapX");
                                mapY = (int)eventCommand.GetParameter("MapEventMapY");

                                MapEvent mapEvent = Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID);
                                if (mapEvent.MapX == mapX && mapEvent.MapY == mapY)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.ItemEquipped:
                                if (client == null || !client.Connected()) break;

                                itemID = (int)eventCommand.GetParameter("EquippedItemID");
                                if (itemID == -1) break;
                                if (client.GetPacket().Data.ItemEquipped(itemID))
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.ItemInInventory:
                                if (client == null) break;

                                itemID = (int)eventCommand.GetParameter("InventoryItemID");
                                itemAmount = (int)eventCommand.GetParameter("InventoryItemAmount");
                                if (itemID == -1) break;
                                if (client.GetPacket().Data.ItemInInventory(itemID, itemAmount))
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.SystemVariable:

                                variableID = (int)eventCommand.GetParameter("VariableID");
                                if (variableID == -1) break;
                                variableType = (VariableType)eventCommand.GetParameter("VariableType");
                                if (SystemVariable.GetSystemVariable(variableID).Type == variableType)
                                {
                                    ConditionValueCheck valueCheck = (ConditionValueCheck)eventCommand.GetParameter("ValueCondition");
                                    ConditionalTextCheck textCheck = (ConditionalTextCheck)eventCommand.GetParameter("TextCondition");

                                    switch (variableType)
                                    {
                                        case VariableType.Integer:
                                            variableValue = eventCommand.GetParameter("VariableIntegerValue");
                                            int intVal = (int)SystemVariable.GetSystemVariable(variableID).Value;
                                            switch (valueCheck)
                                            {
                                                case ConditionValueCheck.Equal:
                                                    if (intVal == (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.Greater:
                                                    if (intVal > (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.GreaterOrEqual:
                                                    if (intVal >= (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.Lower:
                                                    if (intVal < (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.LowerOrEqual:
                                                    if (intVal <= (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.NotEqual:
                                                    if (intVal != (int)variableValue)
                                                        conditionMet = true;
                                                    break;
                                            }
                                            break;
                                        case VariableType.Float:
                                            variableValue = eventCommand.GetParameter("VariableFloatValue");
                                            float floatVal = (float)SystemVariable.GetSystemVariable(variableID).Value;
                                            switch (valueCheck)
                                            {
                                                case ConditionValueCheck.Equal:
                                                    if (floatVal == (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.Greater:
                                                    if (floatVal > (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.GreaterOrEqual:
                                                    if (floatVal >= (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.Lower:
                                                    if (floatVal < (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.LowerOrEqual:
                                                    if (floatVal <= (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionValueCheck.NotEqual:
                                                    if (floatVal != (float)variableValue)
                                                        conditionMet = true;
                                                    break;
                                            }
                                            break;
                                        case VariableType.Bool:
                                            variableValue = eventCommand.GetParameter("VariableBoolValue");
                                            bool boolVal = (bool)SystemVariable.GetSystemVariable(variableID).Value;
                                            if (boolVal == (bool)variableValue)
                                                conditionMet = true;
                                            break;
                                        case VariableType.Text:
                                            variableValue = eventCommand.GetParameter("VariableTextValue");
                                            string textVal = (string)SystemVariable.GetSystemVariable(variableID).Value;
                                            switch (textCheck)
                                            {
                                                case ConditionalTextCheck.Equal:
                                                    if (textVal == (string)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionalTextCheck.NotEqual:
                                                    if (textVal != (string)variableValue)
                                                        conditionMet = true;
                                                    break;
                                                case ConditionalTextCheck.Includes:
                                                    if (textVal.Contains((string)variableValue))
                                                        conditionMet = true;
                                                    break;
                                            }
                                            break;
                                    }
                                }


                                break;
                            case ConditionalBranchType.QuestStatus:

                                if (client == null || !client.Connected()) break;

                                QuestStatus status = (QuestStatus)eventCommand.GetParameter("QuestStatus");
                                // check quest status here

                                break;
                            case ConditionalBranchType.SelectedOption:

                                int option = (int)eventCommand.GetParameter("SelectedOption");
                                if (option == triggeringEvent.SelectedOption)
                                    conditionMet = true;

                                break;
                        }

                        if (!conditionMet)
                        {
                            int conditionDepth = triggeringEvent.ConditionDepth;
                            for (int i = triggeringEvent.CommandID + 1; i < eventData.EventCommands.Count; i++)
                            {
                                triggeringEvent.CommandID++;

                                if (eventData.EventCommands[i].Type == EventCommand.CommandType.ConditionalBranchStart)
                                    conditionDepth++;

                                if (eventData.EventCommands[i].Type == EventCommand.CommandType.ConditionalBranchElse ||
                                    eventData.EventCommands[i].Type == EventCommand.CommandType.ConditionalBranchEnd)
                                {
                                    if (conditionDepth > triggeringEvent.ConditionDepth)
                                        conditionDepth--;
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            triggeringEvent.ConditionDepth++;
                        }

                        break;
                    case EventCommand.CommandType.ConditionalBranchElse:
                        for (int i = triggeringEvent.CommandID + 1; i < eventData.EventCommands.Count; i++)
                        {
                            triggeringEvent.CommandID++;
                            if (eventData.EventCommands[i].Type == EventCommand.CommandType.ConditionalBranchEnd)
                            {
                                break;
                            }
                        }
                        break;
                    case EventCommand.CommandType.ConditionalBranchEnd:
                        if (triggeringEvent.ConditionDepth > 0)
                            triggeringEvent.ConditionDepth -= 1;
                        break;
                    case EventCommand.CommandType.AddInventoryItem:
                        if (client == null || !client.Connected()) break;

                        itemID = (int)eventCommand.GetParameter("ItemID");
                        itemAmount = (int)eventCommand.GetParameter("ItemAmount");
                        int remainder = itemAmount - client.GetPacket().Data.AddInventoryItem(itemID, itemAmount);

                        //do something with the remainder, the players inventory is full, drop on floor?

                        break;
                    case EventCommand.CommandType.RemoveInventoryItem:
                        if (client == null || !client.Connected()) break;

                        itemID = (int)eventCommand.GetParameter("ItemID");
                        itemAmount = (int)eventCommand.GetParameter("ItemAmount");
                        client.GetPacket().Data.RemoveInventoryItem(itemID, itemAmount);

                        break;
                    case EventCommand.CommandType.ChangePlayerSprite:
                        if (client == null || !client.Connected()) break;

                        int spriteID = (int)eventCommand.GetParameter("SpriteID");
                        client.GetPacket().SpriteID = spriteID;

                        break;
                    case EventCommand.CommandType.ChangeMapEventSprite:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        spriteID = (int)eventCommand.GetParameter("SpriteID");
                        Server.Instance.GetMapInstance(mapID).ChangeMapEventSprite(eventID, spriteID);

                        break;
                }
            }
            else
            {
                triggeringEvent.FinishTriggering();
            }
        }

        public void TriggerEventData(GameClient client, MapEvent mapEvent)
        {
            if (mapEvent.Locked) // add player lock id so others can interact
                return;
            _triggeringEvents.Add(new TriggeringEvent(client, mapEvent));
        }



    }
}

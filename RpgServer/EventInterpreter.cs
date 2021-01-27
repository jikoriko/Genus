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
        private MapInstance _mapInstance;
        private List<TriggeringEvent> _triggeringEvents;

        public EventInterpreter(MapInstance mapInstance)
        {
            _mapInstance = mapInstance;
            _triggeringEvents = new List<TriggeringEvent>();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _triggeringEvents.Count; i++)
            {
                if (_triggeringEvents[i] != null)
                {
                    TriggerEvent(_triggeringEvents[i], deltaTime);
                    if (_triggeringEvents[i].Complete)
                    {
                        _triggeringEvents.RemoveAt(i);
                        i--;
                    }
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

            if (triggeringEvent.GetParent() != null)
            {
                if (triggeringEvent.GetParent().CommandID <= triggeringEvent.CommandID)
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
                MapEvent mapEvent;
                FacingDirection facingDirection;
                MovementDirection movementDirection;
                int spriteID;
                int itemID;
                int itemAmount;
                int added;
                int remainder;
                int variableID;
                VariableType variableType;
                object variableValue;
                int gold;
                int questID;

                switch (eventCommand.Type)
                {
                    case EventCommand.CommandType.WaitTimer:
                        if (client != null) break;

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

                        movementDirection = (MovementDirection)eventCommand.GetParameter("Direction");
                        client.Move(movementDirection, true);

                        break;
                    case EventCommand.CommandType.ChangePlayerDirection:

                        if (client == null || !client.Connected()) break;

                        facingDirection = (FacingDirection)eventCommand.GetParameter("Direction");
                        client.ChangeDirection(facingDirection);

                        break;

                    case EventCommand.CommandType.ChangeMapEvent:

                        if (client != null) break;

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");

                        if (!Server.Instance.GetMapInstance(mapID).ChangeMapEvent(eventID, eventCommand))
                            triggeringEvent.CommandID--;

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
                        if (client != null) break;

                        variableID = (int)eventCommand.GetParameter("VariableID");
                        variableType = (VariableType)eventCommand.GetParameter("VariableType");
                        variableValue = eventCommand.GetParameter("VariableValue");

                        bool randomInt = (bool)eventCommand.GetParameter("RandomInt");
                        bool randomFloat = (bool)eventCommand.GetParameter("RandomFloat");

                        if (randomInt)
                        {
                            int randomMin = (int)eventCommand.GetParameter("RandomMin");
                            int randomMax = (int)eventCommand.GetParameter("RandomMax");
                            int randomValue = new Random().Next(randomMin, randomMax);
                            SystemVariable.GetSystemVariable(variableID).SetVariableType(variableType);
                            SystemVariable.GetSystemVariable(variableID).SetValue(randomValue);
                        }
                        else if (randomFloat)
                        {
                            float randomMin = (float)eventCommand.GetParameter("RandomMin");
                            float randomMax = (float)eventCommand.GetParameter("RandomMax");
                            float randomValue = (float)new Random().NextDouble();
                            float difference = randomMax - randomMin;
                            randomValue = randomMin + (difference * randomValue);
                            SystemVariable.GetSystemVariable(variableID).SetVariableType(variableType);
                            SystemVariable.GetSystemVariable(variableID).SetValue(randomValue);
                        }
                        else
                        {
                            SystemVariable.GetSystemVariable(variableID).SetVariableType(variableType);
                            SystemVariable.GetSystemVariable(variableID).SetValue(variableValue);
                        }

                        break;

                    case EventCommand.CommandType.ConditionalBranchStart:

                        ConditionalBranchType type = (ConditionalBranchType)eventCommand.GetParameter("ConditionalBranchType");

                        bool conditionMet = false;
                        bool result = (bool)eventCommand.GetParameter("Result");

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

                                mapEvent = Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID);
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

                                questID = (int)eventCommand.GetParameter("QuestID");
                                if (questID != -1)
                                {
                                    QuestStatusCheck statusCheck = (QuestStatusCheck)eventCommand.GetParameter("QuestStatus");
                                    switch (statusCheck)
                                    {
                                        case QuestStatusCheck.Started:
                                            if (client.GetPacket().Data.QuestStarted(questID))
                                                conditionMet = true;
                                            break;
                                        case QuestStatusCheck.Complete:
                                            if (client.GetPacket().Data.QuestComplete(questID))
                                                conditionMet = true;
                                            break;
                                        case QuestStatusCheck.Progression:
                                            int progression = (int)eventCommand.GetParameter("QuestProgression");
                                            if (progression != -1)
                                            {
                                                int greaterCondition = (int)eventCommand.GetParameter("QuestProgressionCondition");
                                                if (greaterCondition == 0)
                                                {
                                                    if (client.GetPacket().Data.GetQuestProgression(questID) == progression)
                                                        conditionMet = true;
                                                }
                                                else
                                                {
                                                    if (client.GetPacket().Data.GetQuestProgression(questID) >= progression)
                                                        conditionMet = true;
                                                }
                                            }
                                            break;
                                    }
                                }
                                // check quest status here

                                break;
                            case ConditionalBranchType.SelectedOption:

                                int option = (int)eventCommand.GetParameter("SelectedOption");
                                if (option == triggeringEvent.SelectedOption)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.TerrainTag:
                                if (client == null || !client.Connected()) break;

                                int tag = (int)eventCommand.GetParameter("TerrainTag");
                                if (client.TerrainTagCheck(tag))
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.PlayerDirection:
                                if (client == null || !client.Connected()) break;

                                FacingDirection dir = (FacingDirection)eventCommand.GetParameter("PlayerDirection");
                                if (client.GetPacket().Direction == dir)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.PlayerGold:
                                if (client == null || !client.Connected()) break;

                                gold = (int)eventCommand.GetParameter("Gold");
                                if (client.GetPacket().Data.Gold >= gold)
                                    conditionMet = true;

                                break;
                        }

                        if (conditionMet != result)
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
                        added = client.GetPacket().Data.AddInventoryItem(itemID, itemAmount);
                        remainder = itemAmount - added;
                        if (remainder > 0)
                        {
                            mapX = client.GetPacket().PositionX;
                            mapY = client.GetPacket().PositionY;
                            MapItem mapItem = new MapItem(itemID, remainder, mapX, mapY, client.GetPacket().PlayerID, client.GetPacket().OnBridge);
                            _mapInstance.AddMapItem(mapItem);
                        }

                        break;
                    case EventCommand.CommandType.RemoveInventoryItem:
                        if (client == null || !client.Connected()) break;

                        itemID = (int)eventCommand.GetParameter("ItemID");
                        itemAmount = (int)eventCommand.GetParameter("ItemAmount");
                        client.GetPacket().Data.RemoveInventoryItem(itemID, itemAmount);

                        break;
                    case EventCommand.CommandType.ChangePlayerSprite:
                        if (client == null || !client.Connected()) break;

                        spriteID = (int)eventCommand.GetParameter("SpriteID");
                        client.GetPacket().SpriteID = spriteID;

                        break;
                    case EventCommand.CommandType.WaitForMovementCompletion:
                        if (client == null || !client.Connected()) break;

                        if (client.Moving())
                        {
                            triggeringEvent.CommandID--;
                        }

                        break;
                    case EventCommand.CommandType.AddGold:
                        if (client == null || !client.Connected()) break;

                        gold = (int)eventCommand.GetParameter("Gold");
                        client.GetPacket().Data.Gold += gold;

                        break;
                    case EventCommand.CommandType.RemoveGold:
                        if (client == null || !client.Connected()) break;

                        gold = (int)eventCommand.GetParameter("Gold");
                        client.GetPacket().Data.Gold -= gold;
                        if (client.GetPacket().Data.Gold < 0)
                            client.GetPacket().Data.Gold = 0;

                        break;
                    case EventCommand.CommandType.SpawnEnemy:
                        if (client != null) break;

                        int enemyID = (int)eventCommand.GetParameter("EnemyID");
                        if (enemyID == -1) break;

                        int enemyCount = (int)eventCommand.GetParameter("Count");
                        float respawnTime = (float)eventCommand.GetParameter("RespawnTime");
                        int spawnRadius = (int)eventCommand.GetParameter("SpawnRadius");

                        mapEvent = triggeringEvent.GetMapEvent();

                        Dictionary<MapEnemy, float> enemyTracker = _mapInstance.GetEnemyTracker(mapEvent);
                        Random r = new Random();
                        while (enemyTracker.Count < enemyCount)
                        {
                            int minX = mapEvent.MapX - spawnRadius;
                            int maxX = mapEvent.MapX + spawnRadius;
                            int minY = mapEvent.MapY - spawnRadius;
                            int maxY = mapEvent.MapY + spawnRadius;

                            while (true)
                            {
                                int spawnX = r.Next(minX, maxX);
                                int spawnY = r.Next(minY, maxY);
                                bool onBridge = mapEvent.OnBridge && _mapInstance.GetBridgeFlag(spawnX, spawnY);
                                bool bridgeEntry = onBridge && _mapInstance.MapTilesetPassable(spawnX, spawnY);
                                if (_mapInstance.MapTileCharacterPassable(spawnX, spawnY, false, onBridge, bridgeEntry, MovementDirection.Down))
                                {
                                    MapEnemy enemy = new MapEnemy(enemyID, spawnX, spawnY, onBridge);
                                    enemyTracker.Add(enemy, respawnTime);
                                    _mapInstance.AddMapEnemy(enemy);
                                    break;
                                }
                            }
                        }

                        break;
                    case EventCommand.CommandType.ProgressQuest:
                        if (client == null || !client.Connected()) break;

                        questID = (int)eventCommand.GetParameter("QuestID");
                        if (questID != -1)
                        {
                            if (client.GetPacket().Data.QuestStarted(questID))
                            {
                                int progression = client.GetPacket().Data.GetQuestProgression(questID);
                                if (client.GetPacket().Data.ProgressQuest(questID))
                                {
                                    QuestData data = QuestData.GetData(questID);
                                    QuestData.QuestObective objective = data.Objectives[progression];
                                    for (int i = 0; i < objective.ItemRewards.Count; i++)
                                    {
                                        itemID = objective.ItemRewards[i].Item1;
                                        itemAmount = objective.ItemRewards[i].Item2;
                                        added = client.GetPacket().Data.AddInventoryItem(itemID, itemAmount);
                                        remainder = itemAmount - added;
                                        if (remainder > 0)
                                        {
                                            mapX = client.GetPacket().PositionX;
                                            mapY = client.GetPacket().PositionY;
                                            MapItem mapItem = new MapItem(itemID, remainder, mapX, mapY, client.GetPacket().PlayerID, client.GetPacket().OnBridge);
                                            _mapInstance.AddMapItem(mapItem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                client.GetPacket().Data.StartQuest(questID);
                            }
                        }

                        break;
                    case EventCommand.CommandType.ShowShop:
                        if (client == null || !client.Connected()) break;

                        int shopID = (int)eventCommand.GetParameter("ShopID");
                        client.ShowShop(shopID);

                        break;
                    case EventCommand.CommandType.StartBanking:
                        if (client == null || !client.Connected()) break;

                        client.StartBanking();

                        break;
                    case EventCommand.CommandType.ShowWorkbench:
                        if (client == null || !client.Connected()) break;

                        int workbenchID = (int)eventCommand.GetParameter("WorkbenchID");
                        client.ShowWorkbench(workbenchID);

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
            if (mapEvent == null || mapEvent.GetEventData() == null)
                return;

            if (mapEvent.Locked) // add player lock id list so others can interact
                //we did this because sometimes a player could double trigger an event as it started / finished (event touch for example spamming the trigger)
            {
                return;
            }

            TriggeringEvent parent = new TriggeringEvent(null, mapEvent);
            _triggeringEvents.Add(parent);
            if (client == null)
            {
                GameClient[] clients = _mapInstance.GetClients();
                for (int i = 0; i < clients.Length; i++)
                {
                    _triggeringEvents.Add(new TriggeringEvent(clients[i], mapEvent, parent));
                }
            }
            else
            {
                _triggeringEvents.Add(new TriggeringEvent(client, mapEvent, parent));
            }

            mapEvent.Locked = true;

        }



    }
}

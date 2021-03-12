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
        private ServerMap _serverMap;
        private List<TriggeringEvent> _triggeringEvents;

        public EventInterpreter(ServerMap serverMap)
        {
            _serverMap = serverMap;
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
            MapPlayer mapPlayer = triggeringEvent.GetMapPlayer();
            EventData eventData = triggeringEvent.GetEventData();

            if (triggeringEvent.MessageShowing)
            {
                if (!mapPlayer.MessageShowing || !mapPlayer.Connected)
                {
                    triggeringEvent.MessageShowing = false;
                    mapPlayer.MovementDisabled = false;
                }

                return;
            }
            else if (triggeringEvent.OptionsShowing)
            {
                if (!mapPlayer.MessageShowing || !mapPlayer.Connected)
                {
                    triggeringEvent.OptionsShowing = false;
                    mapPlayer.MovementDisabled = false;
                    triggeringEvent.SelectedOption = mapPlayer.SelectedOption;
                    mapPlayer.SelectedOption = -1;
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
                        if (mapPlayer != null) break;

                        float timer = (float)eventCommand.GetParameter("Time");
                        triggeringEvent.WaitTimer = timer;

                        break;
                    case EventCommand.CommandType.TeleportPlayer:

                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        mapID = (int)eventCommand.GetParameter("MapID");
                        mapX = (int)eventCommand.GetParameter("MapX");
                        mapY = (int)eventCommand.GetParameter("MapY");
                        mapPlayer.SetMapID(mapID);
                        mapPlayer.SetMapPosition(mapX, mapY);

                        break;
                    case EventCommand.CommandType.MovePlayer:

                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        movementDirection = (MovementDirection)eventCommand.GetParameter("Direction");
                        mapPlayer.Move(movementDirection, true);

                        break;
                    case EventCommand.CommandType.ChangePlayerDirection:

                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        facingDirection = (FacingDirection)eventCommand.GetParameter("Direction");
                        mapPlayer.ChangeDirection(facingDirection);

                        break;

                    case EventCommand.CommandType.ChangeMapEvent:

                        if (mapPlayer != null) break;

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");

                        if (!Server.Instance.GetServerMap(mapID).GetMapInstance().ChangeMapEvent(eventID, eventCommand))
                            triggeringEvent.CommandID--;

                        break;
                    case EventCommand.CommandType.ShowMessage:

                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        triggeringEvent.MessageShowing = true;
                        mapPlayer.ShowMessage((string)eventCommand.GetParameter("Message"));
                        
                        break;
                    case EventCommand.CommandType.ShowOptions:

                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        List<string> options = (List<string>)eventCommand.GetParameter("Options");
                        mapPlayer.SelectedOption = 0;
                        int OptionsCount = options.Count;

                        string optionStrings = "";
                        for (int i = 0; i < options.Count; i++)
                        {
                            optionStrings += options[i];
                            if (i < options.Count - 1)
                                optionStrings += ",";
                        }

                        string message = (string)eventCommand.GetParameter("Message");
                        triggeringEvent.OptionsShowing = true;
                        mapPlayer.ShowMessage(message, optionStrings);

                        break;

                    case EventCommand.CommandType.ChangeSystemVariable:
                        if (mapPlayer != null) break;

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
                                if (mapPlayer == null) break;

                                mapID = (int)eventCommand.GetParameter("PlayerMapID");
                                if (mapID == -1) break;
                                mapX = (int)eventCommand.GetParameter("PlayerMapX");
                                mapY = (int)eventCommand.GetParameter("PlayerMapY");

                                if (mapPlayer.GetPlayerPacket().MapID == mapID && mapPlayer.GetPlayerPacket().PositionX == mapX &&
                                        mapPlayer.GetPlayerPacket().PositionY == mapY)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.MapEventPosition:
                                mapID = (int)eventCommand.GetParameter("MapEventMapID");
                                eventID = (int)eventCommand.GetParameter("MapEventID");
                                if (mapID == -1 || eventID == -1) break;
                                mapX = (int)eventCommand.GetParameter("MapEventMapX");
                                mapY = (int)eventCommand.GetParameter("MapEventMapY");

                                mapEvent = Server.Instance.GetServerMap(mapID).GetMapInstance().GetMapData().GetMapEvent(eventID);
                                if (mapEvent.MapX == mapX && mapEvent.MapY == mapY)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.ItemEquipped:
                                if (mapPlayer == null || !mapPlayer.Connected) break;

                                itemID = (int)eventCommand.GetParameter("EquippedItemID");
                                if (itemID == -1) break;
                                if (mapPlayer.GetPlayerPacket().Data.ItemEquipped(itemID))
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.ItemInInventory:
                                if (mapPlayer == null) break;

                                itemID = (int)eventCommand.GetParameter("InventoryItemID");
                                itemAmount = (int)eventCommand.GetParameter("InventoryItemAmount");
                                if (itemID == -1) break;
                                if (mapPlayer.GetPlayerPacket().Data.ItemInInventory(itemID, itemAmount))
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

                                if (mapPlayer == null || !mapPlayer.Connected) break;

                                questID = (int)eventCommand.GetParameter("QuestID");
                                if (questID != -1)
                                {
                                    QuestStatusCheck statusCheck = (QuestStatusCheck)eventCommand.GetParameter("QuestStatus");
                                    switch (statusCheck)
                                    {
                                        case QuestStatusCheck.Started:
                                            if (mapPlayer.GetPlayerPacket().Data.QuestStarted(questID))
                                                conditionMet = true;
                                            break;
                                        case QuestStatusCheck.Complete:
                                            if (mapPlayer.GetPlayerPacket().Data.QuestComplete(questID))
                                                conditionMet = true;
                                            break;
                                        case QuestStatusCheck.Progression:
                                            int progression = (int)eventCommand.GetParameter("QuestProgression");
                                            if (progression != -1)
                                            {
                                                int greaterCondition = (int)eventCommand.GetParameter("QuestProgressionCondition");
                                                if (greaterCondition == 0)
                                                {
                                                    if (mapPlayer.GetPlayerPacket().Data.GetQuestProgression(questID) == progression)
                                                        conditionMet = true;
                                                }
                                                else
                                                {
                                                    if (mapPlayer.GetPlayerPacket().Data.GetQuestProgression(questID) >= progression)
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
                                if (mapPlayer == null || !mapPlayer.Connected) break;

                                int tag = (int)eventCommand.GetParameter("TerrainTag");
                                if (mapPlayer.TerrainTagCheck(tag))
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.PlayerDirection:
                                if (mapPlayer == null || !mapPlayer.Connected) break;

                                FacingDirection dir = (FacingDirection)eventCommand.GetParameter("PlayerDirection");
                                if (mapPlayer.GetPlayerPacket().Direction == dir)
                                    conditionMet = true;

                                break;
                            case ConditionalBranchType.PlayerGold:
                                if (mapPlayer == null || !mapPlayer.Connected) break;

                                gold = (int)eventCommand.GetParameter("Gold");
                                if (mapPlayer.GetPlayerPacket().Data.Gold >= gold)
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
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        itemID = (int)eventCommand.GetParameter("ItemID");
                        itemAmount = (int)eventCommand.GetParameter("ItemAmount");
                        added = mapPlayer.GetPlayerPacket().Data.AddInventoryItem(itemID, itemAmount);
                        remainder = itemAmount - added;
                        if (remainder > 0)
                        {
                            mapX = mapPlayer.GetPlayerPacket().PositionX;
                            mapY = mapPlayer.GetPlayerPacket().PositionY;
                            MapItem mapItem = new MapItem(itemID, remainder, mapX, mapY, mapPlayer.GetPlayerPacket().PlayerID, mapPlayer.GetPlayerPacket().OnBridge);
                            _serverMap.GetMapInstance().AddMapItem(mapItem);
                        }

                        break;
                    case EventCommand.CommandType.RemoveInventoryItem:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        itemID = (int)eventCommand.GetParameter("ItemID");
                        itemAmount = (int)eventCommand.GetParameter("ItemAmount");
                        mapPlayer.GetPlayerPacket().Data.RemoveInventoryItem(itemID, itemAmount);

                        break;
                    case EventCommand.CommandType.ChangePlayerSprite:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        spriteID = (int)eventCommand.GetParameter("SpriteID");
                        mapPlayer.GetPlayerPacket().SpriteID = spriteID;

                        break;
                    case EventCommand.CommandType.WaitForMovementCompletion:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        if (mapPlayer.Moving())
                        {
                            triggeringEvent.CommandID--;
                        }

                        break;
                    case EventCommand.CommandType.AddGold:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        gold = (int)eventCommand.GetParameter("Gold");
                        mapPlayer.GetPlayerPacket().Data.Gold += gold;

                        break;
                    case EventCommand.CommandType.RemoveGold:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        gold = (int)eventCommand.GetParameter("Gold");
                        mapPlayer.GetPlayerPacket().Data.Gold -= gold;
                        if (mapPlayer.GetPlayerPacket().Data.Gold < 0)
                            mapPlayer.GetPlayerPacket().Data.Gold = 0;

                        break;
                    case EventCommand.CommandType.SpawnEnemy:
                        if (mapPlayer != null) break;

                        int enemyID = (int)eventCommand.GetParameter("EnemyID");
                        if (enemyID == -1) break;

                        int enemyCount = (int)eventCommand.GetParameter("Count");
                        float respawnTime = (float)eventCommand.GetParameter("RespawnTime");
                        int spawnRadius = (int)eventCommand.GetParameter("SpawnRadius");

                        mapEvent = triggeringEvent.GetMapEvent();

                        Dictionary<MapEnemy, float> enemyTracker = _serverMap.GetMapInstance().GetEnemyTracker(mapEvent);
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
                                bool onBridge = mapEvent.OnBridge && _serverMap.GetMapInstance().GetBridgeFlag(spawnX, spawnY);
                                bool bridgeEntry = onBridge && _serverMap.GetMapInstance().MapTilesetPassable(spawnX, spawnY);
                                if (_serverMap.GetMapInstance().MapTileCharacterPassable(spawnX, spawnY, false, onBridge, bridgeEntry, MovementDirection.Down))
                                {
                                    MapEnemy enemy = new MapEnemy(enemyID, spawnX, spawnY, onBridge);
                                    enemyTracker.Add(enemy, respawnTime);
                                    _serverMap.GetMapInstance().AddMapEnemy(enemy);
                                    break;
                                }
                            }
                        }

                        break;
                    case EventCommand.CommandType.ProgressQuest:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        questID = (int)eventCommand.GetParameter("QuestID");
                        if (questID != -1)
                        {
                            if (mapPlayer.GetPlayerPacket().Data.QuestStarted(questID))
                            {
                                int progression = mapPlayer.GetPlayerPacket().Data.GetQuestProgression(questID);
                                if (mapPlayer.GetPlayerPacket().Data.ProgressQuest(questID))
                                {
                                    QuestData data = QuestData.GetData(questID);
                                    QuestData.QuestObective objective = data.Objectives[progression];
                                    for (int i = 0; i < objective.ItemRewards.Count; i++)
                                    {
                                        itemID = objective.ItemRewards[i].Item1;
                                        itemAmount = objective.ItemRewards[i].Item2;
                                        added = mapPlayer.GetPlayerPacket().Data.AddInventoryItem(itemID, itemAmount);
                                        remainder = itemAmount - added;
                                        if (remainder > 0)
                                        {
                                            mapX = mapPlayer.GetPlayerPacket().PositionX;
                                            mapY = mapPlayer.GetPlayerPacket().PositionY;
                                            MapItem mapItem = new MapItem(itemID, remainder, mapX, mapY, mapPlayer.GetPlayerPacket().PlayerID, mapPlayer.GetPlayerPacket().OnBridge);
                                            _serverMap.GetMapInstance().AddMapItem(mapItem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                mapPlayer.GetPlayerPacket().Data.StartQuest(questID);
                            }
                        }

                        break;
                    case EventCommand.CommandType.ShowShop:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        int shopID = (int)eventCommand.GetParameter("ShopID");
                        mapPlayer.ShowShop(shopID);

                        break;
                    case EventCommand.CommandType.StartBanking:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        mapPlayer.StartBanking();

                        break;
                    case EventCommand.CommandType.ShowWorkbench:
                        if (mapPlayer == null || !mapPlayer.Connected) break;

                        int workbenchID = (int)eventCommand.GetParameter("WorkbenchID");
                        mapPlayer.ShowWorkbench(workbenchID);

                        break;
                }
            }
            else
            {
                triggeringEvent.FinishTriggering();
            }
        }

        public void TriggerEventData(MapPlayer mapPlayer, MapEvent mapEvent)
        {
            if (mapEvent == null || mapEvent.GetEventData() == null)
            {
                return;
            }

            if (mapEvent.Locked) // add player lock id list so others can interact
                //we did this because sometimes a player could double trigger an event as it started / finished (event touch for example spamming the trigger)
            {
                return;
            }

            TriggeringEvent parent = new TriggeringEvent(null, mapEvent);
            _triggeringEvents.Add(parent);
            if (mapPlayer == null)
            {
                List<MapPlayer> players = _serverMap.GetMapInstance().GetMapPlayers();
                for (int i = 0; i < players.Count; i++)
                {
                    _triggeringEvents.Add(new TriggeringEvent(players[i], mapEvent, parent));
                }
            }
            else
            {
                _triggeringEvents.Add(new TriggeringEvent(mapPlayer, mapEvent, parent));
            }

            mapEvent.Locked = true;

        }



    }
}

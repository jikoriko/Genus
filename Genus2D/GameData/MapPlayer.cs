using Genus2D.Networking;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class MapPlayer
    {

        private PlayerPacket _playerPacket;
        private MapInstance _mapInstance;

        public bool Connected;
        public int PlayerID { get; private set; }
        public string Username { get; private set; }

        public bool Running;
        public bool AttemptedMove;
        public MovementDirection AttemptedDirection;

        public bool Dead;
        public CharacterType EnemyCharacterType;
        public int EnemyCharacterID;

        public bool MessageShowing;
        public bool MovementDisabled;
        public int SelectedOption;
        public int ShopID { get; private set; }
        public int WorkbenchID { get; private set; }

        public bool Banking { get; private set; }
        private BankData _bankData;
        public bool BankUpdated;

        public bool Trading { get; private set; }
        public int TradePlayerID { get; private set; }
        private TradeRequest _tradeRequest;
        private bool _ignoreEvents;

        private float _movementTimer;
        private float _combatTimer;
        private float _attackTimer;
        private float _regenTimer;

        public MapPlayer(PlayerPacket playerPacket, BankData bankData)
        {
            _playerPacket = playerPacket;
            _mapInstance = null;

            Connected = true;
            PlayerID = playerPacket.PlayerID;
            Username = playerPacket.Username;

            Running = false;
            AttemptedMove = false;
            
            Dead = false;
            EnemyCharacterType = CharacterType.Player;
            EnemyCharacterID = -1;

            MessageShowing = false;
            MovementDisabled = false;
            SelectedOption = -1;
            ShopID = -1;
            WorkbenchID = -1;

            Banking = false;
            _bankData = bankData;
            BankUpdated = false;

            _movementTimer = 0f;
            _combatTimer = 0f;
            _attackTimer = 0f;
            _regenTimer = 0f;


            TradePlayerID = -1;
            _tradeRequest = null;
            Trading = false;
            _ignoreEvents = false;

        }

        public void SetPlayerPacket(PlayerPacket playerPacket)
        {
            _playerPacket = playerPacket;
        }

        public PlayerPacket GetPlayerPacket()
        {
            return _playerPacket;
        }

        public MapInstance GetMapInstance()
        {
            return _mapInstance;
        }

        public void SetMapInstance(MapInstance instance)
        {
            _mapInstance = instance;
        }

        public delegate void ChangeMapHandler();
        public event ChangeMapHandler OnChangeMap;

        public void SetMapID(int mapID)
        {
            if (GetPlayerPacket().MapID != mapID)
            {
                _playerPacket.MapID = mapID;
                _mapInstance.RemoveMapPlayer(this);
                OnChangeMap?.Invoke();
            }
        }

        public int GetMapID()
        {
            return _playerPacket.MapID;
        }

        public BankData GetBankData()
        {
            return _bankData;
        }

        public delegate void ShowMessageHandler(string message, string options);
        public event ShowMessageHandler OnShowMessage;

        public void ShowMessage(string message, string options = null)
        {
            MovementDisabled = true;
            MessageShowing = true;
            OnShowMessage?.Invoke(message, options);
        }



        public float GetMovementSpeed()
        {
            float speed = 64f;
            if (Running && _playerPacket.Data.Stamina > 0)
                speed *= 2.5f;
            return speed;
        }

        public Hitbox GetHitbox()
        {
            Hitbox hitbox = new Hitbox();

            hitbox.X = _playerPacket.RealX;
            hitbox.Y = _playerPacket.RealY;
            SpriteData data = SpriteData.GetSpriteData(_playerPacket.SpriteID);
            if (data != null)
            {
                if (_playerPacket.Direction == FacingDirection.Left || _playerPacket.Direction == FacingDirection.Right)
                {
                    hitbox.Width = data.HorizontalBounds.X;
                    hitbox.Height = data.HorizontalBounds.Y;
                }
                else
                {
                    hitbox.Width = data.VerticalBounds.X;
                    hitbox.Height = data.VerticalBounds.Y;
                }
            }

            return hitbox;
        }

        public Vector2 GetSourcePos()
        {
            return new Vector2(_playerPacket.PrevMapX * 32, _playerPacket.PrevMapY * 32);
        }

        public Vector2 GetTargetPos()
        {
            return new Vector2(_playerPacket.PositionX * 32, _playerPacket.PositionY * 32);
        }

        public float DistanceFromTarget()
        {
            Vector2 currentPos = new Vector2(_playerPacket.RealX, _playerPacket.RealY);
            Vector2 target = GetTargetPos();
            Vector2 dir = currentPos - target;
            return dir.Length;
        }

        public void AttemptMove(MovementDirection direction)
        {
            AttemptedMove = true;
            AttemptedDirection = direction;
        }

        public bool Moving()
        {
            float posX = _playerPacket.PositionX * 32;
            float posY = _playerPacket.PositionY * 32;
            return (posX != _playerPacket.RealX || posY != _playerPacket.RealY);
        }

        public bool CanMove()
        {
            if (_movementTimer > 0f) return false;
            if (MovementDisabled) return false;
            if (Moving()) return false;
            return true;
        }

        public Vector2 GetMapPosition()
        {
            return new Vector2(_playerPacket.PositionX, _playerPacket.PositionY);
        }

        public void SetMapPosition(int x, int y)
        {
            _playerPacket.PositionX = x;
            _playerPacket.PositionY = y;
            _playerPacket.RealX = x * 32;
            _playerPacket.RealY = y * 32;
        }

        public void Move(MovementDirection direction, bool ignoreEvents = false)
        {
            if (CanMove())
            {
                AttemptedMove = false;
                _ignoreEvents = ignoreEvents;

                _playerPacket.PrevMapX = _playerPacket.PositionX;
                _playerPacket.PrevMapY = _playerPacket.PositionY;
                int targetX = _playerPacket.PositionX;
                int targetY = _playerPacket.PositionY;
                FacingDirection facingDirection = FacingDirection.Down;
                MovementDirection entryDirection = MovementDirection.Down;

                switch (direction)
                {
                    case MovementDirection.UpperLeft:
                        targetX -= 1;
                        targetY -= 1;
                        facingDirection = FacingDirection.Left;
                        entryDirection = MovementDirection.LowerRight;
                        break;
                    case MovementDirection.Up:
                        targetY -= 1;
                        facingDirection = FacingDirection.Up;
                        entryDirection = MovementDirection.Down;
                        break;
                    case MovementDirection.UpperRight:
                        targetX += 1;
                        targetY -= 1;
                        facingDirection = FacingDirection.Right;
                        entryDirection = MovementDirection.LowerLeft;
                        break;
                    case MovementDirection.Left:
                        targetX -= 1;
                        facingDirection = FacingDirection.Left;
                        entryDirection = MovementDirection.Right;
                        break;
                    case MovementDirection.Right:
                        targetX += 1;
                        facingDirection = FacingDirection.Right;
                        entryDirection = MovementDirection.Left;
                        break;
                    case MovementDirection.LowerLeft:
                        targetX -= 1;
                        targetY += 1;
                        facingDirection = FacingDirection.Left;
                        entryDirection = MovementDirection.UpperRight;
                        break;
                    case MovementDirection.Down:
                        targetY += 1;
                        facingDirection = FacingDirection.Down;
                        entryDirection = MovementDirection.Up;
                        break;
                    case MovementDirection.LowerRight:
                        targetX += 1;
                        targetY += 1;
                        facingDirection = FacingDirection.Right;
                        entryDirection = MovementDirection.UpperLeft;
                        break;
                }
                if (_playerPacket.Direction != facingDirection)
                {
                    _playerPacket.Direction = facingDirection;
                    _movementTimer = 0.15f;
                }
                else
                {
                    bool bridgeEntry = _mapInstance.MapTilesetPassable(_playerPacket.PrevMapX, _playerPacket.PrevMapY) && _playerPacket.OnBridge;
                    if (_mapInstance.MapTileCharacterPassable(_playerPacket.PrevMapX, _playerPacket.PrevMapY, true, _playerPacket.OnBridge, bridgeEntry, direction) &&
                        _mapInstance.MapTileCharacterPassable(targetX, targetY, true, _playerPacket.OnBridge, bridgeEntry, entryDirection))
                    {
                        _playerPacket.PositionX = targetX;
                        _playerPacket.PositionY = targetY;
                        if (_mapInstance.GetBridgeFlag(targetX, targetY))
                        {
                            if (_mapInstance.MapTilesetPassable(targetX, targetY))
                                _playerPacket.OnBridge = true;
                        }
                    }
                    else
                    {
                        CheckEventTriggers(targetX, targetY, EventTriggerType.PlayerTouch);
                    }
                }
            }
        }

        public void ChangeDirection(FacingDirection direction)
        {
            _playerPacket.Direction = direction;
        }


        public void SelectItem(int itemIndex)
        {
            if (CanMove())
            {
                Tuple<int, int> itemInfo = GetPlayerPacket().Data.GetInventoryItem(itemIndex);
                if (itemInfo != null)
                {
                    ItemData data = ItemData.GetItemData(itemInfo.Item1);
                    switch (data.GetItemType())
                    {
                        case ItemData.ItemType.Tool:

                            break;
                        case ItemData.ItemType.Consumable:

                            break;
                        case ItemData.ItemType.Material:

                            break;
                        case ItemData.ItemType.Equipment:
                            GetPlayerPacket().Data.EquipItem(itemIndex);
                            break;
                        case ItemData.ItemType.Ammo:
                            GetPlayerPacket().Data.EquipAmmo(itemIndex);
                            break;
                    }
                }
            }
        }

        public void DropItem(int itemIndex, int count)
        {
            if (CanMove())
            {
                Tuple<int, int> itemInfo = GetPlayerPacket().Data.GetInventoryItem(itemIndex);

                if (itemInfo != null)
                {
                    if (count > itemInfo.Item2)
                        count = itemInfo.Item2;
                    MapItem mapItem = new MapItem(itemInfo.Item1, count, _playerPacket.PositionX, _playerPacket.PositionY, _playerPacket.PlayerID, _playerPacket.OnBridge);
                    GetPlayerPacket().Data.RemoveInventoryItemAt(itemIndex, count);
                    _mapInstance.AddMapItem(mapItem);
                }
            }
        }

        public void RecieveClientCommand(ClientCommand command)
        {
            int itemIndex, count, playerID;
            Tuple<int, int> itemInfo;
            MapItem mapItem;
            MapPlayer otherPlayer;

            switch (command.GetCommandType())
            {
                case ClientCommand.CommandType.CloseMessage:
                    MessageShowing = false;
                    break;
                case ClientCommand.CommandType.SelectOption:
                    int option = (int)command.GetParameter("Option");
                    SelectedOption = option;
                    MessageShowing = false;
                    break;
                case ClientCommand.CommandType.MovePlayer:
                    MovementDirection direction = (MovementDirection)command.GetParameter("Direction");
                    AttemptMove(direction);
                    break;
                case ClientCommand.CommandType.ToggleRunning:
                    bool running = (bool)command.GetParameter("Running");
                    Running = running;
                    break;
                case ClientCommand.CommandType.ActionTrigger:
                    ActionTrigger();
                    break;
                case ClientCommand.CommandType.SelectItem:
                    itemIndex = (int)command.GetParameter("ItemIndex");
                    SelectItem(itemIndex);
                    break;
                case ClientCommand.CommandType.DropItem:
                    if (CanMove())
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");
                        itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);

                        if (itemInfo != null)
                        {
                            if (count > itemInfo.Item2)
                                count = itemInfo.Item2;
                            mapItem = new MapItem(itemInfo.Item1, count, _playerPacket.PositionX, _playerPacket.PositionY, _playerPacket.PlayerID, _playerPacket.OnBridge);
                            _playerPacket.Data.RemoveInventoryItemAt(itemIndex, count);
                            _mapInstance.AddMapItem(mapItem);
                        }
                    }
                    break;
                case ClientCommand.CommandType.RemoveEquipment:
                    if (CanMove())
                    {
                        EquipmentSlot slot = (EquipmentSlot)command.GetParameter("EquipmentIndex");
                        _playerPacket.Data.UnequipItem(slot);
                    }
                    break;
                case ClientCommand.CommandType.RemoveAmmo:
                    if (CanMove())
                    {
                        _playerPacket.Data.UnequipAmmo();
                    }
                    break;
                case ClientCommand.CommandType.PickupItem:
                    if (CanMove())
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        int signature = (int)command.GetParameter("Signature");
                        mapItem = _mapInstance.GetMapItem(itemIndex);
                        if (mapItem != null)
                        {
                            if (!mapItem.PickedUp && (mapItem.PlayerID == -1 || mapItem.PlayerID == _playerPacket.PlayerID))
                            {
                                if (mapItem.GetSignature() == signature && mapItem.OnBridge == _playerPacket.OnBridge)
                                {
                                    int distance = (int)new Vector2(mapItem.MapX - _playerPacket.PositionX, mapItem.MapY - _playerPacket.PositionY).Length;
                                    if (distance <= 1)
                                    {
                                        int added = _playerPacket.Data.AddInventoryItem(mapItem.ItemID, mapItem.Count);
                                        if (added != 0)
                                        {
                                            if (added < mapItem.Count)
                                            {
                                                mapItem.Count = mapItem.Count - added;
                                                mapItem.Changed = true;
                                            }
                                            else
                                            {
                                                mapItem.PickedUp = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ClientCommand.CommandType.AttackPlayer:
                    if (_mapInstance.GetMapData().PvpEnabled == false)
                        break;

                    playerID = (int)command.GetParameter("PlayerID");
                    if (EnemyCanAttack(CharacterType.Player, playerID, _mapInstance.GetMapData().MultiCombat))
                    {
                        otherPlayer = _mapInstance.FindMapPlayer(playerID);
                        if (otherPlayer != null && otherPlayer.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, _mapInstance.GetMapData().MultiCombat))
                        {
                            int pX = _playerPacket.PositionX;
                            int pY = _playerPacket.PositionY;
                            EnemyCharacterType = CharacterType.Player;
                            EnemyCharacterID = playerID;
                            if (otherPlayer.GetPlayerPacket().PositionX < pX && otherPlayer.GetPlayerPacket().PositionY == pY)
                            {
                                ChangeDirection(FacingDirection.Left);
                                CombatCheck(pX - 1, pY);
                            }
                            else if (otherPlayer.GetPlayerPacket().PositionX > pX && otherPlayer.GetPlayerPacket().PositionY == pY)
                            {
                                ChangeDirection(FacingDirection.Right);
                                CombatCheck(pX + 1, pY);
                            }
                            else if (otherPlayer.GetPlayerPacket().PositionX == pX && otherPlayer.GetPlayerPacket().PositionY < pY)
                            {
                                ChangeDirection(FacingDirection.Up);
                                CombatCheck(pX, pY - 1);
                            }
                            else if (otherPlayer.GetPlayerPacket().PositionX == pX && otherPlayer.GetPlayerPacket().PositionY > pY)
                            {
                                ChangeDirection(FacingDirection.Down);
                                CombatCheck(pX, pY + 1);
                            }
                        }

                    }
                    break;
                case ClientCommand.CommandType.AttackEnemy:
                    int enemyID = (int)command.GetParameter("EnemyID");
                    if (EnemyCanAttack(CharacterType.Enemy, enemyID, _mapInstance.GetMapData().MultiCombat))
                    {
                        MapEnemy otherEnemy = _mapInstance.FindMapEnemy(enemyID);
                        bool multiCombat = _mapInstance.GetMapData().MultiCombat;
                        if (otherEnemy != null && otherEnemy.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat))
                        {
                            EnemyCharacterType = CharacterType.Enemy;
                            EnemyCharacterID = enemyID;

                            int pX = _playerPacket.PositionX;
                            int pY = _playerPacket.PositionY;
                            if (otherEnemy.MapX < pX && otherEnemy.MapY == pY)
                            {
                                ChangeDirection(FacingDirection.Left);
                                CombatCheck(pX - 1, pY);
                            }
                            else if (otherEnemy.MapX > pX && otherEnemy.MapY == pY)
                            {
                                ChangeDirection(FacingDirection.Right);
                                CombatCheck(pX + 1, pY);
                            }
                            else if (otherEnemy.MapX == pX && otherEnemy.MapY < pY)
                            {
                                ChangeDirection(FacingDirection.Up);
                                CombatCheck(pX, pY - 1);
                            }
                            else if (otherEnemy.MapX == pX && otherEnemy.MapY > pY)
                            {
                                ChangeDirection(FacingDirection.Down);
                                CombatCheck(pX, pY + 1);
                            }
                        }

                    }
                    break;
                case ClientCommand.CommandType.CloseShop:

                    this.CloseShop();

                    break;
                case ClientCommand.CommandType.BuyShopItem:
                    if (ShopID != -1)
                    {
                        ShopData data = ShopData.GetData(ShopID);
                        if (data != null)
                        {
                            itemIndex = (int)command.GetParameter("ItemIndex");
                            count = (int)command.GetParameter("Count");
                            if (itemIndex >= 0 && itemIndex < data.ShopItems.Count && count > 0)
                            {
                                ShopData.ShopItem shopItem = data.ShopItems[itemIndex];
                                if (shopItem.ItemID != -1)
                                {
                                    int totalCost = shopItem.Cost * count;
                                    if (_playerPacket.Data.Gold >= totalCost)
                                    {
                                        _playerPacket.Data.Gold -= totalCost;
                                        int added = _playerPacket.Data.AddInventoryItem(shopItem.ItemID, count);
                                        int remainder = count - added;
                                        for (int j = 0; j < remainder; j++)
                                        {
                                            _playerPacket.Data.Gold += shopItem.Cost;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
                case ClientCommand.CommandType.SellShopItem:
                    itemIndex = (int)command.GetParameter("ItemIndex");
                    count = (int)command.GetParameter("Count");

                    itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);
                    if (itemInfo != null && count > 0)
                    {
                        ItemData itemData = ItemData.GetItemData(itemInfo.Item1);
                        if (itemData.Sellable)
                        {
                            if (count > itemInfo.Item2)
                            {
                                count = itemInfo.Item2;
                            }
                            _playerPacket.Data.RemoveInventoryItemAt(itemIndex, count);
                            int sellPrice = count * itemData.SellPrice;
                            _playerPacket.Data.Gold += sellPrice;
                        }
                        else
                        {
                            OnSendMessage?.Invoke("Item cannot be sold.");
                        }
                    }

                    break;
                case ClientCommand.CommandType.TradeRequest:
                    playerID = (int)command.GetParameter("PlayerID");
                    otherPlayer = _mapInstance.FindMapPlayer(playerID);
                    if (otherPlayer != null)
                    {
                        Vector2 pos1 = GetMapPosition();
                        Vector2 pos2 = otherPlayer.GetMapPosition();
                        if ((pos1 - pos2).Length <= 1)
                        {
                            TradePlayerID = playerID;
                            if (otherPlayer.TradePlayerID != _playerPacket.PlayerID)
                                RequestTrade(otherPlayer);
                            else
                                StartTrade(otherPlayer);
                        }
                    }

                    break;
                case ClientCommand.CommandType.AcceptTrade:
                    if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                    {
                        playerID = _tradeRequest.TradeOffer2.PlayerID;
                        _tradeRequest.TradeOffer1.Accepted = true;
                    }
                    else
                    {
                        playerID = _tradeRequest.TradeOffer1.PlayerID;
                        _tradeRequest.TradeOffer2.Accepted = true;
                    }

                    AcceptTrade(playerID);

                    break;
                case ClientCommand.CommandType.CancelTrade:
                    StopTrading();

                    break;
                case ClientCommand.CommandType.AddTradeItem:
                    if (Trading)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");
                        AddTradeItem(itemIndex, count);
                    }

                    break;
                case ClientCommand.CommandType.RemoveTradeItem:
                    if (Trading)
                    {
                        itemIndex = (int)command.GetParameter("ItemIndex");
                        count = (int)command.GetParameter("Count");
                        RemoveTradeItem(itemIndex, count);
                    }

                    break;
                case ClientCommand.CommandType.CloseBank:

                    StopBanking();

                    break;
                case ClientCommand.CommandType.AddBankItem:
                    itemIndex = (int)command.GetParameter("ItemIndex");
                    count = (int)command.GetParameter("Count");
                    AddBankItem(itemIndex, count);

                    break;
                case ClientCommand.CommandType.RemoveBankItem:
                    itemIndex = (int)command.GetParameter("ItemIndex");
                    count = (int)command.GetParameter("Count");
                    RemoveBankItem(itemIndex, count);

                    break;
                case ClientCommand.CommandType.CloseWorkbench:

                    CloseWorkbench();

                    break;
                case ClientCommand.CommandType.CraftItem:
                    int craftID = (int)command.GetParameter("CraftID");
                    count = (int)command.GetParameter("Count");

                    CraftItem(craftID, count);

                    break;

            }
        }

        public delegate void SendMessageHandler(string message);
        public event SendMessageHandler OnSendMessage;

        public delegate void TradeRequestHandler(int otherID);
        public event TradeRequestHandler OnRequestTrade, OnStartTrade, OnAcceptTrade, OnEndTrade;

        public delegate void CantTradeRequestHandler(int otherID, int freeSlots);
        public event CantTradeRequestHandler OnCantTrade;

        public delegate void UpdateTradeHandler(int otherID, int itemID, int count);
        public event UpdateTradeHandler OnAddTradeItem, OnRemoveTradeItem;

        private void RequestTrade(MapPlayer other)
        {
            OnRequestTrade?.Invoke(other.PlayerID);
        }

        private void StartTrade(MapPlayer other)
        {
            if (CanOpenInterface() && other.CanOpenInterface())
            {
                TradeRequest tradeRequest = new TradeRequest(other._playerPacket.PlayerID, _playerPacket.PlayerID);
                tradeRequest.TradeOffer1.FreeSlots = other._playerPacket.Data.GetFreeInventorySlots();
                tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

                _tradeRequest = tradeRequest;
                other._tradeRequest = tradeRequest;
                Trading = true;
                other.Trading = true;
                this.MovementDisabled = true;
                other.MovementDisabled = true;

                OnStartTrade?.Invoke(other.PlayerID);
            }
        }

        private void AcceptTrade(int playerID)
        {
            OnAcceptTrade?.Invoke(playerID);
        }

        public void StopTrading()
        {
            if (Trading)
            {
                TradeRequest.TradeOffer myOffer, othersOffer;
                if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                {
                    myOffer = _tradeRequest.TradeOffer1;
                    othersOffer = _tradeRequest.TradeOffer2;
                }
                else
                {
                    myOffer = _tradeRequest.TradeOffer2;
                    othersOffer = _tradeRequest.TradeOffer1;
                }

                bool accepted = _tradeRequest.Accepted();
                EndTrade(myOffer, othersOffer, accepted);
            }
        }

        private void EndTrade(TradeRequest.TradeOffer myOffer, TradeRequest.TradeOffer othersOffer, bool accepted)
        {
            if (accepted)
            {
                if (myOffer.FreeSlots >= othersOffer.NumItems() && othersOffer.FreeSlots >= myOffer.NumItems())
                {
                    for (int i = 0; i < othersOffer.NumItems(); i++)
                    {
                        Tuple<int, int> itemInfo = othersOffer.GetItem(i);
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, itemInfo.Item2);
                    }
                }
                else
                {
                    if (myOffer.FreeSlots < othersOffer.NumItems())
                    {
                        myOffer.Accepted = false;
                        othersOffer.Accepted = false;
                        OnCantTrade?.Invoke(othersOffer.PlayerID, myOffer.FreeSlots);
                    }
                    return;
                }
            }
            else
            {
                for (int i = 0; i < myOffer.NumItems(); i++)
                {
                    Tuple<int, int> itemInfo = myOffer.GetItem(i);
                    _playerPacket.Data.AddInventoryItem(itemInfo.Item1, itemInfo.Item2);
                }
            }

            OnEndTrade?.Invoke(-1);
            this.TradePlayerID = -1;
            _tradeRequest = null;
            Trading = false;
            MovementDisabled = false;
        }

        private void AddTradeItem(int itemIndex, int count)
        {
            Tuple<int, int> itemInfo = _playerPacket.Data.GetInventoryItem(itemIndex);
            if (itemInfo != null)
            {
                if (count > itemInfo.Item2)
                    count = itemInfo.Item2;

                MapPlayer other;
                int added = 0;

                if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                {
                    added = _tradeRequest.TradeOffer1.AddItem(itemInfo.Item1, count);
                    _playerPacket.Data.RemoveInventoryItemAt(itemIndex, added);
                    _tradeRequest.TradeOffer1.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

                    other = _mapInstance.FindMapPlayer(_tradeRequest.TradeOffer2.PlayerID);
                }
                else
                {
                    added = _tradeRequest.TradeOffer2.AddItem(itemInfo.Item1, count);
                    _playerPacket.Data.RemoveInventoryItemAt(itemIndex, added);
                    _tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();

                    other = _mapInstance.FindMapPlayer(_tradeRequest.TradeOffer1.PlayerID);
                }

                if (other != null && added > 0)
                {
                    OnAddTradeItem?.Invoke(other.PlayerID, itemInfo.Item1, added);
                    _tradeRequest.TradeOffer1.Accepted = false;
                    _tradeRequest.TradeOffer2.Accepted = false;
                }
            }
        }

        private void RemoveTradeItem(int itemIndex, int count)
        {
            Tuple<int, int> itemInfo = null;
            MapPlayer other = null;
            int removed = 0;

            if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
            {
                itemInfo = _tradeRequest.TradeOffer1.GetItem(itemIndex);
                if (itemInfo != null)
                {
                    removed = _tradeRequest.TradeOffer1.RemoveItem(itemIndex, count);
                    if (removed > 0)
                    {
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, removed);
                        _tradeRequest.TradeOffer1.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();
                        other = _mapInstance.FindMapPlayer(_tradeRequest.TradeOffer2.PlayerID);
                        _tradeRequest.TradeOffer1.Accepted = false;
                        _tradeRequest.TradeOffer2.Accepted = false;
                    }
                }

            }
            else
            {
                itemInfo = _tradeRequest.TradeOffer2.GetItem(itemIndex);
                if (itemInfo != null)
                {
                    removed = _tradeRequest.TradeOffer2.RemoveItem(itemIndex, count);
                    if (removed > 0)
                    {
                        _playerPacket.Data.AddInventoryItem(itemInfo.Item1, removed);
                        _tradeRequest.TradeOffer2.FreeSlots = _playerPacket.Data.GetFreeInventorySlots();
                        other = _mapInstance.FindMapPlayer(_tradeRequest.TradeOffer1.PlayerID);
                        _tradeRequest.TradeOffer1.Accepted = false;
                        _tradeRequest.TradeOffer2.Accepted = false;
                    }
                }
            }

            if (other != null && removed > 0)
            {
                OnRemoveTradeItem?.Invoke(other.PlayerID, itemInfo.Item1, removed);
            }
        }

        public delegate void ShopShopHandler(int shopID);
        public event ShopShopHandler OnShowShop;

        public void ShowShop(int shopID)
        {
            if (CanOpenInterface())
            {
                if (shopID > -1 && shopID < ShopData.DataCount())
                {
                    ShopID = shopID;
                    MovementDisabled = true;
                    OnShowShop?.Invoke(shopID);
                }
            }
        }

        public void CloseShop()
        {
            if (ShopID != -1)
            {
                ShopID = -1;
                MovementDisabled = false;
            }
        }

        public delegate void BankingHandler();
        public event BankingHandler OnStartBanking;

        public void StartBanking()
        {
            if (CanOpenInterface())
            {
                Banking = true;
                MovementDisabled = true;
                BankUpdated = true;
                OnStartBanking?.Invoke();
                
            }
        }

        public void StopBanking()
        {
            if (Banking)
            {
                Banking = false;
                MovementDisabled = false;
            }
        }

        public void AddBankItem(int index, int count)
        {
            if (Banking)
            {
                Tuple<int, int> itemInfo = _playerPacket.Data.GetInventoryItem(index);
                if (itemInfo != null)
                {
                    if (count >= itemInfo.Item2)
                    {
                        count = itemInfo.Item2;
                        _playerPacket.Data.RemoveInventoryItem(index);
                    }
                    else
                    {
                        _playerPacket.Data.RemoveInventoryItemAt(index, count);
                    }
                    _bankData.AddBankItem(itemInfo.Item1, count);
                    BankUpdated = true;
                }
            }
        }

        public void RemoveBankItem(int index, int count)
        {
            if (Banking)
            {
                _bankData.RemoveBankItem(index, count, _playerPacket.Data);
                BankUpdated = true;
            }
        }

        public delegate void ShowWorkbenchHandler(int workbenchID);
        public event ShowWorkbenchHandler OnShowWorkbench;

        public void ShowWorkbench(int workbenchID)
        {
            if (CanOpenInterface())
            {
                if (workbenchID > -1 && workbenchID < CraftableData.GetWorkbenchDataCount())
                {
                    WorkbenchID = workbenchID;
                    MovementDisabled = true;
                    OnShowWorkbench?.Invoke(workbenchID);
                }
            }
        }

        public void CloseWorkbench()
        {
            if (WorkbenchID != -1)
            {
                WorkbenchID = -1;
                MovementDisabled = false;
            }
        }

        public void CraftItem(int craftID, int count)
        {
            if (WorkbenchID > -1 && count > 0)
            {
                CraftableData data = CraftableData.GetCraftableData(craftID);
                if (data != null && data.WorkbenchID == WorkbenchID)
                {
                    bool hasMaterials = true;

                    for (int i = 0; i < count; i++)
                    {

                        for (int j = 0; j < data.Materials.Count; j++)
                        {
                            Tuple<int, int> material = data.Materials[j];
                            if (material.Item1 < 0 || material.Item2 < 1)
                                continue;

                            if (!_playerPacket.Data.ItemInInventory(material.Item1, material.Item2))
                            {
                                hasMaterials = false;
                                break;
                            }
                        }

                        if (hasMaterials)
                        {
                            if (_playerPacket.Data.SpaceInInventory(data.CraftedItemID, data.CraftedItemCount))
                            {
                                _playerPacket.Data.AddInventoryItem(data.CraftedItemID, data.CraftedItemCount);
                                for (int j = 0; j < data.Materials.Count; j++)
                                {
                                    Tuple<int, int> material = data.Materials[j];
                                    _playerPacket.Data.RemoveInventoryItem(material.Item1, material.Item2);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool CanOpenInterface()
        {
            if (this.WorkbenchID == -1 && this.ShopID == -1 && !Banking && !Trading)
            {
                return true;
            }
            return false;
        }

        public void CloseInterface()
        {
            if (this.WorkbenchID != -1)
            {
                CloseWorkbench();
            }
            if (this.ShopID != -1)
            {
                this.CloseShop();
            }
            if (Trading)
            {
                StopTrading();
            }
            if (Banking)
            {
                StopBanking();
            }
        }

        public void ActionTrigger()
        {
            if (CanMove())
            {
                int targetX = _playerPacket.PositionX;
                int targetY = _playerPacket.PositionY;
                switch (_playerPacket.Direction)
                {
                    case FacingDirection.Down:
                        targetY += 1;
                        break;
                    case FacingDirection.Left:
                        targetX -= 1;
                        break;
                    case FacingDirection.Right:
                        targetX += 1;
                        break;
                    case FacingDirection.Up:
                        targetY -= 1;
                        break;
                }

                for (int i = 0; i < 3; i++)
                {
                    Tuple<int, int> tileInfo = _mapInstance.GetMapData().GetTile(i, targetX, targetY);
                    if (tileInfo.Item2 == -1)
                        continue;
                    if (TilesetData.GetTileset(tileInfo.Item2).GetCounterFlag(tileInfo.Item1))
                    {
                        targetX += _playerPacket.Direction == FacingDirection.Left ? -1 : _playerPacket.Direction == FacingDirection.Right ? 1 : 0;
                        targetY += _playerPacket.Direction == FacingDirection.Down ? 1 : _playerPacket.Direction == FacingDirection.Up ? -1 : 0;
                        break;
                    }
                }

                if (!CheckEventTriggers(targetX, targetY, EventTriggerType.Action))
                {
                    if (_mapInstance.GetMapData().MultiCombat)
                        EnemyCharacterID = -1;
                    CombatCheck(targetX, targetY);
                }
            }
        }

        public delegate void EventTriggerHandler(MapPlayer mapPlayer, MapEvent mapEvent);
        public event EventTriggerHandler OnEventTrigger;

        private bool CheckEventTriggers(int x, int y, EventTriggerType triggerType)
        {
            if (_ignoreEvents)
                return false;

            MapData mapData = _mapInstance.GetMapData();
            for (int i = 0; i < mapData.MapEventsCount(); i++)
            {
                MapEvent mapEvent = mapData.GetMapEvent(i);
                if (!mapEvent.Enabled) continue;
                if (mapEvent.MapX == x && mapEvent.MapY == y)
                {
                    if (mapEvent.EventDataID == -1)
                        return false;
                    if (mapEvent.TriggerType == triggerType)
                    {
                        OnEventTrigger?.Invoke(this, mapEvent);
                        return true;
                    }
                }
            }
            return false;
        }

        public void GainExperience(int experience)
        {
            if (experience <= 0)
                return;

            if (_playerPacket.Data.Level < SystemData.GetData().MaxLvl)
            {
                int xpNeeded = _playerPacket.Data.ExperienceToLevel();
                int xpCurrent = _playerPacket.Data.Experience;
                int addedXp = xpCurrent + experience;

                if (addedXp < xpNeeded)
                {
                    _playerPacket.Data.Experience = addedXp;
                }
                else
                {
                    int remainder = addedXp - xpNeeded;
                    _playerPacket.Data.Experience = 0;
                    _playerPacket.Data.Level++;
                    GainExperience(remainder);
                }
            }
        }



        private void CombatCheck(int x, int y)
        {
            if (_attackTimer <= 0)
            {
                int weaponID = _playerPacket.Data.GetEquipedItemID((int)EquipmentSlot.Weapon);
                if (weaponID != -1)
                {
                    ItemData data = ItemData.GetItemData(weaponID);
                    bool multiCombat = _mapInstance.GetMapData().MultiCombat;

                    switch ((AttackStyle)data.GetItemStat("AttackStyle"))
                    {
                        case AttackStyle.Melee:

                            bool attacked = false;

                            if (EnemyCharacterID == -1 || EnemyCharacterType == CharacterType.Player)
                            {
                                if (_mapInstance.GetMapData().PvpEnabled)
                                {
                                    List<MapPlayer> players = _mapInstance.GetMapPlayers();
                                    for (int i = 0; i < players.Count; i++)
                                    {
                                        MapPlayer mapPlayer = players[i];
                                        if (EnemyCharacterID == -1 || EnemyCharacterID == mapPlayer.GetPlayerPacket().PlayerID)
                                        {
                                            if (mapPlayer.GetPlayerPacket().PositionX == x && mapPlayer.GetPlayerPacket().PositionY == y)
                                            {
                                                if (mapPlayer.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat)
                                                    && EnemyCanAttack(CharacterType.Player, mapPlayer.PlayerID, multiCombat))
                                                {
                                                    MeleeTrigger(mapPlayer);
                                                    attacked = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!attacked)
                            {
                                if (EnemyCharacterID == -1 || EnemyCharacterType == CharacterType.Enemy)
                                {
                                    for (int i = 0; i < _mapInstance.GetMapPacket().Enemies.Count; i++)
                                    {
                                        if (EnemyCharacterID == -1 || EnemyCharacterID == i)
                                        {
                                            MapEnemy other = _mapInstance.GetMapPacket().Enemies[i];
                                            if (other.MapX == x && other.MapY == y)
                                            {
                                                if (other.EnemyCanAttack(CharacterType.Player, _playerPacket.PlayerID, multiCombat)
                                                    && EnemyCanAttack(CharacterType.Enemy, i, multiCombat))
                                                {
                                                    MeleeTrigger(other);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        case AttackStyle.Ranged:

                            RangeTrigger();

                            break;
                        case AttackStyle.Magic:

                            int mpDrain = (int)data.GetItemStat("MP");
                            if (mpDrain <= _playerPacket.Data.MP)
                            {
                                _playerPacket.Data.MP -= mpDrain;
                                int projectileID = (int)data.GetItemStat("ProjectileID");
                                MagicTrigger(projectileID);
                            }

                            break;
                    }
                }
            }
        }

        private void MeleeTrigger(MapPlayer other)
        {
            Random rand = new Random();
            CombatStats stats1 = _playerPacket.Data.GetCombinedCombatStats();
            CombatStats stats2 = other._playerPacket.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            other.TakeDamage(CharacterType.Player, _playerPacket.PlayerID, meleePower, _mapInstance.GetMapData().MultiCombat);
            _attackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;
        }

        private void MeleeTrigger(MapEnemy other)
        {
            Random rand = new Random();
            CombatStats stats1 = _playerPacket.Data.GetCombinedCombatStats();
            CombatStats stats2 = other.GetEnemyData().BaseStats;
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats1.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats1.Agility / stats1.Strength, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int meleePower = (int)((stats1.Strength + critModifier) * accuracy) - (stats2.MeleeDefence / 2);
            meleePower = Math.Max(meleePower, 0);
            other.TakeDamage(CharacterType.Player, _playerPacket.PlayerID, meleePower, _mapInstance.GetMapData().MultiCombat);
            _attackTimer = Math.Max((1 / stats1.Agility) - 1.0f, 0.1f) * 10;

            if (other.Dead)
            {
                GainExperience(other.GetEnemyData().Experience);
                _mapInstance.DropItem(other, this);
            }
        }

        private void RangeTrigger()
        {
            int projectileID = _playerPacket.Data.ConsumeAmmo();
            if (projectileID != -1)
            {
                Hitbox hitbox = GetHitbox();
                Vector2 position = new Vector2(hitbox.X, hitbox.Y);
                MapProjectile projectile = new MapProjectile(projectileID, CharacterType.Player, _playerPacket.PlayerID, position, _playerPacket.Direction);
                projectile.OnBridge = _playerPacket.OnBridge;
                projectile.Style = AttackStyle.Ranged;
                projectile.TargetType = EnemyCharacterType;
                projectile.TargetID = EnemyCharacterID;

                Random rand = new Random();
                CombatStats stats = _playerPacket.Data.GetCombinedCombatStats();
                int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
                double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Strength, 1.0);
                double accuracy = rand.NextDouble() * maxAccuracy;
                int rangePower = (int)((stats.Strength + critModifier) * accuracy);
                projectile.AttackPower = rangePower;
                //need to add custom player targeting here later

                _mapInstance.AddMapProjectile(projectile);
                _attackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
            }
        }

        private void MagicTrigger(int projectileID)
        {
            Hitbox hitbox = GetHitbox();
            Vector2 position = new Vector2(hitbox.X, hitbox.Y);
            MapProjectile projectile = new MapProjectile(projectileID, CharacterType.Player, _playerPacket.PlayerID, position, _playerPacket.Direction);
            projectile.OnBridge = _playerPacket.OnBridge;
            projectile.Style = AttackStyle.Magic;
            projectile.TargetType = EnemyCharacterType;
            projectile.TargetID = EnemyCharacterID;

            Random rand = new Random();
            CombatStats stats = _playerPacket.Data.GetCombinedCombatStats();
            int critModifier = rand.Next(1, 6) > 4 ? (int)(stats.Strength * 0.2f) : 0;
            double maxAccuracy = 1;// Math.Min(stats.Agility / stats.Inteligence, 1.0);
            double accuracy = rand.NextDouble() * maxAccuracy;
            int magicPower = (int)((stats.Inteligence + critModifier) * accuracy);
            projectile.AttackPower = magicPower;
            //need to add custom player targeting here later

            _mapInstance.AddMapProjectile(projectile);
            _attackTimer = Math.Max((1 / stats.Agility) - 1.0f, 0.1f) * 10;
        }

        public void TakeDamage(CharacterType enemyType, int enemyID, int damage, bool multiCombat)
        {
            if (EnemyCanAttack(enemyType, enemyID, multiCombat))
            {
                if (enemyID != -1 && !_mapInstance.GetMapData().MultiCombat)
                {
                    EnemyCharacterType = enemyType;
                    EnemyCharacterID = enemyID;
                    _combatTimer = 5f;
                }

                _playerPacket.Data.HP -= damage;

                if (_playerPacket.Data.HP <= 0)
                {
                    Dead = true;
                    _playerPacket.Data.HP = 0;
                }
            }
        }

        public bool EnemyCanAttack(CharacterType enemyType, int enemyID, bool multiCombat)
        {
            if (Dead)
                return false;
            if (EnemyCharacterID == -1)
                return true;
            if (enemyID == -1)
                return true;
            if (multiCombat)
                return true;
            if (EnemyCharacterType == enemyType && EnemyCharacterID == enemyID)
                return true;
            return false;
        }


        public bool TerrainTagCheck(int tag)
        {
            return _mapInstance.TerrainTagCheck(_playerPacket.PositionX, _playerPacket.PositionY, tag);
        }

        public void Update(float deltaTime)
        {
            //trading
            if (Trading)
            {
                if (_tradeRequest.Accepted())
                {
                    StopTrading();
                }
                else
                {
                    //check for disconnects
                    int playerID;
                    if (_tradeRequest.TradeOffer1.PlayerID == _playerPacket.PlayerID)
                        playerID = _tradeRequest.TradeOffer2.PlayerID;
                    else
                        playerID = _tradeRequest.TradeOffer1.PlayerID;
                    MapPlayer other = _mapInstance.FindMapPlayer(playerID);
                    if (other == null || !other.Trading || !other.Connected || !Connected)
                    {
                        this.StopTrading();
                    }
                }
            }

            //respawn
            if (Dead)
            {
                SpawnPoint spawn = MapInfo.GetSpawnPoint(0);
                MapInstance prevInstance = _mapInstance;
                SetMapID(spawn.MapID);
                SetMapPosition(spawn.MapX, spawn.MapY);
                _playerPacket.Data.HP = _playerPacket.Data.GetMaxHP();
                Dead = false;
                return;
            }

            //update timers
            if (_movementTimer > 0)
                _movementTimer -= deltaTime;

            if (_combatTimer > 0) _combatTimer -= deltaTime;
            else EnemyCharacterID = -1;

            if (_attackTimer > 0)
                _attackTimer -= deltaTime;

            //regen stats
            if (_regenTimer <= 0)
            {
                if (_combatTimer <= 0)
                {
                    _playerPacket.Data.HP++;
                    if (_playerPacket.Data.HP > _playerPacket.Data.GetMaxHP())
                        _playerPacket.Data.HP = _playerPacket.Data.GetMaxHP();

                    _playerPacket.Data.MP++;
                    if (_playerPacket.Data.MP > _playerPacket.Data.GetMaxMP())
                        _playerPacket.Data.MP = _playerPacket.Data.GetMaxMP();
                }

                if (!Running || !Moving())
                {
                    _playerPacket.Data.Stamina++;
                    if (_playerPacket.Data.Stamina > _playerPacket.Data.GetMaxStamina())
                        _playerPacket.Data.Stamina = _playerPacket.Data.GetMaxStamina();
                }
                else
                {
                    if (Moving())
                    {
                        _playerPacket.Data.Stamina--;
                        if (_playerPacket.Data.Stamina < 0)
                            _playerPacket.Data.Stamina = 0;
                    }
                }

                _regenTimer = 1;
            }
            else
            {
                _regenTimer -= deltaTime;
            }

            //movement
            if (!MovementDisabled)
            {
                _playerPacket.MovementSpeed = GetMovementSpeed();

                if (AttemptedMove)
                {
                    Move(AttemptedDirection);
                }

                if (UpdateMovement(deltaTime))
                {

                    if (!Moving())
                    {
                        CheckEventTriggers(_playerPacket.PositionX, _playerPacket.PositionY, EventTriggerType.PlayerTouch);

                        if (!_mapInstance.GetBridgeFlag(_playerPacket.PositionX, _playerPacket.PositionY))
                        {
                            _playerPacket.OnBridge = false;
                        }
                        _ignoreEvents = false;

                        if (AttemptedMove)
                        {
                            Move(AttemptedDirection);
                        }

                    }
                }
            }
        }

        public bool UpdateMovement(float deltaTime)
        {
            if (Moving())
            {
                Vector2 prevPos = GetSourcePos();
                Vector2 realPos = new Vector2(_playerPacket.RealX, _playerPacket.RealY);
                Vector2 targetPos = GetTargetPos();

                Vector2 dir = targetPos - prevPos;
                Vector2 endPos = realPos + (dir.Normalized() * (_playerPacket.MovementSpeed * deltaTime));

                Vector2 dir2 = endPos - prevPos;

                if (dir2.Length > dir.Length)
                {
                    realPos = targetPos;
                }
                else
                {
                    realPos = endPos;
                }

                _playerPacket.RealX = realPos.X;
                _playerPacket.RealY = realPos.Y;

                return true;
            }

            return false;
        }


    }
}

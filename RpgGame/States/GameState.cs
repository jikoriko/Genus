using System;

using Genus2D.Core;
using Genus2D.Entities;

using RpgGame.EntityComponents;
using Genus2D.Graphics;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using Genus2D.Networking;

using RpgGame.GUI;
using Genus2D.GameData;
using Genus2D;

using System.Linq;

namespace RpgGame.States
{
    public class GameState : State
    {

        public static GameState Instance { get; private set; }
        private RpgClientConnection _connection;

        private MessagePanel _messagePanel;
        private MenuPanel _menuPanel;

        public Entity MapEntity;
        private float _movementTimer;

        public GameState()
            : base()
        {
            Instance = this;
            MapEntity = Entity.CreateInstance(_entityManager);
            new MapComponent(MapEntity);

            _connection = null;

            _messagePanel = new MessagePanel(this);
            _menuPanel = new MenuPanel(this);

            _movementTimer = 0.0f;

            this.AddControl(_messagePanel);
            this.AddControl(_menuPanel);

            ClickBox mapClickBox = new ClickBox(0, 0, Renderer.GetResoultion().X, Renderer.GetResoultion().Y, MouseButton.Right);
            mapClickBox.OnTrigger += MapClick;
            this.AddMouseListener(mapClickBox);
        }

        private void MapClick()
        {
            if (!GuiSelectable())
            {
                Vector2 mousePos = StateWindow.Instance.GetMousePosition();
                int tileX = (int)Math.Floor((mousePos.X - MapEntity.GetTransform().Position.X) / 32);
                int tileY = (int)Math.Floor((mousePos.Y - MapEntity.GetTransform().Position.Y) / 32);
                List<MapClickOption> options = new List<MapClickOption>();

                for (int i = 0; i < RpgClientConnection.Instance.MapItemEntities.Count; i++)
                {
                    MapItem item = RpgClientConnection.Instance.MapItemEntities[i].FindComponent<MapItemComponent>().GetMapItem();
                    if (item.PlayerID == -1 || item.PlayerID == RpgClientConnection.Instance.GetLocalPlayerPacket().PlayerID)
                    {
                        if (item.MapX == tileX && item.MapY == tileY)
                        {
                            string label = "Pickup: " + ItemData.GetItemData(item.ItemID).Name + "(" + item.Count + ")";
                            MapClickOption option = new MapClickOption(MapClickOption.OptionType.PickupItem, label);
                            option.Parameters["MapItem"] = item;
                            option.Parameters["ItemIndex"] = i;
                            options.Add(option);
                        }
                    }
                }

                for (int i = 0; i < RpgClientConnection.Instance.PlayerEntities.Count; i++)
                {
                    int playerID = RpgClientConnection.Instance.PlayerEntities.ElementAt(i).Key;
                    if (playerID != RpgClientConnection.Instance.GetLocalPlayerPacket().PlayerID)
                    {

                        PlayerPacket packet = RpgClientConnection.Instance.PlayerEntities[playerID].FindComponent<PlayerComponent>().GetPlayerPacket();
                        if (packet.PositionX == tileX && packet.PositionY == tileY)
                        {
                            string label = "Attack Player: " + packet.Username;
                            MapClickOption option = new MapClickOption(MapClickOption.OptionType.AttackPlayer, label);
                            option.Parameters["PlayerID"] = packet.PlayerID;
                            options.Add(option);

                            label = "Trade Player: " + packet.Username;
                            option = new MapClickOption(MapClickOption.OptionType.Trade, label);
                            option.Parameters["PlayerID"] = packet.PlayerID;
                            options.Add(option);
                        }
                    }
                }

                for (int i = 0; i < RpgClientConnection.Instance.EnemyEntites.Count; i++)
                {
                    MapEnemy mapEnemy = RpgClientConnection.Instance.EnemyEntites[i].FindComponent<MapEnemyComponent>().GetMapEnemy();
                    if (mapEnemy.MapX == tileX && mapEnemy.MapY == tileY)
                    {
                        string label = "Attack Enemy: " + mapEnemy.GetEnemyData().Name;
                        MapClickOption option = new MapClickOption(MapClickOption.OptionType.AttackEnemy, label);
                        option.Parameters["EnemyID"] = i;
                        //perhaps we need to add some sort of signature to the map enemies, if the targeted enemy dies during target attack this index might target a differnt enemy
                        options.Add(option);
                    }
                }

                if (options.Count > 0)
                {
                    MapClickOptionsPanel panel = new MapClickOptionsPanel((int)mousePos.X, (int)mousePos.Y, options, this);
                    this.AddControl(panel);
                }
            }
            
        }

        private bool CanToggleMenu()
        {
            if (TradePanel.Instance != null || BankPanel.Instance != null || 
                ShopPanel.Instance != null || WorkbenchPanel.Instance != null)
                return false;

            return true;
        }

        public void ToggleInventory()
        {
            if (!CanToggleMenu())
                return;

            if (EquipmentPanel.Instance != null)
                EquipmentPanel.Instance.Close();
            if (StatsPanel.Instance != null)
                StatsPanel.Instance.Close();

            if (InventoryPanel.Instance != null)
                InventoryPanel.Instance.Close();
            else
                this.AddControl(new InventoryPanel(this));
        }

        public void ToggleEquipmentPanel()
        {
            if (!CanToggleMenu())
                return;

            if (InventoryPanel.Instance != null)
                InventoryPanel.Instance.Close();
            if (StatsPanel.Instance != null)
                StatsPanel.Instance.Close();

            if (EquipmentPanel.Instance != null)
                EquipmentPanel.Instance.Close();
            else
                this.AddControl(new EquipmentPanel(this));
        }

        public void ToggleStatsPanel()
        {
            if (!CanToggleMenu())
                return;

            if (InventoryPanel.Instance != null)
                InventoryPanel.Instance.Close();
            if (EquipmentPanel.Instance != null)
                EquipmentPanel.Instance.Close();

            if (StatsPanel.Instance != null)
                StatsPanel.Instance.Close();
                this.AddControl(new StatsPanel(this));
        }

        public void SetRpgClientConnection(RpgClientConnection connection)
        {
            _connection = connection;
        }

        public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                StateWindow.Instance.PopState();
                this.Destroy();
                return;
            }
            else if (e.Key == OpenTK.Input.Key.Enter && e.Alt)
            {
                Renderer.SetFulscreen(Renderer.GetFulscreen() ? false : true);
            }
            else
            {
                if (e.Key == Key.Space)
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.ActionTrigger);
                    RpgClientConnection.Instance.AddClientCommand(command);
                }
                else if (e.Key == Key.Enter)
                {
                    RpgClientConnection.Instance.CloseMessageBox();
                }
                else if (e.Key == Key.LShift)
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.ToggleRunning);
                    command.SetParameter("Running", true);
                    RpgClientConnection.Instance.AddClientCommand(command);
                }
            }
        }

        public override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.LShift)
            {
                ClientCommand command = new ClientCommand(ClientCommand.CommandType.ToggleRunning);
                command.SetParameter("Running", false);
                RpgClientConnection.Instance.AddClientCommand(command);
            }
        }

        public override void OnUpdateFrame(FrameEventArgs e)
        {
            if (_connection != null)
            {
                if (_connection.Connected())
                {
                    _connection.Update();
                }
                else
                {
                    StateWindow.Instance.PopState();
                    this.Destroy();
                    return;
                }
            }

            base.OnUpdateFrame(e);

            if (_movementTimer > 0)
                _movementTimer -= (float)e.Time;

            KeyboardState keyState = Keyboard.GetState();
            bool moving = false;
            MovementDirection direction = MovementDirection.Down;

            if (keyState.IsKeyDown(Key.W))
            {
                moving = true;
                if (keyState.IsKeyDown(Key.A))
                {
                    direction = MovementDirection.UpperLeft;
                }
                else if (keyState.IsKeyDown(Key.D))
                {
                    direction = MovementDirection.UpperRight;
                }
                else
                {
                    direction = MovementDirection.Up;
                }
            }
            else if (keyState.IsKeyDown(Key.S))
            {
                moving = true;
                if (keyState.IsKeyDown(Key.A))
                {
                    direction = MovementDirection.LowerLeft;
                }
                else if (keyState.IsKeyDown(Key.D))
                {
                    direction = MovementDirection.LowerRight;
                }
                else
                {
                    direction = MovementDirection.Down;
                }
            }
            else if (keyState.IsKeyDown(Key.A))
            {
                moving = true;
                direction = MovementDirection.Left;
            }
            else if (keyState.IsKeyDown(Key.D))
            {
                moving = true;
                direction = MovementDirection.Right;
            }

            if (moving && _movementTimer <= 0)
            {
                ClientCommand command = new ClientCommand(ClientCommand.CommandType.MovePlayer);
                command.SetParameter("Direction", (int)direction);
                RpgClientConnection.Instance.AddClientCommand(command);
            }

            if (_movementTimer <= 0)
                _movementTimer = 0.05f;
        }

        public override void Destroy()
        {
            base.Destroy();
            if (_connection != null)
                _connection.Disconnect();

            if (TradePanel.Instance != null)
                TradePanel.Instance.Close();
            if (InventoryPanel.Instance != null)
                InventoryPanel.Instance.Close();
            if (EquipmentPanel.Instance != null)
                EquipmentPanel.Instance.Close();
            if (StatsPanel.Instance != null)
                StatsPanel.Instance.Close();
        }
    }
}

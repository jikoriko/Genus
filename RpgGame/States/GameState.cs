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

namespace RpgGame.States
{
    public class GameState : State
    {

        public static GameState Instance { get; private set; }
        private RpgClientConnection _connection;

        private MessagePanel _messagePanel;
        private MenuPanel _menuPanel;
        private InventoryPanel _inventoryPanel;
        private EquipmentPanel _equipmentPanel;
        private StatsPanel _statsPanel;

        public Entity MapEntity;

        public GameState()
            : base()
        {
            Instance = this;
            MapEntity = Entity.CreateInstance(_entityManager);
            new MapComponent(MapEntity);

            _connection = null;

            _messagePanel = new MessagePanel(this);
            _menuPanel = new MenuPanel(this);
            _inventoryPanel = null;
            _equipmentPanel = null;
            _statsPanel = null;

            this.AddControl(_messagePanel);
            this.AddControl(_menuPanel);
        }

        public void ToggleInventory()
        {
            if (_equipmentPanel != null)
            {
                _equipmentPanel.Close();
                _equipmentPanel = null;
            }
            if (_statsPanel != null)
            {
                _statsPanel.Close();
                _statsPanel = null;
            }

            if (_inventoryPanel != null)
            {
                _inventoryPanel.Close();
                _inventoryPanel = null;
            }
            else
            {
                _inventoryPanel = new InventoryPanel(this);
                this.AddControl(_inventoryPanel);
            }
        }

        public void ToggleEquipmentPanel()
        {
            if (_inventoryPanel != null)
            {
                _inventoryPanel.Close();
                _inventoryPanel = null;
            }
            if (_statsPanel != null)
            {
                _statsPanel.Close();
                _statsPanel = null;
            }

            if (_equipmentPanel != null)
            {
                _equipmentPanel.Close();
                _equipmentPanel = null;
            }
            else
            {
                _equipmentPanel = new EquipmentPanel(this);
                this.AddControl(_equipmentPanel);
            }
        }

        public void ToggleStatsPanel()
        {
            if (_inventoryPanel != null)
            {
                _inventoryPanel.Close();
                _inventoryPanel = null;
            }
            if (_equipmentPanel != null)
            {
                _equipmentPanel.Close();
                _equipmentPanel = null;
            }

            if (_statsPanel != null)
            {
                _statsPanel.Close();
                _statsPanel = null;
            }
            else
            {
                _statsPanel = new StatsPanel(this);
                this.AddControl(_statsPanel);
            }
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
                    command.SetParameter("Running", "true");
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
                command.SetParameter("Running", "false");
                RpgClientConnection.Instance.AddClientCommand(command);
            }
        }

        public override void OnUpdateFrame(FrameEventArgs e)
        {
            if (_connection != null)
                _connection.Update();

            base.OnUpdateFrame(e);

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

            if (moving)
            {
                ClientCommand command = new ClientCommand(ClientCommand.CommandType.MovePlayer);
                command.SetParameter("Direction", ((int)direction).ToString());
                RpgClientConnection.Instance.AddClientCommand(command);
            }

        }

        public override void Destroy()
        {
            base.Destroy();
            if (_connection != null)
                _connection.Disconnect();
        }
    }
}

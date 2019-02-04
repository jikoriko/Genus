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

namespace RpgGame.States
{
    public class GameState : State
    {

        public static GameState Instance { get; private set; }
        private RpgClientConnection _connection;
        private List<int> _keysDown;

        private MessagePanel _messagePanel;

        public Entity MapEntity;

        public GameState()
            : base()
        {
            Instance = this;
            MapEntity = Entity.CreateInstance(_entityManager);
            new MapComponent(MapEntity);

            _connection = null;
            _keysDown = new List<int>();

            _messagePanel = new MessagePanel(this);
            this.AddControl(_messagePanel);
        }

        public void SetRpgClientConnection(RpgClientConnection connection)
        {
            _connection = connection;
        }

        List<int> GetKeysDown()
        {
            return _keysDown;
        }

        public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!_keysDown.Contains((int)e.Key))
            {
                _keysDown.Add((int)e.Key);
            }

            if (e.Key == Key.Escape)
            {
                StateWindow.Instance.PopState();
                this.Destroy();
            }

            if (e.Key == OpenTK.Input.Key.Enter && e.Alt)
            {
                Renderer.SetFulscreen(Renderer.GetFulscreen() ? false : true);
            }
        }

        public override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            _keysDown.Remove((int)e.Key);
        }


        public InputPacket GetInputPacket()
        {
            InputPacket packet = new InputPacket();
            packet.KeysDown = _keysDown;
            //packet.MouseState = OpenTK.Input.Mouse.GetState();
            return packet;
        }

        public override void OnUpdateFrame(FrameEventArgs e)
        {
            if (_connection != null)
                _connection.Update();

            base.OnUpdateFrame(e);
        }

        public override void Destroy()
        {
            base.Destroy();
            if (_connection != null)
                _connection.Disconnect();
        }
    }
}

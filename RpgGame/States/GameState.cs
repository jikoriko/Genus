using System;

using Genus2D.Core;
using Genus2D.Entities;

using RpgGame.EntityComponents;
using Genus2D.Graphics;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using Genus2D.Networking;

namespace RpgGame.States
{
    public class GameState : State
    {

        private RpgClientConnection _connection;
        private List<int> _keysDown;

        public Entity MapEntity;

        public GameState()
            : base()
        {
            MapEntity = Entity.CreateInstance(_entityManager);
            new MapComponent(MapEntity);
            _entityManager.AddEntity(MapEntity);

            _connection = null;
            _keysDown = new List<int>();
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

            /*
            Vector2 playerPos = _playerEntity.GetTransform().LocalPosition.Xy;
            Vector2 resolutionCenter = Renderer.GetResoultion() / 2;

            Vector2 worldPos = resolutionCenter - playerPos;
            Transform.Position = new Vector3((int)worldPos.X, (int)worldPos.Y, 0);
            */
        }

        public override void Destroy()
        {
            base.Destroy();
            if (_connection != null)
                _connection.Disconnect();
        }
    }
}

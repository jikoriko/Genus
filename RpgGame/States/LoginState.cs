using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RpgGame.States
{
    public class LoginState : State
    {
        public static LoginState INSTANCE { get; private set; }

        private Panel _loginPanel;
        private TextField _usernameTextBox;
        private TextField _passwordTextBox;
        private Button _loginButton;
        private Button _registerButton;

        public LoginState()
            : base()
        {
            INSTANCE = this;

            Renderer.SetResolutionMode(Renderer.ResolutionMode.Fixed);
            Renderer.SetResolution(640, 480);

            _loginPanel = new Panel(70, 90, 500, 300, Panel.BarMode.Empty, this);
            _usernameTextBox = new TextField(30, 50, 440, 40, this);
            _passwordTextBox = new TextField(30, 100, 440, 40, this);

            _loginButton = new Button("Login", 30, 170, 210, 40, this);
            _loginButton.OnTrigger += ClickLogin;

            _registerButton = new Button("Register", 260, 170, 210, 40, this);
            _registerButton.OnTrigger += ClickRegister;

            _loginPanel.AddControl(_usernameTextBox);
            _loginPanel.AddControl(_passwordTextBox);
            _loginPanel.AddControl(_loginButton);
            _loginPanel.AddControl(_registerButton);
            this.AddControl(_loginPanel);
        }

        private void ClickLogin()
        {
            if (_usernameTextBox.GetText() != "" && _passwordTextBox.GetText() != "")
            {
                GameState gameState = new GameState();
                RpgClientConnection connection = new RpgClientConnection(gameState, _usernameTextBox.GetText(), _passwordTextBox.GetText(), false);
                if (connection.Connected())
                {
                    StateWindow.Instance.PushState(gameState);
                }
                else
                {
                    gameState.Destroy();
                }
            }
        }

        private void ClickRegister()
        {
            if (_usernameTextBox.GetText() != "" && _passwordTextBox.GetText() != "")
            {
                RpgClientConnection connection = new RpgClientConnection(null, _usernameTextBox.GetText(), _passwordTextBox.GetText(), true);
            }
        }

        public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == OpenTK.Input.Key.Enter && e.Alt)
            {
                Renderer.SetFulscreen(Renderer.GetFulscreen() ? false : true);
            }
        }

        public override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);


        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}

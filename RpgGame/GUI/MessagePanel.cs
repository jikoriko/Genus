using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{
    public class MessagePanel : Panel
    {

        public static MessagePanel Instance { get; private set; }
        private GameState _gameState;

        private ScrollPanel _textPanel;
        private Label _textLabel;
        private TextField _messageField;
        private Button _sendMessageButton;

        public MessagePanel(GameState state) 
            : base(0, (int)Renderer.GetResoultion().Y - 200, (int)Renderer.GetResoultion().X, 200, BarMode.Empty, state)
        {
            Instance = this;
            _gameState = state;

            _textPanel = new ScrollPanel(0, 0, GetContentWidth(), GetContentHeight() - 50, BarMode.Empty, state);
            _textPanel.DisableHorizontalScroll();
            this.AddControl(_textPanel);

            _textLabel = new Label(0, 0, _textPanel.GetContentWidth(), 0, state);
            _textLabel.SetText("");
            _textPanel.AddControl(_textLabel);

            _messageField = new TextField(10, GetContentHeight() - 45, GetContentWidth() - 120, 40, state);
            this.AddControl(_messageField);

            _sendMessageButton = new Button("Send", GetContentWidth() - 90, GetContentHeight() - 45, 80, 40, state);
            _sendMessageButton.OnTrigger += SendMessage;
            this.AddControl(_sendMessageButton);
        }

        public void AddMessage(MessagePacket packet)
        {
            _textLabel.SetText(_textLabel.GetText() + packet.Message + '\n');
            int textHeight = Renderer.GetFont().GetTextHeight(_textLabel.GetText());
            _textLabel.SetSize((int)_textLabel.GetBodySize().X, textHeight);
            _textPanel.SetScrollableHeight(textHeight);
        }

        private void SendMessage()
        {
            if (_messageField.GetText() != "")
            {
                RpgClientConnection.Instance.SendMessage(new MessagePacket(_messageField.GetText()));
                _messageField.SetText("");
            }
        }

    }
}

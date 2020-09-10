﻿using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using Genus2D.Utililities;
using OpenTK;
using OpenTK.Graphics;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{
    public class TradePanel : Panel
    {

        public static TradePanel Instance { get; private set; }
        private GameState _gameState;

        private Button _cancelButton, _acceptButton;

        private int _playerID;
        public TradeRequest TradeRequest;

        public TradePanel(GameState state, int playerID, string name) 
            : base(((int)MessagePanel.Instance.GetBodySize().X / 2) - 200, ((int)Renderer.GetResoultion().Y / 2) - 325, 400, 450, BarMode.Label, state)
        {
            if (Instance != null)
                Instance.Close();
            Instance = this;
            _gameState = state;

            this.SetPanelLabel("Trading with " + name);

            _playerID = playerID;
            this.TradeRequest = new TradeRequest(RpgClientConnection.Instance.GetLocalPlayerPacket().PlayerID, playerID);

            _cancelButton = new Button("Cancel", 10, GetContentHeight() - 50, (GetContentWidth() / 2) - 20, 40, state);
            _cancelButton.OnTrigger += Cancel;
            this.AddControl(_cancelButton);

            _acceptButton = new Button("Accept", (GetContentWidth() / 2) + 10, GetContentHeight() - 50, (GetContentWidth() / 2) - 20, 40, state);
            _acceptButton.OnTrigger += Accept;
            this.AddControl(_acceptButton);
        }

        private void Cancel()
        {
            ClientCommand command = new ClientCommand(ClientCommand.CommandType.CancelTrade);
            RpgClientConnection.Instance.AddClientCommand(command);
        }

        private void Accept()
        {
            ClientCommand command = new ClientCommand(ClientCommand.CommandType.AcceptTrade);
            RpgClientConnection.Instance.AddClientCommand(command);
            this.TradeRequest.TradeOffer1.Accepted = true;
        }

        public override void Close()
        {
            Instance = null;
            base.Close();
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int width = (GetContentWidth() / 2) - 8;
            int height = GetContentHeight() - 8;
            Vector3 pos;
            Vector3 size;
            Color4 colour;

            int iconSize = width / 5;
            size = new Vector3(iconSize, iconSize, 1);
            for (int i = 0; i < this.TradeRequest.TradeOffer1.Items.Count; i++)
            {
                Tuple<int, int> itemInfo = this.TradeRequest.TradeOffer1.Items[i];
                int x = 4 + ((i % 5) * iconSize);
                int y = 4 + ((i / 5) * iconSize);
                ItemData data = ItemData.GetItemData(itemInfo.Item1);

                if (data != null)
                {
                    pos = new Vector3(x, y, 0);
                    Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                    Rectangle dest = new Rectangle(x, y, iconSize, iconSize);
                    Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                    colour = Color4.White;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);

                    int amount = itemInfo.Item2;
                    if (amount > 1)
                    {
                        string text = amount.ToString();
                        pos.X += 32 - Renderer.GetFont().GetTextWidth(text);
                        pos.Y += 32 - Renderer.GetFont().GetTextHeight(text);
                        colour = Color4.Red;
                        Renderer.PrintText(text, ref pos, ref colour);
                    }
                }
            }

            for (int i = 0; i < this.TradeRequest.TradeOffer2.Items.Count; i++)
            {
                Tuple<int, int> itemInfo = this.TradeRequest.TradeOffer2.Items[i];
                int x = 4 + ((i % 5) * iconSize);
                int y = 4 + ((i / 5) * iconSize);
                x += GetContentWidth() / 2;
                ItemData data = ItemData.GetItemData(itemInfo.Item1);

                if (data != null)
                {
                    pos = new Vector3(x, y, 0);
                    Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                    Rectangle dest = new Rectangle(x, y, iconSize, iconSize);
                    Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                    colour = Color4.White;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);

                    int amount = itemInfo.Item2;
                    if (amount > 1)
                    {
                        string text = amount.ToString();
                        pos.X += 32 - Renderer.GetFont().GetTextWidth(text);
                        pos.Y += 32 - Renderer.GetFont().GetTextHeight(text);
                        colour = Color4.Red;
                        Renderer.PrintText(text, ref pos, ref colour);
                    }
                }
            }

            pos = new Vector3(4, 4, 0);
            size = new Vector3(width, height - 50, 1f);
            colour = this.TradeRequest.TradeOffer1.Accepted ? Color4.LightGreen : Color4.White;
            Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref size, 1f, ref colour);

            pos = new Vector3((GetContentWidth() / 2) + 4, 4, 0);
            colour = this.TradeRequest.TradeOffer2.Accepted ? Color4.LightGreen : Color4.White;
            Renderer.DrawShape(ShapeFactory.Rectangle, ref pos, ref size, 1f, ref colour);

        }

    }
}
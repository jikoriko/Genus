using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using Genus2D.Utililities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using RpgGame.EntityComponents;
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

        public static int ItemsPerRow = 3;

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
            this.TradeRequest = new TradeRequest(MapComponent.Instance.GetLocalPlayerPacket().PlayerID, playerID);

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

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (ContentSelectable())
            {
                if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
                {
                    Vector2 mouse = GetLocalMousePosition();
                    int width = (GetContentWidth() / 2) - 8;
                    int slotSize = width / ItemsPerRow;

                    Rectangle selectionRect = new Rectangle(4, 4, width, GetContentHeight() - 58);
                    if (selectionRect.Contains((int)mouse.X, (int)mouse.Y))
                    {
                        mouse.X -= 4;
                        mouse.Y -= 4;
                        mouse.X /= slotSize;
                        mouse.Y /= slotSize;
                        int itemIndex = (int)mouse.X + ((int)mouse.Y * ItemsPerRow);

                        if (itemIndex >= 0 && itemIndex < this.TradeRequest.TradeOffer1.NumItems())
                        {
                            if (e.Button == MouseButton.Left)
                            {
                                if (this.TradeRequest.TradeOffer1.GetItem(itemIndex) != null)
                                {
                                    int removed = this.TradeRequest.TradeOffer1.RemoveItem(itemIndex, 1);
                                    if (removed > 0)
                                    {
                                        ClientCommand command = new ClientCommand(ClientCommand.CommandType.RemoveTradeItem);
                                        command.SetParameter("ItemIndex", itemIndex);
                                        command.SetParameter("Count", 1);
                                        RpgClientConnection.Instance.AddClientCommand(command);
                                    }
                                }
                            }
                            else if (e.Button == MouseButton.Right)
                            {
                                Vector2 mousePos = StateWindow.Instance.GetMousePosition();
                                ItemClickOptionsPanel.OptionType optionType = ItemClickOptionsPanel.OptionType.RemoveTrade;
                                int itemID = this.TradeRequest.TradeOffer1.GetItem(itemIndex).Item1;
                                int max = this.TradeRequest.TradeOffer1.GetItem(itemIndex).Item2;
                                ItemClickOptionsPanel optionPanel = new ItemClickOptionsPanel((int)mousePos.X, (int)mousePos.Y, optionType, itemIndex, itemID, max, _gameState);
                                GameState.Instance.AddControl(optionPanel);
                            }
                        }
                    }
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int width = (GetContentWidth() / 2) - 8;
            int height = GetContentHeight() - 8;
            Vector3 pos;
            Vector3 size;
            Color4 colour;

            int iconSize = width / ItemsPerRow;
            size = new Vector3(iconSize, iconSize, 1);
            for (int i = 0; i < this.TradeRequest.TradeOffer1.NumItems(); i++)
            {
                Tuple<int, int> itemInfo = this.TradeRequest.TradeOffer1.GetItem(i);
                int x = 4 + ((i % ItemsPerRow) * iconSize);
                int y = 4 + ((i / ItemsPerRow) * iconSize);
                ItemData data = ItemData.GetItemData(itemInfo.Item1);

                if (data != null)
                {
                    pos = new Vector3(x, y, 0);
                    Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                    Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                    colour = Color4.White;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);

                    int amount = itemInfo.Item2;
                    if (amount > 1)
                    {
                        string text = amount.ToString();
                        pos.X += iconSize - Renderer.GetFont().GetTextWidth(text);
                        pos.Y += iconSize - Renderer.GetFont().GetTextHeight(text);
                        colour = Color4.Red;
                        Renderer.PrintText(text, ref pos, ref colour);
                    }
                }
            }

            for (int i = 0; i < this.TradeRequest.TradeOffer2.NumItems(); i++)
            {
                Tuple<int, int> itemInfo = this.TradeRequest.TradeOffer2.GetItem(i);
                int x = 4 + ((i % ItemsPerRow) * iconSize);
                int y = 4 + ((i / ItemsPerRow) * iconSize);
                x += GetContentWidth() / 2;
                ItemData data = ItemData.GetItemData(itemInfo.Item1);

                if (data != null)
                {
                    pos = new Vector3(x, y, 0);
                    Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
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

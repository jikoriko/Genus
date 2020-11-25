using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using Genus2D.Utililities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{
    public class InventoryPanel : Panel
    {
        public static InventoryPanel Instance { get; private set; }
        private GameState _gameState;

        public InventoryPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, 0, 400, 0, BarMode.Empty, state)
        {
            Instance = this;
            _gameState = state;

            SetContentSize(GetContentWidth(), 30 + ((GetContentWidth() / 5) * 6));
            SetPosition((int)GetBodyPosition().X, (int)(Renderer.GetResoultion().Y - 60 - GetBodySize().Y));
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (ContentSelectable())
            {
                if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
                {
                    ClientCommand command = null;

                    Vector2 mouse = GetLocalMousePosition();
                    int slotSize = GetContentWidth() / 5;
                    mouse.X /= slotSize;
                    if (mouse.Y >= 30)
                    {
                        mouse.Y -= 30;
                        mouse.Y /= slotSize;
                        int itemIndex = (int)mouse.X + ((int)mouse.Y * 5);

                        Tuple<int, int> itemInfo = RpgClientConnection.Instance.GetLocalPlayerPacket().Data.GetInventoryItem(itemIndex);
                        if (itemInfo != null)
                        {
                            if (e.Button == MouseButton.Left)
                            {
                                if (ShopPanel.Instance != null)
                                {
                                    command = new ClientCommand(ClientCommand.CommandType.SellShopItem);
                                    command.SetParameter("Count", 1);
                                }
                                else if (TradePanel.Instance != null)
                                {

                                    int added = TradePanel.Instance.TradeRequest.TradeOffer1.AddItem(itemInfo.Item1, 1);
                                    if (added > 0)
                                    {
                                        command = new ClientCommand(ClientCommand.CommandType.AddTradeItem);
                                        command.SetParameter("Count", 1);
                                        TradePanel.Instance.TradeRequest.TradeOffer1.Accepted = false;
                                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = false;
                                    }
                                }
                                else if (BankPanel.Instance != null)
                                {
                                    command = new ClientCommand(ClientCommand.CommandType.AddBankItem);
                                    command.SetParameter("Count", 1);
                                }
                                else
                                {
                                    command = new ClientCommand(ClientCommand.CommandType.SelectItem);
                                }
                            }
                            else if (e.Button == MouseButton.Right)
                            {
                                ItemClickOptionsPanel.OptionType optionType = ItemClickOptionsPanel.OptionType.Drop;

                                if (ShopPanel.Instance != null)
                                {
                                    optionType = ItemClickOptionsPanel.OptionType.Sell;
                                }
                                else if (TradePanel.Instance != null)
                                {
                                    optionType = ItemClickOptionsPanel.OptionType.AddTrade;
                                }
                                else if (BankPanel.Instance != null)
                                {
                                    optionType = ItemClickOptionsPanel.OptionType.AddBank;
                                }
                                else
                                {
                                    optionType = ItemClickOptionsPanel.OptionType.Drop;
                                }

                                Vector2 mousePos = StateWindow.Instance.GetMousePosition();
                                ItemClickOptionsPanel optionPanel = new ItemClickOptionsPanel((int)mousePos.X, (int)mousePos.Y, optionType, itemIndex, itemInfo.Item1, itemInfo.Item2, _gameState);
                                GameState.Instance.AddControl(optionPanel);
                            }
                        }

                        if (command != null)
                        {
                            command.SetParameter("ItemIndex", itemIndex);
                            RpgClientConnection.Instance.AddClientCommand(command);
                        }
                    }
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            PlayerPacket playerPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (playerPacket != null)
            {
                Vector3 pos = new Vector3(10, 5, 0);
                Color4 colour = Color4.Black;
                Renderer.PrintText("Gold: " + playerPacket.Data.Gold, ref pos, ref colour);
                int slotSize = GetContentWidth() / 5;
                Vector3 size = new Vector3(slotSize, slotSize, 1);
                for (int i = 0; i < 30; i++)
                {
                    Tuple<int, int> item = playerPacket.Data.GetInventoryItem(i);
                    if (item == null) break;

                    int x = (i % 5) * slotSize;
                    int y = 30 + ((i / 5) * slotSize);
                    ItemData data = ItemData.GetItemData(item.Item1);
                    if (data != null)
                    {
                        pos = new Vector3(x, y, 0);
                        Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                        Rectangle dest = new Rectangle(x, y, slotSize, slotSize);
                        Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                        colour = Color4.White;
                        Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);

                        int amount = item.Item2;
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
            }
        }

        public override void Close()
        {
            Instance = null;
            base.Close();
        }
    }
}

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
    public class ShopPanel : ScrollPanel
    {
        public static ShopPanel Instance { get; private set; }
        private GameState _gameState;
        private ShopData _shopData;

        public ShopPanel(GameState state, ShopData shopData)
            : base((int)(Renderer.GetResoultion().X / 2) - 200, (int)(Renderer.GetResoultion().Y / 2) - 200, 400, 400, BarMode.Close, state)
        {
            if (Instance != null)
                Instance.Close();
            Instance = this;

            _gameState = state;
            _shopData = shopData;
            this.SetPanelLabel(_shopData.Name);

            DisableHorizontalScroll();
            //EnableVerticalScroll();
            int slotSize = GetContentWidth() / 5;
            int rows = (int)Math.Ceiling(_shopData.ShopItems.Count / 5f);

            SetScrollableHeight(slotSize * rows);
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (ContentSelectable())
            {
                if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
                {
                    Vector2 mouse = GetLocalMousePosition();
                    int slotSize = GetContentWidth() / 5;
                    mouse.X /= slotSize;
                    mouse.Y /= slotSize;
                    int itemIndex = (int)mouse.X + ((int)mouse.Y * 5);
                    if (itemIndex >= 0 && itemIndex < _shopData.ShopItems.Count)
                    {

                        if (e.Button == MouseButton.Left)
                        {
                            ClientCommand command = new ClientCommand(ClientCommand.CommandType.BuyShopItem);
                            command.SetParameter("ItemIndex", itemIndex);
                            command.SetParameter("Count", 1);
                            RpgClientConnection.Instance.AddClientCommand(command);
                        }
                        else if (e.Button == MouseButton.Right)
                        {
                            Vector2 mousePos = StateWindow.Instance.GetMousePosition();
                            ItemClickOptionsPanel.OptionType optionType = ItemClickOptionsPanel.OptionType.Buy;
                            int itemID = _shopData.ShopItems[itemIndex].ItemID;
                            ItemClickOptionsPanel optionPanel = new ItemClickOptionsPanel((int)mousePos.X, (int)mousePos.Y, optionType, itemIndex, itemID, -1, _gameState);
                            GameState.Instance.AddControl(optionPanel);
                        }
                    }
                }
            }
        }

        public override void Close()
        {
            Instance = null;
            ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.CloseShop);
            RpgClientConnection.Instance.AddClientCommand(clientCommand);
            base.Close();
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = new Vector3();
            int slotSize = GetContentWidth() / 5;
            Vector3 size = new Vector3(slotSize, slotSize, 1);
            Color4 colour;
            string text;

            for (int i = 0; i < _shopData.ShopItems.Count; i++)
            {
                ShopData.ShopItem shopItem = _shopData.ShopItems[i];
                pos.X = (i % 5) * slotSize;
                pos.Y = (i / 5) * slotSize;
                ItemData itemData = ItemData.GetItemData(shopItem.ItemID);

                if (itemData != null)
                {
                    Rectangle source = new Rectangle((itemData.IconID % 8) * 32, (itemData.IconID / 8) * 32, 32, 32);
                    colour = Color4.White;
                    Texture texture = Assets.GetTexture("Icons/" + itemData.IconSheetImage);
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);
                }
                else
                {
                    colour = Color4.Red;
                    colour.A = 0.5f;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref colour);
                }

                text = shopItem.Cost.ToString();
                pos.X = pos.X + (slotSize / 2) - (Renderer.GetFont().GetTextWidth(text) / 2);
                pos.Y = pos.Y + slotSize - (Renderer.GetFont().GetTextHeight(text));
                colour = Color4.Gold;
                Renderer.PrintText(text, ref pos, ref colour);
            }
        }
    }
}

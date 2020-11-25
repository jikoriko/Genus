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
    public class BankPanel : ScrollPanel
    {

        public static int ItemsPerRow = 8;

        public static BankPanel Instance { get; private set; }
        private GameState _gameState;

        private BankData _bankData;

        public BankPanel(GameState state) 
            : base(((int)MessagePanel.Instance.GetBodySize().X / 2) - 200, ((int)Renderer.GetResoultion().Y / 2) - 325, 400, 450, BarMode.Close, state)
        {
            if (Instance != null)
                Instance.Close();
            Instance = this;
            _gameState = state;

            this.SetPanelLabel("Bank");
            this.DisableHorizontalScroll();

            _bankData = null;
        }

        public void SetBankData(BankData bankData)
        {
            _bankData = bankData;

            int width = GetContentWidth() - 8;
            int slotSize = width / ItemsPerRow;

            int rows = (int)Math.Ceiling(_bankData.Items.Count / 8f);

            SetScrollableHeight((rows * slotSize) + 8);
        }

        public override void Close()
        {
            Instance = null;
            ClientCommand command = new ClientCommand(ClientCommand.CommandType.CloseBank);
            RpgClientConnection.Instance.AddClientCommand(command);
            base.Close();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (ContentSelectable() && _bankData != null)
            {
                if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
                {
                    Vector2 mouse = GetLocalMousePosition();
                    int width = GetContentWidth() - 8;
                    int slotSize = width / ItemsPerRow;

                    Rectangle selectionRect = new Rectangle(4, 4, width, GetContentHeight() - 58);
                    if (selectionRect.Contains((int)mouse.X, (int)mouse.Y))
                    {
                        mouse.X -= 4;
                        mouse.Y -= 4;
                        mouse.X /= slotSize;
                        mouse.Y /= slotSize;
                        int itemIndex = (int)mouse.X + ((int)mouse.Y * ItemsPerRow);

                        if (itemIndex >= 0 && itemIndex < _bankData.Items.Count)
                        {
                            if (e.Button == MouseButton.Left)
                            {
                                ClientCommand command = new ClientCommand(ClientCommand.CommandType.RemoveBankItem);
                                command.SetParameter("ItemIndex", itemIndex);
                                command.SetParameter("Count", 1);
                                RpgClientConnection.Instance.AddClientCommand(command);

                            }
                            else if (e.Button == MouseButton.Right)
                            {
                                Vector2 mousePos = StateWindow.Instance.GetMousePosition();
                                ItemClickOptionsPanel.OptionType optionType = ItemClickOptionsPanel.OptionType.RemoveBank;
                                int itemID = _bankData.Items[itemIndex].Item1;
                                int max = _bankData.Items[itemIndex].Item2;
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

            if (_bankData == null) return;

            int width = GetContentWidth() - 8;
            //int height = GetContentHeight() - 8;
            Vector3 pos;
            Vector3 size;
            Color4 colour;

            int iconSize = width / ItemsPerRow;
            size = new Vector3(iconSize, iconSize, 1);
            for (int i = 0; i < _bankData.Items.Count; i++)
            {
                Tuple<int, int> itemInfo = _bankData.Items[i];
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

        }

    }
}

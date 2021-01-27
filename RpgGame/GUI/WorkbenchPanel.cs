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
    public class WorkbenchPanel : Panel
    {
        public static WorkbenchPanel Instance { get; private set; }
        private GameState _gameState;
        private int _workbenchID;
        private List<int> _craftableIDs;

        private ScrollPanel _craftingSelectionPanel, _craftingInfoPanel;
        private int _selectedCraftable = -1;

        private NumberControl _countControl;
        private Button _craftButton;

        public WorkbenchPanel(GameState state, int workbenchID)
            : base(((int)MessagePanel.Instance.GetBodySize().X / 2) - 300, ((int)Renderer.GetResoultion().Y / 2) - 325, 600, 400, BarMode.Close, state)
        {
            if (Instance != null)
                Instance.Close();
            Instance = this;

            _gameState = state;
            _workbenchID = workbenchID;
            this.SetPanelLabel(CraftableData.GetWorkbench(workbenchID));

            _craftableIDs = new List<int>();
            for (int i = 0; i < CraftableData.GetCraftableDataCount(); i++)
            {
                if (CraftableData.GetCraftableData(i).WorkbenchID == workbenchID)
                {
                    _craftableIDs.Add(i);
                }
            }

            _craftingSelectionPanel = new ScrollPanel(0, 0, GetContentWidth() / 2, GetContentHeight(), BarMode.Empty, state);
            _craftingInfoPanel = new ScrollPanel(GetContentWidth() / 2, 0, GetContentWidth() / 2, GetContentHeight(), BarMode.Empty, state);

            _craftingSelectionPanel.OnTrigger += SelectCraftable;
            _craftingSelectionPanel.OnRenderContent += RenderCraftables;
            _craftingInfoPanel.OnRenderContent += RenderCraftableInfo;

            _craftingSelectionPanel.DisableHorizontalScroll();
            _craftingInfoPanel.DisableHorizontalScroll();

            _countControl = new NumberControl(10, 10, state);
            _countControl.SetMinimum(1);
            _craftButton = new Button("Craft", 20 + (int)_countControl.GetBodySize().X, 10, _craftingInfoPanel.GetContentWidth() - 30 - (int)_countControl.GetBodySize().X, 40, state);
            _craftButton.OnTrigger += CraftTrigger;
            
            _craftingInfoPanel.AddControl(_countControl);
            _craftingInfoPanel.AddControl(_craftButton);

            this.AddControl(_craftingSelectionPanel);
            this.AddControl(_craftingInfoPanel);

            int slotSize = _craftingSelectionPanel.GetContentWidth() / 5;
            int rows = (int)Math.Ceiling(_craftableIDs.Count / 5f);

            _craftingSelectionPanel.SetScrollableHeight(slotSize * rows);
        }

        private void SelectCraftable()
        {
            if (_craftingSelectionPanel.ContentSelectable())
            {
                Vector2 mouse = _craftingSelectionPanel.GetLocalMousePosition();
                int slotSize = _craftingSelectionPanel.GetContentWidth() / 5;
                mouse.X /= slotSize;
                mouse.Y /= slotSize;
                int craftIndex = (int)mouse.X + ((int)mouse.Y * 5);
                if (craftIndex >= 0 && craftIndex < _craftableIDs.Count)
                {
                    _selectedCraftable = craftIndex;
                }
                else
                {
                    _selectedCraftable = -1;
                }
            }
        }

        private void CraftTrigger()
        {
            if (_selectedCraftable != -1)
            {
                ClientCommand command = new ClientCommand(ClientCommand.CommandType.CraftItem);
                command.SetParameter("CraftID", _craftableIDs[_selectedCraftable]);
                command.SetParameter("Count", _countControl.GetIndex());
                RpgClientConnection.Instance.AddClientCommand(command);
            }
        }

        public override void Close()
        {
            Instance = null;
            ClientCommand clientCommand = new ClientCommand(ClientCommand.CommandType.CloseWorkbench);
            RpgClientConnection.Instance.AddClientCommand(clientCommand);
            base.Close();
        }

        private void RenderCraftables()
        {

            Vector3 pos = new Vector3();
            int slotSize = _craftingSelectionPanel.GetContentWidth() / 5;
            Vector3 size = new Vector3(slotSize, slotSize, 1);
            Color4 colour;

            if (_selectedCraftable != -1)
            {
                pos.X = (_selectedCraftable % 5) * slotSize;
                pos.Y = (_selectedCraftable / 5) * slotSize;
                colour = Color4.Gold;
                Renderer.DrawRoundedRectangle(ref pos, ref size, _cornerRadius, 2f, ref colour);
            }

            for (int i = 0; i < _craftableIDs.Count; i++)
            {
                CraftableData data = CraftableData.GetCraftableData(_craftableIDs[i]);
                pos.X = (i % 5) * slotSize;
                pos.Y = (i / 5) * slotSize;
                ItemData itemData = ItemData.GetItemData(data.CraftedItemID);

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
            }
        }

        private void RenderCraftableInfo()
        {
            int scrollHeight = 0;
            Vector3 pos = new Vector3();
            Color4 colour;
            string text;
            int textWidth;

            if (_selectedCraftable != -1)
            {
                CraftableData data = CraftableData.GetCraftableData(_craftableIDs[_selectedCraftable]);

                text = data.Name;
                textWidth = Renderer.GetFont().GetTextWidth(text);
                pos.X = (_craftingInfoPanel.GetContentWidth() / 2) - (textWidth / 2);
                pos.Y = 57;
                colour = Color4.Gold;
                Renderer.PrintText(text, ref pos, ref colour);

                int textHeight = Renderer.GetFont().GetTextHeight(text);
                scrollHeight += textHeight + 67;
                pos.Y += textHeight + 4;

                Vector3 endPos = pos + new Vector3(textWidth, 0, 0);
                Renderer.DrawLine(ref pos, ref endPos, 2f, colour);

                if (data.Materials.Count == 0)
                {
                    colour = Color4.LimeGreen;
                    pos.X = 10;
                    pos.Y = scrollHeight;
                    Renderer.PrintText("No materials required.", ref pos, ref colour);
                }
                else
                {
                    for (int i = 0; i < data.Materials.Count; i++)
                    {
                        pos.X = 10;
                        pos.Y = scrollHeight;
                        int itemID = data.Materials[i].Item1;
                        ItemData matData = ItemData.GetItemData(itemID);

                        if (matData != null)
                        {
                            int count = data.Materials[i].Item2 * _countControl.GetIndex();
                            text = matData.Name + ": ";

                            if (RpgClientConnection.Instance.GetLocalPlayerPacket().Data.ItemInInventory(itemID, count))
                                colour = Color4.LimeGreen;
                            else
                                colour = Color4.Red;

                            Renderer.PrintText(text, ref pos, ref colour);

                            colour = Color4.White;
                            pos.X += Renderer.GetFont().GetTextWidth(text);
                            Renderer.PrintText("x" + count, ref pos, ref colour);

                            scrollHeight += Renderer.GetFont().GetTextHeight(text + 'x' + count) + 15;
                        }
                    }
                }
            }
            else
            {
                text = "Select a craftable.";
                textWidth = Renderer.GetFont().GetTextWidth(text);

                pos.X = (_craftingInfoPanel.GetContentWidth() / 2) - (textWidth / 2);
                pos.Y = 57;
                colour = Color4.Gold;
                Renderer.PrintText(text, ref pos, ref colour);

                int textHeight = Renderer.GetFont().GetTextHeight(text);
                scrollHeight += textHeight + 62;
                pos.Y += textHeight + 4;

                Vector3 endPos = pos + new Vector3(textWidth, 0, 0);
                Renderer.DrawLine(ref pos, ref endPos, 2f, colour);
            }

            _craftingInfoPanel.SetScrollableHeight(scrollHeight);
        }
    }
}

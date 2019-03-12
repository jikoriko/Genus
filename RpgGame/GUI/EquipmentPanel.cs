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
    public class EquipmentPanel : Panel
    {
        public static MessagePanel Instance { get; private set; }
        private GameState _gameState;
        private List<Button> _buttons;

        public EquipmentPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, 0, 400, 0, BarMode.Empty, state)
        {
            _gameState = state;

            SetContentSize(GetContentWidth(), (GetContentWidth() / 5) * 6);
            SetPosition((int)GetBodyPosition().X, (int)(Renderer.GetResoultion().Y - 60 - GetBodySize().Y));

            _buttons = new List<Button>();
            for (int i = 0; i < (int)EquipmentSlot.Ring + 1; i++)
            {
                int width = GetContentWidth() / 3;
                string text = ((EquipmentSlot)i).ToString() + ":";
                Label label = new Label(width - Renderer.GetFont().GetTextWidth(text), 10 + (i * 32) + (i * 10), 100, 32, state);
                label.SetText(text);
                this.AddControl(label);

                Button removeButton = new Button("Remove", GetContentWidth() - width + 10, 10 + (i * 32) + (i * 10), width - 20, 32, state);
                removeButton.OnTrigger += RemoveEquipment;
                _buttons.Add(removeButton);
                this.AddControl(removeButton);
            }
        }

        private void RemoveEquipment()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].BodySelectable())
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.RemoveEquipment);
                    command.SetParameter("EquipmentIndex", i.ToString());
                    RpgClientConnection.Instance.AddClientCommand(command);
                    break;
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            PlayerPacket playerPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (playerPacket != null)
            {
                int width = GetContentWidth() / 3;
                int x = (GetContentWidth() / 2) - 16;
                int y;
                for (int i = 0; i < (int)EquipmentSlot.Ring + 1; i++)
                {
                    y = 10 + (i * 42);
                    int equipmentID = playerPacket.Data.GetEquipedItemID((EquipmentSlot)i);
                    ItemData data = ItemData.GetItemData(equipmentID);
                    if (data != null)
                    {
                        Vector3 pos = new Vector3(x, y, 0);
                        Vector3 size = new Vector3(32, 32, 1);
                        Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                        Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                        Color4 colour = Color4.White;
                        Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);
                    }
                }
            }
        }
    }
}

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
    public class EquipmentPanel : Panel
    {
        public static EquipmentPanel Instance { get; private set; }
        private GameState _gameState;
        private List<Button> _buttons;
        private Button _removeAmmoButton;

        public EquipmentPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, 0, 400, 0, BarMode.Empty, state)
        {
            Instance = this;
            _gameState = state;

            SetContentSize(GetContentWidth(), (GetContentWidth() / 5) * 6);
            SetPosition((int)GetBodyPosition().X, (int)(Renderer.GetResoultion().Y - 60 - GetBodySize().Y));

            _buttons = new List<Button>();
            int y = 0;
            int width = GetContentWidth() / 3;
            for (int i = 0; i < (int)EquipmentSlot.Ring + 1; i++)
            {
                y = 10 + (i * 32) + (i * 10);
                string text = ((EquipmentSlot)i).ToString() + ":";
                Label label = new Label(width - Renderer.GetFont().GetTextWidth(text), 10 + (i * 32) + (i * 10), 100, 32, state);
                label.SetText(text);
                this.AddControl(label);

                Button removeButton = new Button("Remove", GetContentWidth() - width + 10, y, width - 20, 32, state);
                removeButton.OnTrigger += RemoveEquipment;
                _buttons.Add(removeButton);
                this.AddControl(removeButton);
            }

            y += 42;
            string text2 = "Ammo:";
            Label label2 = new Label(width - Renderer.GetFont().GetTextWidth(text2), y, 100, 32, state);
            label2.SetText(text2);
            this.AddControl(label2);
            _removeAmmoButton = new Button("Remove", GetContentWidth() - width + 10, y, width - 20, 32, state);
            _removeAmmoButton.OnTrigger += RemoveAmmo;
            this.AddControl(_removeAmmoButton);
        }

        private void RemoveEquipment()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].BodySelectable())
                {
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.RemoveEquipment);
                    command.SetParameter("EquipmentIndex", i);
                    RpgClientConnection.Instance.AddClientCommand(command);
                    break;
                }
            }
        }

        private void RemoveAmmo()
        {
            ClientCommand command = new ClientCommand(ClientCommand.CommandType.RemoveAmmo);
            RpgClientConnection.Instance.AddClientCommand(command);
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            PlayerPacket playerPacket = MapComponent.Instance.GetLocalPlayerPacket();
            if (playerPacket != null)
            {
                int width = GetContentWidth() / 3;
                int x = (GetContentWidth() / 2) - 16;
                int y = 0;
                Vector3 size = new Vector3(32, 32, 1);
                for (int i = 0; i < 9; i++)
                {
                    y = 10 + (i * 42);
                    int equipmentID = playerPacket.Data.GetEquipedItemID((EquipmentSlot)i);
                    ItemData data = ItemData.GetItemData(equipmentID);
                    if (data != null)
                    {
                        Vector3 pos = new Vector3(x, y, 0);
                        Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                        Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                        Color4 colour = Color4.White;
                        Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);
                    }
                }

                Tuple<int, int> ammoInfo = playerPacket.Data.GetEquipedAmmo();
                ItemData data2 = ItemData.GetItemData(ammoInfo.Item1);
                if (data2 != null)
                {
                    y += 42;
                    Vector3 pos = new Vector3(x, y, 0);
                    Rectangle source = new Rectangle((data2.IconID % 8) * 32, (data2.IconID / 8) * 32, 32, 32);
                    Texture texture = Assets.GetTexture("Icons/" + data2.IconSheetImage);
                    Color4 colour = Color4.White;
                    Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);

                    if (ammoInfo.Item2 > 1)
                    {
                        string text = ammoInfo.Item2.ToString();
                        pos.X += 32;
                        colour = Color4.Red;
                        Renderer.PrintText(text, ref pos, ref colour);
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

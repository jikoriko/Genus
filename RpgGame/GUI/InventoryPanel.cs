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
        public static MessagePanel Instance { get; private set; }
        private GameState _gameState;

        public InventoryPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, 0, 400, 0, BarMode.Empty, state)
        {
            _gameState = state;

            SetContentSize(GetContentWidth(), (GetContentWidth() / 5) * 6);
            SetPosition((int)GetBodyPosition().X, (int)(Renderer.GetResoultion().Y - 60 - GetBodySize().Y));
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (ContentSelectable())
            {
                if (e.Button == MouseButton.Left)
                {
                    Vector2 mouse = GetLocalMousePosition();
                    int slotSize = GetContentWidth() / 5;
                    mouse.X /= slotSize;
                    mouse.Y /= slotSize;
                    int itemIndex = (int)mouse.X + ((int)mouse.Y * 5);
                    ClientCommand command = new ClientCommand(ClientCommand.CommandType.SelectItem);
                    command.SetParameter("ItemIndex", itemIndex.ToString());
                    RpgClientConnection.Instance.AddClientCommand(command);
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            PlayerPacket playerPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (playerPacket != null)
            {
                int slotSize = GetContentWidth() / 5;
                Vector3 size = new Vector3(slotSize, slotSize, 1);
                for (int i = 0; i < 30; i++)
                {
                    Tuple<int, int> item = playerPacket.Data.GetInventoryItem(i);
                    if (item == null) break;

                    int x = (i % 5) * slotSize;
                    int y = (i / 5) * slotSize;
                    ItemData data = ItemData.GetItemData(item.Item1);
                    if (data != null)
                    {
                        Vector3 pos = new Vector3(x, y, 0);
                        Rectangle source = new Rectangle((data.IconID % 8) * 32, (data.IconID / 8) * 32, 32, 32);
                        Rectangle dest = new Rectangle(x, y, slotSize, slotSize);
                        Texture texture = Assets.GetTexture("Icons/" + data.IconSheetImage);
                        Color4 colour = Color4.White;
                        Renderer.FillTexture(texture, ShapeFactory.Rectangle, ref pos, ref size, ref source, ref colour);
                    }
                }
            }
        }
    }
}

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
    public class StatsPanel : Panel
    {
        public static MessagePanel Instance { get; private set; }
        private GameState _gameState;
        private List<Button> _buttons;

        public StatsPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, 0, 400, 0, BarMode.Empty, state)
        {
            _gameState = state;

            SetContentSize(GetContentWidth(), (GetContentWidth() / 5) * 6);
            SetPosition((int)GetBodyPosition().X, (int)(Renderer.GetResoultion().Y - 60 - GetBodySize().Y));
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            PlayerPacket playerPacket = RpgClientConnection.Instance.GetLocalPlayerPacket();
            if (playerPacket != null)
            {
                CombatStats stats = playerPacket.Data.GetCombinedCombatStats();
                int y = 10;
                for (int i = 0; i < 7; i++)
                {
                    string text = "";
                    switch (i)
                    {
                        case 0:
                            text = "Vitality: " + stats.Vitality + '\n' + "HP: " + playerPacket.Data.HP;
                            break;
                        case 1:
                            text = "Inteligence: " + stats.Inteligence + '\n' + "MP: " + playerPacket.Data.MP;
                            break;
                        case 2:
                            text = "Strength: " + stats.Strength;
                            break;
                        case 3:
                            text = "Agility: " + stats.Agility + '\n' + "Stamina: " + playerPacket.Data.Stamina;
                            break;
                        case 4:
                            text = "Melee Defence: " + stats.MeleeDefence;
                            break;
                        case 5:
                            text = "Range Defence: " + stats.RangeDefence;
                            break;
                        case 6:
                            text = "Magic Defence: " + stats.MagicDefence;
                            break;

                    }
                    Vector3 pos = new Vector3(10, y, 0);
                    Color4 colour = Color4.White;
                    Renderer.PrintText(text, ref pos, ref colour);
                    y += 10 + Renderer.GetFont().GetTextHeight(text);
                }
            }
        }
    }
}

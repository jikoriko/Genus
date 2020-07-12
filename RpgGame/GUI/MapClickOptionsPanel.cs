using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using Genus2D.Networking;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{
    public class MapClickOption
    {
        public enum OptionType
        {
            PickupItem,
            AttackPlayer,
            AttackEnemy
        }

        public OptionType Option;
        public string Label;
        public Dictionary<string, object> Parameters;

        public MapClickOption(OptionType option, string label)
        {
            Option = option;
            Parameters = new Dictionary<string, object>();
            Label = label;

            switch (option)
            {
                case OptionType.PickupItem:
                    Parameters.Add("MapItem", null);
                    Parameters.Add("ItemIndex", -1);
                    break;
                case OptionType.AttackPlayer:
                    Parameters.Add("PlayerID", -1);
                    break;
                case OptionType.AttackEnemy:
                    Parameters.Add("EnemyID", -1);
                    break;
            }
        }
    }

    public class MapClickOptionsPanel : Panel
    {

        private List<MapClickOption> _options;

        public MapClickOptionsPanel(int x, int y, List<MapClickOption> options, GameState state)
            : base(x - 100, y, 200, 0, BarMode.Empty, state)
        {
            _options = options;
            SetMargin(2);
            SetBackgroundGradientMode(Renderer.GradientMode.None);
            SetContentSize(GetContentWidth(), options.Count * 32);
            _cornerRadius = 0;

            OnTrigger += Trigger;
        }

        private void Trigger()
        {
            int option = (int)Math.Floor(GetLocalMousePosition().Y / 32);
            if (option >= 0 && option < _options.Count)
            {
                MapClickOption clickOption = _options[option];
                ClientCommand command;
                switch (clickOption.Option)
                {
                    case MapClickOption.OptionType.PickupItem:
                        MapItem item = (MapItem)clickOption.Parameters["MapItem"];
                        command = new ClientCommand(ClientCommand.CommandType.PickupItem);
                        command.SetParameter("ItemIndex", clickOption.Parameters["ItemIndex"]);
                        command.SetParameter("Signature", item.GetSignature());
                        RpgClientConnection.Instance.AddClientCommand(command);
                        break;
                    case MapClickOption.OptionType.AttackPlayer:
                        command = new ClientCommand(ClientCommand.CommandType.AttackPlayer);
                        command.SetParameter("PlayerID", clickOption.Parameters["PlayerID"]);
                        RpgClientConnection.Instance.AddClientCommand(command);
                        break;
                    case MapClickOption.OptionType.AttackEnemy:
                        command = new ClientCommand(ClientCommand.CommandType.AttackEnemy);
                        command.SetParameter("EnemyID", clickOption.Parameters["EnemyID"]);
                        RpgClientConnection.Instance.AddClientCommand(command);
                        break;
                }
            }

            this.Close();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!BodySelectable())
                this.Close();
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int y = 0;

            int option = (int)Math.Floor(GetLocalMousePosition().Y / 32);

            for (int i = 0; i < _options.Count; i++)
            {
                Vector3 pos;
                Color4 color;
                if (option == i && ContentSelectable())
                {
                    pos = new Vector3(0, y, 0);
                    Vector3 size = new Vector3(GetContentWidth(), 32, 1);
                    color = Color4.Yellow;
                    color.A = 0.5f;
                    Renderer.FillShape(ShapeFactory.Rectangle, ref pos, ref size, ref color);
                }

                string text = _options[i].Label;
                pos = new Vector3((GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(text) / 2), y, 0);
                color = Color4.White;
                Renderer.PrintText(text, ref pos, ref color);

                y += 32;

            }
        }
    }
}

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
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{

    public class ItemClickOptionsPanel : Panel
    {

        public enum OptionType
        {
            None,
            Drop,
            AddTrade,
            RemoveTrade,
            Sell,
            Buy,
            AddBank,
            RemoveBank
        }

        private static string[] _labels =
        {
            "Drop",
            "Trade",
            "Remove",
            "Sell",
            "Buy",
            "Bank",
            "Remove"
        };
        
        private static int[] _counts = { 1, 5, 10, 50, 100, 500, 1000 };

        private OptionType _optionType;
        private List<string> _options;
        private int _itemIndex;
        private int _itemID;
        private int _max;

        public ItemClickOptionsPanel(int x, int y, OptionType optionType, int itemIndex, int itemID, int max, GameState state)
            : base(x - 150, y, 300, 0, BarMode.Empty, state)
        {
            _optionType = optionType;
            _options = new List<string>();
            _itemIndex = itemIndex;
            _itemID = itemID;
            _max = max;

            for (int i = 0; i < _counts.Length; i++)
            {
                _options.Add(_labels[((int)optionType) - 1] + " " + _counts[i]);
            }

            if (max > 0)
                _options.Add(_labels[((int)optionType) - 1] + " All");

            SetMargin(2);
            SetBackgroundGradientMode(Renderer.GradientMode.None);
            SetContentSize(GetContentWidth(), _options.Count * 32);
            _cornerRadius = 0;

            OnTrigger += Trigger;
        }

        private void Trigger()
        {
            int option = (int)Math.Floor(GetLocalMousePosition().Y / 32);
            if (option >= 0 && option < _options.Count)
            {
                ClientCommand command = null;

                int count;
                if (option == _options.Count - 1 && _max > 0)
                    count = _max;
                else
                    count = _counts[option];

                if (count > _max && _max > 0)
                    count = _max;

                if (_optionType == OptionType.Drop)
                {
                    command = new ClientCommand(ClientCommand.CommandType.DropItem);
                }
                else if (_optionType == OptionType.AddTrade)
                {
                    int added = TradePanel.Instance.TradeRequest.TradeOffer1.AddItem(_itemID, count);
                    if (added > 0)
                    {
                        command = new ClientCommand(ClientCommand.CommandType.AddTradeItem);
                        TradePanel.Instance.TradeRequest.TradeOffer1.Accepted = false;
                        TradePanel.Instance.TradeRequest.TradeOffer2.Accepted = false;
                    }

                }
                else if (_optionType == OptionType.RemoveTrade)
                {
                    //need to refine this so its like adding
                    int removed = TradePanel.Instance.TradeRequest.TradeOffer1.RemoveItem(_itemIndex, count);
                    if (removed > 0)
                    {
                        command = new ClientCommand(ClientCommand.CommandType.RemoveTradeItem);
                    }
                }
                else if (_optionType == OptionType.Sell)
                {
                    command = new ClientCommand(ClientCommand.CommandType.SellShopItem);
                }
                else if (_optionType == OptionType.Buy)
                {
                    command = new ClientCommand(ClientCommand.CommandType.BuyShopItem);
                }
                else if (_optionType == OptionType.AddBank)
                {
                    command = new ClientCommand(ClientCommand.CommandType.AddBankItem);
                }
                else if (_optionType == OptionType.RemoveBank)
                {
                    command = new ClientCommand(ClientCommand.CommandType.RemoveBankItem);
                }

                if (command != null)
                {
                    command.SetParameter("ItemIndex", _itemIndex);
                    command.SetParameter("Count", count);
                    RpgClientConnection.Instance.AddClientCommand(command);
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

                string text = _options[i];
                pos = new Vector3((GetContentWidth() / 2) - (Renderer.GetFont().GetTextWidth(text) / 2), y, 0);
                color = Color4.White;
                Renderer.PrintText(text, ref pos, ref color);

                y += 32;

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor.CommandDataPresets
{
    public partial class ShowOptionsPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;
        private List<Genus2D.GameData.MessageOption> _messageOptions;

        public ShowOptionsPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();

            _command = command;

            MessageTextBox.Text = (string)command.GetParameter("Message");

            _messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
            for (int i = 0; i < _messageOptions.Count; i++)
            {
                MessageOptions.Items.Add(_messageOptions[i].Option);
            }

            List<string> eventOptions = Genus2D.GameData.EventData.GetEventsDataNames();
            eventOptions.Insert(0, "None");
            OptionEvents.Items.AddRange(eventOptions.ToArray());


        }

        private void MessageOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MessageOptions.SelectedIndex != -1)
            {
                OptionNameBox.Text = _messageOptions[MessageOptions.SelectedIndex].Option;
                OptionEvents.SelectedIndex = _messageOptions[MessageOptions.SelectedIndex].OptionEventID + 1;
            }
            else
            {
                OptionNameBox.Text = "";
                OptionEvents.SelectedIndex = -1;
            }
        }

        private void RemoveOptionButton_Click(object sender, EventArgs e)
        {
            if (MessageOptions.SelectedIndex != -1)
            {
                _messageOptions.RemoveAt(MessageOptions.SelectedIndex);
                MessageOptions.Items.RemoveAt(MessageOptions.SelectedIndex);
                MessageOptions.SelectedIndex = -1;
                MessageOptions.Text = "";
                OptionEvents.SelectedIndex = 0;
            }
        }

        private void AddOptionButton_Click(object sender, EventArgs e)
        {
            if (OptionNameBox.Text != "")
            {
                for (int i = 0; i < _messageOptions.Count; i++)
                {
                    if (_messageOptions[i].Option == OptionNameBox.Text)
                    {
                        MessageBox.Show("Option already exists");
                        return;
                    }
                }

                Genus2D.GameData.MessageOption option = new Genus2D.GameData.MessageOption();
                option.Option = OptionNameBox.Text;
                _messageOptions.Add(option);
                MessageOptions.Items.Add(OptionNameBox.Text);
                MessageOptions.SelectedIndex = MessageOptions.Items.Count - 1;
            }
        }

        private void ChangeOptionButton_Click(object sender, EventArgs e)
        {
            if (MessageOptions.SelectedIndex != -1)
            {
                if (OptionNameBox.Text != "")
                {
                    for (int i = 0; i < _messageOptions.Count; i++)
                    {
                        if (_messageOptions[i].Option == OptionNameBox.Text)
                        {
                            MessageBox.Show("Option already exists");
                            return;
                        }
                    }

                    _messageOptions[MessageOptions.SelectedIndex].Option = OptionNameBox.Text;
                    MessageOptions.Items[MessageOptions.SelectedIndex] = OptionNameBox.Text;
                }
            }
        }

        public void ApplyData()
        {
            _command.SetParameter("Message", MessageTextBox.Text);
        }
    }
}

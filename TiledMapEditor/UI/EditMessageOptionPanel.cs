using Genus2D.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.GameData;

namespace TiledMapEditor.UI
{
    public class EditMessageOptionPanel : Panel
    {
        private EditCommandPanel _commandPanel;
        private MessageOption _messageOption;

        private TextField _optionNameField;
        private DropDownBox _eventIdSelection;
        private Button _applyButton;

        public EditMessageOptionPanel(EditCommandPanel commandPanel, MessageOption option, State state)
             : base((int)(Renderer.GetResoultion().X / 2) - 150, (int)(Renderer.GetResoultion().Y / 2) - 150, 300, 300, BarMode.Close_Drag, state)
        {
            _commandPanel = commandPanel;
            _messageOption = option;

            _optionNameField = new TextField(10, 10, GetContentWidth() - 20, 40, state);
            _optionNameField.SetText(option.Option);

            List<string> eventNames = MapEventData.GetMapEventsDataNames();
            eventNames.Insert(0, "None");
            _eventIdSelection = new DropDownBox(10, 60, GetContentWidth() - 20, eventNames.ToArray(), state);

            _applyButton = new Button("Apply", 10, GetContentHeight() - 50, GetContentWidth() - 20, 40, state);
            _applyButton.OnTrigger += ApplyTrigger;

            this.AddControl(_optionNameField);
            this.AddControl(_eventIdSelection);
            this.AddControl(_applyButton);
        }

        private void ApplyTrigger()
        {
            _messageOption.Option = _optionNameField.GetText();
            _messageOption.OptionEventID = _eventIdSelection.GetSelection() - 1;
            _commandPanel.RefreshMessageOptions();
        }
    }
}

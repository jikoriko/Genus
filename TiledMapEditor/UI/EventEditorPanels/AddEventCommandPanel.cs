using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapEditor.States;

namespace TiledMapEditor.UI
{
    public class AddEventCommandPanel : Panel
    {

        private MapEventData _eventData;

        private DropDownBox _commandTypeSelection;
        private Button _addCommandButton;

        public AddEventCommandPanel(State state, MapEventData eventData) 
            : base((int)(Renderer.GetResoultion().X / 2) - 150, (int)(Renderer.GetResoultion().Y / 2) - 150, 300, 300, BarMode.Close_Drag, state)
        {
            SetPanelLabel("Add Event Command");

            _eventData = eventData;

            string[] commands = Enum.GetNames(typeof(EventCommand.CommandType));
            _commandTypeSelection = new DropDownBox(10, 10, GetContentWidth() - 20, commands, state);
            _commandTypeSelection.SetMaxItemsVisible(6);

            _addCommandButton = new Button("Add Command", 10, GetContentHeight() - 50, GetContentWidth() - 20, 40, state);
            _addCommandButton.OnTrigger += AddCommandTrigger;

            this.AddControl(_commandTypeSelection);
            this.AddControl(_addCommandButton);
        }

        private void AddCommandTrigger()
        {
            int selection = _commandTypeSelection.GetSelection();
            _eventData.AddEventCommand((EventCommand.CommandType)selection);
            if (EditEventPanel.INSTANCE != null)
                EditEventPanel.INSTANCE.RefreshEventCommands();
            MapEventData.SaveMapEventsData();
            this.Close();
            
        }
    }
}

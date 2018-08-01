using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiledMapEditor.UI
{
    public class EditMapEventPanel : Panel
    {
        private MapEvent _mapEvent;

        private DropDownBox _eventSelectionBox;
        private Button _applyButton;

        public EditMapEventPanel(State state, MapEvent mapEvent) 
            : base((int)(Renderer.GetResoultion().X / 2) - 150, (int)(Renderer.GetResoultion().Y / 2) - 150, 300, 300, BarMode.Close_Drag, state)
        {
            SetPanelLabel("Add Event Command");

            _mapEvent = mapEvent;

            _eventSelectionBox = new DropDownBox(10, 10, GetContentWidth() - 20, MapEventData.GetMapEventsDataNames().ToArray(), state);
            _eventSelectionBox.SetMaxItemsVisible(6);

            _applyButton = new Button("Apply", 10, GetContentHeight() - 50, GetContentWidth() - 20, 40, state);
            _applyButton.OnTrigger += ApplyTrigger;

            this.AddControl(_eventSelectionBox);
            this.AddControl(_applyButton);
        }

        private void ApplyTrigger()
        {
            int selection = _eventSelectionBox.GetSelection();
            _mapEvent.EventID = selection;
            this.Close();

        }
    }
}

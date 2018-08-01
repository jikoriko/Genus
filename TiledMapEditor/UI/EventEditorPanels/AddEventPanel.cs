using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using System;
using TiledMapEditor.States;

namespace TiledMapEditor.UI
{
    public class AddEventPanel : Panel
    {

        private TextField _eventNameField;
        private Button _addEventButton;

         public AddEventPanel(EditorState state) 
            : base((int)(Renderer.GetResoultion().X / 2) - 100, (int)(Renderer.GetResoultion().Y / 2) - 100, 200, 200, BarMode.Close_Drag, state)
        {
            SetPanelLabel("Add Event");

            _eventNameField = new TextField(10, 10, GetContentWidth() - 20, 40, state);
            _addEventButton = new Button("Add", 10, 60, GetContentWidth() - 20, 40, state);
            _addEventButton.OnTrigger += AddEventTrigger;

            this.AddControl(_eventNameField);
            this.AddControl(_addEventButton);
        }

        private void AddEventTrigger()
        {
            string name = _eventNameField.GetText();
            if (name != "")
            {
                MapEventData.AddMapEventData(name);
                if (MapEventsPanel.Instance != null)
                    MapEventsPanel.Instance.RefreshEvents();
                this.Close();
            }
        }
    }
}

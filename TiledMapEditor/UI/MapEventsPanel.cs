using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;

using Genus2D.GUI;
using Genus2D.Graphics;
using Genus2D.GameData;

using TiledMapEditor.States;

namespace TiledMapEditor.UI
{

    public class MapEventsPanel : Panel
    {
        
        private static MapEventsPanel _instance = null;
        public static MapEventsPanel Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }
        private EditorState _editorState; 

        private ListBox _eventsListBox;
        private Button _addEventButton, _deleteEventButton,
                        _editEventButton;

        public MapEventsPanel(EditorState state)
            : base((int)(Renderer.GetResoultion().X / 2) - 300, (int)(Renderer.GetResoultion().Y / 2) - 300, 600, 600, BarMode.Close_Drag, state)
        {
            if (_instance != null)
                _instance.Close();
            _instance = this;

            _editorState = state;
            this.SetPanelLabel("Map Events");

            _eventsListBox = new ListBox(10, 10, (GetContentWidth() / 2) - 20, GetContentHeight() - 20, MapEventData.GetMapEventsDataNames(), state);

            _addEventButton = new Button("Add Event", (GetContentWidth() / 2) + 10, 10, (GetContentWidth() / 2) - 20, 40, state);
            _addEventButton.OnTrigger += CreateEventTrigger;
            _deleteEventButton = new Button("Delete Event", (GetContentWidth() / 2) + 10, 60, (GetContentWidth() / 2) - 20, 40, state);
            _deleteEventButton.OnTrigger += DeleteEventTrigger;
            _editEventButton = new Button("Edit Event", (GetContentWidth() / 2) + 10, 110, (GetContentWidth() / 2) - 20, 40, state);
            _editEventButton.OnTrigger += EditEventTrigger;

            this.AddControl(_eventsListBox);
            this.AddControl(_addEventButton);
            this.AddControl(_deleteEventButton);
            this.AddControl(_editEventButton);
        }

        public void RefreshEvents()
        {
            _eventsListBox.GetItems().Clear();
            _eventsListBox.SetSelection(-1);

            List<string> eventNames = MapEventData.GetMapEventsDataNames();
            for (int i = 0; i < eventNames.Count; i++)
            {
                _eventsListBox.GetItems().Add(eventNames[i]);
            }
        }

        private void CreateEventTrigger()
        {
            AddEventPanel panel = new AddEventPanel(_editorState);
            _state.AddControl(panel);
        }

        private void DeleteEventTrigger()
        {
            int selection = _eventsListBox.GetSelection();
            MapEventData.RemoveMapEventData(selection);
            RefreshEvents();
        }

        private void EditEventTrigger()
        {
            int selection = _eventsListBox.GetSelection();
            if (selection != -1)
            {
                EditEventPanel panel = new EditEventPanel(_editorState, selection);
                _state.AddControl(panel);
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            //Vector3 pos = new Vector3(20, 10, 0);
            //Color4 colour = Color4.White;
            //Renderer.PrintText("Events", ref pos, ref colour);
        }

        public override void Close()
        {
            base.Close();
            base.Close();
            _instance = this;
        }

    }
}

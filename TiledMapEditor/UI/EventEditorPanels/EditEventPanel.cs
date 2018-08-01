using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using TiledMapEditor.States;

namespace TiledMapEditor.UI
{
    public class EditEventPanel : Panel
    {
        public static EditEventPanel INSTANCE { get; private set; }
        private MapEventData _eventData;

        private TextField _eventNameField;
        private DropDownBox _triggerTypeSelectionBox;
        private RadioButton _passableRadioButton;
        private Button _applyButton;

        private ListBox _eventCommandsListBox;
        private Button _addCommandButton, _removeCommandButton, _editCommandButton;

        public EditEventPanel(State state, int eventID)
           : base((int)(Renderer.GetResoultion().X / 2) - 300, (int)(Renderer.GetResoultion().Y / 2) - 300, 600, 600, BarMode.Close_Drag, state)
        {
            if (INSTANCE != null)
                INSTANCE.Close();
            INSTANCE = this;
            SetPanelLabel("Edit Event");

            _eventData = MapEventData.GetMapEventData(eventID);

            _eventNameField = new TextField(10, 10, 190, 40, state);
            _eventNameField.SetText(_eventData.Name);
            _applyButton = new Button("Apply", 10, GetContentHeight() - 50, 190, 40, state);
            _applyButton.OnTrigger += ApplyTrigger;

            _triggerTypeSelectionBox = new DropDownBox(10, 60, 190, Enum.GetNames(typeof(MapEventData.TriggerType)), state);
            _triggerTypeSelectionBox.SetMaxItemsVisible(3);
            _triggerTypeSelectionBox.SetSelection((int)_eventData.GetTriggerType());

            _passableRadioButton = new RadioButton(180, 170, state);
            _passableRadioButton.SetCheck(_eventData.Passable());

            _eventCommandsListBox = new ListBox(210, 10, GetContentWidth() - 220, GetContentHeight() -190, 0, state);

            _addCommandButton = new Button("Add Command", 210, GetContentHeight() - 150, GetContentWidth() - 220, 40, state);
            _addCommandButton.OnTrigger += AddCommandTrigger;
            _removeCommandButton = new Button("Remove Command", 210, GetContentHeight() - 100, GetContentWidth() - 220, 40, state);
            _removeCommandButton.OnTrigger += RemoveCommandTrigger;
            _editCommandButton = new Button("Edit Command", 210, GetContentHeight() - 50, GetContentWidth() - 220, 40, state);
            _editCommandButton.OnTrigger += EditCommandTrigger;

            this.AddControl(_eventNameField);
            this.AddControl(_triggerTypeSelectionBox);
            this.AddControl(_passableRadioButton);
            this.AddControl(_applyButton);

            this.AddControl(_eventCommandsListBox);
            this.AddControl(_addCommandButton);
            this.AddControl(_removeCommandButton);
            this.AddControl(_editCommandButton);

            RefreshEventCommands();
        }

        private void ApplyTrigger()
        {
            string name = _eventNameField.GetText();
            if (name != "")
            {
                _eventData.Name = name;
                _eventData.SetTriggerType((MapEventData.TriggerType)_triggerTypeSelectionBox.GetSelection());
                _eventData.SetPassable(_passableRadioButton.IsChecked());
                MapEventData.SaveMapEventsData();
                if (MapEventsPanel.Instance != null)
                    MapEventsPanel.Instance.RefreshEvents();
            }
        }

        public void RefreshEventCommands()
        {
            _eventCommandsListBox.GetItems().Clear();
            _eventCommandsListBox.SetSelection(-1);

            List<string> commandNames = _eventData.GetEventCommandStrings();
            for (int i = 0; i < commandNames.Count; i++)
            {
                _eventCommandsListBox.GetItems().Add(commandNames[i]);
            }
        }

        private void AddCommandTrigger()
        {
            AddEventCommandPanel panel = new AddEventCommandPanel(_state, _eventData);
            _state.AddControl(panel);
        }

        private void RemoveCommandTrigger()
        {
            int selection = _eventCommandsListBox.GetSelection();
            _eventData.RemoveEventCommand(selection);
            MapEventData.SaveMapEventsData();
            RefreshEventCommands();
        }

        private void EditCommandTrigger()
        {
            int selection = _eventCommandsListBox.GetSelection();
            if (selection != -1)
            {
                EditCommandPanel panel = new EditCommandPanel(_state, _eventData.EventCommands[selection]);
                _state.AddControl(panel);
            }
        }

        public override void Close()
        {
            base.Close();
            INSTANCE = null;
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = new Vector3(80, 172, 0);
            Color4 colour = Color4.White;

            Renderer.PrintText("Passable:", ref pos, ref colour);
        }
    }
}

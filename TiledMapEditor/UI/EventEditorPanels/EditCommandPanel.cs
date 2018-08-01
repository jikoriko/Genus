using Genus2D.Core;
using Genus2D.GameData;
using Genus2D.Graphics;
using Genus2D.GUI;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiledMapEditor.UI
{
    public class EditCommandPanel : ScrollPanel
    {

        private EventCommand _eventCommand;

        private List<Control> _parameterFields;
        private Button _applyButton;

        public EditCommandPanel(State state, EventCommand command) 
            : base((int)(Renderer.GetResoultion().X / 2) - 200, (int)(Renderer.GetResoultion().Y / 2) - 200, 400, 400, BarMode.Close_Drag, state)
        {
            SetPanelLabel("Edit Event Command");
            this.DisableHorizontalScroll();

            _eventCommand = command;

            _parameterFields = new List<Control>();

            TextField parameterField;

            switch (_eventCommand.Type)
            {
                case EventCommand.CommandType.MovePlayer:

                    DropDownBox directionSelectionBox = new DropDownBox((GetContentWidth() / 2) + 10, 10, (GetContentWidth() / 2) - 20, Enum.GetNames(typeof(Direction)), state);
                    directionSelectionBox.SetSelection((int)((Direction)_eventCommand.GetParameter(0)));
                    this.AddControl(directionSelectionBox);
                    _parameterFields.Add(directionSelectionBox);

                    break;

                case EventCommand.CommandType.ShowOptions:

                    parameterField = new TextField((GetContentWidth() / 2) + 10, 10, (GetContentWidth() / 2) - 20, 40, state);
                    parameterField.SetText(_eventCommand.GetParameter(0).ToString());
                    this.AddControl(parameterField);
                    _parameterFields.Add(parameterField);

                    ListBox listBox = new ListBox((GetContentWidth() / 2) + 10, 60, (GetContentWidth() / 2) - 20, 100, 0, state);
                    this.AddControl(listBox);
                    _parameterFields.Add(listBox);

                    Button addOptionButton = new Button("Add Option", 10, 170, GetContentWidth() - 20, 40, state);
                    addOptionButton.OnTrigger += AddMessageOptionTrigger;
                    this.AddControl(addOptionButton);
                    _parameterFields.Add(addOptionButton);

                    Button removeOptionButton = new Button("Remove option", 10, 220, GetContentWidth() - 20, 40, state);
                    removeOptionButton.OnTrigger += RemoveMessageOptionTrigger;
                    this.AddControl(removeOptionButton);
                    _parameterFields.Add(removeOptionButton);

                    Button editOptionButton = new Button("Edit Option", 10, 270, GetContentWidth() - 20, 40, state);
                    editOptionButton.OnTrigger += EditMessageOptionTrigger;
                    this.AddControl(editOptionButton);
                    _parameterFields.Add(editOptionButton);

                    RefreshMessageOptions();

                    break;

                default:

                    for (int i = 0; i < _eventCommand.NumParameters(); i++)
                    {
                         parameterField = new TextField((GetContentWidth() / 2) + 10, 10 + (i * 50), (GetContentWidth() / 2) - 20, 40, state);
                        parameterField.SetText(_eventCommand.GetParameter(i).ToString());
                        this.AddControl(parameterField);
                        _parameterFields.Add(parameterField);
                    }

                    break;
            }

            int contentHeight = 60;
            for (int i = 0; i < _parameterFields.Count; i++)
            {
                contentHeight += (int)_parameterFields[i].GetBodySize().Y + 10;
            }
            SetScrollableHeight(contentHeight);

            _applyButton = new Button("Apply", 10, contentHeight - 50, GetContentWidth() - 20, 40, state);
            _applyButton.OnTrigger += ApplyTrigger;
            this.AddControl(_applyButton);
        }

        private void AddMessageOptionTrigger()
        {
            List<MessageOption> options = (List<MessageOption>)_eventCommand.GetParameter(1);
            MessageOption option = new MessageOption();
            options.Add(option);
            RefreshMessageOptions();
        }

        private void RemoveMessageOptionTrigger()
        {
            ListBox listBox = (ListBox)_parameterFields[1];
            int selection = listBox.GetSelection();
            if (selection > -1)
            {
                List<MessageOption> options = (List<MessageOption>)_eventCommand.GetParameter(1);
                listBox.GetItems().RemoveAt(selection);
                options.RemoveAt(selection);
            }
        }

        private void EditMessageOptionTrigger()
        {
            ListBox listBox = (ListBox)_parameterFields[1];
            int selection = listBox.GetSelection();
            if (selection > -1)
            {
                List<MessageOption> options = (List<MessageOption>)_eventCommand.GetParameter(1);
                EditMessageOptionPanel panel = new EditMessageOptionPanel(this, options[selection], _state);
                _state.AddControl(panel);
            }
        }

        public void RefreshMessageOptions()
        {
            if (_eventCommand.Type == EventCommand.CommandType.ShowOptions)
            {
                ListBox listBox = (ListBox)_parameterFields[1];
                listBox.GetItems().Clear();

                List<MessageOption> options = (List<MessageOption>)_eventCommand.GetParameter(1);

                for (int i = 0; i < options.Count; i++)
                {
                    listBox.GetItems().Add("Option: " + options[i].Option);
                }
            }
        }

        private void ApplyTrigger()
        {
            //string error = "";
            try
            {
                switch (_eventCommand.Type)
                {
                    case EventCommand.CommandType.EventWaitTimer:
                        float time = float.Parse(((TextField)_parameterFields[0]).GetText());
                        _eventCommand.SetParameter("Time", time);
                        break;
                    case EventCommand.CommandType.MapTransfer:
                        int mapID = int.Parse(((TextField)_parameterFields[0]).GetText());
                        int mapX = int.Parse(((TextField)_parameterFields[1]).GetText());
                        int mapY = int.Parse(((TextField)_parameterFields[2]).GetText());
                        _eventCommand.SetParameter("MapID", mapID);
                        _eventCommand.SetParameter("MapX", mapX);
                        _eventCommand.SetParameter("MapY", mapY);
                        break;
                    case EventCommand.CommandType.MovePlayer:
                        Direction direction = (Direction)(((DropDownBox)_parameterFields[0]).GetSelection());
                        _eventCommand.SetParameter("Direction", direction);
                        break;
                    case EventCommand.CommandType.ShowMessage:
                        _eventCommand.SetParameter("Message", ((TextField)_parameterFields[0]).GetText());
                        break;
                    case EventCommand.CommandType.ShowOptions:
                        _eventCommand.SetParameter("Message", ((TextField)_parameterFields[0]).GetText());
                        break;
                }
                MapEventData.SaveMapEventsData();
            }
            catch
            {
                //show an error box
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = new Vector3(10, 0, 0);
            Color4 color = Color4.White;

            for (int i = 0; i < _eventCommand.NumParameters(); i++)
            {
                string text = _eventCommand.GetParameterName(i) + ":";
                pos.Y = 10 + (i * 50) + ((40 - Renderer.GetFont().GetTextHeight(text)) / 2);
                Renderer.PrintText(text, ref pos, ref color);

            }
        }

    }
}

using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgServer
{
    public class EventInterpreter
    {
        private GameClient _gameClient;

        private MapEventData _triggeringEvent;
        private int _triggeringEventCommand;
        private float _triggeringEventWaitTimer;
        private bool _messageShowing;
        private bool _optionsShowing;
        private int _selectedOption;
        private int _optionsCount;

        public EventInterpreter(GameClient gameClient)
        {
            _gameClient = gameClient;

            _triggeringEvent = null;
            _triggeringEventCommand = -1;

            _triggeringEventWaitTimer = 0.0f;
            _messageShowing = false;
            _optionsShowing = false;
            _selectedOption = -1;
            _optionsCount = 0;
        }

        public void Update(float deltaTime)
        {
            if (EventTriggering())
            {
                if (_messageShowing)
                {
                    if (_gameClient.KeyDown(OpenTK.Input.Key.Enter))
                    {
                        _gameClient.AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                        _messageShowing = false;
                    }

                    return;
                }
                else if (_optionsShowing)
                {
                    ClientCommand clientCommand;

                    if (_gameClient.KeyDown(OpenTK.Input.Key.Space))
                    {
                        //get the selected option and trigger event

                        _optionsShowing = false;
                        _selectedOption = -1;
                        _optionsCount = 0;
                        _gameClient.AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                    }
                    else if (_gameClient.KeyDown(OpenTK.Input.Key.Down))
                    {
                        _selectedOption++;
                        if (_selectedOption >= _optionsCount)
                            _selectedOption = 0;

                        clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                        clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());
                        _gameClient.AddClientCommand(clientCommand);
                    }
                    if (_gameClient.KeyDown(OpenTK.Input.Key.Up))
                    {
                        _selectedOption--;
                        if (_selectedOption < 0)
                            _selectedOption = _optionsCount - 1;

                        clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                        clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());
                        _gameClient.AddClientCommand(clientCommand);
                    }

                    return;
                }

                if (_triggeringEventWaitTimer > 0)
                {
                    _triggeringEventWaitTimer -= deltaTime;
                    return;
                }

                if (_triggeringEventCommand < _triggeringEvent.EventCommands.Count - 1)
                {
                    _triggeringEventCommand++;
                    EventCommand eventCommand = _triggeringEvent.EventCommands[_triggeringEventCommand];

                    ClientCommand clientCommand;
                    int mapID;
                    int eventID;
                    int mapX;
                    int mapY;
                    Direction direction;

                    switch (eventCommand.Type)
                    {
                        case EventCommand.CommandType.WaitTimer:

                            float timer = (float)eventCommand.GetParameter("Time");
                            _triggeringEventWaitTimer = timer;

                            break;
                        case EventCommand.CommandType.TeleportPlayer:

                            mapID = (int)eventCommand.GetParameter("MapID");
                            mapX = (int)eventCommand.GetParameter("MapX");
                            mapY = (int)eventCommand.GetParameter("MapY");
                            _gameClient.SetMapPosition(mapX, mapY);
                            _gameClient.SetMapID(mapID);

                            break;
                        case EventCommand.CommandType.MovePlayer:

                            if (_gameClient.Moving())
                            {
                                _triggeringEventCommand--;
                                break;
                            }
                            direction = (Direction)eventCommand.GetParameter("Direction");
                            _gameClient.Move(direction);

                            break;
                        case EventCommand.CommandType.ChangePlayerDirection:

                            direction = (Direction)eventCommand.GetParameter("Direction");
                            _gameClient.ChangeDirection(direction);

                            break;
                        case EventCommand.CommandType.TeleportMapEvent:

                            mapID = (int)eventCommand.GetParameter("MapID");
                            eventID = (int)eventCommand.GetParameter("EventID");
                            mapX = (int)eventCommand.GetParameter("MapX");
                            mapY = (int)eventCommand.GetParameter("MapY");
                            Server.Instance.GetMapInstance(mapID).TeleportMapEvent(eventID, mapX, mapY);

                            break;
                        case EventCommand.CommandType.MoveMapEvent:

                            mapID = (int)eventCommand.GetParameter("MapID");
                            eventID = (int)eventCommand.GetParameter("EventID");
                            if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                            {
                                _triggeringEventCommand--;
                                break;
                            }
                            direction = (Direction)eventCommand.GetParameter("Direction");
                            Server.Instance.GetMapInstance(mapID).MoveMapEvent(eventID, direction);

                            break;
                        case EventCommand.CommandType.ChangeMapEventDirection:

                            mapID = (int)eventCommand.GetParameter("MapID");
                            eventID = (int)eventCommand.GetParameter("EventID");
                            direction = (Direction)eventCommand.GetParameter("Direction");
                            Server.Instance.GetMapInstance(mapID).ChangeMapEventDirection(eventID, direction);

                            break;
                        case EventCommand.CommandType.ShowMessage:

                            clientCommand = new ClientCommand(ClientCommand.CommandType.ShowMessage);
                            clientCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                            _gameClient.AddClientCommand(clientCommand);
                            _messageShowing = true;

                            break;
                        case EventCommand.CommandType.ShowOptions:

                            List<MessageOption> options = (List<MessageOption>)eventCommand.GetParameter("Options");
                            _selectedOption = 0;
                            _optionsCount = options.Count;

                            string optionStrings = "";
                            for (int i = 0; i < options.Count; i++)
                            {
                                optionStrings += options[i].Option;
                                if (i < options.Count - 1)
                                    optionStrings += ",";
                            }

                            clientCommand = new ClientCommand(ClientCommand.CommandType.ShowOptions);
                            clientCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                            clientCommand.SetParameter("Options", optionStrings);
                            clientCommand.SetParameter("SelectedOption", _selectedOption.ToString());

                            _gameClient.AddClientCommand(clientCommand);
                            _optionsShowing = true;

                            break;
                    }
                }
                else
                {
                    _triggeringEvent = null;
                    _triggeringEventCommand = -1;
                }
            }
        }

        public bool EventTriggering()
        {
            return _triggeringEvent != null;
        }

        public void TriggerEventData(MapEventData data)
        {
            if (_triggeringEvent == null)
            {
                _triggeringEvent = data;
            }
        }



    }
}

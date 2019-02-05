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
        private List<TriggeringEvent> _triggeringEvents;

        public EventInterpreter()
        {
            _triggeringEvents = new List<TriggeringEvent>();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _triggeringEvents.Count; i++)
            {
                TriggerEvent(_triggeringEvents[i], deltaTime);
                if (_triggeringEvents[i].Complete)
                {
                    _triggeringEvents.RemoveAt(i);
                    i--;
                }
            }
        }

        private void TriggerEvent(TriggeringEvent triggeringEvent, float deltaTime)
        {
            GameClient client = triggeringEvent.GetGameClient();
            EventData eventData = triggeringEvent.GetEventData(); ;

            if (triggeringEvent.MessageShowing)
            {
                if (client.KeyDown(OpenTK.Input.Key.Enter))
                {
                    client.AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                    triggeringEvent.MessageShowing = false;
                    client.MovementDisabled = false;
                }

                return;
            }
            else if (triggeringEvent.OptionsShowing)
            {
                ClientCommand clientCommand;

                if (client.KeyDown(OpenTK.Input.Key.Space))
                {
                    //get the selected option and trigger event

                    triggeringEvent.OptionsShowing = false;
                    client.MovementDisabled = false;
                    triggeringEvent.SelectedOption = -1;
                    triggeringEvent.OptionsCount = 0;
                    client.AddClientCommand(new ClientCommand(ClientCommand.CommandType.CloseMessage));
                }
                else if (client.KeyDown(OpenTK.Input.Key.Down))
                {
                    triggeringEvent.SelectedOption++;
                    if (triggeringEvent.SelectedOption >= triggeringEvent.OptionsCount)
                        triggeringEvent.SelectedOption = 0;

                    clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                    clientCommand.SetParameter("SelectedOption", triggeringEvent.SelectedOption.ToString());
                    client.AddClientCommand(clientCommand);
                }
                if (client.KeyDown(OpenTK.Input.Key.Up))
                {
                    triggeringEvent.SelectedOption--;
                    if (triggeringEvent.SelectedOption < 0)
                        triggeringEvent.SelectedOption = triggeringEvent.OptionsCount - 1;

                    clientCommand = new ClientCommand(ClientCommand.CommandType.UpdateOptions);
                    clientCommand.SetParameter("SelectedOption", triggeringEvent.SelectedOption.ToString());
                    client.AddClientCommand(clientCommand);
                }

                return;
            }

            if (triggeringEvent.WaitTimer > 0)
            {
                triggeringEvent.WaitTimer -= deltaTime;
                return;
            }

            if (triggeringEvent.CommandID < eventData.EventCommands.Count - 1)
            {
                triggeringEvent.CommandID++;
                EventCommand eventCommand = eventData.EventCommands[triggeringEvent.CommandID];

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
                        triggeringEvent.WaitTimer = timer;

                        break;
                    case EventCommand.CommandType.TeleportPlayer:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        mapX = (int)eventCommand.GetParameter("MapX");
                        mapY = (int)eventCommand.GetParameter("MapY");
                        client.SetMapPosition(mapX, mapY);
                        client.SetMapID(mapID);

                        break;
                    case EventCommand.CommandType.MovePlayer:

                        if (client.Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        direction = (Direction)eventCommand.GetParameter("Direction");
                        client.Move(direction);

                        break;
                    case EventCommand.CommandType.ChangePlayerDirection:

                        direction = (Direction)eventCommand.GetParameter("Direction");
                        client.ChangeDirection(direction);

                        break;
                    case EventCommand.CommandType.TeleportMapEvent:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        mapX = (int)eventCommand.GetParameter("MapX");
                        mapY = (int)eventCommand.GetParameter("MapY");
                        if (!Server.Instance.GetMapInstance(mapID).TeleportMapEvent(eventID, mapX, mapY))
                            triggeringEvent.CommandID--;

                        break;
                    case EventCommand.CommandType.MoveMapEvent:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        direction = (Direction)eventCommand.GetParameter("Direction");
                        if (!Server.Instance.GetMapInstance(mapID).MoveMapEvent(eventID, direction))
                            triggeringEvent.CommandID--;

                        break;
                    case EventCommand.CommandType.ChangeMapEventDirection:

                        mapID = (int)eventCommand.GetParameter("MapID");
                        eventID = (int)eventCommand.GetParameter("EventID");
                        if (Server.Instance.GetMapInstance(mapID).GetMapData().GetMapEvent(eventID).Moving())
                        {
                            triggeringEvent.CommandID--;
                            break;
                        }
                        direction = (Direction)eventCommand.GetParameter("Direction");
                        Server.Instance.GetMapInstance(mapID).ChangeMapEventDirection(eventID, direction);

                        break;
                    case EventCommand.CommandType.ShowMessage:

                        clientCommand = new ClientCommand(ClientCommand.CommandType.ShowMessage);
                        clientCommand.SetParameter("Message", (string)eventCommand.GetParameter("Message"));
                        client.AddClientCommand(clientCommand);
                        triggeringEvent.MessageShowing = true;
                        client.MovementDisabled = true;

                        break;
                    case EventCommand.CommandType.ShowOptions:

                        List<MessageOption> options = (List<MessageOption>)eventCommand.GetParameter("Options");
                        triggeringEvent.SelectedOption = 0;
                        triggeringEvent.OptionsCount = options.Count;

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
                        clientCommand.SetParameter("SelectedOption", triggeringEvent.SelectedOption.ToString());

                        client.AddClientCommand(clientCommand);
                        triggeringEvent.OptionsShowing = true;
                        client.MovementDisabled = true;

                        break;
                }
            }
            else
            {
                triggeringEvent.FinishTriggering();
            }
        }

        public void TriggerEventData(GameClient client, MapEvent mapEvent)
        {
            if (mapEvent.Locked)
                return;
            _triggeringEvents.Add(new TriggeringEvent(client, mapEvent));
        }



    }
}

using Genus2D.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgServer
{
    public class TriggeringEvent
    {

        private GameClient _gameClient;
        private MapEvent _mapEvent;
        public int CommandID;

        public float WaitTimer;
        public bool MessageShowing;
        public bool OptionsShowing;
        public int SelectedOption;
        public int OptionsCount;

        public bool Complete { get; private set; }

        public TriggeringEvent(GameClient client, MapEvent mapEvent)
        {
            _gameClient = client;
            _mapEvent = mapEvent;
            CommandID = -1;

            WaitTimer = 0f;
            MessageShowing = false;
            OptionsShowing = false;
            SelectedOption = -1;
            OptionsCount = 0;

            Complete = false;
            mapEvent.Locked = true;
        }

        public EventData GetEventData()
        {
            return EventData.GetEventData(_mapEvent.EventID);
        }

        public void FinishTriggering()
        {
            Complete = true;
            _mapEvent.Locked = false;
        }

        public GameClient GetGameClient()
        {
            return _gameClient;
        }

    }
}

﻿using Genus2D.GameData;
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
        private TriggeringEvent _parent;
        public int CommandID;

        public int ConditionDepth;

        public float WaitTimer;
        public bool MessageShowing;
        public bool OptionsShowing;
        public int SelectedOption;

        public bool Complete { get; private set; }

        public TriggeringEvent(GameClient client, MapEvent mapEvent, TriggeringEvent parentEvent = null)
        {
            _gameClient = client;
            _mapEvent = mapEvent;
            _parent = parentEvent;
            CommandID = -1;

            ConditionDepth = 0;

            WaitTimer = 0f;
            MessageShowing = false;
            OptionsShowing = false;
            SelectedOption = -1;

            Complete = false;
        }

        public TriggeringEvent GetParent()
        {
            return _parent;
        }

        public MapEvent GetMapEvent()
        {
            return _mapEvent;
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

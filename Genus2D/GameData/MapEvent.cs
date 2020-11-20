using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Genus2D.GameData
{

    [Serializable]
    public class MapEvent
    {
        public string Name;
        public int EventID;
        public int MapX;
        public int MapY;
        public float RealX;
        public float RealY;
        public FacingDirection EventDirection;
        public int SpriteID;
        public EventTriggerType TriggerType;
        public bool Passable;
        public RenderPriority Priority;
        public MovementSpeed Speed;
        public MovementFrequency Frequency;
        public bool RandomMovement;

        public bool Enabled = true;
        public bool Moved = false;
        public bool Locked = false;
        public bool OnBridge = false;

        private float _frequencyTimer = 0;

        public MapEvent()
        {
            Initialize("", -1, -1, -1);
        }

        public MapEvent(string name, int id, int x, int y)
        {
            Initialize(name, id, x, y);
        }

        private void Initialize(string name, int id, int x, int y)
        {
            Name = name;
            EventID = id;
            MapX = x;
            MapY = y;
            RealX = x * 32;
            RealY = y * 32;
            EventDirection = FacingDirection.Down;
            SpriteID = -1;
            TriggerType = EventTriggerType.None;
            Passable = false;
            Priority = RenderPriority.BelowPlayer;
            Speed = MovementSpeed.Normal;
            Frequency = MovementFrequency.Normal;
            RandomMovement = false;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(Name);
                stream.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
                stream.Write(nameBytes, 0, nameBytes.Length);
                stream.Write(BitConverter.GetBytes(EventID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes((int)EventDirection), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(SpriteID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)TriggerType), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Priority), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Speed), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Enabled), 0, sizeof(bool));
                stream.Write(BitConverter.GetBytes(OnBridge), 0, sizeof(bool));
                return stream.ToArray();
            }
        }

        public static MapEvent FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int nameLength = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[nameLength];
                stream.Read(tempBytes, 0, nameLength);
                string name = new string(Encoding.UTF8.GetChars(tempBytes, 0, nameLength));

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int eventID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(float)];
                stream.Read(tempBytes, 0, sizeof(float));
                float realX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realY = BitConverter.ToSingle(tempBytes, 0);

                tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                FacingDirection direction = (FacingDirection)BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int spriteID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                EventTriggerType triggerType = (EventTriggerType)BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                RenderPriority priority = (RenderPriority)BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                MovementSpeed speed = (MovementSpeed)BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool enabled = BitConverter.ToBoolean(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(bool));
                bool onBridge = BitConverter.ToBoolean(tempBytes, 0);

                MapEvent mapEvent = new MapEvent(name, eventID, mapX, mapY);
                mapEvent.RealX = realX;
                mapEvent.RealY = realY;
                mapEvent.EventDirection = direction;
                mapEvent.SpriteID = spriteID;
                mapEvent.TriggerType = triggerType;
                mapEvent.Enabled = enabled;
                mapEvent.Priority = priority;
                mapEvent.Speed = speed;
                mapEvent.OnBridge = onBridge;
                return mapEvent;
            }
        }

        public EventData GetEventData()
        {
            return EventData.GetEventData(EventID);
        }

        public bool Moving()
        {
            if (MapX * 32 != RealX || MapY * 32 != RealY)
                return true;
            return false;
        }

        public float GetMovementSpeed()
        {
            float speed = 0;
            switch (Speed)
            {
                case MovementSpeed.ExtraFast:
                    speed = 80f;
                    break;
                case MovementSpeed.Fast:
                    speed = 64f;
                    break;
                case MovementSpeed.Normal:
                    speed = 48f;
                    break;
                case MovementSpeed.Slow:
                    speed = 32f;
                    break;
                case MovementSpeed.ExtraSlow:
                    speed = 16f;
                    break;
            }
            return speed;
        }

        public bool Move(int x, int y)
        {
            if (Enabled)
            {
                if (!Moving() && _frequencyTimer <= 0f)
                {
                    MapX = x;
                    MapY = y;
                    return true;
                }
            }

            return false;
        }

        public void UpdateMovement(float deltaTime)
        {
            if (Enabled)
            {
                if (_frequencyTimer > 0)
                    _frequencyTimer -= deltaTime;

                if (Moving())
                {
                    Vector2 realPos = new Vector2(RealX, RealY);
                    Vector2 dir = new Vector2(MapX * 32, MapY * 32) - realPos;
                    dir.Normalize();
                    realPos += (dir * GetMovementSpeed() * deltaTime);

                    dir = new Vector2(MapX * 32, MapY * 32) - realPos;
                    if (dir.Length <= 2f)
                    {
                        realPos = new OpenTK.Vector2(MapX * 32, MapY * 32);
                        switch (Frequency)
                        {
                            case MovementFrequency.VeryLow:
                                _frequencyTimer = 2f;
                                break;
                            case MovementFrequency.Low:
                                _frequencyTimer = 1.5f;
                                break;
                            case MovementFrequency.Normal:
                                _frequencyTimer = 1f;
                                break;
                            case MovementFrequency.High:
                                _frequencyTimer = 0.5f;
                                break;
                            case MovementFrequency.Instant:
                                _frequencyTimer = 0f;
                                break;
                        }
                    }

                    RealX = realPos.X;
                    RealY = realPos.Y;
                    Moved = true;
                }
            }
        }
    }

}

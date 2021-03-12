using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class MapEnemy
    {

        private int _enemyID;
        public int MaxHP;
        public int HP;
        public int SpawnX { get; private set; }
        public int SpawnY { get; private set; }
        public int MapX;
        public int MapY;
        public float RealX;
        public float RealY;
        public FacingDirection Direction;
        public bool OnBridge;
        public bool Dead;

        public int TargetPlayerID;
        public float AttackTimer;
        public float MovementTimer;

        private CharacterType EnemyCharacterType;
        private int EnemyCharacterID;
        private float _combatTimer;

        public bool Changed;

        public MapEnemy(int enemyID, int mapX, int mapY, bool onBridge)
        {
            _enemyID = enemyID;
            MaxHP = GetEnemyData().BaseStats.Vitality * 10;
            HP = MaxHP;
            MapX = mapX;
            MapY = mapY;
            SpawnX = mapX;
            SpawnY = mapY;
            RealX = mapX * 32;
            RealY = mapY * 32;
            Direction = FacingDirection.Down;
            OnBridge = onBridge;
            Dead = false;

            TargetPlayerID = -1;
            AttackTimer = 0;
            MovementTimer = 0f;

            EnemyCharacterType = CharacterType.Player;
            EnemyCharacterID = -1;
            _combatTimer = 0f;

            Changed = false;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(_enemyID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(HP), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapX), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(MapY), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Direction), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(RealX), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(RealY), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(OnBridge), 0, sizeof(bool));
                stream.Write(BitConverter.GetBytes(Dead), 0, sizeof(bool));
                return stream.ToArray();
            }
        }

        public static MapEnemy FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int enemyID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int hp = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapX = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int mapY = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                FacingDirection direction = (FacingDirection)BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(float)];
                stream.Read(tempBytes, 0, sizeof(float));
                float realX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float realY = BitConverter.ToSingle(tempBytes, 0);


                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool onBridge = BitConverter.ToBoolean(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(bool));
                bool dead = BitConverter.ToBoolean(tempBytes, 0);

                MapEnemy mapEvent = new MapEnemy(enemyID, mapX, mapY, onBridge);
                mapEvent.HP = hp;
                mapEvent.RealX = realX;
                mapEvent.RealY = realY;
                mapEvent.Direction = direction;
                mapEvent.Dead = dead;
                return mapEvent;
            }
        }

        public int GetEnemyID()
        {
            return _enemyID;
        }

        public EnemyData GetEnemyData()
        {
            return EnemyData.GetEnemy(_enemyID);
        }

        public Hitbox GetHitbox()
        {
            Hitbox hitbox = new Hitbox();

            hitbox.X = RealX;
            hitbox.Y = RealY;
            SpriteData data = SpriteData.GetSpriteData(GetEnemyData().SpriteID);
            if (data != null)
            {
                if (Direction == FacingDirection.Left || Direction == FacingDirection.Right)
                {
                    hitbox.Width = data.HorizontalBounds.X;
                    hitbox.Height = data.HorizontalBounds.Y;
                }
                else
                {
                    hitbox.Width = data.VerticalBounds.X;
                    hitbox.Height = data.VerticalBounds.Y;
                }
            }

            return hitbox;
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
            switch (GetEnemyData().Speed)
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

        public bool Move(MovementDirection direction)
        {
            if (!Moving() && !Dead)
            {
                switch (direction)
                {
                    case MovementDirection.Down:
                        MapY++;
                        Direction = FacingDirection.Down;
                        break;
                    case MovementDirection.Left:
                        MapX--;
                        Direction = FacingDirection.Left;
                        break;
                    case MovementDirection.Right:
                        MapX++;
                        Direction = FacingDirection.Right;
                        break;
                    case MovementDirection.Up:
                        MapY--;
                        Direction = FacingDirection.Up;
                        break;
                    case MovementDirection.UpperLeft:
                        MapX--;
                        MapY--;
                        Direction = FacingDirection.Left;
                        break;
                    case MovementDirection.UpperRight:
                        MapX++;
                        MapY--;
                        Direction = FacingDirection.Right;
                        break;
                    case MovementDirection.LowerLeft:
                        MapX--;
                        MapY++;
                        Direction = FacingDirection.Left;
                        break;
                    case MovementDirection.LowerRight:
                        MapX++;
                        MapY++;
                        Direction = FacingDirection.Right;
                        break;
                }

                Changed = true;

                return true;
            }

            return false;
        }

        public void Update(float deltaTime)
        {
            if (_combatTimer >= 0) _combatTimer -= deltaTime;
            else EnemyCharacterID = -1;

            UpdateMovement(deltaTime);
        }

        public bool UpdateMovement(float deltaTime)
        {
            if (Moving())
            {
                Vector2 realPos = new Vector2(RealX, RealY);
                Vector2 targetPos = new Vector2(MapX * 32, MapY * 32);
                Vector2 dir = targetPos - realPos;
                Vector2 endPos = realPos + (dir.Normalized() * GetMovementSpeed() * deltaTime);

                Vector2 dir2 = targetPos - endPos;

                if (dir.Length < dir2.Length || dir.Length < 0.5f)
                {
                    realPos = targetPos;
                }
                else
                {
                    realPos = endPos;
                }

                RealX = realPos.X;
                RealY = realPos.Y;

                Changed = true;

                return true;
            }

            return false;
        }

        public void TakeDamage(CharacterType enemyType, int enemyID, int damage, bool multiCombat)
        {
            if (EnemyCanAttack(enemyType, enemyID, multiCombat))
            {
                if (enemyID != -1)
                {
                    EnemyCharacterType = enemyType;
                    EnemyCharacterID = enemyID;
                    _combatTimer = 5f;

                    TargetPlayerID = enemyID;
                }

                HP -= damage;

                if (HP <= 0)
                {
                    HP = 0;
                    Dead = true;
                }

                Changed = true;
            }
        }

        public bool EnemyCanAttack(CharacterType enemyType, int enemyID, bool multiCombat)
        {
            if (Dead)
                return false;
            if (EnemyCharacterID == -1)
                return true;
            if (enemyID == -1)
                return true;
            if (multiCombat)
                return true;
            if (EnemyCharacterType == enemyType && EnemyCharacterID == enemyID)
                return true;
            return false;
        }
    }
}

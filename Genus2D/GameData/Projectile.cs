using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class Projectile
    {
        public int ProjectileID { get; private set; }
        public CharacterType ParentType { get; private set; }
        public int CharacterID { get; private set; }
        public CharacterType TargetType;
        public int TargetID;
        public Vector2 Position;
        public FacingDirection Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float Lifespan;
        public bool OnBridge;
        public bool Destroyed;

        public AttackStyle Style;
        public int AttackPower;

        public Projectile(int projectileID, CharacterType parentType, int characterID, Vector2 position, FacingDirection direction)
        {
            ProjectileID = projectileID;
            ParentType = parentType;
            CharacterID = characterID;
            TargetType = CharacterType.Player;
            TargetID = -1;
            Position = position;
            Direction = direction;

            float velocityX = (direction == FacingDirection.Down || direction == FacingDirection.Up) ? 0 : GetData().Speed * 32;
            if (direction == FacingDirection.Left) velocityX = -velocityX;
            float velocityY = (direction == FacingDirection.Left || direction == FacingDirection.Right) ? 0 : GetData().Speed * 32;
            if (direction == FacingDirection.Up) velocityY = -velocityY;
            Velocity = new Vector2(velocityX, velocityY);

            Lifespan = 0;
            OnBridge = false;
            Destroyed = false;

            Style = AttackStyle.None;
            AttackPower = 0;
        }

        public ProjectileData GetData()
        {
            return ProjectileData.GetProjectileData(ProjectileID);
        }

        public Hitbox GetHitBox()
        {
            Hitbox hitbox = new Hitbox();

            hitbox.X = Position.X;
            hitbox.Y = Position.Y;

            ProjectileData data = GetData();
            if (data != null)
            {
                hitbox.Width = data.BoundsWidth;
                hitbox.Height = data.BoundsHeight;
            }

            return hitbox;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(ProjectileID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)Direction), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Position.X), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(Position.Y), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(OnBridge), 0, sizeof(bool));
                stream.Write(BitConverter.GetBytes(Destroyed), 0, sizeof(bool));

                return stream.ToArray();
            }
        }

        public static Projectile FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                byte[] tempBytes = new byte[sizeof(int)];
                stream.Read(tempBytes, 0, sizeof(int));
                int projectileID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                FacingDirection direction = (FacingDirection)BitConverter.ToInt32(tempBytes, 0);

                tempBytes = new byte[sizeof(float)];
                stream.Read(tempBytes, 0, sizeof(float));
                float positionX = BitConverter.ToSingle(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(float));
                float positionY = BitConverter.ToSingle(tempBytes, 0);

                tempBytes = new byte[sizeof(bool)];
                stream.Read(tempBytes, 0, sizeof(bool));
                bool onBridge = BitConverter.ToBoolean(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(bool));
                bool destroyed = BitConverter.ToBoolean(tempBytes, 0);

                Vector2 position = new Vector2(positionX, positionY);
                Projectile projectile = new Projectile(projectileID, CharacterType.Player, -1, position, direction);
                projectile.OnBridge = onBridge;
                projectile.Destroyed = destroyed;
                return projectile;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class Hitbox
    {

        public float X, Y, Width, Height;

        public Hitbox()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public Hitbox(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Top()
        {
            return Y - (Height / 2);
        }

        public float Left()
        {
            return X - (Width / 2);
        }

        public float Right()
        {
            return Left() + Width;
        }

        public float Bottom()
        {
            return Top() + Height;
        }

        public bool Intersects(Hitbox other)
        {
            return (Left() <= other.Right() && Right() >= other.Left() && Top() <= other.Bottom() && Bottom() >= other.Top());
        }
    }
}

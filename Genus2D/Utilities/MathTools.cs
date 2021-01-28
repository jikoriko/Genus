using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.Utilities
{
    public class MathTools
    {

        public static float Lerp(float first, float second, float by)
        {
            return first * (1 - by) + second * by;
        }

        public static Vector2 Lerp(ref Vector2 first, ref Vector2 second, float by)
        {
            float retX = Lerp(first.X, second.X, by);
            float retY = Lerp(first.Y, second.Y, by);
            return new Vector2(retX, retY);
        }

        public static Color4 Lerp(ref Color4 first, ref Color4 second, float by)
        {
            float r = Lerp(first.R, second.R, by);
            float g = Lerp(first.G, second.G, by);
            float b = Lerp(first.B, second.B, by);
            float a = Lerp(first.A, second.A, by);
            return new Color4(r, g, b, a);
        }
    }
}

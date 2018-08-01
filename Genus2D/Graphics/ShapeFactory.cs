using System;

using OpenTK;

namespace Genus2D.Graphics
{
    public class ShapeFactory
    {
        private enum CornerType
        {
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        private static Shape _rectangle = null;
        public static Shape Rectangle
        {
            get
            {
                if (_rectangle == null)
                    _rectangle = GenerateRectangle();
                return _rectangle;
            }
            private set
            {
                _rectangle = value;
            }
        }

        private static Shape _roundedCornerTL = null;
        public static Shape RoundedCornerTL
        {
            get
            {
                if (_roundedCornerTL == null)
                    _roundedCornerTL = GenerateRoundedCorner(CornerType.TopLeft);
                return _roundedCornerTL;
            }
            private set
            {
                _roundedCornerTL = value;
            }
        }

        private static Shape _roundedCornerTR = null;
        public static Shape RoundedCornerTR
        {
            get
            {
                if (_roundedCornerTR == null)
                    _roundedCornerTR = GenerateRoundedCorner(CornerType.TopRight);
                return _roundedCornerTR;
            }
            private set
            {
                _roundedCornerTR = value;
            }
        }

        private static Shape _roundedCornerBL = null;
        public static Shape RoundedCornerBL
        {
            get
            {
                if (_roundedCornerBL == null)
                    _roundedCornerBL = GenerateRoundedCorner(CornerType.BottomLeft);
                return _roundedCornerBL;
            }
            private set
            {
                _roundedCornerBL = value;
            }
        }

        private static Shape _roundedCornerBR = null;
        public static Shape RoundedCornerBR
        {
            get
            {
                if (_roundedCornerBR == null)
                    _roundedCornerBR = GenerateRoundedCorner(CornerType.BottomRight);
                return _roundedCornerBR;
            }
            private set
            {
                _roundedCornerBR = value;
            }
        }

        private static Shape _circle = null;
        public static Shape Circle
        {
            get
            {
                if (_circle == null)
                    _circle = GenerateCircle();
                return _circle;
            }
            private set
            {
                _circle = value;
            }
        }

        private static Shape _triangle = null;
        public static Shape Triangle
        {
            get
            {
                if (_triangle == null)
                    _triangle = GenerateTriangle();
                return _triangle;
            }
            private set
            {
                _triangle = value;
            }
        }

        private static Shape _star = null;
        public static Shape Star
        {
            get
            {
                if (_star == null)
                    _star = GenerateStar();
                return _star;
            }
            private set
            {
                _star = value;
            }
        }

        private static Shape _point = null;
        public static Shape Point
        {
            get
            {
                if (_point == null)
                    _point = GeneratePoint();
                return _point;
            }
            private set
            {
                _point = value;
            }
        }


        public static Shape GenerateRectangle()
        {
            float[] verts = new float[] {
                0, 0, 0,
                1, 0, 0,
                1, 1, 0,
                0, 1, 0
            };

            Shape shape = new Shape(verts);

            return shape;
        }

        private static Shape GenerateRoundedCorner(CornerType type)
        {
            float[] verts = new float[92 * 3];

            float rotOffset = 0;
            float xOffset = 0;
            float yOffset = 0;
            switch (type)
            {
                case CornerType.TopLeft:
                    rotOffset = (float)Math.PI;
                    xOffset = 1;
                    yOffset = 1;
                    break;
                case CornerType.TopRight:
                    rotOffset = (float)(Math.PI / 2) * 3;
                    xOffset = 0;
                    yOffset = 1;
                    break;
                case CornerType.BottomLeft:
                    rotOffset = (float)(Math.PI / 2);
                    xOffset = 1;
                    break;
                case CornerType.BottomRight:
                    rotOffset = 0;
                    break;
            }

            for (int i = 0; i < 91; i++)
            {
                float degInRad = MathHelper.DegreesToRadians(i);

                float x = ((float)Math.Cos(degInRad + rotOffset)) + xOffset;
                float y = ((float)Math.Sin(degInRad + rotOffset)) + yOffset;

                verts[i * 3] = x;
                verts[(i * 3) + 1] = y;
                verts[(i * 3) + 2] = 0f;
            }

            Vector3 corner = new Vector3(-0.5f, -0.5f, 0f);
            corner = Vector3.TransformPosition(corner, Matrix4.CreateRotationZ(rotOffset));

            corner.X += 0.5f;
            corner.Y += 0.5f;

            verts[(91 * 3)] = corner.X;
            verts[(91 * 3) + 1] = corner.Y;
            verts[(91 * 3) + 2] = 0f;

            Shape shape = new Shape(verts);
            return shape;
        }

        private static Shape GenerateCircle()
        {
            float[] verts = new float[360 * 3];
            for (int i = 0; i < 360; i++)
            {
                float degInRad = MathHelper.DegreesToRadians(i);

                float x = ((float)Math.Cos(degInRad) * 0.5f) + 0.5f;
                float y = ((float)Math.Sin(degInRad) * 0.5f) + 0.5f;

                verts[i * 3] = x;
                verts[(i * 3) + 1] = y;
                verts[(i * 3) + 2] = 0.0f;
            }
            Shape shape = new Shape(verts);
            return shape;
        }

        private static Shape GenerateTriangle()
        {
            float[] verts = new float[] {
                0.5f, 0, 0,
                1, 1, 0,
                0, 1, 0,
            };

            Shape shape = new Shape(verts);

            return shape;
        }

        private static Shape GenerateStar()
        {
            int numVerts = 10;
            float[] verts = new float[numVerts * 3];

            float degPerVert = (float)(Math.PI * 2) / numVerts;
            float rotOffset = (float)Math.PI / numVerts;

            for (int i = 0; i < numVerts; i++)
            {
                float rotation = degPerVert * i;
                float x = ((float)Math.Cos(rotation + rotOffset) * 0.5f);
                float y = ((float)Math.Sin(rotation + rotOffset) * 0.5f);

                if (i % 2 == 0)
                {
                    x *= 0.5f;
                    y *= 0.5f;
                }

                x += 0.5f;
                y += 0.5f;

                verts[i * 3] = x;
                verts[(i * 3) + 1] = y;
                verts[(i * 3) + 2] = 0;

            }

            Shape shape = new Shape(verts);

            return shape;
        }

        private static Shape GeneratePoint()
        {
            float[] verts = new float[] {
                0, 0, 0,
            };

            Shape shape = new Shape(verts);

            return shape;
        }


        public static void Destroy()
        {
            if (_rectangle != null)
                _rectangle.Destroy();
            if (_roundedCornerTL != null)
                _roundedCornerTL.Destroy();
            if (_roundedCornerTR != null)
                _roundedCornerTR.Destroy();
            if (_roundedCornerBL != null)
                _roundedCornerBL.Destroy();
            if (_roundedCornerBR != null)
                _roundedCornerBR.Destroy();
            if (_circle != null)
                _circle.Destroy();
            if (_triangle != null)
                _triangle.Destroy();
            if (_star != null)
                _star.Destroy();
            if (_point != null)
                _point.Destroy();

            _rectangle = null;
            _roundedCornerTL = null;
            _roundedCornerTR = null;
            _roundedCornerBL = null;
            _roundedCornerBR = null;
            _circle = null;
            _triangle = null;
            _star = null;
            _point = null;
        }

    }
}

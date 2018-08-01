using System;
using System.Collections.Generic;

using OpenTK;

namespace Genus2D.Graphics
{
    public class Triangulator
    {

        public static Vector3 GetTriangleCenter(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float centerX = (p1.X + p2.X + p3.X) / 3;
            float centerY = (p1.Y + p2.Y + p3.Y) / 3;
            return new Vector3(centerX, centerY, 0);
        }

        public static bool CheckForIntersections(ref List<Vector3> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                Vector3 start = source[i];
                Vector3 end = source[(i + 1) % source.Count];

                for (int j = 0; j < source.Count; j++)
                {

                    Vector3 start2 = source[j];
                    Vector3 end2 = source[(j + 1) % source.Count];

                    if (start2 == end || start == end2)
                        continue;

                    Vector3 intersection;
                    if (GetLineIntersection(start, end, start2, end2, out intersection))
                    {
                        if (intersection != start && intersection != end && intersection != start2 && intersection != end2)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool GetLineIntersection(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End, out Vector3 result)
        {
            result = Vector3.Zero;

            float a1 = line1End.Y - line1Start.Y;
            float b1 = line1Start.X - line1End.X;
            float c1 = (a1 * line1Start.X) + (b1 * line1Start.Y);

            float a2 = line2End.Y - line2Start.Y;
            float b2 = line2Start.X - line2End.X;
            float c2 = (a2 * line2Start.X) + (b2 * line2Start.Y);

            float delta = (a1 * b2) - (a2 * b1);
            if (delta == 0) //adjecency
            {
                return false;
            }

            result.X = ((b2 * c1) - (b1 * c2)) / delta;
            result.Y = ((a1 * c2) - (a2 * c1)) / delta;

            if (PointOnLine(line1Start, line1End, result) && PointOnLine(line2Start, line2End, result))
                return true;


            result = Vector3.Zero;

            return false;


        }

        private static readonly float EPSILON = 0.1f;

        private static bool PointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            float crossproduct = (point.Y - lineStart.Y) * (lineEnd.X - lineStart.X) - (point.X - lineStart.X) * (lineEnd.Y - lineStart.Y);
            if (Math.Abs(crossproduct) > EPSILON) return false;

            float dotproduct = (point.X - lineStart.X) * (lineEnd.X - lineStart.X) + (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y);
            if (dotproduct < 0) return false;

            float squaredlengthba = (lineEnd.X - lineStart.X) * (lineEnd.X - lineStart.X) + (lineEnd.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y);
            if (dotproduct > squaredlengthba) return false;

            return true;
        }

        private static float Distance(Vector3 A, Vector3 B)
        {
            return (float)Math.Sqrt((A.X - B.X) + (A.Y - B.Y));
        }

        private static bool IsClockwise(List<Vector3> points)
        {

            float sum = 0.0f;

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 current = points[i];
                Vector3 next = points[(i + 1) % points.Count];

                sum += (current.X * next.Y) - (next.X * current.Y);
            }

            return sum > 0.0f;
        }

        private static float Wedge(Vector3 A, Vector3 B)
        {
            float wedge = (A.X * B.Y) - (A.Y * B.X);
            return wedge;
        }

        public static List<Vector3> Triangulate(List<Vector3> source)
        {
            if (IsClockwise(source))
            {
                source.Reverse();
            }

            List<Vector3> initialList = new List<Vector3>(source);
            List<Vector3> result = new List<Vector3>();

            while (true)
            {
                bool triangleClipped = false;
                for (int i = 1; i < source.Count - 1; i++)
                {
                    Vector3 prev = i == 0 ? source[source.Count - 1] : source[i - 1];
                    Vector3 current = source[i];
                    Vector3 next = i == source.Count - 1 ? source[0] : source[i + 1];

                    Vector3 A = prev - current;
                    Vector3 B = next - current;
                    float wedge = Wedge(A, B);

                    if (wedge <= 0)
                    {
                        continue;
                    }

                    bool pointInsideTriangle = false;
                    for (int j = 0; j < initialList.Count; j++)
                    {
                        Vector3 other = initialList[j];
                        if (other == prev || other == current || other == next)
                            continue;

                        if (PointInTriangle(other, prev, current, next))
                        {
                            pointInsideTriangle = true;
                            break;
                        }
                    }
                    if (pointInsideTriangle)
                        continue;

                    triangleClipped = true;
                    result.Add(prev);
                    result.Add(current);
                    result.Add(next);
                    source.RemoveAt(i);
                    i--;

                }

                if (!triangleClipped)
                    break;
            }

            return result;
        }

        private static float Sign(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool PointInTriangle(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            bool b1, b2, b3;

            b1 = Sign(pt, v1, v2) < 0.0f;
            b2 = Sign(pt, v2, v3) < 0.0f;
            b3 = Sign(pt, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }
    }
}

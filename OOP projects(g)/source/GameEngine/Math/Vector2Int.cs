using System;

namespace OOP_projects.GameEngine
{
    static class CustomMath
    {
        static public double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public struct Vector2Int
        {
            public int X { set; get; }
            public int Y { set; get; }

            public Vector2Int(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Vector2Int operator +(Vector2Int a, Vector2Int b)
            {
                return new Vector2Int(a.X + b.X, a.Y + b.Y);
            }
        }
    }
}

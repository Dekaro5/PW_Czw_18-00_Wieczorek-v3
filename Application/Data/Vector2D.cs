using System;

namespace Data
{
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Vector2D Normalize() =>
            Length > 0
                ? new Vector2D { X = X / Length, Y = Y / Length }
                : new Vector2D();

        public static Vector2D operator +(Vector2D a, Vector2D b) =>
            new Vector2D { X = a.X + b.X, Y = a.Y + b.Y };

        public static Vector2D operator -(Vector2D a, Vector2D b) =>
            new Vector2D { X = a.X - b.X, Y = a.Y - b.Y };

        public static Vector2D operator *(Vector2D v, double scalar) =>
            new Vector2D { X = v.X * scalar, Y = v.Y * scalar };

        public static double Dot(Vector2D a, Vector2D b) =>
            a.X * b.X + a.Y * b.Y;
    }
}

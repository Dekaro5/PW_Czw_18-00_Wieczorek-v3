using System;

namespace Data.Models
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

        public Vector2D InvertX() => new() { X = -X, Y = Y };
        public Vector2D InvertY() => new() { X = X,  Y = -Y };
    }
}
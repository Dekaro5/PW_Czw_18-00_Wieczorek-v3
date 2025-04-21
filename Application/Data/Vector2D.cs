using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Vector2D Normalize()
        {
            var length = Length;
            return length > 0 ? new Vector2D { X = X / length, Y = Y / length } : this;
        }

        public Vector2D InvertX() => new Vector2D { X = -X, Y = Y };
        public Vector2D InvertY() => new Vector2D { X = X, Y = -Y };
    }
}


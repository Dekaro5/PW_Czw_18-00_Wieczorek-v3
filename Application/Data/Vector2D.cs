using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Vector2D Normalize()
        {
            var length = Length;
            if (length == 0) return new Vector2D { X = 0, Y = 0 };
            return new Vector2D { X = X / length, Y = Y / length };
        }
    }
}


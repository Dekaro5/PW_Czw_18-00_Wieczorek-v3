﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public interface IBall
    {
        double X { get; set; }
        double Y { get; set; }
        double Diameter { get; set; }
        Vector2D Velocity { get; set; }
    }
}


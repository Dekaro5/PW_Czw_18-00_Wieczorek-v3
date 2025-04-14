using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;

namespace BusinessLogic.Abstractions
{
    public interface IBallService
    {
        IBall CreateBall(double x, double y);
        void UpdatePosition(IBall ball, double tableWidth, double tableHeight);
    }
}

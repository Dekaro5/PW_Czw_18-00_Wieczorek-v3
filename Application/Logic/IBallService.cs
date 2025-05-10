using Data;

namespace BusinessLogic.Abstractions
{
    public interface IBallService
    {
        IBall CreateBall(double x, double y);
        void UpdatePosition(IBall ball, double tableWidth, double tableHeight);
    }
}
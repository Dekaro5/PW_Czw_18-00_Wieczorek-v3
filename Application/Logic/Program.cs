using Data.Models;

namespace BusinessLogic.Services
{
    public class BallService
    {
        public Ball CreateBall(double x, double y)
        {
            return new Ball
            {
                X = x,
                Y = y,
                VelocityX = 0,
                VelocityY = 0
            };
        }

        public void UpdatePosition(Ball ball)
        {
            ball.X += ball.VelocityX;
            ball.Y += ball.VelocityY;
        }
    }
}
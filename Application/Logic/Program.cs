using Data.Models;
using BusinessLogic.Abstractions;

namespace BusinessLogic.Services
{
    public class BallService : IBallService
    {
        public IBall CreateBall(double x, double y)
        {
            return new Ball
            {
                X = x,
                Y = y,
                Diameter = 20,
                Velocity = new Vector2D { X = 2, Y = 1.5 } // przykładowa prędkość
            };
        }

        public void UpdatePosition(IBall ball, double tableWidth, double tableHeight)
        {
            // Aktualizacja pozycji
            ball.X += ball.Velocity.X;
            ball.Y += ball.Velocity.Y;

            // Ograniczenie ruchu
            if (ball.X <= 0)
            {
                ball.X = 0;
                ball.Velocity = new Vector2D { X = 0, Y = ball.Velocity.Y };
            }
            else if (ball.X + ball.Diameter >= tableWidth)
            {
                ball.X = tableWidth - ball.Diameter;
                ball.Velocity = new Vector2D { X = 0, Y = ball.Velocity.Y };
            }

            if (ball.Y <= 0)
            {
                ball.Y = 0;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = 0 };
            }
            else if (ball.Y + ball.Diameter >= tableHeight)
            {
                ball.Y = tableHeight - ball.Diameter;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = 0 };
            }
        }
    }
}

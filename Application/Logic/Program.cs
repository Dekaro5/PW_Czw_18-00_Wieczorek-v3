using BusinessLogic.Abstractions;
using Data.Models;

namespace BusinessLogic.Services;

public class BallService : IBallService
{
    private readonly Random _random = new();

    public IBall CreateBall(double x, double y)
    {
        Console.WriteLine($"Creating ball at ({x}, {y})");
        return new Ball
        {
            X = x,
            Y = y,
            Velocity = new Vector2D
            {
                X = _random.NextDouble() * 2 - 1,
                Y = _random.NextDouble() * 2 - 1
            }
        };
    }

    public void UpdatePosition(IBall ball, double tableWidth, double tableHeight)
    {
        ball.X += ball.Velocity.X;
        ball.Y += ball.Velocity.Y;

        if (ball.X <= 0 || ball.X + ball.Diameter >= tableWidth)
        {
            ball.Velocity = ball.Velocity.InvertX();
        }

        if (ball.Y <= 0 || ball.Y + ball.Diameter >= tableHeight)
        {
            ball.Velocity = ball.Velocity.InvertY();
        }
    }
}


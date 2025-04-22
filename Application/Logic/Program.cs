using System;
using Data.Models;
using BusinessLogic.Abstractions;

namespace BusinessLogic.Services
{
    public class BallService : IBallService
    {
        private readonly Random _rnd = new();

        public IBall CreateBall(double x, double y)
        {
            // ustalamy średnicę i prędkość startową
            return new Ball
            {
                X = x,
                Y = y,
                Diameter = 20,
                Velocity = new Vector2D
                {
                    X = (_rnd.NextDouble() - 0.5) * 200,
                    Y = (_rnd.NextDouble() - 0.5) * 200
                }
            };
        }

        public void UpdatePosition(IBall ball, double tableWidth, double tableHeight)
        {
            // Zakładamy 60 FPS → dt ≈ 1/60s
            const double dt = 1.0 / 60;

            ball.X += ball.Velocity.X * dt;
            ball.Y += ball.Velocity.Y * dt;

            // Odbicia od ścian
            if (ball.X <= 0 || ball.X + ball.Diameter >= tableWidth)
                ball.Velocity = ball.Velocity.InvertX();

            if (ball.Y <= 0 || ball.Y + ball.Diameter >= tableHeight)
                ball.Velocity = ball.Velocity.InvertY();
        }
    }
}
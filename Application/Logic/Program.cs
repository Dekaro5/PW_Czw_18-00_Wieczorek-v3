using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Data;
using BusinessLogic.Abstractions;

namespace BusinessLogic.Services
{
    public class BallService : IBallService, IDisposable
    {
        public double Width { get; }
        public double Height { get; }

        private readonly ObservableCollection<IBall> _balls = new ObservableCollection<IBall>();
        public ObservableCollection<IBall> Balls => _balls;

        private readonly Random _random = new Random();
        private readonly object _ballsLock = new object();

        private const double TimeStep = 0.1;

        // Dodane pole loggera
        private readonly BallLogger _logger = new BallLogger();

        public BallService(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public async Task CreateBalls(int count, double defaultDiameter = 20, double defaultMass = 10)
        {
            var tempGeneratedBalls = await Task.Run(() =>
            {
                var list = new List<IBall>();
                for (int i = 0; i < count; i++)
                {
                    bool placed = false;
                    int attempts = 0;
                    const int maxAttempts = 100;

                    while (!placed && attempts < maxAttempts)
                    {
                        double mass = (_random.NextDouble() * 100) + defaultMass;
                        double diameter = mass / 2;
                        double x = _random.NextDouble() * (Width - diameter);
                        double y = _random.NextDouble() * (Height - diameter);
                        double vx = (_random.NextDouble() * 100) - 50;
                        double vy = (_random.NextDouble() * 100) - 50;

                        var ball = new Ball(x, y, diameter, mass, new Vector2D { X = vx, Y = vy });

                        if (!list.Any(existing =>
                        {
                            var dx = existing.X - ball.X;
                            var dy = existing.Y - ball.Y;
                            var minDist = (existing.Diameter + ball.Diameter) / 2;
                            return dx * dx + dy * dy < minDist * minDist;
                        }))
                        {
                            list.Add(ball);
                            placed = true;
                        }
                        attempts++;
                    }
                }
                return list;
            });

            lock (_ballsLock)
            {
                _balls.Clear();
                foreach (var ball in tempGeneratedBalls)
                    _balls.Add(ball);
            }
        }

        public void UpdateSimulationStep()
        {
            List<IBall> snapshot;
            lock (_ballsLock)
                snapshot = _balls.ToList();

            foreach (var ball in snapshot)
            {
                lock (_ballsLock)
                {
                    ball.Move(TimeStep);
                    HandleWallCollision(ball);
                    // Logowanie pozycji po ruchu
                    if (ball is Ball concreteBall)
                        _logger.LogBallPosition(concreteBall);
                }
            }

            for (int i = 0; i < snapshot.Count; i++)
            {
                lock (snapshot[i])
                {
                    for (int j = i + 1; j < snapshot.Count; j++)
                    {
                        HandleBallPairCollision(snapshot[i], snapshot[j]);
                    }
                }
            }
        }

        private void HandleWallCollision(IBall ball)
        {
            bool collision = false;
            if (ball.X < 0)
            {
                ball.X = 0;
                ball.Velocity = new Vector2D { X = Math.Abs(ball.Velocity.X), Y = ball.Velocity.Y };
                collision = true;
            }
            else if (ball.X + ball.Diameter > Width)
            {
                ball.X = Width - ball.Diameter;
                ball.Velocity = new Vector2D { X = -Math.Abs(ball.Velocity.X), Y = ball.Velocity.Y };
                collision = true;
            }

            if (ball.Y < 0)
            {
                ball.Y = 0;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = Math.Abs(ball.Velocity.Y) };
                collision = true;
            }
            else if (ball.Y + ball.Diameter > Height)
            {
                ball.Y = Height - ball.Diameter;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = -Math.Abs(ball.Velocity.Y) };
                collision = true;
            }

            // Logowanie kolizji ze ścianą
            if (collision && ball is Ball concreteBall)
            {
                _logger.LogCollision($"WALL;X={concreteBall.X:F3};Y={concreteBall.Y:F3};Vx={concreteBall.Velocity.X:F3};Vy={concreteBall.Velocity.Y:F3}");
            }
        }

        private void HandleBallPairCollision(IBall ball1, IBall ball2)
        {
            double r1 = ball1.Diameter / 2;
            double r2 = ball2.Diameter / 2;
            double c1x = ball1.X + r1;
            double c1y = ball1.Y + r1;
            double c2x = ball2.X + r2;
            double c2y = ball2.Y + r2;

            var dx = c2x - c1x;
            var dy = c2y - c1y;
            double distSq = dx * dx + dy * dy;
            double sumR = r1 + r2;

            if (distSq <= sumR * sumR && distSq > 0)
            {
                double dist = Math.Sqrt(distSq);
                var nx = dx / dist;
                var ny = dy / dist;
                var tx = -ny;
                var ty = nx;

                double v1n = Vector2D.Dot(ball1.Velocity, new Vector2D { X = nx, Y = ny });
                double v1t = Vector2D.Dot(ball1.Velocity, new Vector2D { X = tx, Y = ty });
                double v2n = Vector2D.Dot(ball2.Velocity, new Vector2D { X = nx, Y = ny });
                double v2t = Vector2D.Dot(ball2.Velocity, new Vector2D { X = tx, Y = ty });

                double m1 = ball1.Mass;
                double m2 = ball2.Mass;
                double newV1n = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
                double newV2n = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                ball1.Velocity = new Vector2D { X = tx * v1t + nx * newV1n, Y = ty * v1t + ny * newV1n };
                ball2.Velocity = new Vector2D { X = tx * v2t + nx * newV2n, Y = ty * v2t + ny * newV2n };

                double overlap = sumR - dist;
                if (overlap > 0)
                {
                    double total = m1 + m2;
                    c1x -= nx * (overlap * (m2 / total));
                    c1y -= ny * (overlap * (m2 / total));
                    c2x += nx * (overlap * (m1 / total));
                    c2y += ny * (overlap * (m1 / total));

                    ball1.X = c1x - r1;
                    ball1.Y = c1y - r1;
                    ball2.X = c2x - r2;
                    ball2.Y = c2y - r2;
                }

                // Logowanie kolizji kul
                if (ball1 is Ball b1 && ball2 is Ball b2)
                {
                    _logger.LogCollision($"BALL;B1:X={b1.X:F3},Y={b1.Y:F3},Vx={b1.Velocity.X:F3},Vy={b1.Velocity.Y:F3};B2:X={b2.X:F3},Y={b2.Y:F3},Vx={b2.Velocity.X:F3},Vy={b2.Velocity.Y:F3}");
                }
            }
        }

        // Dodaj IDisposable, by zwolnić loggera
        public void Dispose()
        {
            _logger.Dispose();
        }
    }
}

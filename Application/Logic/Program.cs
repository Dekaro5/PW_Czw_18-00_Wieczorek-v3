using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Data;
using BusinessLogic.Abstractions;

namespace BusinessLogic.Services
{
    public class BallService : IBallService
    {
        public double Width { get; }
        public double Height { get; }

        private readonly ObservableCollection<IBall> _balls = new ObservableCollection<IBall>();
        public ObservableCollection<IBall> Balls => _balls;

        private readonly Random _random = new Random();
        private readonly object _ballsLock = new object();

        private const double TimeStep = 0.1;

        public BallService(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public async Task CreateBalls(int count, double defaultDiameter = 20, double defaultMass = 10)
        {
            List<IBall> tempGeneratedBalls = await Task.Run(() =>
            {
                var localBallsList = new List<IBall>();
                for (int i = 0; i < count; i++)
                {
                    bool placedSuccessfully = false;
                    IBall newBall = null;
                    int placementAttempts = 0;
                    const int maxPlacementAttempts = 100;

                    while (!placedSuccessfully && placementAttempts < maxPlacementAttempts)
                    {
                        double x = _random.NextDouble() * (Width - defaultDiameter);
                        double y = _random.NextDouble() * (Height - defaultDiameter);
                        double velocityX = (_random.NextDouble() * 100) - 50; 
                        double velocityY = (_random.NextDouble() * 100) - 50;

                        newBall = new Ball(x, y, defaultDiameter, defaultMass, new Vector2D { X = velocityX, Y = velocityY });

                        bool overlapsWithExisting = false;
                        foreach (var existingBall in localBallsList)
                        {
                            double dxCheck = existingBall.X - newBall.X;
                            double dyCheck = existingBall.Y - newBall.Y;
                            double distanceSquaredCheck = dxCheck * dxCheck + dyCheck * dyCheck;
                            double sumDiameters = existingBall.Diameter / 2 + newBall.Diameter / 2;
                            if (distanceSquaredCheck < sumDiameters * sumDiameters)
                            {
                                overlapsWithExisting = true;
                                break;
                            }
                        }

                        if (!overlapsWithExisting)
                        {
                            localBallsList.Add(newBall);
                            placedSuccessfully = true;
                        }
                        placementAttempts++;
                    }
                }
                return localBallsList;
            });

            lock (_ballsLock)
            {
                _balls.Clear();
                foreach (var ball in tempGeneratedBalls)
                {
                    _balls.Add(ball);
                }
            }
        }

        public void UpdateSimulationStep()
        {
            var currentBallsSnapshot = new List<IBall>();
            lock (_ballsLock)
            {
                currentBallsSnapshot = _balls.ToList();
            }

            foreach (var ball in currentBallsSnapshot)
            {
                ball.Move(TimeStep);
            }

            foreach (var ball in currentBallsSnapshot)
            {
                HandleWallCollision(ball);
            }

            for (int i = 0; i < currentBallsSnapshot.Count; i++)
            {
                for (int j = i + 1; j < currentBallsSnapshot.Count; j++)
                {
                    HandleBallPairCollision(currentBallsSnapshot[i], currentBallsSnapshot[j]);
                }
            }
        }

        private void HandleWallCollision(IBall ball)
        {
            if (ball.X < 0)
            {
                ball.X = 0;
                ball.Velocity = new Vector2D { X = Math.Abs(ball.Velocity.X), Y = ball.Velocity.Y };
            }
            else if (ball.X + ball.Diameter > Width)
            {
                ball.X = Width - ball.Diameter;
                ball.Velocity = new Vector2D { X = -Math.Abs(ball.Velocity.X), Y = ball.Velocity.Y };
            }

            if (ball.Y < 0)
            {
                ball.Y = 0;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = Math.Abs(ball.Velocity.Y) };
            }
            else if (ball.Y + ball.Diameter > Height)
            {
                ball.Y = Height - ball.Diameter;
                ball.Velocity = new Vector2D { X = ball.Velocity.X, Y = -Math.Abs(ball.Velocity.Y) };
            }
        }


        private void HandleBallPairCollision(IBall ball1, IBall ball2)
        {
            var pos1 = new Vector2D { X = ball1.X, Y = ball1.Y };
            var pos2 = new Vector2D { X = ball2.X, Y = ball2.Y };

            Vector2D delta = pos2 - pos1;
            double distanceSquared = delta.X * delta.X + delta.Y * delta.Y;
            double sumRadii = (ball1.Diameter + ball2.Diameter) / 2;

            if (distanceSquared <= sumRadii * sumRadii && distanceSquared > 0)
            {
                double distance = Math.Sqrt(distanceSquared);
                Vector2D normal = delta * (1.0 / distance);
                Vector2D tangent = new Vector2D { X = -normal.Y, Y = normal.X };

                double dpTan1 = Vector2D.Dot(ball1.Velocity, tangent);
                double dpTan2 = Vector2D.Dot(ball2.Velocity, tangent);

                double dpNorm1 = Vector2D.Dot(ball1.Velocity, normal);
                double dpNorm2 = Vector2D.Dot(ball2.Velocity, normal);

                double m1 = ball1.Mass;
                double m2 = ball2.Mass;

                double newDpNorm1 = (dpNorm1 * (m1 - m2) + 2 * m2 * dpNorm2) / (m1 + m2);
                double newDpNorm2 = (dpNorm2 * (m2 - m1) + 2 * m1 * dpNorm1) / (m1 + m2);

                ball1.Velocity = tangent * dpTan1 + normal * newDpNorm1;
                ball2.Velocity = tangent * dpTan2 + normal * newDpNorm2;
            }
        }

    }
}
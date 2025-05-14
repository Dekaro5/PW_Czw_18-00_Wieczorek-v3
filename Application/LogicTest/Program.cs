using BusinessLogic.Services;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogic.Tests
{
    [TestClass]
    public class BallServiceTests
    {
        private BallService _ballService;

        [TestInitialize]
        public void Setup()
        {
            _ballService = new BallService(500, 300);
        }

        [TestMethod]
        public async Task GenerateCorrectNumberOfBalls()
        {
            await _ballService.CreateBalls(5);

            Assert.AreEqual(5, _ballService.Balls.Count);
        }

        [TestMethod]
        public async Task ShouldNotOverlapBalls()
        {
            await _ballService.CreateBalls(10);

            var balls = _ballService.Balls;
            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    double dx = balls[i].X - balls[j].X;
                    double dy = balls[i].Y - balls[j].Y;
                    double distanceSquared = dx * dx + dy * dy;
                    double sumDiameters = balls[i].Diameter / 2 + balls[j].Diameter / 2;

                    Assert.IsTrue(distanceSquared >= sumDiameters * sumDiameters, "Balls are overlapping!");
                }
            }
        }

        [TestMethod]
        public async Task MoveBalls()
        {
            await _ballService.CreateBalls(1);
            var ball = _ballService.Balls.First();

            double initialX = ball.X;
            double initialY = ball.Y;

            _ballService.UpdateSimulationStep();

            Assert.AreNotEqual(initialX, ball.X);
            Assert.AreNotEqual(initialY, ball.Y);
        }

        [TestMethod]
        public async Task WallCollision()
        {
            await _ballService.CreateBalls(1, 20, 10);
            var ball = _ballService.Balls.First();
            ball.X = -5;
            ball.Y = -5;

            _ballService.UpdateSimulationStep();

            Assert.IsTrue(ball.X >= 0);
            Assert.IsTrue(ball.Y >= 0);
        }
    }
}
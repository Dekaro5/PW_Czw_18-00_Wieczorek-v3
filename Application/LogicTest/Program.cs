using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.Services;
using Data;

namespace Logic.Tests
{
    [TestClass]
    public class BallServiceTests
    {
        private BallService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new BallService();
        }

        [TestMethod]
        public void SetsInitialProperties()
        {
            var ball = _service.CreateBall(10.0, 20.0);

            Assert.AreEqual(10.0, ball.X, 1e-6);
            Assert.AreEqual(20.0, ball.Y, 1e-6);
            Assert.AreEqual(20.0, ball.Diameter, 1e-6);
            Assert.IsTrue(ball.Velocity.X >= -100 && ball.Velocity.X <= 100);
            Assert.IsTrue(ball.Velocity.Y >= -100 && ball.Velocity.Y <= 100);
        }

        [TestMethod]
        public void MovesBallWithoutBounce()
        {
            var ball = _service.CreateBall(50.0, 50.0);
            ball.Velocity = new Vector2D { X = 60.0, Y = 30.0 };
            double tableWidth = 1000.0, tableHeight = 1000.0;
            double initialX = ball.X, initialY = ball.Y;
            double dt = 1.0 / 60.0;

            _service.UpdatePosition(ball, tableWidth, tableHeight);

            Assert.AreEqual(initialX + 60.0 * dt, ball.X, 1e-6);
            Assert.AreEqual(initialY + 30.0 * dt, ball.Y, 1e-6);
            Assert.AreEqual(60.0, ball.Velocity.X, 1e-6);
            Assert.AreEqual(30.0, ball.Velocity.Y, 1e-6);
        }

        [TestMethod]
        public void InvertsXVelocity()
        {
            var ball = _service.CreateBall(0.0, 50.0);
            ball.Velocity = new Vector2D { X = -60.0, Y = 0.0 };

            _service.UpdatePosition(ball, 100.0, 100.0);

            Assert.AreEqual(60.0, ball.Velocity.X, 1e-6);
            Assert.AreEqual(0.0, ball.Velocity.Y, 1e-6);
        }

        [TestMethod]
        public void InvertsYVelocity()
        {
            var ball = _service.CreateBall(50.0, 0.0);
            ball.Velocity = new Vector2D { X = 0.0, Y = -120.0 };

            _service.UpdatePosition(ball, 100.0, 100.0);

            Assert.AreEqual(0.0, ball.Velocity.X, 1e-6);
            Assert.AreEqual(120.0, ball.Velocity.Y, 1e-6);
        }
    }
}

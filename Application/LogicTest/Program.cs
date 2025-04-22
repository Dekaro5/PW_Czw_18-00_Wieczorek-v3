using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Models;
using BusinessLogic.Services;

namespace BusinessLogicTest
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
     /*   public void TestChangeCoordinates()
        {
            var ball = new Ball { X = 0, Y = 0, VelocityX = 5, VelocityY = 3 };
            var service = new BallService();

            service.UpdatePosition(ball);

            Assert.AreEqual(5, ball.X);
            Assert.AreEqual(3, ball.Y);
        }

        [TestMethod]
*/
        public void TestStartingCoordinates()
        {
            var service = new BallService();
            var ball = service.CreateBall(10, 20);
            Assert.AreEqual(10, ball.X);
            Assert.AreEqual(20, ball.Y);
        }

        [TestMethod]

        public void TestBadStartingCoordinates()
        {
            var service = new BallService();
            var ball = service.CreateBall(20, 10);
            Assert.AreNotEqual(10, ball.X);
            Assert.AreNotEqual(20, ball.Y);
        }
    }
}
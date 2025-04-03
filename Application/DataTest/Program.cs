using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Models;

namespace DataTest
{
    [TestClass]
    public class BallTests
    {
        [TestMethod]
        public void TestSize()
        {
            var ball = new Ball();

            Assert.AreEqual(20, ball.Diameter);
        }

        [TestMethod]
        
        public void TestBadSize()
        {
            var ball = new Ball();
            ball.Diameter = 20;
            Assert.AreNotEqual(30, ball.Diameter);
        }
    }
}
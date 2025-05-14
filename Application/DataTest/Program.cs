using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System.ComponentModel;

namespace Data.Tests
{
    [TestClass]
    public class BallTests
    {
        [TestMethod]
        public void PropertiesCorrect()
        {
            double x = 10.0;
            double y = 20.0;
            double diameter = 30.0;
            double mass = 40.0;
            var velocity = new Vector2D { X = 1.0, Y = -1.0 };

            var ball = new Ball(x, y, diameter, mass, velocity);

            Assert.AreEqual(x, ball.X);
            Assert.AreEqual(y, ball.Y);
            Assert.AreEqual(diameter, ball.Diameter);
            Assert.AreEqual(mass, ball.Mass);
            Assert.AreEqual(velocity.X, ball.Velocity.X);
            Assert.AreEqual(velocity.Y, ball.Velocity.Y);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullVelocity()
        {
            var ball = new Ball(0, 0, 1.0, 1.0, null);
        }

        [TestMethod]
        public void FiresEvent()
        {
            var ball = new Ball(0, 0, 10, 10, new Vector2D());
            string changedProperty = null;
            ball.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            ball.X = 5.0;

            Assert.AreEqual("X", changedProperty);
        }

        [TestMethod]
        public void NotFiredWhenSameValue()
        {
            var ball = new Ball(5.0, 0, 10, 10, new Vector2D());
            bool eventFired = false;
            ball.PropertyChanged += (s, e) => eventFired = true;

            ball.X = 5.0;

            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void YFiresEvent()
        {
            var ball = new Ball(0, 0, 10, 10, new Vector2D());
            string changedProperty = null;
            ball.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            ball.Y = 5.0;

            Assert.AreEqual("Y", changedProperty);
        }

        [TestMethod]
        public void VFiresEvent()
        {
            var ball = new Ball(0, 0, 10, 10, new Vector2D());
            string changedProperty = null;
            ball.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            ball.Velocity = new Vector2D { X = 1.0, Y = -1.0 };

            Assert.AreEqual("Velocity", changedProperty);
        }

        [TestMethod]
        public void VNotFiredWhenSameValue()
        {
            var velocity = new Vector2D { X = 2.0, Y = 3.0 };
            var ball = new Ball(0, 0, 10, 10, velocity);
            bool eventFired = false;
            ball.PropertyChanged += (s, e) => eventFired = true;

            ball.Velocity = velocity;

            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void UpdatesPositionCorrectly()
        {
            var ball = new Ball(10.0, 20.0, 10, 10, new Vector2D { X = 2.0, Y = 3.0 });
            double timeStep = 0.5;

            ball.Move(timeStep);

            Assert.AreEqual(11.0, ball.X);
            Assert.AreEqual(21.5, ball.Y);
        }

        [TestMethod]
        public void FiresPropertyChangedEvents()
        {
            var ball = new Ball(10.0, 20.0, 10, 10, new Vector2D { X = 2.0, Y = 3.0 });
            int eventCount = 0;
            ball.PropertyChanged += (s, e) => eventCount++;

            ball.Move(0.5);

            Assert.AreEqual(2, eventCount);
        }
    }
}
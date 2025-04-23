using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Models;

namespace Data.Tests
{
    [TestClass]
    public class BallTests
    {
        [TestMethod]
        public void FiresEventWhenChanged()
        {
            var ball = new Ball();
            string changedProperty = null;
            ball.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            ball.X = 5.0;

            Assert.AreEqual("X", changedProperty);
        }

        [TestMethod]
        public void DoesNotFireWhenSameValue()
        {
            var ball = new Ball { X = 5.0 };
            bool eventFired = false;
            ball.PropertyChanged += (s, e) => eventFired = true;

            ball.X = 5.0;

            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void VelocityPropertyChanged()
        {
            var ball = new Ball();
            string changedProperty = null;
            ball.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            ball.Velocity = new Vector2D { X = 1.0, Y = -1.0 };

            Assert.AreEqual("Velocity", changedProperty);
        }

        [TestMethod]
        public void VelocityPropertyChanged_not()
        {
            var vec = new Vector2D { X = 2.0, Y = 3.0 };
            var ball = new Ball { Velocity = vec };
            bool eventFired = false;
            ball.PropertyChanged += (s, e) => eventFired = true;

            ball.Velocity = vec;

            Assert.IsFalse(eventFired);
        }
    }
}
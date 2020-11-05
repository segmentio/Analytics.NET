using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RudderStack.Request;

namespace RudderStack.Test
{
    [TestFixture]
    class BackoTests
    {
        [TestCase(1, ExpectedResult = 100)]
        [TestCase(2, ExpectedResult = 200)]
        [TestCase(3, ExpectedResult = 400)]
        public int TimeShouldIncreaseExponentially(int calls)
        {
            //Arrange
            var backo = new Backo(jitter: 0);

            //Act
            for (var i = 0; i < calls - 1; i++)
                backo.AttemptTime();
            var time = backo.AttemptTime();

            //Assert
            return time;
        }

        [TestCase(1, 500, ExpectedResult = 100)]
        [TestCase(3, 500, ExpectedResult = 400)]
        [TestCase(4, 500, ExpectedResult = 500)]
        public int TimeShouldBeLessOrEqualToMax(int calls, int max)
        {
            //Arrange
            var backo = new Backo(jitter: 0, max: max);

            //Act
            for (var i = 0; i < calls - 1; i++)
                backo.AttemptTime();

            var time = backo.AttemptTime();

            //Assert
            return time;
        }

        [TestCase(3, 500,ExpectedResult = 2000)]
        [TestCase(3, 250, ExpectedResult = 1000)]
        [TestCase(3, 75, ExpectedResult = 300)]
        public int TimeShouldDependOnMin(int calls, int min)
        {
            //Arrange
            var backo = new Backo(jitter: 0, min: min);

            //Act
            for (var i = 0; i < calls - 1; i++)
                backo.AttemptTime();

            var time = backo.AttemptTime();

            //Assert
            return time;
        }

        [TestCase(3, 2, ExpectedResult = 400)]
        [TestCase(3, 4, ExpectedResult = 1600)]
        [TestCase(3, 10, ExpectedResult = 10000)]
        public int TimeShouldDependOnFactor(int calls, byte factor)
        {
            //Arrange
            var backo = new Backo(jitter: 0, factor: factor);

            //Act
            for (var i = 0; i < calls - 1; i++)
                backo.AttemptTime();

            var time = backo.AttemptTime();

            //Assert
            return time;
        }

        [TestCase(1, (ushort)200, 100)]
        [TestCase(3, (ushort)400, 400)]
        [TestCase(5, (ushort)800, 1600)]
        public void AJitterShouldBeAdded(int calls, ushort jitter, int expectedTime)
        {
            //Arrange
            var backo = new Backo(jitter: jitter);

            //Act
            for (var i = 0; i < calls - 1; i++)
                backo.AttemptTime();

            var time = backo.AttemptTime();

            //Assert
            Assert.IsTrue(time >= expectedTime);
            Assert.IsTrue(time - expectedTime <= jitter);
        }
    }
}

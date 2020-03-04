using Microsoft.VisualStudio.TestTools.UnitTesting;
using Segment.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Segment.Test.Stats
{
    [TestClass]
    public class StatisticsTests
    {
        Statistics _statistics;
        [TestInitialize]
        public void Init()
        {
            _statistics = new Statistics();
        }

        [TestMethod]
        public void IncrementStatisticWorks()
        {
            _statistics.IncrementSubmitted();
            _statistics.IncrementSubmitted();
            _statistics.IncrementSubmitted();
            _statistics.IncrementSucceeded();
            _statistics.IncrementSucceeded();
            _statistics.IncrementFailed();

            Assert.AreEqual(1, _statistics.Failed);
            Assert.AreEqual(2, _statistics.Succeeded);
            Assert.AreEqual(3, _statistics.Submitted);
        }

        [TestMethod]
        public void IncrementSetToZeroWhenStatisticHasReachedMaxValue()
        {
            SetIntMaxValueToStatistic("_submitted");
            SetIntMaxValueToStatistic("_succeeded");
            SetIntMaxValueToStatistic("_failed");

            _statistics.IncrementSubmitted();
            _statistics.IncrementSucceeded();
            _statistics.IncrementFailed();

            Assert.AreEqual(0, _statistics.Failed);
            Assert.AreEqual(0, _statistics.Succeeded);
            Assert.AreEqual(0, _statistics.Submitted);
        }

        private void SetIntMaxValueToStatistic(string field)
        {
            var type = _statistics.GetType();
            var prop = type.GetField(field,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            prop.SetValue(_statistics, int.MaxValue);

        }
    }
}

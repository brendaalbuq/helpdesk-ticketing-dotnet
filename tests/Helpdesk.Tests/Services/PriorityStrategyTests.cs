//-----------------------------------------------------------------
//    <copyright file="PriorityStrategyTests.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>10-07-2026</date>
//    <time>18:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Services;

namespace Helpdesk.Tests.Services
{
    /// <summary>
    /// Unit tests for the default priority strategy.
    /// </summary>
    [TestFixture]
    public class PriorityStrategyTests
    {
        #region Tests

        /// <summary>
        /// Verifies that critical tickets have a four-hour limit.
        /// </summary>
        [Test]
        public void CalculateLimitDate_WithCriticalPriority_ShouldAddFourHours()
        {
            DefaultPriorityStrategy strategy = new DefaultPriorityStrategy();
            DateTime createdAt = new DateTime(2026, 7, 14, 10, 0, 0);

            DateTime limitDate = strategy.CalculateLimitDate(TicketPriority.Critical, createdAt);

            Assert.That(limitDate, Is.EqualTo(createdAt.AddHours(4)));
        }

        /// <summary>
        /// Verifies that low priority tickets have a seventy-two-hour limit.
        /// </summary>
        [Test]
        public void CalculateLimitDate_WithLowPriority_ShouldAddSeventyTwoHours()
        {
            DefaultPriorityStrategy strategy = new DefaultPriorityStrategy();
            DateTime createdAt = new DateTime(2026, 7, 14, 10, 0, 0);

            DateTime limitDate = strategy.CalculateLimitDate(TicketPriority.Low, createdAt);

            Assert.That(limitDate, Is.EqualTo(createdAt.AddHours(72)));
        }

        /// <summary>
        /// Verifies the remaining priority limits.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="hours">The expected hours.</param>
        [TestCase(TicketPriority.High, 8)]
        [TestCase(TicketPriority.Normal, 24)]
        public void CalculateLimitDate_WithOtherPriorities_ShouldAddExpectedHours(TicketPriority priority, int hours)
        {
            DefaultPriorityStrategy strategy = new DefaultPriorityStrategy();
            DateTime createdAt = new DateTime(2026, 7, 14, 10, 0, 0);

            DateTime limitDate = strategy.CalculateLimitDate(priority, createdAt);

            Assert.That(limitDate, Is.EqualTo(createdAt.AddHours(hours)));
        }

        #endregion
    }
}


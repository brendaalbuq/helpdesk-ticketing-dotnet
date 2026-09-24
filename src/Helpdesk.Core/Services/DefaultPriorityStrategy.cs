//-----------------------------------------------------------------
//    <copyright file="DefaultPriorityStrategy.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>06-07-2026</date>
//    <time>18:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Services
{
    /// <summary>
    /// Represents the default strategy for ticket limit dates.
    /// </summary>
    [CLSCompliant(true)]
    public class DefaultPriorityStrategy : IPriorityStrategy
    {
        #region Methods

        /// <summary>
        /// Calculates the limit date based on ticket priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="createdAt">The creation date.</param>
        /// <returns>The calculated limit date.</returns>
        public DateTime CalculateLimitDate(TicketPriority priority, DateTime createdAt)
        {
            return priority switch
            {
                TicketPriority.Critical => createdAt.AddHours(4),
                TicketPriority.High => createdAt.AddHours(8),
                TicketPriority.Normal => createdAt.AddHours(24),
                TicketPriority.Low => createdAt.AddHours(72),
                _ => createdAt.AddHours(24)
            };
        }

        #endregion
    }
}


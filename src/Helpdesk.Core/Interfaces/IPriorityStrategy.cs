//-----------------------------------------------------------------
//    <copyright file="IPriorityStrategy.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>11:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Interfaces
{
    /// <summary>
    /// Represents a strategy for calculating ticket limit dates.
    /// </summary>
    [CLSCompliant(true)]
    public interface IPriorityStrategy
    {
        #region Methods

        /// <summary>
        /// Calculates the service limit date based on priority.
        /// </summary>
        /// <param name="priority">The ticket priority.</param>
        /// <param name="createdAt">The ticket creation date.</param>
        /// <returns>The calculated limit date.</returns>
        DateTime CalculateLimitDate(TicketPriority priority, DateTime createdAt);

        #endregion
    }
}


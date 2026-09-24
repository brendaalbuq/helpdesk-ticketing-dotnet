//-----------------------------------------------------------------
//    <copyright file="ITicketObserver.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>11:45</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Interfaces
{
    /// <summary>
    /// Represents an observer notified when a ticket status changes.
    /// </summary>
    [CLSCompliant(true)]
    public interface ITicketObserver
    {
        #region Methods

        /// <summary>
        /// Handles a ticket status change.
        /// </summary>
        /// <param name="ticket">The changed ticket.</param>
        /// <param name="previousStatus">The previous status.</param>
        /// <param name="currentStatus">The current status.</param>
        void OnTicketStatusChanged(Ticket ticket, TicketStatus previousStatus, TicketStatus currentStatus);

        #endregion
    }
}

//-----------------------------------------------------------------
//    <copyright file="TicketLogObserver.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>08-07-2026</date>
//    <time>18:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Interfaces;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Services
{
    /// <summary>
    /// Observes ticket status changes and writes them to the log.
    /// </summary>
    [CLSCompliant(true)]
    public class TicketLogObserver : ITicketObserver
    {
        #region Fields

        private readonly ILogService _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketLogObserver"/> class.
        /// </summary>
        /// <param name="logger">The log service.</param>
        public TicketLogObserver(ILogService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles ticket status changes.
        /// </summary>
        /// <param name="ticket">The changed ticket.</param>
        /// <param name="previousStatus">The previous status.</param>
        /// <param name="currentStatus">The current status.</param>
        public void OnTicketStatusChanged(Ticket ticket, TicketStatus previousStatus, TicketStatus currentStatus)
        {
            _logger.Info($"Ticket {ticket.TicketNumber} changed from {previousStatus} to {currentStatus}");
        }

        #endregion
    }
}

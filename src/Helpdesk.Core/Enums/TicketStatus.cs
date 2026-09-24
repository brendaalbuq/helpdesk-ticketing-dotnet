//-----------------------------------------------------------------
//    <copyright file="TicketStatus.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>20:45</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Enums
{
    /// <summary>
    /// Represents the current state of a ticket.
    /// </summary>
    [CLSCompliant(true)]
    public enum TicketStatus
    {
        WaitingAssignment = 1,
        InProgress = 2,
        WaitingClient = 3,
        Resolved = 4,
        Closed = 5,
        Cancelled = 6
    }
}


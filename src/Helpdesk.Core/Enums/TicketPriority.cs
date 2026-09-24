//-----------------------------------------------------------------
//    <copyright file="TicketPriority.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>20:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Enums
{
    /// <summary>
    /// Represents the ticket priority.
    /// </summary>
    [CLSCompliant(true)]
    public enum TicketPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Critical = 4
    }
}


//-----------------------------------------------------------------
//    <copyright file="TicketPriorityQueue.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>07-07-2026</date>
//    <time>19:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Collections
{
    /// <summary>
    /// Represents a waiting queue grouped by ticket priority.
    /// </summary>
    [CLSCompliant(true)]
    public class TicketPriorityQueue
    {
        #region Fields

        private readonly Dictionary<TicketPriority, Queue<Ticket>> _queues;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketPriorityQueue"/> class.
        /// </summary>
        public TicketPriorityQueue()
        {
            _queues = Enum
                .GetValues<TicketPriority>()
                .ToDictionary(priority => priority, _ => new Queue<Ticket>());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of waiting tickets.
        /// </summary>
        public int Count => _queues.Values.Sum(queue => queue.Count);

        #endregion

        #region Methods

        /// <summary>
        /// Adds a ticket to the correct priority queue.
        /// </summary>
        /// <param name="ticket">The ticket to enqueue.</param>
        public void Enqueue(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null.");

            if (Contains(ticket.ID))
                return;

            _queues[ticket.Priority].Enqueue(ticket);
        }

        /// <summary>
        /// Removes a ticket from all queues.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <returns>True if the ticket was removed; otherwise, false.</returns>
        public bool Remove(Guid ticketId)
        {
            bool removed = false;

            foreach (TicketPriority priority in _queues.Keys.ToList())
            {
                Queue<Ticket> rebuiltQueue = new Queue<Ticket>();

                while (_queues[priority].Count > 0)
                {
                    Ticket ticket = _queues[priority].Dequeue();

                    if (ticket.ID == ticketId)
                    {
                        removed = true;
                        continue;
                    }

                    rebuiltQueue.Enqueue(ticket);
                }

                _queues[priority] = rebuiltQueue;
            }

            return removed;
        }

        /// <summary>
        /// Gets all waiting tickets ordered by priority.
        /// </summary>
        /// <returns>The ordered tickets.</returns>
        public IEnumerable<Ticket> GetAllOrdered()
        {
            return new[]
                {
                    TicketPriority.Critical,
                    TicketPriority.High,
                    TicketPriority.Normal,
                    TicketPriority.Low
                }
                .SelectMany(priority => _queues[priority].ToList())
                .ToList();
        }

        /// <summary>
        /// Checks if a ticket is already in the queue.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <returns>True if it exists; otherwise, false.</returns>
        private bool Contains(Guid ticketId)
        {
            return _queues.Values.Any(queue => queue.Any(ticket => ticket.ID == ticketId));
        }

        #endregion
    }
}

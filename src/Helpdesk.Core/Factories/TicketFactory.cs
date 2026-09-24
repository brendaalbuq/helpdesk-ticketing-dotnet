//-----------------------------------------------------------------
//    <copyright file="TicketFactory.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>07-07-2026</date>
//    <time>18:45</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;
using Helpdesk.Core.Interfaces;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Factories
{
    /// <summary>
    /// Creates tickets with consistent default data.
    /// </summary>
    [CLSCompliant(true)]
    public static class TicketFactory
    {
        #region Methods

        /// <summary>
        /// Creates a new ticket.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="product">The product.</param>
        /// <param name="assistanceType">The assistance type.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="description">The description.</param>
        /// <param name="priorityStrategy">The priority strategy.</param>
        /// <returns>The created ticket.</returns>
        public static Ticket Create(
            Client client,
            Product product,
            AssistanceType assistanceType,
            TicketPriority priority,
            string subject,
            string description,
            IPriorityStrategy priorityStrategy)
        {
            if (priorityStrategy == null)
                throw new ArgumentNullException(nameof(priorityStrategy), "Priority strategy cannot be null.");

            DateTime createdAt = DateTime.Now;
            DateTime limitDate = priorityStrategy.CalculateLimitDate(priority, createdAt);

            return new Ticket(
                CreateTicketNumber(),
                client,
                product,
                assistanceType,
                priority,
                subject,
                description,
                createdAt,
                limitDate);
        }

        /// <summary>
        /// Creates a readable unique ticket number.
        /// </summary>
        /// <returns>The ticket number.</returns>
        private static string CreateTicketNumber()
        {
            string suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            return $"HD-{DateTime.Now:yyyyMMdd}-{suffix}";
        }

        #endregion
    }
}


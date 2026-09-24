//-----------------------------------------------------------------
//    <copyright file="TicketInteraction.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>03-07-2026</date>
//    <time>18:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents an interaction in a ticket history.
    /// </summary>
    [CLSCompliant(true)]
    public class TicketInteraction : BaseEntity
    {
        #region Fields

        private string _authorName = string.Empty;
        private string _message = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        public string AuthorName
        {
            get => _authorName;
            set => _authorName = Guard.RequiredText(value, nameof(AuthorName));
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => _message;
            set => _message = Guard.RequiredText(value, nameof(Message));
        }

        /// <summary>
        /// Gets or sets the interaction date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the interaction was sent by the client.
        /// </summary>
        public bool FromClient { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketInteraction"/> class.
        /// </summary>
        public TicketInteraction()
        {
            Date = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketInteraction"/> class.
        /// </summary>
        /// <param name="authorName">The author name.</param>
        /// <param name="message">The message.</param>
        /// <param name="fromClient">True if the author is a client.</param>
        public TicketInteraction(string authorName, string message, bool fromClient)
        {
            AuthorName = authorName;
            Message = message;
            FromClient = fromClient;
            Date = DateTime.Now;
        }

        #endregion
    }
}


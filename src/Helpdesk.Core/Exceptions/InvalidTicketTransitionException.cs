//-----------------------------------------------------------------
//    <copyright file="InvalidTicketTransitionException.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>04-07-2026</date>
//    <time>11:50</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Exceptions
{
    /// <summary>
    /// Represents an error caused by an invalid ticket state transition.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class InvalidTicketTransitionException : HelpdeskException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTicketTransitionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidTicketTransitionException(string message)
            : base(message)
        {
        }

        #endregion
    }
}


//-----------------------------------------------------------------
//    <copyright file="HelpdeskException.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>04-07-2026</date>
//    <time>10:15</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Exceptions
{
    /// <summary>
    /// Represents the base exception for the Helpdesk domain.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class HelpdeskException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpdeskException"/> class.
        /// </summary>
        public HelpdeskException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpdeskException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HelpdeskException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpdeskException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HelpdeskException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}


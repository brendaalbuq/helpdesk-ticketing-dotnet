//-----------------------------------------------------------------
//    <copyright file="DuplicateEntityException.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>04-07-2026</date>
//    <time>11:00</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Exceptions
{
    /// <summary>
    /// Represents an error caused by a duplicated entity.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class DuplicateEntityException : HelpdeskException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DuplicateEntityException(string message)
            : base(message)
        {
        }

        #endregion
    }
}


//-----------------------------------------------------------------
//    <copyright file="DomainValidationException.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>04-07-2026</date>
//    <time>10:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Exceptions
{
    /// <summary>
    /// Represents a validation error in the Helpdesk domain.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class DomainValidationException : HelpdeskException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
        /// </summary>
        /// <param name="message">The validation message.</param>
        public DomainValidationException(string message)
            : base(message)
        {
        }

        #endregion
    }
}


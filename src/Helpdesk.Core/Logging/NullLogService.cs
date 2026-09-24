//-----------------------------------------------------------------
//    <copyright file="NullLogService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>07-07-2026</date>
//    <time>20:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Logging
{
    /// <summary>
    /// Represents a log service that ignores all messages.
    /// </summary>
    [CLSCompliant(true)]
    public class NullLogService : ILogService
    {
        #region Methods

        /// <summary>
        /// Ignores an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
        }

        /// <summary>
        /// Ignores a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
        }

        /// <summary>
        /// Ignores an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception? exception = null)
        {
        }

        #endregion
    }
}


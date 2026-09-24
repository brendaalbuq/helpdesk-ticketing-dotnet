//-----------------------------------------------------------------
//    <copyright file="ILogService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>10:55</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Interfaces
{
    /// <summary>
    /// Represents a simple structured logging service.
    /// </summary>
    [CLSCompliant(true)]
    public interface ILogService
    {
        #region Methods

        /// <summary>
        /// Writes an information log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Writes a warning log.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);

        /// <summary>
        /// Writes an error log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(string message, Exception? exception = null);

        #endregion
    }
}


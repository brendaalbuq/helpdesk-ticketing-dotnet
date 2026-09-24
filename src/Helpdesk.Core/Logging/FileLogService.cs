//-----------------------------------------------------------------
//    <copyright file="FileLogService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>07-07-2026</date>
//    <time>21:00</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Logging
{
    /// <summary>
    /// Represents a file-based structured log service.
    /// </summary>
    [CLSCompliant(true)]
    public class FileLogService : ILogService
    {
        #region Fields

        private static readonly object _instanceLock = new object();
        private static FileLogService? _instance;

        private readonly object _writeLock;
        private readonly string _filePath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogService"/> class.
        /// </summary>
        /// <param name="filePath">The log file path.</param>
        private FileLogService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Log path cannot be empty.", nameof(filePath));

            _filePath = filePath;
            _writeLock = new object();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the singleton instance of the file log service.
        /// </summary>
        /// <param name="filePath">The log file path used when the instance is first created.</param>
        /// <returns>The singleton log service.</returns>
        public static FileLogService GetInstance(string filePath)
        {
            lock (_instanceLock)
            {
                _instance ??= new FileLogService(filePath);
                return _instance;
            }
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            Write("WARN", message);
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception? exception = null)
        {
            string finalMessage = exception == null ? message : $"{message} | {exception.GetType().Name}: {exception.Message}";
            Write("ERROR", finalMessage);
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        private void Write(string level, string message)
        {
            lock (_writeLock)
            {
                try
                {
                    string? directory = Path.GetDirectoryName(_filePath);

                    if (!string.IsNullOrWhiteSpace(directory))
                        Directory.CreateDirectory(directory);

                    string line = $"{DateTime.Now:O} | {level} | {message}{Environment.NewLine}";
                    File.AppendAllText(_filePath, line);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        #endregion
    }
}


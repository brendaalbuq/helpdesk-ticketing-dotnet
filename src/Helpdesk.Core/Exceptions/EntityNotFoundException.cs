//-----------------------------------------------------------------
//    <copyright file="EntityNotFoundException.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>04-07-2026</date>
//    <time>11:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Exceptions
{
    /// <summary>
    /// Represents an error caused by a missing entity.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class EntityNotFoundException : HelpdeskException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <param name="id">The entity identifier.</param>
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName} with ID {id} was not found.")
        {
        }

        #endregion
    }
}


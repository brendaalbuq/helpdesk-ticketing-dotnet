//-----------------------------------------------------------------
//    <copyright file="IIdentifiable.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>18:50</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Interfaces
{
    /// <summary>
    /// Represents an entity that has a unique identifier.
    /// </summary>
    [CLSCompliant(true)]
    public interface IIdentifiable
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid ID { get; }

        #endregion
    }
}


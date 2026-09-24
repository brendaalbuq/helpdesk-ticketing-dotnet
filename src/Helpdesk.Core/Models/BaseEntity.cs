//-----------------------------------------------------------------
//    <copyright file="BaseEntity.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>19:10</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Text.Json.Serialization;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a base entity with a unique identifier.
    /// </summary>
    [CLSCompliant(true)]
    public abstract class BaseEntity : IIdentifiable
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        [JsonInclude]
        public Guid ID { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        protected BaseEntity()
        {
            GenerateID();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a unique identifier.
        /// </summary>
        private void GenerateID()
        {
            if (ID == Guid.Empty)
                ID = Guid.NewGuid();
        }

        #endregion
    }
}


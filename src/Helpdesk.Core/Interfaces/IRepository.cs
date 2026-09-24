//-----------------------------------------------------------------
//    <copyright file="IRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>10:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Interfaces
{
    /// <summary>
    /// Represents a generic repository contract.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [CLSCompliant(true)]
    public interface IRepository<T> where T : IIdentifiable
    {
        #region Methods

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>True if the entity was removed; otherwise, false.</returns>
        bool Remove(Guid id);

        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity if it exists; otherwise, null.</returns>
        T? GetById(Guid id);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>All entities.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Finds entities using a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to apply.</param>
        /// <returns>The matching entities.</returns>
        IEnumerable<T> Find(Func<T, bool> predicate);

        /// <summary>
        /// Counts the entities.
        /// </summary>
        /// <returns>The number of entities.</returns>
        int Count();

        #endregion
    }
}


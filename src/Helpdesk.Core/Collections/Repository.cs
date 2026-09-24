//-----------------------------------------------------------------
//    <copyright file="Repository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>13:30</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Collections
{
    /// <summary>
    /// Represents a generic in-memory repository.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [CLSCompliant(true)]
    public class Repository<T> : IRepository<T> where T : IIdentifiable
    {
        #region Fields

        private readonly Dictionary<Guid, T> _items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        public Repository()
        {
            _items = new Dictionary<Guid, T>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            if (_items.ContainsKey(entity.ID))
                throw new DuplicateEntityException($"{typeof(T).Name} with ID {entity.ID} already exists.");

            _items.Add(entity.ID, entity);
        }

        /// <summary>
        /// Updates an entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            if (!_items.ContainsKey(entity.ID))
                throw new EntityNotFoundException(typeof(T).Name, entity.ID);

            _items[entity.ID] = entity;
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>True if the entity was removed; otherwise, false.</returns>
        public virtual bool Remove(Guid id)
        {
            return _items.Remove(id);
        }

        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public virtual T? GetById(Guid id)
        {
            _items.TryGetValue(id, out T? entity);
            return entity;
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>A copy of the entities.</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _items.Values.ToList();
        }

        /// <summary>
        /// Finds entities using a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to apply.</param>
        /// <returns>The matching entities.</returns>
        public virtual IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null.");

            return _items.Values.Where(predicate).ToList();
        }

        /// <summary>
        /// Counts the entities.
        /// </summary>
        /// <returns>The number of entities.</returns>
        public virtual int Count()
        {
            return _items.Count;
        }

        /// <summary>
        /// Replaces all items in the repository.
        /// </summary>
        /// <param name="entities">The entities to load.</param>
        protected void ReplaceAll(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "Entities cannot be null.");

            _items.Clear();

            foreach (T entity in entities)
                Add(entity);
        }

        #endregion
    }
}


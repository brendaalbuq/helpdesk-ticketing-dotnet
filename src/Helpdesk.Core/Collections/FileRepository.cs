//-----------------------------------------------------------------
//    <copyright file="FileRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>05-07-2026</date>
//    <time>15:10</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Text.Json;
using Helpdesk.Core.Interfaces;

namespace Helpdesk.Core.Collections
{
    /// <summary>
    /// Represents a repository that can persist data in a JSON file.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [CLSCompliant(true)]
    public class FileRepository<T> : Repository<T> where T : IIdentifiable
    {
        #region Fields

        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepository{T}"/> class.
        /// </summary>
        /// <param name="filePath">The JSON file path.</param>
        public FileRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));

            _filePath = filePath;
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the repository data to a JSON file.
        /// </summary>
        public void Save()
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(GetAll().ToList(), _options);
            File.WriteAllText(_filePath, json);
        }

        /// <summary>
        /// Loads the repository data from a JSON file.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_filePath))
                return;

            string json = File.ReadAllText(_filePath);
            List<T>? entities = JsonSerializer.Deserialize<List<T>>(json, _options);

            ReplaceAll(entities ?? new List<T>());
        }

        #endregion
    }
}


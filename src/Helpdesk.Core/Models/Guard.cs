//-----------------------------------------------------------------
//    <copyright file="Guard.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>21:15</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Exceptions;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Provides reusable validation helpers for domain models.
    /// </summary>
    internal static class Guard
    {
        #region Methods

        /// <summary>
        /// Validates a required text value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The trimmed value.</returns>
        internal static string RequiredText(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException($"{fieldName} cannot be empty.");

            return value.Trim();
        }

        /// <summary>
        /// Validates that an object is not null.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The validated value.</returns>
        internal static T RequiredObject<T>(T? value, string fieldName) where T : class
        {
            if (value == null)
                throw new DomainValidationException($"{fieldName} cannot be null.");

            return value;
        }

        /// <summary>
        /// Validates an evaluation score.
        /// </summary>
        /// <param name="score">The score.</param>
        /// <returns>The validated score.</returns>
        internal static int Score(int score)
        {
            if (score < 1 || score > 10)
                throw new DomainValidationException("Evaluation score must be between 1 and 10.");

            return score;
        }

        #endregion
    }
}


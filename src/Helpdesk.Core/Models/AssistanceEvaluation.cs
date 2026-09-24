//-----------------------------------------------------------------
//    <copyright file="AssistanceEvaluation.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>03-07-2026</date>
//    <time>20:35</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents the evaluation of a completed assistance.
    /// </summary>
    [CLSCompliant(true)]
    public class AssistanceEvaluation : BaseEntity
    {
        #region Fields

        private string _comment = string.Empty;
        private int _score = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the score from 1 to 10.
        /// </summary>
        public int Score
        {
            get => _score;
            set => _score = Guard.Score(value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the issue was solved.
        /// </summary>
        public bool WasSolved { get; set; }

        /// <summary>
        /// Gets or sets the evaluation comment.
        /// </summary>
        public string Comment
        {
            get => _comment;
            set => _comment = value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the evaluation date.
        /// </summary>
        public DateTime EvaluatedAt { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssistanceEvaluation"/> class.
        /// </summary>
        public AssistanceEvaluation()
        {
            EvaluatedAt = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssistanceEvaluation"/> class.
        /// </summary>
        /// <param name="score">The score from 1 to 10.</param>
        /// <param name="wasSolved">True if the issue was solved.</param>
        /// <param name="comment">The evaluation comment.</param>
        public AssistanceEvaluation(int score, bool wasSolved, string comment)
        {
            Score = score;
            WasSolved = wasSolved;
            Comment = comment;
            EvaluatedAt = DateTime.Now;
        }

        #endregion
    }
}


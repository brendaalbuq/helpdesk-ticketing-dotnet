//-----------------------------------------------------------------
//    <copyright file="KnownProblem.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>03-07-2026</date>
//    <time>19:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a known problem and its resolution.
    /// </summary>
    [CLSCompliant(true)]
    public class KnownProblem : BaseEntity
    {
        #region Fields

        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _solution = string.Empty;
        private string _productFamily = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the problem title.
        /// </summary>
        public string Title
        {
            get => _title;
            set => _title = Guard.RequiredText(value, nameof(Title));
        }

        /// <summary>
        /// Gets or sets the problem description.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = Guard.RequiredText(value, nameof(Description));
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        public string Solution
        {
            get => _solution;
            set => _solution = Guard.RequiredText(value, nameof(Solution));
        }

        /// <summary>
        /// Gets or sets the product family.
        /// </summary>
        public string ProductFamily
        {
            get => _productFamily;
            set => _productFamily = Guard.RequiredText(value, nameof(ProductFamily));
        }

        /// <summary>
        /// Gets or sets the assistance type.
        /// </summary>
        public AssistanceType AssistanceType { get; set; }

        /// <summary>
        /// Gets or sets the tutorials.
        /// </summary>
        public List<Tutorial> Tutorials { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownProblem"/> class.
        /// </summary>
        public KnownProblem()
        {
            Tutorials = new List<Tutorial>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownProblem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="productFamily">The product family.</param>
        /// <param name="assistanceType">The assistance type.</param>
        public KnownProblem(
            string title,
            string description,
            string solution,
            string productFamily,
            AssistanceType assistanceType)
        {
            Title = title;
            Description = description;
            Solution = solution;
            ProductFamily = productFamily;
            AssistanceType = assistanceType;
            Tutorials = new List<Tutorial>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a tutorial to the known problem.
        /// </summary>
        /// <param name="tutorial">The tutorial to add.</param>
        public void AddTutorial(Tutorial tutorial)
        {
            if (tutorial == null)
                throw new ArgumentNullException(nameof(tutorial), "Tutorial cannot be null.");

            Tutorials.Add(tutorial);
        }

        #endregion
    }
}


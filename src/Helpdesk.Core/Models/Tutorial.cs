//-----------------------------------------------------------------
//    <copyright file="Tutorial.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>03-07-2026</date>
//    <time>19:00</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a tutorial used to solve a known problem.
    /// </summary>
    [CLSCompliant(true)]
    public class Tutorial : BaseEntity
    {
        #region Fields

        private string _title = string.Empty;
        private string _content = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => _title;
            set => _title = Guard.RequiredText(value, nameof(Title));
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content
        {
            get => _content;
            set => _content = Guard.RequiredText(value, nameof(Content));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tutorial"/> class.
        /// </summary>
        public Tutorial()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tutorial"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        public Tutorial(string title, string content)
        {
            Title = title;
            Content = content;
        }

        #endregion
    }
}


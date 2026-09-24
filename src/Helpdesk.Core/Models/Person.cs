//-----------------------------------------------------------------
//    <copyright file="Person.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>02-07-2026</date>
//    <time>18:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a base person in the Helpdesk system.
    /// </summary>
    [CLSCompliant(true)]
    public abstract class Person : BaseEntity
    {
        #region Fields

        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phoneNumber = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName
        {
            get => _fullName;
            set => _fullName = Guard.RequiredText(value, nameof(FullName));
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email
        {
            get => _email;
            set => _email = Guard.RequiredText(value, nameof(Email));
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = Guard.RequiredText(value, nameof(PhoneNumber));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        protected Person()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        protected Person(string fullName, string email, string phoneNumber)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        #endregion
    }
}


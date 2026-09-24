//-----------------------------------------------------------------
//    <copyright file="SupportOperator.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>02-07-2026</date>
//    <time>21:10</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a support operator.
    /// </summary>
    [CLSCompliant(true)]
    public class SupportOperator : Person
    {
        #region Fields

        private string _employeeNumber = string.Empty;
        private string _department = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set => _employeeNumber = Guard.RequiredText(value, nameof(EmployeeNumber));
        }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        public string Department
        {
            get => _department;
            set => _department = Guard.RequiredText(value, nameof(Department));
        }

        /// <summary>
        /// Gets or sets the operator level.
        /// </summary>
        public OperatorLevel Level { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operator is available.
        /// </summary>
        public bool IsAvailable { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportOperator"/> class.
        /// </summary>
        public SupportOperator()
        {
            IsAvailable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportOperator"/> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="employeeNumber">The employee number.</param>
        /// <param name="department">The department.</param>
        /// <param name="level">The operator level.</param>
        public SupportOperator(
            string fullName,
            string email,
            string phoneNumber,
            string employeeNumber,
            string department,
            OperatorLevel level)
            : base(fullName, email, phoneNumber)
        {
            EmployeeNumber = employeeNumber;
            Department = department;
            Level = level;
            IsAvailable = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the operator can handle a ticket priority.
        /// </summary>
        /// <param name="priority">The ticket priority.</param>
        /// <returns>True if the operator can handle it; otherwise, false.</returns>
        public bool CanHandle(TicketPriority priority)
        {
            return priority != TicketPriority.Critical || Level != OperatorLevel.Junior;
        }

        /// <summary>
        /// Returns a readable operator summary.
        /// </summary>
        /// <returns>The operator summary.</returns>
        public override string ToString()
        {
            return $"{FullName} - {Department} - {Level} - Available: {IsAvailable}";
        }

        #endregion
    }
}


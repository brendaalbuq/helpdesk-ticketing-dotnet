//-----------------------------------------------------------------
//    <copyright file="Ticket.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>06-07-2026</date>
//    <time>20:15</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Text.Json.Serialization;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a technical assistance ticket.
    /// </summary>
    [CLSCompliant(true)]
    public class Ticket : BaseEntity
    {
        #region Fields

        private string _ticketNumber = string.Empty;
        private string _subject = string.Empty;
        private string _description = string.Empty;
        private Client? _client;
        private Product? _product;
        private SupportOperator? _assignedOperator;
        private KnownProblem? _knownProblem;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the human-readable ticket number.
        /// </summary>
        public string TicketNumber
        {
            get => _ticketNumber;
            set => _ticketNumber = Guard.RequiredText(value, nameof(TicketNumber));
        }

        /// <summary>
        /// Gets or sets the ticket subject.
        /// </summary>
        public string Subject
        {
            get => _subject;
            set => _subject = Guard.RequiredText(value, nameof(Subject));
        }

        /// <summary>
        /// Gets or sets the detailed description.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = Guard.RequiredText(value, nameof(Description));
        }

        /// <summary>
        /// Gets the client identifier stored in JSON.
        /// </summary>
        [JsonInclude]
        public Guid ClientId { get; private set; }

        /// <summary>
        /// Gets the product identifier stored in JSON.
        /// </summary>
        [JsonInclude]
        public Guid ProductId { get; private set; }

        /// <summary>
        /// Gets the assigned operator identifier stored in JSON.
        /// </summary>
        [JsonInclude]
        public Guid? AssignedOperatorId { get; private set; }

        /// <summary>
        /// Gets the known problem identifier stored in JSON.
        /// </summary>
        [JsonInclude]
        public Guid? KnownProblemId { get; private set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        [JsonIgnore]
        public Client Client
        {
            get => _client ?? throw new DomainValidationException("Ticket client cannot be null.");
            set
            {
                _client = Guard.RequiredObject(value, nameof(Client));
                ClientId = _client.ID;
            }
        }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        [JsonIgnore]
        public Product Product
        {
            get => _product ?? throw new DomainValidationException("Ticket product cannot be null.");
            set
            {
                _product = Guard.RequiredObject(value, nameof(Product));
                ProductId = _product.ID;
            }
        }

        /// <summary>
        /// Gets or sets the assigned operator.
        /// </summary>
        [JsonIgnore]
        public SupportOperator? AssignedOperator
        {
            get => _assignedOperator;
            set
            {
                _assignedOperator = value;
                AssignedOperatorId = value?.ID;
            }
        }

        /// <summary>
        /// Gets or sets the linked known problem.
        /// </summary>
        [JsonIgnore]
        public KnownProblem? KnownProblem
        {
            get => _knownProblem;
            set
            {
                _knownProblem = value;
                KnownProblemId = value?.ID;
            }
        }

        /// <summary>
        /// Gets or sets the assistance type.
        /// </summary>
        public AssistanceType AssistanceType { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public TicketPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        [JsonInclude]
        public TicketStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [JsonInclude]
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        [JsonInclude]
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Gets or sets the service limit date.
        /// </summary>
        public DateTime LimitDate { get; set; }

        /// <summary>
        /// Gets or sets the closing date.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Gets or sets the ticket interactions.
        /// </summary>
        public List<TicketInteraction> Interactions { get; set; }

        /// <summary>
        /// Gets or sets the ticket evaluation.
        /// </summary>
        public AssistanceEvaluation? Evaluation { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ticket is overdue.
        /// </summary>
        public bool IsOverdue => Status != TicketStatus.Closed &&
                                 Status != TicketStatus.Cancelled &&
                                 DateTime.Now > LimitDate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class.
        /// </summary>
        public Ticket()
        {
            Interactions = new List<TicketInteraction>();
            CreatedAt = DateTime.Now;
            UpdatedAt = CreatedAt;
            Status = TicketStatus.WaitingAssignment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class.
        /// </summary>
        /// <param name="ticketNumber">The ticket number.</param>
        /// <param name="client">The client.</param>
        /// <param name="product">The product.</param>
        /// <param name="assistanceType">The assistance type.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="description">The description.</param>
        /// <param name="createdAt">The creation date.</param>
        /// <param name="limitDate">The service limit date.</param>
        public Ticket(
            string ticketNumber,
            Client client,
            Product product,
            AssistanceType assistanceType,
            TicketPriority priority,
            string subject,
            string description,
            DateTime createdAt,
            DateTime limitDate)
        {
            TicketNumber = ticketNumber;
            Client = client;
            Product = product;
            AssistanceType = assistanceType;
            Priority = priority;
            Subject = subject;
            Description = description;
            CreatedAt = createdAt;
            UpdatedAt = createdAt;
            LimitDate = limitDate;
            Status = TicketStatus.WaitingAssignment;
            Interactions = new List<TicketInteraction>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assigns the ticket to an operator.
        /// </summary>
        /// <param name="supportOperator">The support operator.</param>
        public void AssignTo(SupportOperator supportOperator)
        {
            if (Status == TicketStatus.Closed || Status == TicketStatus.Cancelled)
                throw new InvalidTicketTransitionException("Closed or cancelled tickets cannot be assigned.");

            AssignedOperator = Guard.RequiredObject(supportOperator, nameof(supportOperator));
            Status = TicketStatus.InProgress;
            Touch();
        }

        /// <summary>
        /// Adds an interaction to the ticket.
        /// </summary>
        /// <param name="interaction">The interaction to add.</param>
        public void AddInteraction(TicketInteraction interaction)
        {
            if (Status == TicketStatus.Closed || Status == TicketStatus.Cancelled)
                throw new InvalidTicketTransitionException("Closed or cancelled tickets cannot receive new interactions.");

            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction), "Interaction cannot be null.");

            Interactions.Add(interaction);
            Touch();
        }

        /// <summary>
        /// Links a known problem to the ticket.
        /// </summary>
        /// <param name="knownProblem">The known problem.</param>
        public void LinkKnownProblem(KnownProblem knownProblem)
        {
            KnownProblem = Guard.RequiredObject(knownProblem, nameof(knownProblem));
            Touch();
        }

        /// <summary>
        /// Marks the ticket as waiting for the client.
        /// </summary>
        public void WaitClient()
        {
            if (Status != TicketStatus.InProgress)
                throw new InvalidTicketTransitionException("Only in-progress tickets can wait for client feedback.");

            Status = TicketStatus.WaitingClient;
            Touch();
        }

        /// <summary>
        /// Resolves the ticket.
        /// </summary>
        /// <param name="resolution">The resolution message.</param>
        public void Resolve(string resolution)
        {
            if (AssignedOperator == null)
                throw new InvalidTicketTransitionException("Ticket must be assigned before resolution.");

            if (Status == TicketStatus.Closed || Status == TicketStatus.Cancelled)
                throw new InvalidTicketTransitionException("Closed or cancelled tickets cannot be resolved.");

            AddInteraction(new TicketInteraction(AssignedOperator.FullName, resolution, false));
            Status = TicketStatus.Resolved;
            Touch();
        }

        /// <summary>
        /// Closes the ticket.
        /// </summary>
        public void Close()
        {
            if (Status != TicketStatus.Resolved)
                throw new InvalidTicketTransitionException("Only resolved tickets can be closed.");

            Status = TicketStatus.Closed;
            ClosedAt = DateTime.Now;
            Touch();
        }

        /// <summary>
        /// Cancels the ticket.
        /// </summary>
        public void Cancel()
        {
            if (Status == TicketStatus.Closed)
                throw new InvalidTicketTransitionException("Closed tickets cannot be cancelled.");

            Status = TicketStatus.Cancelled;
            ClosedAt = DateTime.Now;
            Touch();
        }

        /// <summary>
        /// Evaluates a closed ticket.
        /// </summary>
        /// <param name="evaluation">The evaluation.</param>
        public void Evaluate(AssistanceEvaluation evaluation)
        {
            if (Status != TicketStatus.Closed)
                throw new InvalidTicketTransitionException("Only closed tickets can be evaluated.");

            Evaluation = Guard.RequiredObject(evaluation, nameof(evaluation));
            Touch();
        }

        /// <summary>
        /// Restores navigation references after loading identifiers from JSON.
        /// </summary>
        /// <param name="client">The canonical client.</param>
        /// <param name="product">The canonical product.</param>
        /// <param name="supportOperator">The canonical assigned operator.</param>
        /// <param name="knownProblem">The canonical known problem.</param>
        public void RestoreReferences(
            Client client,
            Product product,
            SupportOperator? supportOperator,
            KnownProblem? knownProblem)
        {
            if (client.ID != ClientId)
                throw new DomainValidationException("The client does not match the ticket client identifier.");

            if (product.ID != ProductId)
                throw new DomainValidationException("The product does not match the ticket product identifier.");

            if (supportOperator?.ID != AssignedOperatorId)
                throw new DomainValidationException("The operator does not match the ticket operator identifier.");

            if (knownProblem?.ID != KnownProblemId)
                throw new DomainValidationException("The known problem does not match the ticket known problem identifier.");

            _client = client;
            _product = product;
            _assignedOperator = supportOperator;
            _knownProblem = knownProblem;
        }

        /// <summary>
        /// Updates the last modification date.
        /// </summary>
        private void Touch()
        {
            UpdatedAt = DateTime.Now;
        }

        #endregion
    }
}


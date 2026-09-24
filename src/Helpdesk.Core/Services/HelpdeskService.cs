//-----------------------------------------------------------------
//    <copyright file="HelpdeskService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>08-07-2026</date>
//    <time>19:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Factories;
using Helpdesk.Core.Interfaces;
using Helpdesk.Core.Models;

namespace Helpdesk.Core.Services
{
    /// <summary>
    /// Represents the application service for Helpdesk operations.
    /// </summary>
    [CLSCompliant(true)]
    public class HelpdeskService
    {
        #region Fields

        private readonly IRepository<Client> _clients;
        private readonly IRepository<SupportOperator> _operators;
        private readonly IRepository<Product> _products;
        private readonly IRepository<KnownProblem> _knownProblems;
        private readonly IRepository<Ticket> _tickets;
        private readonly ILogService _logger;
        private readonly IPriorityStrategy _priorityStrategy;
        private readonly TicketPriorityQueue _waitingQueue;
        private readonly List<ITicketObserver> _observers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpdeskService"/> class.
        /// </summary>
        /// <param name="clients">The client repository.</param>
        /// <param name="operators">The operator repository.</param>
        /// <param name="products">The product repository.</param>
        /// <param name="knownProblems">The known problems repository.</param>
        /// <param name="tickets">The ticket repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="priorityStrategy">The priority strategy.</param>
        public HelpdeskService(
            IRepository<Client> clients,
            IRepository<SupportOperator> operators,
            IRepository<Product> products,
            IRepository<KnownProblem> knownProblems,
            IRepository<Ticket> tickets,
            ILogService logger,
            IPriorityStrategy? priorityStrategy = null)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _operators = operators ?? throw new ArgumentNullException(nameof(operators));
            _products = products ?? throw new ArgumentNullException(nameof(products));
            _knownProblems = knownProblems ?? throw new ArgumentNullException(nameof(knownProblems));
            _tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _priorityStrategy = priorityStrategy ?? new DefaultPriorityStrategy();
            _waitingQueue = new TicketPriorityQueue();
            _observers = new List<ITicketObserver>();

            RestoreClientProductReferences();
            RestoreTicketReferences();
            AddObserver(new TicketLogObserver(_logger));

            foreach (Ticket ticket in _tickets.Find(t => t.Status == TicketStatus.WaitingAssignment))
                _waitingQueue.Enqueue(ticket);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an observer to ticket status changes.
        /// </summary>
        /// <param name="observer">The observer.</param>
        public void AddObserver(ITicketObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer), "Observer cannot be null.");

            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        /// <summary>
        /// Removes an observer from ticket status changes.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns>True if removed; otherwise, false.</returns>
        public bool RemoveObserver(ITicketObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer), "Observer cannot be null.");

            return _observers.Remove(observer);
        }

        /// <summary>
        /// Registers a client.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="taxNumber">The fiscal number.</param>
        /// <returns>The registered client.</returns>
        public Client RegisterClient(string fullName, string email, string phoneNumber, string taxNumber)
        {
            if (_clients.GetAll().Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateEntityException($"Client with email {email} already exists.");

            Client client = new Client(fullName, email, phoneNumber, taxNumber);
            _clients.Add(client);
            _logger.Info($"Client registered: {client.FullName}");
            return client;
        }

        /// <summary>
        /// Registers a support operator.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="employeeNumber">The employee number.</param>
        /// <param name="department">The department.</param>
        /// <param name="level">The operator level.</param>
        /// <returns>The registered operator.</returns>
        public SupportOperator RegisterOperator(
            string fullName,
            string email,
            string phoneNumber,
            string employeeNumber,
            string department,
            OperatorLevel level)
        {
            if (_operators.GetAll().Any(o => o.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateEntityException($"Operator with employee number {employeeNumber} already exists.");

            SupportOperator supportOperator = new SupportOperator(fullName, email, phoneNumber, employeeNumber, department, level);
            _operators.Add(supportOperator);
            _logger.Info($"Operator registered: {supportOperator.FullName}");
            return supportOperator;
        }

        /// <summary>
        /// Registers a product.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="name">The product name.</param>
        /// <param name="brand">The brand.</param>
        /// <param name="model">The model.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="warrantyEndDate">The warranty end date.</param>
        /// <returns>The registered product.</returns>
        public Product RegisterProduct(
            Guid clientId,
            string name,
            string brand,
            string model,
            string serialNumber,
            DateTime warrantyEndDate)
        {
            if (_products.GetAll().Any(p => p.SerialNumber.Equals(serialNumber, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateEntityException($"Product with serial number {serialNumber} already exists.");

            Client client = GetRequired(_clients, clientId);
            Product product = new Product(name, brand, model, serialNumber, warrantyEndDate);
            _products.Add(product);
            client.AddProduct(product);
            _clients.Update(client);
            _logger.Info($"Product registered: {product.SerialNumber}");
            return product;
        }

        /// <summary>
        /// Registers a known problem.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="productFamily">The product family.</param>
        /// <param name="assistanceType">The assistance type.</param>
        /// <returns>The registered known problem.</returns>
        public KnownProblem RegisterKnownProblem(
            string title,
            string description,
            string solution,
            string productFamily,
            AssistanceType assistanceType)
        {
            KnownProblem knownProblem = new KnownProblem(title, description, solution, productFamily, assistanceType);
            _knownProblems.Add(knownProblem);
            _logger.Info($"Known problem registered: {knownProblem.Title}");
            return knownProblem;
        }

        /// <summary>
        /// Adds a tutorial to a known problem.
        /// </summary>
        /// <param name="knownProblemId">The known problem ID.</param>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        /// <returns>The created tutorial.</returns>
        public Tutorial AddTutorial(Guid knownProblemId, string title, string content)
        {
            KnownProblem knownProblem = GetRequired(_knownProblems, knownProblemId);
            Tutorial tutorial = new Tutorial(title, content);
            knownProblem.AddTutorial(tutorial);
            _knownProblems.Update(knownProblem);
            _logger.Info($"Tutorial added to known problem: {knownProblem.Title}");
            return tutorial;
        }

        /// <summary>
        /// Opens a new ticket.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="productId">The product ID.</param>
        /// <param name="assistanceType">The assistance type.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="description">The description.</param>
        /// <returns>The created ticket.</returns>
        public Ticket OpenTicket(
            Guid clientId,
            Guid productId,
            AssistanceType assistanceType,
            TicketPriority priority,
            string subject,
            string description)
        {
            Client client = GetRequired(_clients, clientId);
            Product product = GetRequired(_products, productId);

            Ticket ticket = TicketFactory.Create(
                client,
                product,
                assistanceType,
                priority,
                subject,
                description,
                _priorityStrategy);

            ticket.AddInteraction(new TicketInteraction(client.FullName, description, true));
            _tickets.Add(ticket);
            _waitingQueue.Enqueue(ticket);
            _logger.Info($"Ticket opened: {ticket.TicketNumber}");
            return ticket;
        }

        /// <summary>
        /// Assigns a ticket to an operator.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="operatorId">The operator ID.</param>
        public void AssignTicket(Guid ticketId, Guid operatorId)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            SupportOperator supportOperator = GetRequired(_operators, operatorId);

            if (!supportOperator.IsAvailable)
                throw new DomainValidationException("Operator is not available.");

            if (!supportOperator.CanHandle(ticket.Priority))
                throw new DomainValidationException("Junior operators cannot handle critical tickets.");

            TicketStatus previousStatus = ticket.Status;
            ticket.AssignTo(supportOperator);
            _waitingQueue.Remove(ticket.ID);
            _tickets.Update(ticket);
            _logger.Info($"Ticket {ticket.TicketNumber} assigned to {supportOperator.FullName}");
            NotifyStatusChange(ticket, previousStatus, ticket.Status);
        }

        /// <summary>
        /// Adds an interaction to a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="authorName">The author name.</param>
        /// <param name="message">The message.</param>
        /// <param name="fromClient">True if the interaction is from the client.</param>
        public void AddInteraction(Guid ticketId, string authorName, string message, bool fromClient)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            ticket.AddInteraction(new TicketInteraction(authorName, message, fromClient));
            _tickets.Update(ticket);
            _logger.Info($"Interaction added to ticket {ticket.TicketNumber}");
        }

        /// <summary>
        /// Links a known problem to a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="knownProblemId">The known problem ID.</param>
        public void LinkKnownProblem(Guid ticketId, Guid knownProblemId)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            KnownProblem knownProblem = GetRequired(_knownProblems, knownProblemId);

            ticket.LinkKnownProblem(knownProblem);
            _tickets.Update(ticket);
            _logger.Info($"Known problem linked to ticket {ticket.TicketNumber}");
        }

        /// <summary>
        /// Marks a ticket as waiting for client feedback.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        public void MarkWaitingClient(Guid ticketId)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            TicketStatus previousStatus = ticket.Status;
            ticket.WaitClient();
            _tickets.Update(ticket);
            _logger.Info($"Ticket {ticket.TicketNumber} is waiting for client feedback");
            NotifyStatusChange(ticket, previousStatus, ticket.Status);
        }

        /// <summary>
        /// Resolves a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="resolution">The resolution.</param>
        public void ResolveTicket(Guid ticketId, string resolution)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            TicketStatus previousStatus = ticket.Status;
            ticket.Resolve(resolution);
            _tickets.Update(ticket);
            _logger.Info($"Ticket resolved: {ticket.TicketNumber}");
            NotifyStatusChange(ticket, previousStatus, ticket.Status);
        }

        /// <summary>
        /// Closes a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        public void CloseTicket(Guid ticketId)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            TicketStatus previousStatus = ticket.Status;
            ticket.Close();
            _tickets.Update(ticket);
            _logger.Info($"Ticket closed: {ticket.TicketNumber}");
            NotifyStatusChange(ticket, previousStatus, ticket.Status);
        }

        /// <summary>
        /// Cancels a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        public void CancelTicket(Guid ticketId)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            TicketStatus previousStatus = ticket.Status;
            ticket.Cancel();
            _waitingQueue.Remove(ticket.ID);
            _tickets.Update(ticket);
            _logger.Info($"Ticket cancelled: {ticket.TicketNumber}");
            NotifyStatusChange(ticket, previousStatus, ticket.Status);
        }

        /// <summary>
        /// Evaluates a closed ticket.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="score">The score.</param>
        /// <param name="wasSolved">True if the issue was solved.</param>
        /// <param name="comment">The evaluation comment.</param>
        public void EvaluateTicket(Guid ticketId, int score, bool wasSolved, string comment)
        {
            Ticket ticket = GetRequired(_tickets, ticketId);
            ticket.Evaluate(new AssistanceEvaluation(score, wasSolved, comment));
            _tickets.Update(ticket);
            _logger.Info($"Ticket evaluated: {ticket.TicketNumber} - Score {score}");
        }

        /// <summary>
        /// Gets tickets by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The tickets with the provided status.</returns>
        public IEnumerable<Ticket> GetTicketsByStatus(TicketStatus status)
        {
            return _tickets.Find(t => t.Status == status);
        }

        /// <summary>
        /// Gets tickets assigned to an operator.
        /// </summary>
        /// <param name="operatorId">The operator ID.</param>
        /// <returns>The operator tickets.</returns>
        public IEnumerable<Ticket> GetTicketsByOperator(Guid operatorId)
        {
            return _tickets.Find(t => t.AssignedOperator != null && t.AssignedOperator.ID == operatorId);
        }

        /// <summary>
        /// Gets open tickets ordered by priority and creation date.
        /// </summary>
        /// <returns>The ordered tickets.</returns>
        public IEnumerable<Ticket> GetOpenTicketsOrderedByPriority()
        {
            return _tickets
                .GetAll()
                .Where(t => t.Status != TicketStatus.Closed && t.Status != TicketStatus.Cancelled)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Gets waiting tickets using the priority queue structure.
        /// </summary>
        /// <returns>The waiting tickets ordered by priority and insertion order.</returns>
        public IEnumerable<Ticket> GetWaitingTicketsByPriorityQueue()
        {
            return _waitingQueue.GetAllOrdered();
        }

        /// <summary>
        /// Gets known problems by assistance type.
        /// </summary>
        /// <param name="assistanceType">The assistance type.</param>
        /// <returns>The matching known problems.</returns>
        public IEnumerable<KnownProblem> GetKnownProblemsByType(AssistanceType assistanceType)
        {
            return _knownProblems.Find(k => k.AssistanceType == assistanceType);
        }

        /// <summary>
        /// Restores each client's products from the canonical product repository.
        /// </summary>
        private void RestoreClientProductReferences()
        {
            foreach (Client client in _clients.GetAll())
            {
                List<Product> products = client.ProductIds
                    .Select(productId => GetRequired(_products, productId))
                    .ToList();

                client.RestoreProducts(products);
            }
        }

        /// <summary>
        /// Restores ticket navigation properties from persisted identifiers.
        /// </summary>
        private void RestoreTicketReferences()
        {
            foreach (Ticket ticket in _tickets.GetAll())
            {
                Client client = GetRequired(_clients, ticket.ClientId);
                Product product = GetRequired(_products, ticket.ProductId);
                SupportOperator? supportOperator = ticket.AssignedOperatorId.HasValue
                    ? GetRequired(_operators, ticket.AssignedOperatorId.Value)
                    : null;
                KnownProblem? knownProblem = ticket.KnownProblemId.HasValue
                    ? GetRequired(_knownProblems, ticket.KnownProblemId.Value)
                    : null;

                ticket.RestoreReferences(client, product, supportOperator, knownProblem);
            }
        }

        /// <summary>
        /// Gets an entity or throws an exception.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity.</returns>
        private static T GetRequired<T>(IRepository<T> repository, Guid id) where T : IIdentifiable
        {
            T? entity = repository.GetById(id);

            if (entity == null)
                throw new EntityNotFoundException(typeof(T).Name, id);

            return entity;
        }

        /// <summary>
        /// Notifies observers about a status change.
        /// </summary>
        /// <param name="ticket">The changed ticket.</param>
        /// <param name="previousStatus">The previous status.</param>
        /// <param name="currentStatus">The current status.</param>
        private void NotifyStatusChange(Ticket ticket, TicketStatus previousStatus, TicketStatus currentStatus)
        {
            if (previousStatus == currentStatus)
                return;

            foreach (ITicketObserver observer in _observers.ToList())
                observer.OnTicketStatusChanged(ticket, previousStatus, currentStatus);
        }

        #endregion
    }
}


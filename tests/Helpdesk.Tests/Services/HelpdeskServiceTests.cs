//-----------------------------------------------------------------
//    <copyright file="HelpdeskServiceTests.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>10-07-2026</date>
//    <time>19:30</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Logging;
using Helpdesk.Core.Models;
using Helpdesk.Core.Services;

namespace Helpdesk.Tests.Services
{
    /// <summary>
    /// Unit tests for the Helpdesk service.
    /// </summary>
    [TestFixture]
    public class HelpdeskServiceTests
    {
        #region Tests

        /// <summary>
        /// Verifies that a ticket is opened with the correct initial state.
        /// </summary>
        [Test]
        public void OpenTicket_ShouldCreateWaitingAssignmentTicket()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out Repository<Ticket> tickets);
            (Client client, Product product, _) = CreateBaseData(service, TicketPriority.Normal);

            Ticket ticket = service.OpenTicket(
                client.ID,
                product.ID,
                AssistanceType.Software,
                TicketPriority.Normal,
                "Application error",
                "Application closes after login.");

            Assert.That(tickets.Count(), Is.EqualTo(1));
            Assert.That(ticket.Status, Is.EqualTo(TicketStatus.WaitingAssignment));
            Assert.That(ticket.Interactions, Has.Count.EqualTo(1));
            Assert.That(ticket.LimitDate, Is.GreaterThan(ticket.CreatedAt));
        }

        /// <summary>
        /// Verifies that assigning a ticket changes its state.
        /// </summary>
        [Test]
        public void AssignTicket_WithValidOperator_ShouldSetTicketInProgress()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out _);
            (Client client, Product product, SupportOperator supportOperator) = CreateBaseData(service, TicketPriority.High);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Hardware, TicketPriority.High, "Screen flickers", "Screen flickers every minute.");

            service.AssignTicket(ticket.ID, supportOperator.ID);

            Assert.That(ticket.Status, Is.EqualTo(TicketStatus.InProgress));
            Assert.That(ticket.AssignedOperator?.ID, Is.EqualTo(supportOperator.ID));
        }

        /// <summary>
        /// Verifies that junior operators cannot handle critical tickets.
        /// </summary>
        [Test]
        public void AssignTicket_WithJuniorOperatorAndCriticalPriority_ShouldThrowException()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out _);
            Client client = service.RegisterClient("Maria Lopes", "maria@email.pt", "+351911111111", "123456789");
            Product product = service.RegisterProduct(client.ID, "Router", "TP-Link", "AX55", "SER-001", DateTime.Now.AddYears(1));
            SupportOperator junior = service.RegisterOperator("Joao Reis", "joao@helpdesk.pt", "+351922222222", "OP-100", "First Line", OperatorLevel.Junior);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Network, TicketPriority.Critical, "Network down", "Office network is unavailable.");

            Assert.Throws<DomainValidationException>(() => service.AssignTicket(ticket.ID, junior.ID));
        }

        /// <summary>
        /// Verifies the normal ticket life cycle.
        /// </summary>
        [Test]
        public void ResolveCloseEvaluate_ShouldCompleteTicketLifeCycle()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out _);
            (Client client, Product product, SupportOperator supportOperator) = CreateBaseData(service, TicketPriority.Normal);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Account, TicketPriority.Normal, "Password problem", "Cannot access the portal.");

            service.AssignTicket(ticket.ID, supportOperator.ID);
            service.ResolveTicket(ticket.ID, "Password was reset and access was validated.");
            service.CloseTicket(ticket.ID);
            service.EvaluateTicket(ticket.ID, 10, true, "Problem solved.");

            Assert.That(ticket.Status, Is.EqualTo(TicketStatus.Closed));
            Assert.That(ticket.ClosedAt, Is.Not.Null);
            Assert.That(ticket.Evaluation, Is.Not.Null);
            Assert.That(ticket.Evaluation?.Score, Is.EqualTo(10));
        }

        /// <summary>
        /// Verifies duplicate email validation.
        /// </summary>
        [Test]
        public void RegisterClient_WithDuplicateEmail_ShouldThrowException()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out _);
            service.RegisterClient("Ana Martins", "ana@email.pt", "+351933333333", "111222333");

            Assert.Throws<DuplicateEntityException>(() =>
                service.RegisterClient("Ana Silva", "ana@email.pt", "+351944444444", "222333444"));
        }

        /// <summary>
        /// Verifies that tickets cannot be evaluated before closing.
        /// </summary>
        [Test]
        public void EvaluateTicket_WhenTicketIsNotClosed_ShouldThrowException()
        {
            HelpdeskService service = CreateService(out _, out _, out _, out _, out _);
            (Client client, Product product, _) = CreateBaseData(service, TicketPriority.Low);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Other, TicketPriority.Low, "Question", "Need help with product setup.");

            Assert.Throws<InvalidTicketTransitionException>(() =>
                service.EvaluateTicket(ticket.ID, 8, true, "Good support."));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a test service with memory repositories.
        /// </summary>
        private static HelpdeskService CreateService(
            out Repository<Client> clients,
            out Repository<SupportOperator> operators,
            out Repository<Product> products,
            out Repository<KnownProblem> knownProblems,
            out Repository<Ticket> tickets)
        {
            clients = new Repository<Client>();
            operators = new Repository<SupportOperator>();
            products = new Repository<Product>();
            knownProblems = new Repository<KnownProblem>();
            tickets = new Repository<Ticket>();

            return new HelpdeskService(
                clients,
                operators,
                products,
                knownProblems,
                tickets,
                new NullLogService());
        }

        /// <summary>
        /// Creates common data for tests.
        /// </summary>
        private static (Client Client, Product Product, SupportOperator Operator) CreateBaseData(
            HelpdeskService service,
            TicketPriority priority)
        {
            Client client = service.RegisterClient("Carlos Pinto", "carlos@email.pt", "+351955555555", "555666777");
            Product product = service.RegisterProduct(client.ID, "Laptop", "Lenovo", "ThinkPad", $"SER-{Guid.NewGuid():N}", DateTime.Now.AddYears(1));
            OperatorLevel level = priority == TicketPriority.Critical ? OperatorLevel.Coordinator : OperatorLevel.Specialist;
            SupportOperator supportOperator = service.RegisterOperator("Rita Costa", "rita@helpdesk.pt", "+351966666666", $"OP-{Guid.NewGuid():N}", "Support", level);

            return (client, product, supportOperator);
        }

        #endregion
    }
}


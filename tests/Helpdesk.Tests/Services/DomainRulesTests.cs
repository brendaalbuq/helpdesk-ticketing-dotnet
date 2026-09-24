//-----------------------------------------------------------------
//    <copyright file="DomainRulesTests.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>11-07-2026</date>
//    <time>10:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Interfaces;
using Helpdesk.Core.Logging;
using Helpdesk.Core.Models;
using Helpdesk.Core.Services;

namespace Helpdesk.Tests.Services
{
    /// <summary>
    /// Unit tests for domain rules and repository behavior.
    /// </summary>
    [TestFixture]
    public class DomainRulesTests
    {
        #region Tests

        /// <summary>
        /// Verifies duplicate product validation.
        /// </summary>
        [Test]
        public void RegisterProduct_WithDuplicateSerialNumber_ShouldThrowException()
        {
            HelpdeskService service = CreateService();
            Client client = service.RegisterClient("Ana Martins", "ana1@email.pt", "+351912345678", "123456789");
            service.RegisterProduct(client.ID, "Laptop", "Lenovo", "E14", "SER-100", DateTime.Now.AddYears(1));

            Assert.Throws<DuplicateEntityException>(() =>
                service.RegisterProduct(client.ID, "Laptop", "Lenovo", "E15", "SER-100", DateTime.Now.AddYears(1)));
        }

        /// <summary>
        /// Verifies missing entity validation.
        /// </summary>
        [Test]
        public void RegisterProduct_WithUnknownClient_ShouldThrowException()
        {
            HelpdeskService service = CreateService();

            Assert.Throws<EntityNotFoundException>(() =>
                service.RegisterProduct(Guid.NewGuid(), "Laptop", "Lenovo", "E14", "SER-200", DateTime.Now.AddYears(1)));
        }

        /// <summary>
        /// Verifies that unresolved tickets cannot be closed.
        /// </summary>
        [Test]
        public void CloseTicket_WhenTicketIsNotResolved_ShouldThrowException()
        {
            HelpdeskService service = CreateService();
            Ticket ticket = CreateAssignedTicket(service);

            Assert.Throws<InvalidTicketTransitionException>(() => service.CloseTicket(ticket.ID));
        }

        /// <summary>
        /// Verifies that tickets cannot be resolved before assignment.
        /// </summary>
        [Test]
        public void ResolveTicket_WhenTicketIsNotAssigned_ShouldThrowException()
        {
            HelpdeskService service = CreateService();
            Client client = service.RegisterClient("Rui Santos", "rui@email.pt", "+351913333333", "333444555");
            Product product = service.RegisterProduct(client.ID, "Monitor", "Dell", "P2422H", "MON-001", DateTime.Now.AddYears(2));
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Hardware, TicketPriority.Normal, "Monitor problem", "The monitor does not turn on.");

            Assert.Throws<InvalidTicketTransitionException>(() =>
                service.ResolveTicket(ticket.ID, "Cable replaced."));
        }

        /// <summary>
        /// Verifies the waiting client state.
        /// </summary>
        [Test]
        public void MarkWaitingClient_WithInProgressTicket_ShouldChangeStatus()
        {
            HelpdeskService service = CreateService();
            Ticket ticket = CreateAssignedTicket(service);

            service.MarkWaitingClient(ticket.ID);

            Assert.That(ticket.Status, Is.EqualTo(TicketStatus.WaitingClient));
        }

        /// <summary>
        /// Verifies known problem linking.
        /// </summary>
        [Test]
        public void LinkKnownProblem_ShouldAssociateProblemWithTicket()
        {
            HelpdeskService service = CreateService();
            Ticket ticket = CreateAssignedTicket(service);
            KnownProblem knownProblem = service.RegisterKnownProblem(
                "Router needs restart",
                "Router firmware freezes under heavy traffic.",
                "Restart and update firmware.",
                "Routers",
                AssistanceType.Network);

            service.AddTutorial(knownProblem.ID, "Restart router", "Disconnect power for thirty seconds.");
            service.LinkKnownProblem(ticket.ID, knownProblem.ID);

            Assert.That(ticket.KnownProblem?.ID, Is.EqualTo(knownProblem.ID));
            Assert.That(knownProblem.Tutorials, Has.Count.EqualTo(1));
            Assert.That(service.GetKnownProblemsByType(AssistanceType.Network).Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Verifies repository find and remove operations.
        /// </summary>
        [Test]
        public void Repository_FindAndRemove_ShouldWork()
        {
            Repository<Client> repository = new Repository<Client>();
            Client client = new Client("Ines Rocha", "ines@email.pt", "+351914444444", "444555666");

            repository.Add(client);
            IEnumerable<Client> clients = repository.Find(c => c.Email.Contains("ines", StringComparison.OrdinalIgnoreCase));
            bool removed = repository.Remove(client.ID);

            Assert.That(clients.Count(), Is.EqualTo(1));
            Assert.That(removed, Is.True);
            Assert.That(repository.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies that file logging writes a log entry.
        /// </summary>
        [Test]
        [Order(1)]
        public void FileLogService_Info_ShouldWriteMessage()
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"helpdesk-log-{Guid.NewGuid():N}.log");

            try
            {
                FileLogService logger = FileLogService.GetInstance(filePath);

                logger.Info("Test message");

                string log = File.ReadAllText(filePath);
                Assert.That(log, Does.Contain("INFO"));
                Assert.That(log, Does.Contain("Test message"));
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        /// <summary>
        /// Verifies that waiting tickets are returned by priority queue order.
        /// </summary>
        [Test]
        public void GetWaitingTicketsByPriorityQueue_ShouldReturnCriticalFirst()
        {
            HelpdeskService service = CreateService();
            Client client = service.RegisterClient("Luis Antunes", "luis@email.pt", "+351917777777", "777888999");
            Product product = service.RegisterProduct(client.ID, "Laptop", "HP", "ProBook", "HP-001", DateTime.Now.AddYears(1));

            Ticket lowTicket = service.OpenTicket(client.ID, product.ID, AssistanceType.Other, TicketPriority.Low, "Low question", "Low priority question.");
            Ticket criticalTicket = service.OpenTicket(client.ID, product.ID, AssistanceType.Network, TicketPriority.Critical, "Network down", "No network access.");

            List<Ticket> waitingTickets = service.GetWaitingTicketsByPriorityQueue().ToList();

            Assert.That(waitingTickets.First().ID, Is.EqualTo(criticalTicket.ID));
            Assert.That(waitingTickets.Last().ID, Is.EqualTo(lowTicket.ID));
        }

        /// <summary>
        /// Verifies that assigned tickets leave the waiting priority queue.
        /// </summary>
        [Test]
        public void AssignTicket_ShouldRemoveTicketFromPriorityQueue()
        {
            HelpdeskService service = CreateService();
            Client client = service.RegisterClient("Tiago Rocha", "tiago@email.pt", "+351918888888", "888999000");
            Product product = service.RegisterProduct(client.ID, "Router", "Asus", "AX3000", "ASUS-001", DateTime.Now.AddYears(1));
            SupportOperator supportOperator = service.RegisterOperator("Sara Dias", "sara@helpdesk.pt", "+351919999999", "OP-500", "Support", OperatorLevel.Coordinator);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Network, TicketPriority.Critical, "Internet down", "No office internet.");

            service.AssignTicket(ticket.ID, supportOperator.ID);

            Assert.That(service.GetWaitingTicketsByPriorityQueue().Any(t => t.ID == ticket.ID), Is.False);
        }

        /// <summary>
        /// Verifies that observers are notified when ticket status changes.
        /// </summary>
        [Test]
        public void AssignTicket_ShouldNotifyObservers()
        {
            HelpdeskService service = CreateService();
            TestTicketObserver observer = new TestTicketObserver();
            service.AddObserver(observer);
            Client client = service.RegisterClient("Eva Moreira", "eva@email.pt", "+351910000000", "900000001");
            Product product = service.RegisterProduct(client.ID, "Laptop", "Dell", "Latitude", "DELL-001", DateTime.Now.AddYears(1));
            SupportOperator supportOperator = service.RegisterOperator("Bruno Lima", "bruno@helpdesk.pt", "+351910000001", "OP-501", "Support", OperatorLevel.Specialist);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Software, TicketPriority.High, "Application error", "The application does not open.");

            service.AssignTicket(ticket.ID, supportOperator.ID);

            Assert.That(observer.NotificationCount, Is.EqualTo(1));
            Assert.That(observer.LastPreviousStatus, Is.EqualTo(TicketStatus.WaitingAssignment));
            Assert.That(observer.LastCurrentStatus, Is.EqualTo(TicketStatus.InProgress));
        }

        /// <summary>
        /// Verifies that the file logger uses a singleton instance.
        /// </summary>
        [Test]
        [Order(2)]
        public void FileLogService_GetInstance_ShouldReturnSameInstance()
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"singleton-log-{Guid.NewGuid():N}.log");

            FileLogService logger1 = FileLogService.GetInstance(filePath);
            FileLogService logger2 = FileLogService.GetInstance(filePath);

            Assert.That(logger2, Is.SameAs(logger1));
        }

        /// <summary>
        /// Verifies that the evaluation remains valid after construction.
        /// </summary>
        [Test]
        public void AssistanceEvaluation_SetInvalidScore_ShouldThrowException()
        {
            AssistanceEvaluation evaluation = new AssistanceEvaluation(8, true, "Good support.");

            Assert.Throws<DomainValidationException>(() => evaluation.Score = 99);
            Assert.That(evaluation.Score, Is.EqualTo(8));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a service for tests.
        /// </summary>
        /// <returns>The helpdesk service.</returns>
        private static HelpdeskService CreateService()
        {
            return new HelpdeskService(
                new Repository<Client>(),
                new Repository<SupportOperator>(),
                new Repository<Product>(),
                new Repository<KnownProblem>(),
                new Repository<Ticket>(),
                new NullLogService());
        }

        /// <summary>
        /// Creates an assigned ticket.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>The created ticket.</returns>
        private static Ticket CreateAssignedTicket(HelpdeskService service)
        {
            Client client = service.RegisterClient($"Client {Guid.NewGuid():N}", $"{Guid.NewGuid():N}@email.pt", "+351915555555", "555666777");
            Product product = service.RegisterProduct(client.ID, "Router", "TP-Link", "AX55", $"RTR-{Guid.NewGuid():N}", DateTime.Now.AddYears(1));
            SupportOperator supportOperator = service.RegisterOperator($"Operator {Guid.NewGuid():N}", $"{Guid.NewGuid():N}@helpdesk.pt", "+351916666666", $"OP-{Guid.NewGuid():N}", "Support", OperatorLevel.Specialist);
            Ticket ticket = service.OpenTicket(client.ID, product.ID, AssistanceType.Network, TicketPriority.High, "Network problem", "The network is unstable.");
            service.AssignTicket(ticket.ID, supportOperator.ID);

            return ticket;
        }

        /// <summary>
        /// Test observer used to validate the Observer pattern.
        /// </summary>
        private class TestTicketObserver : ITicketObserver
        {
            #region Properties

            /// <summary>
            /// Gets the number of notifications received.
            /// </summary>
            public int NotificationCount { get; private set; }

            /// <summary>
            /// Gets the last previous status.
            /// </summary>
            public TicketStatus LastPreviousStatus { get; private set; }

            /// <summary>
            /// Gets the last current status.
            /// </summary>
            public TicketStatus LastCurrentStatus { get; private set; }

            #endregion

            #region Methods

            /// <summary>
            /// Handles ticket status changes.
            /// </summary>
            /// <param name="ticket">The ticket.</param>
            /// <param name="previousStatus">The previous status.</param>
            /// <param name="currentStatus">The current status.</param>
            public void OnTicketStatusChanged(Ticket ticket, TicketStatus previousStatus, TicketStatus currentStatus)
            {
                NotificationCount++;
                LastPreviousStatus = previousStatus;
                LastCurrentStatus = currentStatus;
            }

            #endregion
        }

        #endregion
    }
}


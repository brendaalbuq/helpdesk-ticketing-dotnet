//-----------------------------------------------------------------
//    <copyright file="PersistenceTests.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>12-07-2026</date>
//    <time>11:05</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Logging;
using Helpdesk.Core.Models;
using Helpdesk.Core.Services;

namespace Helpdesk.Tests.Services
{
    /// <summary>
    /// Unit tests for JSON persistence.
    /// </summary>
    [TestFixture]
    public class PersistenceTests
    {
        #region Tests

        /// <summary>
        /// Verifies that entities are saved and loaded from JSON.
        /// </summary>
        [Test]
        public void FileRepository_SaveAndLoad_ShouldRestoreClients()
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"clients-{Guid.NewGuid():N}.json");

            try
            {
                FileRepository<Client> repository = new FileRepository<Client>(filePath);
                Client client = new Client("Ana Martins", "ana@email.pt", "+351912345678", "123456789");
                repository.Add(client);
                repository.Save();

                FileRepository<Client> loadedRepository = new FileRepository<Client>(filePath);
                loadedRepository.Load();

                Client? loadedClient = loadedRepository.GetById(client.ID);

                Assert.That(loadedRepository.Count(), Is.EqualTo(1));
                Assert.That(loadedClient, Is.Not.Null);
                Assert.That(loadedClient?.Email, Is.EqualTo(client.Email));
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        /// <summary>
        /// Verifies that tickets store identifiers and restore canonical references.
        /// </summary>
        [Test]
        public void FileRepository_SaveAndLoadTicket_ShouldRestoreReferencesById()
        {
            string folderPath = Path.Combine(Path.GetTempPath(), $"helpdesk-data-{Guid.NewGuid():N}");

            try
            {
                FileRepository<Client> clients = new FileRepository<Client>(Path.Combine(folderPath, "clients.json"));
                FileRepository<SupportOperator> operators = new FileRepository<SupportOperator>(Path.Combine(folderPath, "operators.json"));
                FileRepository<Product> products = new FileRepository<Product>(Path.Combine(folderPath, "products.json"));
                FileRepository<KnownProblem> knownProblems = new FileRepository<KnownProblem>(Path.Combine(folderPath, "known-problems.json"));
                FileRepository<Ticket> tickets = new FileRepository<Ticket>(Path.Combine(folderPath, "tickets.json"));
                HelpdeskService service = new HelpdeskService(clients, operators, products, knownProblems, tickets, new NullLogService());

                Client client = service.RegisterClient("Ana Martins", "ana@email.pt", "+351912345678", "123456789");
                Product product = service.RegisterProduct(client.ID, "Laptop", "Lenovo", "E14", "SER-001", DateTime.Now.AddYears(1));
                SupportOperator supportOperator = service.RegisterOperator(
                    "Rita Costa",
                    "rita@helpdesk.pt",
                    "+351911111111",
                    "OP-001",
                    "Support",
                    OperatorLevel.Specialist);
                Ticket ticket = service.OpenTicket(
                    client.ID,
                    product.ID,
                    AssistanceType.Software,
                    TicketPriority.High,
                    "Application error",
                    "The application does not open.");
                service.AssignTicket(ticket.ID, supportOperator.ID);

                clients.Save();
                operators.Save();
                products.Save();
                knownProblems.Save();
                tickets.Save();

                string ticketJson = File.ReadAllText(Path.Combine(folderPath, "tickets.json"));
                string clientJson = File.ReadAllText(Path.Combine(folderPath, "clients.json"));
                Assert.That(ticketJson, Does.Contain("\"ClientId\""));
                Assert.That(ticketJson, Does.Contain("\"ProductId\""));
                Assert.That(ticketJson, Does.Not.Contain("\"Client\":"));
                Assert.That(ticketJson, Does.Not.Contain("\"Product\":"));
                Assert.That(clientJson, Does.Contain("\"ProductIds\""));
                Assert.That(clientJson, Does.Not.Contain("\"Products\":"));

                FileRepository<Client> loadedClients = new FileRepository<Client>(Path.Combine(folderPath, "clients.json"));
                FileRepository<SupportOperator> loadedOperators = new FileRepository<SupportOperator>(Path.Combine(folderPath, "operators.json"));
                FileRepository<Product> loadedProducts = new FileRepository<Product>(Path.Combine(folderPath, "products.json"));
                FileRepository<KnownProblem> loadedKnownProblems = new FileRepository<KnownProblem>(Path.Combine(folderPath, "known-problems.json"));
                FileRepository<Ticket> loadedTickets = new FileRepository<Ticket>(Path.Combine(folderPath, "tickets.json"));

                loadedClients.Load();
                loadedOperators.Load();
                loadedProducts.Load();
                loadedKnownProblems.Load();
                loadedTickets.Load();

                _ = new HelpdeskService(
                    loadedClients,
                    loadedOperators,
                    loadedProducts,
                    loadedKnownProblems,
                    loadedTickets,
                    new NullLogService());

                Ticket? loadedTicket = loadedTickets.GetById(ticket.ID);
                Client? loadedClient = loadedClients.GetById(client.ID);
                Product? loadedProduct = loadedProducts.GetById(product.ID);

                Assert.That(loadedTicket, Is.Not.Null);
                Assert.That(loadedTicket?.Client, Is.SameAs(loadedClient));
                Assert.That(loadedTicket?.Product, Is.SameAs(loadedProduct));
                Assert.That(loadedClient?.Products.Single(), Is.SameAs(loadedProduct));
                Assert.That(loadedTicket?.AssignedOperator?.ID, Is.EqualTo(supportOperator.ID));
            }
            finally
            {
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);
            }
        }

        #endregion
    }
}


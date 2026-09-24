//-----------------------------------------------------------------
//    <copyright file="ApplicationDataService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>14-07-2026</date>
//    <time>18:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Logging;
using Helpdesk.Core.Models;
using Helpdesk.Core.Services;
using System.IO;

namespace Helpdesk.WpfApp.Services
{
    /// <summary>
    /// Provides repositories and services for the WPF application.
    /// </summary>
    public class ApplicationDataService
    {
        #region Properties

        /// <summary>
        /// Gets the data folder.
        /// </summary>
        public string DataFolder { get; }

        /// <summary>
        /// Gets the client repository.
        /// </summary>
        public FileRepository<Client> Clients { get; }

        /// <summary>
        /// Gets the operator repository.
        /// </summary>
        public FileRepository<SupportOperator> Operators { get; }

        /// <summary>
        /// Gets the product repository.
        /// </summary>
        public FileRepository<Product> Products { get; }

        /// <summary>
        /// Gets the known problem repository.
        /// </summary>
        public FileRepository<KnownProblem> KnownProblems { get; }

        /// <summary>
        /// Gets the ticket repository.
        /// </summary>
        public FileRepository<Ticket> Tickets { get; }

        /// <summary>
        /// Gets the Helpdesk service.
        /// </summary>
        public HelpdeskService Helpdesk { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDataService"/> class.
        /// </summary>
        public ApplicationDataService()
        {
            DataFolder = Path.Combine(AppContext.BaseDirectory, "Data");

            Clients = new FileRepository<Client>(Path.Combine(DataFolder, "clients.json"));
            Operators = new FileRepository<SupportOperator>(Path.Combine(DataFolder, "operators.json"));
            Products = new FileRepository<Product>(Path.Combine(DataFolder, "products.json"));
            KnownProblems = new FileRepository<KnownProblem>(Path.Combine(DataFolder, "known-problems.json"));
            Tickets = new FileRepository<Ticket>(Path.Combine(DataFolder, "tickets.json"));

            Load();

            FileLogService logger = FileLogService.GetInstance(Path.Combine(DataFolder, "logs", "helpdesk-wpf.log"));

            Helpdesk = new HelpdeskService(
                Clients,
                Operators,
                Products,
                KnownProblems,
                Tickets,
                logger);

            if (Clients.Count() == 0)
            {
                Seed();
                Save();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads all repositories.
        /// </summary>
        public void Load()
        {
            Clients.Load();
            Operators.Load();
            Products.Load();
            KnownProblems.Load();
            Tickets.Load();
        }

        /// <summary>
        /// Saves all repositories.
        /// </summary>
        public void Save()
        {
            Clients.Save();
            Operators.Save();
            Products.Save();
            KnownProblems.Save();
            Tickets.Save();
        }

        /// <summary>
        /// Creates demonstration data when no data exists.
        /// </summary>
        private void Seed()
        {
            Client client1 = Helpdesk.RegisterClient("Ana Martins", "ana.martins@email.pt", "+351912345678", "245678901");
            Client client2 = Helpdesk.RegisterClient("Pedro Silva", "pedro.silva@email.pt", "+351913456789", "246789012");

            SupportOperator operator1 = Helpdesk.RegisterOperator(
                "Rita Fernandes",
                "rita.fernandes@helpdesk.pt",
                "+351221000111",
                "OP-001",
                "Software Support",
                OperatorLevel.Specialist);

            SupportOperator operator2 = Helpdesk.RegisterOperator(
                "Miguel Costa",
                "miguel.costa@helpdesk.pt",
                "+351221000222",
                "OP-002",
                "Hardware Support",
                OperatorLevel.Coordinator);

            Product laptop = Helpdesk.RegisterProduct(
                client1.ID,
                "Laptop",
                "Lenovo",
                "ThinkPad E14",
                "LNV-E14-2026-001",
                DateTime.Now.AddYears(2));

            Product router = Helpdesk.RegisterProduct(
                client2.ID,
                "Router",
                "TP-Link",
                "AX55",
                "TPL-AX55-2026-050",
                DateTime.Now.AddMonths(18));

            KnownProblem knownProblem = Helpdesk.RegisterKnownProblem(
                "VPN does not connect after password change",
                "The VPN client refuses authentication after the user changes the account password.",
                "Clear saved credentials and authenticate again with the new password.",
                "Corporate VPN",
                AssistanceType.Account);

            Helpdesk.AddTutorial(
                knownProblem.ID,
                "Clear Windows saved credentials",
                "Open Credential Manager, remove the VPN credential and reconnect.");

            Ticket ticket1 = Helpdesk.OpenTicket(
                client1.ID,
                laptop.ID,
                AssistanceType.Software,
                TicketPriority.High,
                "Accounting software crashes on startup",
                "The accounting application closes immediately after login.");

            Helpdesk.AssignTicket(ticket1.ID, operator1.ID);
            Helpdesk.ResolveTicket(ticket1.ID, "Reinstalled the missing runtime package and validated startup.");
            Helpdesk.CloseTicket(ticket1.ID);
            Helpdesk.EvaluateTicket(ticket1.ID, 9, true, "Fast support and clear explanation.");

            Ticket ticket2 = Helpdesk.OpenTicket(
                client2.ID,
                router.ID,
                AssistanceType.Network,
                TicketPriority.Critical,
                "Internet is unavailable in the office",
                "All workstations lost network connectivity.");

            Helpdesk.AssignTicket(ticket2.ID, operator2.ID);
        }

        #endregion
    }
}


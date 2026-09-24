//-----------------------------------------------------------------
//    <copyright file="Program.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>09-07-2026</date>
//    <time>18:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using Helpdesk.Core.Collections;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Logging;
using Helpdesk.Core.Models;
using Helpdesk.Core.Services;

namespace Helpdesk.ConsoleApp
{
    /// <summary>
    /// Main console application for Helpdesk system demonstration.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// Application entry point.
        /// </summary>
        private static void Main()
        {
            string dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");

            FileRepository<Client> clientRepo = new FileRepository<Client>(Path.Combine(dataFolder, "clients.json"));
            FileRepository<SupportOperator> operatorRepo = new FileRepository<SupportOperator>(Path.Combine(dataFolder, "operators.json"));
            FileRepository<Product> productRepo = new FileRepository<Product>(Path.Combine(dataFolder, "products.json"));
            FileRepository<KnownProblem> knownProblemRepo = new FileRepository<KnownProblem>(Path.Combine(dataFolder, "known-problems.json"));
            FileRepository<Ticket> ticketRepo = new FileRepository<Ticket>(Path.Combine(dataFolder, "tickets.json"));

            LoadAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);

            FileLogService logger = FileLogService.GetInstance(Path.Combine(dataFolder, "logs", "helpdesk.log"));

            HelpdeskService helpdesk = new HelpdeskService(
                clientRepo,
                operatorRepo,
                productRepo,
                knownProblemRepo,
                ticketRepo,
                logger);

            if (clientRepo.Count() == 0)
            {
                Seed(helpdesk);
                SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
            }

            RunMenu(helpdesk, clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo, dataFolder);
        }

        /// <summary>
        /// Runs the interactive menu.
        /// </summary>
        private static void RunMenu(
            HelpdeskService helpdesk,
            FileRepository<Client> clientRepo,
            FileRepository<SupportOperator> operatorRepo,
            FileRepository<Product> productRepo,
            FileRepository<KnownProblem> knownProblemRepo,
            FileRepository<Ticket> ticketRepo,
            string dataFolder)
        {
            bool exit = false;

            while (!exit)
            {
                SafeClear();
                Console.WriteLine("=== Helpdesk Management System ===");
                Console.WriteLine("1 - Ver dashboard");
                Console.WriteLine("2 - Listar clientes e produtos");
                Console.WriteLine("3 - Listar operadores");
                Console.WriteLine("4 - Listar tickets");
                Console.WriteLine("5 - Abrir novo ticket");
                Console.WriteLine("6 - Atribuir ticket a operador");
                Console.WriteLine("7 - Resolver ticket");
                Console.WriteLine("8 - Fechar ticket");
                Console.WriteLine("9 - Avaliar ticket");
                Console.WriteLine("10 - Listar problemas conhecidos");
                Console.WriteLine("11 - Registar cliente");
                Console.WriteLine("12 - Registar produto");
                Console.WriteLine("0 - Guardar e sair");
                Console.WriteLine();

                int option = ReadInt("Escolha uma opcao: ", 0, 12);
                Console.WriteLine();

                try
                {
                    switch (option)
                    {
                        case 1:
                            DisplayDashboard(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo, helpdesk);
                            Pause();
                            break;

                        case 2:
                            DisplayClients(clientRepo);
                            Pause();
                            break;

                        case 3:
                            DisplayOperators(operatorRepo);
                            Pause();
                            break;

                        case 4:
                            DisplayTickets(ticketRepo);
                            Pause();
                            break;

                        case 5:
                            OpenTicket(helpdesk, clientRepo, productRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 6:
                            AssignTicket(helpdesk, ticketRepo, operatorRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 7:
                            ResolveTicket(helpdesk, ticketRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 8:
                            CloseTicket(helpdesk, ticketRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 9:
                            EvaluateTicket(helpdesk, ticketRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 10:
                            DisplayKnownProblems(knownProblemRepo);
                            Pause();
                            break;

                        case 11:
                            RegisterClient(helpdesk);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 12:
                            RegisterProduct(helpdesk, clientRepo);
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Pause();
                            break;

                        case 0:
                            SaveAll(clientRepo, operatorRepo, productRepo, knownProblemRepo, ticketRepo);
                            Console.WriteLine($"Dados guardados em: {dataFolder}");
                            exit = true;
                            break;
                    }
                }
                catch (HelpdeskException ex)
                {
                    Console.WriteLine($"Erro de dominio: {ex.Message}");
                    Pause();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    Pause();
                }
            }
        }

        /// <summary>
        /// Creates demonstration data.
        /// </summary>
        /// <param name="helpdesk">The helpdesk service.</param>
        private static void Seed(HelpdeskService helpdesk)
        {
            //-------------------------------------------------------------
            // Create clients
            //-------------------------------------------------------------
            Client client1 = helpdesk.RegisterClient("Ana Martins", "ana.martins@email.pt", "+351912345678", "245678901");
            Client client2 = helpdesk.RegisterClient("Pedro Silva", "pedro.silva@email.pt", "+351913456789", "246789012");

            //-------------------------------------------------------------
            // Create support operators
            //-------------------------------------------------------------
            SupportOperator operator1 = helpdesk.RegisterOperator(
                "Rita Fernandes",
                "rita.fernandes@helpdesk.pt",
                "+351221000111",
                "OP-001",
                "Software Support",
                OperatorLevel.Specialist);

            SupportOperator operator2 = helpdesk.RegisterOperator(
                "Miguel Costa",
                "miguel.costa@helpdesk.pt",
                "+351221000222",
                "OP-002",
                "Hardware Support",
                OperatorLevel.Coordinator);

            //-------------------------------------------------------------
            // Create products
            //-------------------------------------------------------------
            Product laptop = helpdesk.RegisterProduct(
                client1.ID,
                "Laptop",
                "Lenovo",
                "ThinkPad E14",
                "LNV-E14-2026-001",
                DateTime.Now.AddYears(2));

            Product router = helpdesk.RegisterProduct(
                client2.ID,
                "Router",
                "TP-Link",
                "AX55",
                "TPL-AX55-2026-050",
                DateTime.Now.AddMonths(18));

            //-------------------------------------------------------------
            // Create known problems and tutorials
            //-------------------------------------------------------------
            KnownProblem knownProblem = helpdesk.RegisterKnownProblem(
                "VPN does not connect after password change",
                "The VPN client refuses authentication after the user changes the account password.",
                "Clear saved credentials and authenticate again with the new password.",
                "Corporate VPN",
                AssistanceType.Account);

            helpdesk.AddTutorial(
                knownProblem.ID,
                "Clear Windows saved credentials",
                "Open Credential Manager, remove the VPN credential and reconnect.");

            //-------------------------------------------------------------
            // Create and process tickets
            //-------------------------------------------------------------
            Ticket ticket1 = helpdesk.OpenTicket(
                client1.ID,
                laptop.ID,
                AssistanceType.Software,
                TicketPriority.High,
                "Accounting software crashes on startup",
                "The accounting application closes immediately after login.");

            helpdesk.AssignTicket(ticket1.ID, operator1.ID);
            helpdesk.AddInteraction(ticket1.ID, operator1.FullName, "Requested application logs and Windows event viewer details.", false);
            helpdesk.ResolveTicket(ticket1.ID, "Reinstalled the missing runtime package and validated startup.");
            helpdesk.CloseTicket(ticket1.ID);
            helpdesk.EvaluateTicket(ticket1.ID, 9, true, "Fast support and clear explanation.");

            Ticket ticket2 = helpdesk.OpenTicket(
                client2.ID,
                router.ID,
                AssistanceType.Network,
                TicketPriority.Critical,
                "Internet is unavailable in the office",
                "All workstations lost network connectivity.");

            helpdesk.AssignTicket(ticket2.ID, operator2.ID);
            helpdesk.AddInteraction(ticket2.ID, operator2.FullName, "Remote diagnosis started. Suspected router firmware issue.", false);
        }

        /// <summary>
        /// Opens a ticket using console input.
        /// </summary>
        private static void OpenTicket(HelpdeskService helpdesk, FileRepository<Client> clientRepo, FileRepository<Product> productRepo)
        {
            Client? client = SelectItem(clientRepo.GetAll(), c => $"{c.FullName} ({c.Email})");
            if (client == null)
                return;

            List<Product> clientProducts = productRepo
                .GetAll()
                .Where(p => client.Products.Any(cp => cp.ID == p.ID))
                .ToList();

            Product? product = SelectItem(clientProducts, p => $"{p.Brand} {p.Model} - {p.SerialNumber}");
            if (product == null)
                return;

            AssistanceType assistanceType = SelectEnum<AssistanceType>("Tipo de assistencia");
            TicketPriority priority = SelectEnum<TicketPriority>("Prioridade");
            string subject = ReadRequired("Assunto: ");
            string description = ReadRequired("Descricao: ");

            Ticket ticket = helpdesk.OpenTicket(client.ID, product.ID, assistanceType, priority, subject, description);
            Console.WriteLine($"Ticket criado: {ticket.TicketNumber}");
        }

        /// <summary>
        /// Assigns a ticket using console input.
        /// </summary>
        private static void AssignTicket(HelpdeskService helpdesk, FileRepository<Ticket> ticketRepo, FileRepository<SupportOperator> operatorRepo)
        {
            Ticket? ticket = SelectItem(
                ticketRepo.GetAll().Where(t => t.Status == TicketStatus.WaitingAssignment),
                t => $"{t.TicketNumber} - {t.Subject} - {t.Priority}");

            if (ticket == null)
                return;

            SupportOperator? supportOperator = SelectItem(
                operatorRepo.GetAll().Where(o => o.IsAvailable),
                o => $"{o.FullName} - {o.Level} - {o.Department}");

            if (supportOperator == null)
                return;

            helpdesk.AssignTicket(ticket.ID, supportOperator.ID);
            Console.WriteLine("Ticket atribuido com sucesso.");
        }

        /// <summary>
        /// Resolves a ticket using console input.
        /// </summary>
        private static void ResolveTicket(HelpdeskService helpdesk, FileRepository<Ticket> ticketRepo)
        {
            Ticket? ticket = SelectItem(
                ticketRepo.GetAll().Where(t => t.Status == TicketStatus.InProgress || t.Status == TicketStatus.WaitingClient),
                t => $"{t.TicketNumber} - {t.Subject} - {t.Status}");

            if (ticket == null)
                return;

            string resolution = ReadRequired("Resolucao: ");
            helpdesk.ResolveTicket(ticket.ID, resolution);
            Console.WriteLine("Ticket resolvido com sucesso.");
        }

        /// <summary>
        /// Closes a ticket using console input.
        /// </summary>
        private static void CloseTicket(HelpdeskService helpdesk, FileRepository<Ticket> ticketRepo)
        {
            Ticket? ticket = SelectItem(
                ticketRepo.GetAll().Where(t => t.Status == TicketStatus.Resolved),
                t => $"{t.TicketNumber} - {t.Subject}");

            if (ticket == null)
                return;

            helpdesk.CloseTicket(ticket.ID);
            Console.WriteLine("Ticket fechado com sucesso.");
        }

        /// <summary>
        /// Evaluates a closed ticket using console input.
        /// </summary>
        private static void EvaluateTicket(HelpdeskService helpdesk, FileRepository<Ticket> ticketRepo)
        {
            Ticket? ticket = SelectItem(
                ticketRepo.GetAll().Where(t => t.Status == TicketStatus.Closed),
                t => $"{t.TicketNumber} - {t.Subject}");

            if (ticket == null)
                return;

            int score = ReadInt("Avaliacao de 1 a 10: ", 1, 10);
            bool wasSolved = ReadBool("O problema ficou resolvido? (s/n): ");
            string comment = ReadRequired("Comentario: ");

            helpdesk.EvaluateTicket(ticket.ID, score, wasSolved, comment);
            Console.WriteLine("Ticket avaliado com sucesso.");
        }

        /// <summary>
        /// Registers a client using console input.
        /// </summary>
        private static void RegisterClient(HelpdeskService helpdesk)
        {
            string fullName = ReadRequired("Nome completo: ");
            string email = ReadRequired("Email: ");
            string phone = ReadRequired("Telefone: ");
            string taxNumber = ReadRequired("NIF: ");

            Client client = helpdesk.RegisterClient(fullName, email, phone, taxNumber);
            Console.WriteLine($"Cliente registado: {client.FullName}");
        }

        /// <summary>
        /// Registers a product using console input.
        /// </summary>
        private static void RegisterProduct(HelpdeskService helpdesk, FileRepository<Client> clientRepo)
        {
            Client? client = SelectItem(clientRepo.GetAll(), c => $"{c.FullName} ({c.Email})");
            if (client == null)
                return;

            string name = ReadRequired("Nome do produto: ");
            string brand = ReadRequired("Marca: ");
            string model = ReadRequired("Modelo: ");
            string serialNumber = ReadRequired("Numero de serie: ");
            int warrantyMonths = ReadInt("Garantia em meses: ", 1, 120);

            Product product = helpdesk.RegisterProduct(
                client.ID,
                name,
                brand,
                model,
                serialNumber,
                DateTime.Now.AddMonths(warrantyMonths));

            Console.WriteLine($"Produto registado: {product.Brand} {product.Model}");
        }

        /// <summary>
        /// Displays a simple operational dashboard.
        /// </summary>
        private static void DisplayDashboard(
            FileRepository<Client> clientRepo,
            FileRepository<SupportOperator> operatorRepo,
            FileRepository<Product> productRepo,
            FileRepository<KnownProblem> knownProblemRepo,
            FileRepository<Ticket> ticketRepo,
            HelpdeskService helpdesk)
        {
            Console.WriteLine("=== Dashboard ===");
            Console.WriteLine($"Clientes: {clientRepo.Count()}");
            Console.WriteLine($"Operadores: {operatorRepo.Count()}");
            Console.WriteLine($"Produtos: {productRepo.Count()}");
            Console.WriteLine($"Problemas conhecidos: {knownProblemRepo.Count()}");
            Console.WriteLine($"Tickets: {ticketRepo.Count()}");
            Console.WriteLine();

            Console.WriteLine("=== Tickets abertos ordenados por prioridade ===");
            foreach (Ticket ticket in helpdesk.GetOpenTicketsOrderedByPriority())
            {
                DisplayTicket(ticket);
                Console.WriteLine();
            }

            Console.WriteLine("=== Fila de espera por prioridade ===");
            foreach (Ticket ticket in helpdesk.GetWaitingTicketsByPriorityQueue())
            {
                DisplayTicket(ticket);
                Console.WriteLine();
            }

            Console.WriteLine("=== Tickets fechados ===");
            foreach (Ticket ticket in helpdesk.GetTicketsByStatus(TicketStatus.Closed))
            {
                DisplayTicket(ticket);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays clients and products.
        /// </summary>
        private static void DisplayClients(FileRepository<Client> clientRepo)
        {
            Console.WriteLine("=== Clientes ===");

            foreach (Client client in clientRepo.GetAll())
            {
                Console.WriteLine($"{client.FullName} | {client.Email} | {client.PhoneNumber}");

                foreach (Product product in client.Products)
                    Console.WriteLine($"  - {product.Brand} {product.Model} ({product.SerialNumber})");
            }
        }

        /// <summary>
        /// Displays operators.
        /// </summary>
        private static void DisplayOperators(FileRepository<SupportOperator> operatorRepo)
        {
            Console.WriteLine("=== Operadores ===");

            foreach (SupportOperator supportOperator in operatorRepo.GetAll())
                Console.WriteLine(supportOperator);
        }

        /// <summary>
        /// Displays tickets.
        /// </summary>
        private static void DisplayTickets(FileRepository<Ticket> ticketRepo)
        {
            Console.WriteLine("=== Tickets ===");

            foreach (Ticket ticket in ticketRepo.GetAll().OrderByDescending(t => t.CreatedAt))
            {
                DisplayTicket(ticket);

                if (ticket.Evaluation != null)
                    Console.WriteLine($"Evaluation: {ticket.Evaluation.Score}/10 - Solved: {ticket.Evaluation.WasSolved}");

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays known problems.
        /// </summary>
        private static void DisplayKnownProblems(FileRepository<KnownProblem> knownProblemRepo)
        {
            Console.WriteLine("=== Problemas conhecidos ===");

            foreach (KnownProblem knownProblem in knownProblemRepo.GetAll())
            {
                Console.WriteLine($"{knownProblem.Title} - {knownProblem.AssistanceType} - {knownProblem.ProductFamily}");
                Console.WriteLine($"Solution: {knownProblem.Solution}");

                foreach (Tutorial tutorial in knownProblem.Tutorials)
                    Console.WriteLine($"  Tutorial: {tutorial.Title}");

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays ticket details in the console application.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        private static void DisplayTicket(Ticket ticket)
        {
            Console.WriteLine($"Ticket {ticket.TicketNumber}: {ticket.Subject}");
            Console.WriteLine($"Client: {ticket.Client.FullName}");
            Console.WriteLine($"Product: {ticket.Product.Brand} {ticket.Product.Model}");
            Console.WriteLine($"Type: {ticket.AssistanceType} | Priority: {ticket.Priority} | Status: {ticket.Status}");
            Console.WriteLine($"Limit date: {ticket.LimitDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Interactions: {ticket.Interactions.Count}");
        }

        /// <summary>
        /// Selects an item from a numbered list.
        /// </summary>
        private static T? SelectItem<T>(IEnumerable<T> items, Func<T, string> label) where T : class
        {
            List<T> list = items.ToList();

            if (list.Count == 0)
            {
                Console.WriteLine("Nao existem registos disponiveis.");
                return null;
            }

            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1} - {label(list[i])}");

            int option = ReadInt("Escolha: ", 1, list.Count);
            return list[option - 1];
        }

        /// <summary>
        /// Selects an enum value from a numbered list.
        /// </summary>
        private static T SelectEnum<T>(string title) where T : struct, Enum
        {
            T[] values = Enum.GetValues<T>();
            Console.WriteLine(title);

            for (int i = 0; i < values.Length; i++)
                Console.WriteLine($"{i + 1} - {values[i]}");

            int option = ReadInt("Escolha: ", 1, values.Length);
            return values[option - 1];
        }

        /// <summary>
        /// Reads a required text value.
        /// </summary>
        private static string ReadRequired(string message)
        {
            string? value;

            do
            {
                Console.Write(message);
                value = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(value));

            return value.Trim();
        }

        /// <summary>
        /// Reads an integer inside a range.
        /// </summary>
        private static int ReadInt(string message, int min, int max)
        {
            int value;

            do
            {
                Console.Write(message);
            }
            while (!int.TryParse(Console.ReadLine(), out value) || value < min || value > max);

            return value;
        }

        /// <summary>
        /// Reads a boolean answer.
        /// </summary>
        private static bool ReadBool(string message)
        {
            string value;

            do
            {
                Console.Write(message);
                value = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
            }
            while (value != "s" && value != "n");

            return value == "s";
        }

        /// <summary>
        /// Pauses the console.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Prima ENTER para continuar...");
            Console.ReadLine();
        }

        /// <summary>
        /// Clears the console when the output supports it.
        /// </summary>
        private static void SafeClear()
        {
            if (Console.IsOutputRedirected)
                return;

            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        /// Loads all repositories.
        /// </summary>
        private static void LoadAll(
            FileRepository<Client> clientRepo,
            FileRepository<SupportOperator> operatorRepo,
            FileRepository<Product> productRepo,
            FileRepository<KnownProblem> knownProblemRepo,
            FileRepository<Ticket> ticketRepo)
        {
            clientRepo.Load();
            operatorRepo.Load();
            productRepo.Load();
            knownProblemRepo.Load();
            ticketRepo.Load();
        }

        /// <summary>
        /// Saves all repositories.
        /// </summary>
        private static void SaveAll(
            FileRepository<Client> clientRepo,
            FileRepository<SupportOperator> operatorRepo,
            FileRepository<Product> productRepo,
            FileRepository<KnownProblem> knownProblemRepo,
            FileRepository<Ticket> ticketRepo)
        {
            clientRepo.Save();
            operatorRepo.Save();
            productRepo.Save();
            knownProblemRepo.Save();
            ticketRepo.Save();
        }

        #endregion
    }
}


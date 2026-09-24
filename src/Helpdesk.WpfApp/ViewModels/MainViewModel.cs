//-----------------------------------------------------------------
//    <copyright file="MainViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>14-07-2026</date>
//    <time>20:00</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Exceptions;
using Helpdesk.Core.Models;
using Helpdesk.WpfApp.Services;
using Helpdesk.WpfApp.ViewModels.Commands;

namespace Helpdesk.WpfApp.ViewModels
{
    /// <summary>
    /// ViewModel for the main Helpdesk window.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private readonly ApplicationDataService _data;
        private Ticket? _selectedTicket;
        private string _statusMessage = string.Empty;
        private string _resolutionText = string.Empty;
        private string _evaluationComment = string.Empty;
        private int _evaluationScore = 9;
        private bool _evaluationSolved = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the clients.
        /// </summary>
        public ObservableCollection<Client> Clients { get; }

        /// <summary>
        /// Gets the products.
        /// </summary>
        public ObservableCollection<Product> Products { get; }

        /// <summary>
        /// Gets the operators.
        /// </summary>
        public ObservableCollection<SupportOperator> Operators { get; }

        /// <summary>
        /// Gets the known problems.
        /// </summary>
        public ObservableCollection<KnownProblem> KnownProblems { get; }

        /// <summary>
        /// Gets the tickets.
        /// </summary>
        public ObservableCollection<Ticket> Tickets { get; }

        /// <summary>
        /// Gets or sets the selected ticket.
        /// </summary>
        public Ticket? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                if (SetProperty(ref _selectedTicket, value))
                    RefreshCommandStates();
            }
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// Gets or sets the resolution text.
        /// </summary>
        public string ResolutionText
        {
            get => _resolutionText;
            set => SetProperty(ref _resolutionText, value);
        }

        /// <summary>
        /// Gets or sets the evaluation comment.
        /// </summary>
        public string EvaluationComment
        {
            get => _evaluationComment;
            set => SetProperty(ref _evaluationComment, value);
        }

        /// <summary>
        /// Gets or sets the evaluation score.
        /// </summary>
        public int EvaluationScore
        {
            get => _evaluationScore;
            set => SetProperty(ref _evaluationScore, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the issue was solved.
        /// </summary>
        public bool EvaluationSolved
        {
            get => _evaluationSolved;
            set => SetProperty(ref _evaluationSolved, value);
        }

        /// <summary>
        /// Gets the number of clients.
        /// </summary>
        public int ClientCount => Clients.Count;

        /// <summary>
        /// Gets the number of products.
        /// </summary>
        public int ProductCount => Products.Count;

        /// <summary>
        /// Gets the number of operators.
        /// </summary>
        public int OperatorCount => Operators.Count;

        /// <summary>
        /// Gets the number of tickets.
        /// </summary>
        public int TicketCount => Tickets.Count;

        /// <summary>
        /// Gets the number of open tickets.
        /// </summary>
        public int OpenTicketCount => Tickets.Count(t => t.Status != TicketStatus.Closed && t.Status != TicketStatus.Cancelled);

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Gets the assign ticket command.
        /// </summary>
        public ICommand AssignTicketCommand { get; }

        /// <summary>
        /// Gets the wait client command.
        /// </summary>
        public ICommand WaitClientCommand { get; }

        /// <summary>
        /// Gets the resolve ticket command.
        /// </summary>
        public ICommand ResolveTicketCommand { get; }

        /// <summary>
        /// Gets the close ticket command.
        /// </summary>
        public ICommand CloseTicketCommand { get; }

        /// <summary>
        /// Gets the evaluate ticket command.
        /// </summary>
        public ICommand EvaluateTicketCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            _data = new ApplicationDataService();

            Clients = new ObservableCollection<Client>();
            Products = new ObservableCollection<Product>();
            Operators = new ObservableCollection<SupportOperator>();
            KnownProblems = new ObservableCollection<KnownProblem>();
            Tickets = new ObservableCollection<Ticket>();

            RefreshCommand = new RelayCommand(_ => Refresh());
            AssignTicketCommand = new RelayCommand(_ => AssignSelectedTicket(), _ => SelectedTicket != null && SelectedTicket.Status == TicketStatus.WaitingAssignment);
            WaitClientCommand = new RelayCommand(_ => WaitClient(), _ => SelectedTicket != null && SelectedTicket.Status == TicketStatus.InProgress);
            ResolveTicketCommand = new RelayCommand(_ => ResolveSelectedTicket(), _ => SelectedTicket != null && (SelectedTicket.Status == TicketStatus.InProgress || SelectedTicket.Status == TicketStatus.WaitingClient));
            CloseTicketCommand = new RelayCommand(_ => CloseSelectedTicket(), _ => SelectedTicket != null && SelectedTicket.Status == TicketStatus.Resolved);
            EvaluateTicketCommand = new RelayCommand(_ => EvaluateSelectedTicket(), _ => SelectedTicket != null && SelectedTicket.Status == TicketStatus.Closed);

            Refresh();
            StatusMessage = $"Dados carregados de {_data.DataFolder}";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens a new ticket from UI data.
        /// </summary>
        public void OpenTicket(
            Client client,
            Product product,
            AssistanceType assistanceType,
            TicketPriority priority,
            string subject,
            string description)
        {
            ExecuteDomainAction(() =>
            {
                Ticket ticket = _data.Helpdesk.OpenTicket(client.ID, product.ID, assistanceType, priority, subject, description);
                _data.Save();
                Refresh();
                SelectedTicket = Tickets.FirstOrDefault(t => t.ID == ticket.ID);
                StatusMessage = $"Ticket criado: {ticket.TicketNumber}";
            });
        }

        /// <summary>
        /// Registers a new client from UI data.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="taxNumber">The tax number.</param>
        public void RegisterClient(string fullName, string email, string phoneNumber, string taxNumber)
        {
            ExecuteDomainAction(() =>
            {
                Client client = _data.Helpdesk.RegisterClient(fullName, email, phoneNumber, taxNumber);
                _data.Save();
                Refresh();
                StatusMessage = $"Cliente registado: {client.FullName}";
            });
        }

        /// <summary>
        /// Registers a new product from UI data.
        /// </summary>
        /// <param name="client">The owner client.</param>
        /// <param name="name">The product name.</param>
        /// <param name="brand">The brand.</param>
        /// <param name="model">The model.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="warrantyMonths">The warranty months.</param>
        public void RegisterProduct(
            Client client,
            string name,
            string brand,
            string model,
            string serialNumber,
            int warrantyMonths)
        {
            ExecuteDomainAction(() =>
            {
                Product product = _data.Helpdesk.RegisterProduct(
                    client.ID,
                    name,
                    brand,
                    model,
                    serialNumber,
                    DateTime.Now.AddMonths(warrantyMonths));

                _data.Save();
                Refresh();
                StatusMessage = $"Produto registado: {product.Summary}";
            });
        }

        /// <summary>
        /// Refreshes all observable collections.
        /// </summary>
        public void Refresh()
        {
            ReplaceCollection(Clients, _data.Clients.GetAll().OrderBy(c => c.FullName));
            ReplaceCollection(Products, _data.Products.GetAll().OrderBy(p => p.Brand).ThenBy(p => p.Model));
            ReplaceCollection(Operators, _data.Operators.GetAll().OrderBy(o => o.FullName));
            ReplaceCollection(KnownProblems, _data.KnownProblems.GetAll().OrderBy(k => k.Title));
            ReplaceCollection(Tickets, _data.Tickets.GetAll().OrderByDescending(t => t.CreatedAt));

            OnPropertyChanged(nameof(ClientCount));
            OnPropertyChanged(nameof(ProductCount));
            OnPropertyChanged(nameof(OperatorCount));
            OnPropertyChanged(nameof(TicketCount));
            OnPropertyChanged(nameof(OpenTicketCount));
            RefreshCommandStates();
        }

        /// <summary>
        /// Assigns the selected ticket to the first available valid operator.
        /// </summary>
        private void AssignSelectedTicket()
        {
            ExecuteDomainAction(() =>
            {
                if (SelectedTicket == null)
                    return;

                SupportOperator? supportOperator = Operators.FirstOrDefault(o => o.IsAvailable && o.CanHandle(SelectedTicket.Priority));

                if (supportOperator == null)
                {
                    StatusMessage = "Nao existe operador disponivel para este ticket.";
                    return;
                }

                _data.Helpdesk.AssignTicket(SelectedTicket.ID, supportOperator.ID);
                SaveAndRefreshSelected("Ticket atribuido com sucesso.");
            });
        }

        /// <summary>
        /// Marks the selected ticket as waiting for client feedback.
        /// </summary>
        private void WaitClient()
        {
            ExecuteDomainAction(() =>
            {
                if (SelectedTicket == null)
                    return;

                _data.Helpdesk.MarkWaitingClient(SelectedTicket.ID);
                SaveAndRefreshSelected("Ticket marcado como aguardando cliente.");
            });
        }

        /// <summary>
        /// Resolves the selected ticket.
        /// </summary>
        private void ResolveSelectedTicket()
        {
            ExecuteDomainAction(() =>
            {
                if (SelectedTicket == null)
                    return;

                string resolution = string.IsNullOrWhiteSpace(ResolutionText)
                    ? "Resolvido pela equipa de suporte."
                    : ResolutionText;

                _data.Helpdesk.ResolveTicket(SelectedTicket.ID, resolution);
                ResolutionText = string.Empty;
                SaveAndRefreshSelected("Ticket resolvido com sucesso.");
            });
        }

        /// <summary>
        /// Closes the selected ticket.
        /// </summary>
        private void CloseSelectedTicket()
        {
            ExecuteDomainAction(() =>
            {
                if (SelectedTicket == null)
                    return;

                _data.Helpdesk.CloseTicket(SelectedTicket.ID);
                SaveAndRefreshSelected("Ticket fechado com sucesso.");
            });
        }

        /// <summary>
        /// Evaluates the selected ticket.
        /// </summary>
        private void EvaluateSelectedTicket()
        {
            ExecuteDomainAction(() =>
            {
                if (SelectedTicket == null)
                    return;

                string comment = string.IsNullOrWhiteSpace(EvaluationComment)
                    ? "Avaliacao registada pela interface WPF."
                    : EvaluationComment;

                _data.Helpdesk.EvaluateTicket(SelectedTicket.ID, EvaluationScore, EvaluationSolved, comment);
                EvaluationComment = string.Empty;
                SaveAndRefreshSelected("Avaliacao registada com sucesso.");
            });
        }

        /// <summary>
        /// Saves, refreshes and keeps the selected ticket active.
        /// </summary>
        /// <param name="message">The status message.</param>
        private void SaveAndRefreshSelected(string message)
        {
            Guid? selectedId = SelectedTicket?.ID;

            _data.Save();
            Refresh();

            if (selectedId.HasValue)
                SelectedTicket = Tickets.FirstOrDefault(t => t.ID == selectedId.Value);

            StatusMessage = message;
        }

        /// <summary>
        /// Executes a domain action with user-friendly error handling.
        /// </summary>
        /// <param name="action">The action.</param>
        private void ExecuteDomainAction(Action action)
        {
            try
            {
                action();
            }
            catch (HelpdeskException ex)
            {
                StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro inesperado: {ex.Message}";
            }
        }

        /// <summary>
        /// Replaces an observable collection.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The new items.</param>
        private static void ReplaceCollection<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();

            foreach (T item in items)
                collection.Add(item);
        }

        /// <summary>
        /// Refreshes WPF command states.
        /// </summary>
        private static void RefreshCommandStates()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}


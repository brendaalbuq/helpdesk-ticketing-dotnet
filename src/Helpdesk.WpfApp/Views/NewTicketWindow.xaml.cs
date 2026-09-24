//-----------------------------------------------------------------
//    <copyright file="NewTicketWindow.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>16-07-2026</date>
//    <time>20:10</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Windows;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Models;
using Helpdesk.WpfApp.ViewModels;

namespace Helpdesk.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for NewTicketWindow.
    /// </summary>
    public partial class NewTicketWindow : Window
    {
        #region Fields

        private readonly MainViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewTicketWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The main view model.</param>
        public NewTicketWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            ClientBox.ItemsSource = _viewModel.Clients;
            TypeBox.ItemsSource = Enum.GetValues(typeof(AssistanceType));
            PriorityBox.ItemsSource = Enum.GetValues(typeof(TicketPriority));

            TypeBox.SelectedItem = AssistanceType.Software;
            PriorityBox.SelectedItem = TicketPriority.Normal;

            if (_viewModel.Clients.Count > 0)
                ClientBox.SelectedIndex = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates products when the selected client changes.
        /// </summary>
        private void ClientBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientBox.SelectedItem is not Client client)
                return;

            List<Product> products = _viewModel.Products
                .Where(product => client.Products.Any(clientProduct => clientProduct.ID == product.ID))
                .ToList();

            ProductBox.ItemsSource = products;
            ProductBox.DisplayMemberPath = "Summary";

            if (products.Count > 0)
                ProductBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Creates a ticket.
        /// </summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientBox.SelectedItem is not Client client ||
                ProductBox.SelectedItem is not Product product ||
                TypeBox.SelectedItem is not AssistanceType assistanceType ||
                PriorityBox.SelectedItem is not TicketPriority priority)
            {
                MessageBox.Show("Selecione cliente, produto, tipo e prioridade.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SubjectBox.Text) || string.IsNullOrWhiteSpace(DescriptionBox.Text))
            {
                MessageBox.Show("Preencha assunto e descricao.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.OpenTicket(
                client,
                product,
                assistanceType,
                priority,
                SubjectBox.Text,
                DescriptionBox.Text);

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancels ticket creation.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }
}


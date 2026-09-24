//-----------------------------------------------------------------
//    <copyright file="NewProductWindow.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>16-07-2026</date>
//    <time>18:50</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Windows;
using Helpdesk.Core.Models;
using Helpdesk.WpfApp.ViewModels;

namespace Helpdesk.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for NewProductWindow.
    /// </summary>
    public partial class NewProductWindow : Window
    {
        #region Fields

        private readonly MainViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProductWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The main view model.</param>
        public NewProductWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            ClientBox.ItemsSource = _viewModel.Clients;

            if (_viewModel.Clients.Count > 0)
                ClientBox.SelectedIndex = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a product.
        /// </summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientBox.SelectedItem is not Client client)
            {
                MessageBox.Show("Selecione um cliente.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(BrandBox.Text) ||
                string.IsNullOrWhiteSpace(ModelBox.Text) ||
                string.IsNullOrWhiteSpace(SerialNumberBox.Text))
            {
                MessageBox.Show("Preencha produto, marca, modelo e numero de serie.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(WarrantyMonthsBox.Text, out int warrantyMonths) || warrantyMonths < 1 || warrantyMonths > 120)
            {
                MessageBox.Show("A garantia deve estar entre 1 e 120 meses.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.RegisterProduct(
                client,
                NameBox.Text,
                BrandBox.Text,
                ModelBox.Text,
                SerialNumberBox.Text,
                warrantyMonths);

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancels product creation.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }
}


//-----------------------------------------------------------------
//    <copyright file="NewClientWindow.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>15-07-2026</date>
//    <time>19:30</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Windows;
using Helpdesk.WpfApp.ViewModels;

namespace Helpdesk.WpfApp.Views
{
    /// <summary>
    /// Interaction logic for NewClientWindow.
    /// </summary>
    public partial class NewClientWindow : Window
    {
        #region Fields

        private readonly MainViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewClientWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The main view model.</param>
        public NewClientWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a client.
        /// </summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(TaxNumberBox.Text))
            {
                MessageBox.Show("Preencha todos os campos.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.RegisterClient(
                FullNameBox.Text,
                EmailBox.Text,
                PhoneBox.Text,
                TaxNumberBox.Text);

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancels client creation.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }
}


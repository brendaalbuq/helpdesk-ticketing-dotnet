//-----------------------------------------------------------------
//    <copyright file="MainWindow.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>15-07-2026</date>
//    <time>18:45</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Windows;
using Helpdesk.WpfApp.ViewModels;
using Helpdesk.WpfApp.Views;

namespace Helpdesk.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly MainViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            ScoreBox.ItemsSource = Enumerable.Range(1, 10).ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the new client window.
        /// </summary>
        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            NewClientWindow window = new NewClientWindow(_viewModel)
            {
                Owner = this
            };

            window.ShowDialog();
        }

        /// <summary>
        /// Opens the new product window.
        /// </summary>
        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            NewProductWindow window = new NewProductWindow(_viewModel)
            {
                Owner = this
            };

            window.ShowDialog();
        }

        /// <summary>
        /// Opens the new ticket window.
        /// </summary>
        private void NewTicketButton_Click(object sender, RoutedEventArgs e)
        {
            NewTicketWindow window = new NewTicketWindow(_viewModel)
            {
                Owner = this
            };

            window.ShowDialog();
        }

        #endregion
    }
}


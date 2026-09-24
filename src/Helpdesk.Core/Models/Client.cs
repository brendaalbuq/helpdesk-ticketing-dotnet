//-----------------------------------------------------------------
//    <copyright file="Client.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>02-07-2026</date>
//    <time>20:05</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

using System.Text.Json.Serialization;
using Helpdesk.Core.Exceptions;

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a client that requests technical assistance.
    /// </summary>
    [CLSCompliant(true)]
    public class Client : Person
    {
        #region Fields

        private string _taxNumber = string.Empty;
        private readonly List<Product> _products;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the fiscal number.
        /// </summary>
        public string TaxNumber
        {
            get => _taxNumber;
            set => _taxNumber = Guard.RequiredText(value, nameof(TaxNumber));
        }

        /// <summary>
        /// Gets the identifiers of the client's registered products.
        /// </summary>
        [JsonInclude]
        public List<Guid> ProductIds { get; private set; }

        /// <summary>
        /// Gets the client's canonical products during execution.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<Product> Products => _products.AsReadOnly();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            ProductIds = new List<Guid>();
            _products = new List<Product>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="taxNumber">The fiscal number.</param>
        public Client(string fullName, string email, string phoneNumber, string taxNumber)
            : base(fullName, email, phoneNumber)
        {
            TaxNumber = taxNumber;
            ProductIds = new List<Guid>();
            _products = new List<Product>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a product to the client.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            if (_products.Any(p => p.SerialNumber.Equals(product.SerialNumber, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateEntityException($"Product with serial number {product.SerialNumber} already exists for this client.");

            _products.Add(product);
            ProductIds.Add(product.ID);
        }

        /// <summary>
        /// Restores canonical product references after loading identifiers from JSON.
        /// </summary>
        /// <param name="products">The canonical products in identifier order.</param>
        public void RestoreProducts(IEnumerable<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products), "Products cannot be null.");

            List<Product> canonicalProducts = products.ToList();
            bool identifiersMatch = canonicalProducts.Count == ProductIds.Count &&
                                    canonicalProducts.Select(p => p.ID).SequenceEqual(ProductIds);

            if (!identifiersMatch)
                throw new DomainValidationException("The products do not match the client product identifiers.");

            _products.Clear();
            _products.AddRange(canonicalProducts);
        }

        #endregion
    }
}


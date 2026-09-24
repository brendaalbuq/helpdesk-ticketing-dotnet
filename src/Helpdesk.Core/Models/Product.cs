//-----------------------------------------------------------------
//    <copyright file="Product.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>02-07-2026</date>
//    <time>19:20</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Models
{
    /// <summary>
    /// Represents a product supported by the Helpdesk.
    /// </summary>
    [CLSCompliant(true)]
    public class Product : BaseEntity
    {
        #region Fields

        private string _name = string.Empty;
        private string _brand = string.Empty;
        private string _model = string.Empty;
        private string _serialNumber = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = Guard.RequiredText(value, nameof(Name));
        }

        /// <summary>
        /// Gets or sets the product brand.
        /// </summary>
        public string Brand
        {
            get => _brand;
            set => _brand = Guard.RequiredText(value, nameof(Brand));
        }

        /// <summary>
        /// Gets or sets the product model.
        /// </summary>
        public string Model
        {
            get => _model;
            set => _model = Guard.RequiredText(value, nameof(Model));
        }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        public string SerialNumber
        {
            get => _serialNumber;
            set => _serialNumber = Guard.RequiredText(value, nameof(SerialNumber));
        }

        /// <summary>
        /// Gets or sets the warranty end date.
        /// </summary>
        public DateTime WarrantyEndDate { get; set; }

        /// <summary>
        /// Gets a value indicating whether the product is under warranty.
        /// </summary>
        public bool IsUnderWarranty => WarrantyEndDate.Date >= DateTime.Now.Date;

        /// <summary>
        /// Gets a short product summary for user interfaces.
        /// </summary>
        public string Summary => $"{Brand} {Model} ({SerialNumber})";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="brand">The brand.</param>
        /// <param name="model">The model.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="warrantyEndDate">The warranty end date.</param>
        public Product(string name, string brand, string model, string serialNumber, DateTime warrantyEndDate)
        {
            Name = name;
            Brand = brand;
            Model = model;
            SerialNumber = serialNumber;
            WarrantyEndDate = warrantyEndDate;
        }

        #endregion

    }
}


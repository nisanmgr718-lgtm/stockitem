using System;

namespace StockWise
{
    public abstract class StockItem
    {
        private string _name;
        private string _category;
        private int _quantity;
        private int _minimumStockLevel;

        public string Id { get; set; }

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Item name cannot be empty.")
                : value;
        }

        public string Category
        {
            get => _category;
            set => _category = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Category cannot be empty.")
                : value;
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value < 0
                ? throw new ArgumentException("Quantity cannot be negative.")
                : value;
        }

        public int MinimumStockLevel
        {
            get => _minimumStockLevel;
            set => _minimumStockLevel = value < 0
                ? throw new ArgumentException("Minimum stock level cannot be negative.")
                : value;
        }

        protected StockItem(string id, string name, string category, int quantity, int minimumStockLevel)
        {
            Id = id;
            Name = name;
            Category = category;
            Quantity = quantity;
            MinimumStockLevel = minimumStockLevel;
        }

        public bool IsLowStock() => Quantity <= MinimumStockLevel;

        public abstract string GetAlertStatus();
    }
}
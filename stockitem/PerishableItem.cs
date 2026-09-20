using System;

namespace StockWise
{
    public class PerishableItem : StockItem
    {
        public DateTime ExpiryDate { get; set; }

        public PerishableItem(string id, string name, string category, int quantity,
            int minimumStockLevel, DateTime expiryDate)
            : base(id, name, category, quantity, minimumStockLevel)
        {
            ExpiryDate = expiryDate;
        }

        public int DaysUntilExpiry() => (ExpiryDate.Date - DateTime.Today).Days;

        public override string GetAlertStatus()
        {
            int daysLeft = DaysUntilExpiry();

            if (daysLeft < 0)
                return "EXPIRED - discard";
            if (daysLeft <= 2)
                return "Expiring soon - discount or use";
            if (IsLowStock())
                return "Low stock - reorder";

            return "OK";
        }
    }
}

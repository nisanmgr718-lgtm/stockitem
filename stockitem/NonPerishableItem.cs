namespace StockWise
{
    public class NonPerishableItem : StockItem
    {
        public NonPerishableItem(string id, string name, string category, int quantity, int minimumStockLevel)
            : base(id, name, category, quantity, minimumStockLevel)
        {
        }

        public override string GetAlertStatus()
        {
            return IsLowStock() ? "Low stock - reorder" : "OK";
        }
    }
}

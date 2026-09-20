using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace StockWise
{
    internal class StockRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public string Type { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class StockManager
    {
        private readonly string _filePath;
        private List<StockItem> _items;

        public StockManager(string filePath = "stock.json")
        {
            _filePath = filePath;
            _items = LoadFromFile();
        }

        public IReadOnlyList<StockItem> Items => _items.AsReadOnly();

        public void AddItem(StockItem item)
        {
            _items.Add(item);
            Save();
        }

        public void RemoveItem(string id)
        {
            _items.RemoveAll(i => i.Id == id);
            Save();
        }

        public void UpdateItem(StockItem updated)
        {
            int index = _items.FindIndex(i => i.Id == updated.Id);
            if (index >= 0)
            {
                _items[index] = updated;
                Save();
            }
        }

        public List<StockItem> GetAlerts()
        {
            return _items.Where(i => i.GetAlertStatus() != "OK").ToList();
        }

        private List<StockItem> LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<StockItem>();

                string json = File.ReadAllText(_filePath);
                var records = JsonSerializer.Deserialize<List<StockRecord>>(json) ?? new List<StockRecord>();

                var items = new List<StockItem>();
                foreach (var r in records)
                {
                    if (r.Type == "Perishable" && r.ExpiryDate.HasValue)
                        items.Add(new PerishableItem(r.Id, r.Name, r.Category, r.Quantity, r.MinimumStockLevel, r.ExpiryDate.Value));
                    else
                        items.Add(new NonPerishableItem(r.Id, r.Name, r.Category, r.Quantity, r.MinimumStockLevel));
                }
                return items;
            }
            catch
            {
                return new List<StockItem>();
            }
        }

        public void Save()
        {
            try
            {
                var records = _items.Select(i => new StockRecord
                {
                    Id = i.Id,
                    Name = i.Name,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    MinimumStockLevel = i.MinimumStockLevel,
                    Type = i is PerishableItem ? "Perishable" : "NonPerishable",
                    ExpiryDate = (i as PerishableItem)?.ExpiryDate
                }).ToList();

                string json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch
            {
            }
        }
    }
}

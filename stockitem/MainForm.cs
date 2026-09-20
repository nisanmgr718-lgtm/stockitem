using System;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace StockWise
{
    public class MainForm : Form
    {
        private readonly StockManager _manager = new StockManager();

        private DataGridView _grid;
        private TextBox _txtName, _txtCategory, _txtQuantity, _txtMinLevel;
        private DateTimePicker _dtExpiry;
        private CheckBox _chkPerishable;
        private Button _btnAdd, _btnDelete, _btnRefresh;
        private Label _lblAlerts;

        public MainForm()
        {
            BuildUi();
            RefreshGrid();
        }

        private void BuildUi()
        {
            Text = "StockWise - Inventory Manager";
            Width = 900;
            Height = 620;

            _grid = new DataGridView
            {
                Left = 10,
                Top = 10,
                Width = 860,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var lblName = new Label { Text = "Name", Left = 10, Top = 320, Width = 140 };
            _txtName = new TextBox { Left = 10, Top = 340, Width = 140 };

            var lblCategory = new Label { Text = "Category", Left = 160, Top = 320, Width = 140 };
            _txtCategory = new TextBox { Left = 160, Top = 340, Width = 140 };

            var lblQuantity = new Label { Text = "Quantity", Left = 310, Top = 320, Width = 80 };
            _txtQuantity = new TextBox { Left = 310, Top = 340, Width = 70 };

            var lblMin = new Label { Text = "Min Level", Left = 390, Top = 320, Width = 80 };
            _txtMinLevel = new TextBox { Left = 390, Top = 340, Width = 70 };

            _chkPerishable = new CheckBox { Text = "Perishable?", Left = 470, Top = 342, Width = 100 };
            _chkPerishable.CheckedChanged += (s, e) => _dtExpiry.Enabled = _chkPerishable.Checked;

            _dtExpiry = new DateTimePicker { Left = 580, Top = 338, Width = 150, Enabled = false };

            _btnAdd = new Button { Text = "Add Item", Left = 10, Top = 380, Width = 110 };
            _btnAdd.Click += BtnAdd_Click;

            _btnDelete = new Button { Text = "Delete Selected", Left = 130, Top = 380, Width = 130 };
            _btnDelete.Click += BtnDelete_Click;

            _btnRefresh = new Button { Text = "Refresh", Left = 270, Top = 380, Width = 90 };
            _btnRefresh.Click += (s, e) => RefreshGrid();

            _lblAlerts = new Label
            {
                Left = 10,
                Top = 420,
                Width = 860,
                Height = 150,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkRed
            };

            Controls.AddRange(new Control[]
            {
                _grid, lblName, _txtName, lblCategory, _txtCategory,
                lblQuantity, _txtQuantity, lblMin, _txtMinLevel,
                _chkPerishable, _dtExpiry, _btnAdd, _btnDelete, _btnRefresh, _lblAlerts
            });
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int qty = int.Parse(_txtQuantity.Text);
                int min = int.Parse(_txtMinLevel.Text);
                string id = Guid.NewGuid().ToString("N").Substring(0, 8);

                StockItem item = _chkPerishable.Checked
                    ? new PerishableItem(id, _txtName.Text, _txtCategory.Text, qty, min, _dtExpiry.Value)
                    : new NonPerishableItem(id, _txtName.Text, _txtCategory.Text, qty, min);

                _manager.AddItem(item);
                RefreshGrid();
                ClearInputs();
            }
            catch (FormatException)
            {
                MessageBox.Show("Quantity and minimum level must be whole numbers.", "Invalid input");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid input");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null) return;
            string id = _grid.CurrentRow.Cells["Id"].Value.ToString();
            _manager.RemoveItem(id);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            _grid.DataSource = null;
            _grid.DataSource = _manager.Items.Select(i => new
            {
                i.Id,
                i.Name,
                i.Category,
                i.Quantity,
                i.MinimumStockLevel,
                Type = i is PerishableItem ? "Perishable" : "Non-perishable",
                Expiry = (i as PerishableItem)?.ExpiryDate.ToShortDateString() ?? "-",
                Status = i.GetAlertStatus()
            }).ToList();

            var alerts = _manager.GetAlerts();
            _lblAlerts.Text = alerts.Count == 0
                ? "No alerts - all stock levels and expiry dates are fine."
                : "Alerts:\n" + string.Join("\n", alerts.Select(a => $"- {a.Name}: {a.GetAlertStatus()}"));
        }

        private void ClearInputs()
        {
            _txtName.Clear();
            _txtCategory.Clear();
            _txtQuantity.Clear();
            _txtMinLevel.Clear();
            _chkPerishable.Checked = false;
        }
    }
}

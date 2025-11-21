using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace ProjectForEventDriven
{
    public partial class orders : Form
    {
        private string fullName;

        public orders(string fullName)
        {
            InitializeComponent();
            this.fullName = fullName;
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            Customer form = new Customer(fullName);
            form.Show();
            this.Hide();
        }


        private void orders_Load(object sender, EventArgs e)
        {
            MessageBox.Show(fullName);
            this.Load += new System.EventHandler(this.orders_Load);
            LoadCustomerOrders();
        }

        private void LoadCustomerOrders()
        {
            dataGridView1.AutoGenerateColumns = true;
            


            string connectionString = "server=localhost;user id=root;password=;database=ecommerceinventorydb;";
            string query = @"
SELECT 
    t.transaction_id, 
    d.product_name, 
    d.category, 
    d.price, 
    t.total_amount, 
    t.payment, 
    t.change_due, 
    t.transaction_date
FROM tbl_transactions t
INNER JOIN tbl_transaction_details d ON t.transaction_id = d.transaction_id
WHERE t.customer_name = @name
ORDER BY t.transaction_date DESC;";


            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", fullName);

                    MessageBox.Show("Looking for orders by: " + fullName);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    MessageBox.Show("Rows loaded: " + table.Rows.Count);

                    dataGridView1.DataSource = table;

                    if (table.Columns.Contains("transaction_id"))
                        dataGridView1.Columns["transaction_id"].HeaderText = "Transaction ID";
                    if (table.Columns.Contains("product_name"))
                        dataGridView1.Columns["product_name"].HeaderText = "Product";
                    if (table.Columns.Contains("category"))
                        dataGridView1.Columns["category"].HeaderText = "Category";
                    if (table.Columns.Contains("price"))
                        dataGridView1.Columns["price"].HeaderText = "Price";
                    if (table.Columns.Contains("total_amount"))
                        dataGridView1.Columns["total_amount"].HeaderText = "Total";
                    if (table.Columns.Contains("payment"))
                        dataGridView1.Columns["payment"].HeaderText = "Payment";
                    if (table.Columns.Contains("change_due"))
                        dataGridView1.Columns["change_due"].HeaderText = "Change";
                    if (table.Columns.Contains("transaction_date"))
                        dataGridView1.Columns["transaction_date"].HeaderText = "Date";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load orders: " + ex.Message);
            }
        }
    }
}

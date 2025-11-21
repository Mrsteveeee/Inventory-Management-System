using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjectForEventDriven
{
    public partial class Customer : Form
    {
        private string customerName;
        public Customer(string fullName)
        {
            InitializeComponent();
            customerName = fullName;

            dataGridView2.ColumnCount = 4;
            dataGridView2.Columns[0].Name = "Product ID";
            dataGridView2.Columns[1].Name = "Product Name";
            dataGridView2.Columns[2].Name = "Category";
            dataGridView2.Columns[3].Name = "Price";
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "Product ID";
            dataGridView1.Columns[1].Name = "Product Name";
            dataGridView1.Columns[2].Name = "Category";
            dataGridView1.Columns[3].Name = "Price";
            LoadProducts();
            dataGridView2.CellClick += dataGridView2_CellContentClick;

        }

        

        private void btnLoggout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirmation Message",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Form login = new login();
                login.Show();

                this.Hide();

            }
        }
        private void LoadProducts()
        {
            string connectionString = "server=localhost;user id=root;password=;database=ecommerceinventorydb;";
            string query = "SELECT * FROM tbl_product";

            try
            {
                dataGridView2.Rows.Clear();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView2.Rows.Add(
                                reader["product_id"].ToString(),
                                reader["category"].ToString(),
                                reader["name"].ToString(),
                                Convert.ToDecimal(reader["price"]).ToString("C")
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load products: " + ex.Message);
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.Rows.Count)
            {
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];


                string productId = selectedRow.Cells["Product ID"].Value.ToString();
                string productName = selectedRow.Cells["Product Name"].Value.ToString();
                string category = selectedRow.Cells["Category"].Value.ToString();
                string price = selectedRow.Cells["Price"].Value.ToString();


                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == productId)
                    {
                        MessageBox.Show("This product is already in the purchase list.");
                        return;
                    }
                }


                dataGridView1.Rows.Add(productId, productName, category, price);
                UpdateTotal();
            }
        }
        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Price"].Value != null)
                {

                    string priceText = row.Cells["Price"].Value.ToString()
                        .Replace("$", "")
                        .Replace(",", "")
                        .Trim();

                    if (decimal.TryParse(priceText, out decimal price))
                    {
                        total += price;
                    }
                }
            }

            textBox1.Text = total.ToString("F2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;user id=root;password=;database=ecommerceinventorydb;";

            if (!decimal.TryParse(textBox1.Text, out decimal total))
            {
                MessageBox.Show("Invalid total amount.");
                return;
            }

            if (!decimal.TryParse(textBox2.Text, out decimal payment))
            {
                MessageBox.Show("Please enter a valid payment amount.");
                return;
            }

            if (payment < total)
            {
                MessageBox.Show("Insufficient payment.");
                return;
            }

            decimal change = payment - total;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    
                    string insertTransaction = "INSERT INTO tbl_transactions (customer_name, total_amount, payment, change_due) " +
                                               "VALUES (@name, @total, @payment, @change); SELECT LAST_INSERT_ID();";

                    MySqlCommand cmd = new MySqlCommand(insertTransaction, conn);
                    cmd.Parameters.AddWithValue("@name", customerName);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@payment", payment);
                    cmd.Parameters.AddWithValue("@change", change);

                    int transactionId = Convert.ToInt32(cmd.ExecuteScalar());

                   
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow || row.Cells[0].Value == null)
                            continue;

                        string productId = row.Cells[0].Value.ToString();
                        string productName = row.Cells[1].Value.ToString();
                        string category = row.Cells[2].Value.ToString();
                        string priceText = row.Cells[3].Value.ToString().Replace("$", "").Replace(",", "").Trim();

                        if (!decimal.TryParse(priceText, out decimal price))
                            continue;

                        string insertDetail = "INSERT INTO tbl_transaction_details (transaction_id, product_id, product_name, category, price) " +
                                              "VALUES (@tid, @pid, @pname, @cat, @price)";

                        MySqlCommand detailCmd = new MySqlCommand(insertDetail, conn);
                        detailCmd.Parameters.AddWithValue("@tid", transactionId);
                        detailCmd.Parameters.AddWithValue("@pid", productId);
                        detailCmd.Parameters.AddWithValue("@pname", productName);
                        detailCmd.Parameters.AddWithValue("@cat", category);
                        detailCmd.Parameters.AddWithValue("@price", price);

                        detailCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Transaction completed!\nChange: {change:C}");

                textBox1.Clear();
                textBox2.Clear();
                dataGridView1.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing the transaction: " + ex.Message);
            }
        }
    }
}
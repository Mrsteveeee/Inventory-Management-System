using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjectForEventDriven
{
    public partial class addProduct : UserControl
    {
        private int selectedProductId = -1;

        public addProduct()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            InitializeGrid();
            LoadProductsToGrid();
        }

        private void InitializeGrid()
        {
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Name";
            dataGridView1.Columns[1].Name = "Category";
            dataGridView1.Columns[2].Name = "Brand";
            dataGridView1.Columns[3].Name = "Stock";
            dataGridView1.Columns[4].Name = "Price";
        }

        private void LoadProductsToGrid()
        {
            dataGridView1.Rows.Clear();
            string connStr = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
            string query = "SELECT name, category, brand, stock, price FROM tbl_product";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(
                            reader["name"].ToString(),
                            reader["category"].ToString(),
                            reader["brand"].ToString(),
                            reader["stock"].ToString(),
                            Convert.ToDecimal(reader["price"]).ToString("C")
                        );
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string category = cmbCategory.Text.Trim();
            string brand = txtBox.Text.Trim();
            bool stockParsed = int.TryParse(txtStock.Text.Trim(), out int stock);
            bool priceParsed = decimal.TryParse(textBox1.Text.Trim(), out decimal price);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(brand) || !stockParsed || !priceParsed)
            {
                MessageBox.Show("Please fill in all fields correctly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO tbl_product (name, category, brand, stock, price) VALUES (@name, @category, @brand, @stock, @price)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@brand", brand);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            ClearFields();
            LoadProductsToGrid();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            txtName.Text = row.Cells[0].Value?.ToString();
            cmbCategory.Text = row.Cells[1].Value?.ToString();
            txtBox.Text = row.Cells[2].Value?.ToString();
            txtStock.Text = row.Cells[3].Value?.ToString();
            textBox1.Text = row.Cells[4].Value?.ToString().Replace("₱", "").Trim(); // Adjust if currency is different

            string name = txtName.Text;
            string connStr = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT product_id FROM tbl_product WHERE name = @name LIMIT 1";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        selectedProductId = Convert.ToInt32(result);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Please select a row to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtName.Text.Trim();
            string category = cmbCategory.Text.Trim();
            string brand = txtBox.Text.Trim();
            bool stockParsed = int.TryParse(txtStock.Text.Trim(), out int stock);
            bool priceParsed = decimal.TryParse(textBox1.Text.Trim(), out decimal price);

            if (!stockParsed || !priceParsed || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Fill fields properly.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE tbl_product SET name=@name, category=@category, brand=@brand, stock=@stock, price=@price WHERE product_id=@id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@brand", brand);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@id", selectedProductId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product updated successfully.");
                }
            }

            ClearFields();
            LoadProductsToGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string connStr = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "DELETE FROM tbl_product WHERE product_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedProductId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Product deleted.");
                    }
                }

                ClearFields();
                LoadProductsToGrid();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            cmbCategory.SelectedIndex = -1;
            txtBox.Clear();
            txtStock.Clear();
            textBox1.Clear();
            selectedProductId = -1;
            dataGridView1.ClearSelection();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void addProduct_Load(object sender, EventArgs e)
        {
           
        }
    }
}

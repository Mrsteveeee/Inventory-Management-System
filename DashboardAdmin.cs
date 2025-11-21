using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace ProjectForEventDriven
{
    public partial class DashboardAdmin : Form
    {
        public DashboardAdmin()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "RecordID";
            dataGridView1.Columns[1].Name = "User";
            dataGridView1.Columns[2].Name = "Category";
            dataGridView1.Columns[3].Name = "Product";
            dataGridView1.Columns[4].Name = "Income";
            dataGridView1.Columns[5].Name = "RecordDate";
            LoadRecords();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Exit?", "Confirmation Message",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                Application.Exit();
            }

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

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            dataGridView1.Visible = true;

            inventory1.Visible = false;
            addProduct1.Visible = false;
            usersAdmin1.Visible = false;
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            dataGridView1.Visible = false;

            inventory1.Visible = true;
            inventory1.BringToFront();

            addProduct1.Visible = false;
            usersAdmin1.Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            dataGridView1.Visible = false;

            inventory1.Visible = false;
            addProduct1.Visible = true;
            addProduct1.BringToFront();

            usersAdmin1.Visible = false;
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            dataGridView1.Visible = false;

            inventory1.Visible = false;
            addProduct1.Visible = false;
            usersAdmin1.Visible = true;
            usersAdmin1.BringToFront();
        }

        private void LoadRecords()
        {
            string connectionString = "server=localhost;user id=root;password=;database=ecommerceinventorydb;";
            string query = @"
        SELECT 
            t.transaction_id AS RecordID,
            t.customer_name AS User,
            d.category AS Category,
            d.product_name AS Product,
            d.price AS Income,
            t.transaction_date AS RecordDate
        FROM tbl_transactions t
        INNER JOIN tbl_transaction_details d ON t.transaction_id = d.transaction_id
        ORDER BY t.transaction_date DESC;
    ";

            try
            {
                dataGridView1.Rows.Clear();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(
                                reader["RecordID"].ToString(),
                                reader["User"].ToString(),
                                reader["Category"].ToString(),
                                reader["Product"].ToString(),
                                Convert.ToDecimal(reader["Income"]).ToString("C"),
                                Convert.ToDateTime(reader["RecordDate"]).ToString("yyyy-MM-dd HH:mm")
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading records: " + ex.Message);
            }
        }
    }
}





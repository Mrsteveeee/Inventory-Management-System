using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjectForEventDriven
{
    public partial class Inventory : UserControl
    {
        public Inventory()
        {
            InitializeComponent();
            LoadInventoryData();
        }
        private void LoadInventoryData()
        {
            string connectionString = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT product_id, name, category, brand, stock FROM tbl_product";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;

                        
                        dataGridView1.Columns[0].HeaderText = "Product ID";
                        dataGridView1.Columns[1].HeaderText = "Name";
                        dataGridView1.Columns[2].HeaderText = "Category";
                        dataGridView1.Columns[3].HeaderText = "Brand";
                        dataGridView1.Columns[4].HeaderText = "Stock";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading inventory: " + ex.Message);
                }
            }
        }
    }
}


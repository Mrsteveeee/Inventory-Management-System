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
    public partial class usersAdmin : UserControl
    {
        private string connectionString = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";

        public usersAdmin()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "fullname";
            dataGridView1.Columns[1].Name = "username";

            LoadUserData();
        }

        private void LoadUserData()
        {
            dataGridView1.Rows.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT fullname, username FROM tbl_users";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fullname = reader["fullname"].ToString();
                            string username = reader["username"].ToString();

                            dataGridView1.Rows.Add(fullname, username);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users:\n" + ex.Message);
            }
        }
    }
}

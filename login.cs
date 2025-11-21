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
    public partial class login: Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void labelReg_Click(object sender, EventArgs e)
        {
            registration Form = new registration();
            Form.Show();
            this.Hide();
        }

       

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;user=root;password=;database=ECommerceInventoryDB;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM tbl_admins WHERE username = @username AND password = @password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                MessageBox.Show("Admin login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DashboardAdmin dashboard = new DashboardAdmin();
                                dashboard.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect admin username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to database:\n" + ex.Message);
                }
            }
        }
        

        private void label3_Click(object sender, EventArgs e)
        {
            Form1 Form = new Form1();
            Form.Show();

            this.Hide();
        }
    }
    
}

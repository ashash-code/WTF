using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp7
{
    public partial class AdminVerification: Form
    {
        private string sentCode = "";
        private string receptorEmail;
        private string PasswordHash;

        public AdminVerification(string code, string email, string password)
        {
            InitializeComponent();
            this.receptorEmail = email;
            this.PasswordHash = password;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string userInput = txtVerify.Text.Trim();

            if (userInput == sentCode)
            {
                MessageBox.Show("Verification successful!");
                string CreatedAt = DateTime.Today.ToString("yyyy-MM-dd");
                string connection = @"Data Source=laptop-amqpha2s;Initial Catalog=WebApp;Integrated Security=True;Encrypt=False";

                using (SqlConnection con = new SqlConnection(connection))
                {
                    string query = "INSERT INTO Userss (Email, PasswordHash, CreatedAt) VALUES (@Email, @PasswordHash, @dateCreated)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", receptorEmail);
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordHash);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        cmd.Parameters.AddWithValue("@dateCreated", CreatedAt);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                AdminLogin l = new AdminLogin();
                l.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminRegister v = new AdminRegister();
            v.Show();
            this.Hide();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class AdminRegister: Form
    {
        private string sentCode = "";
        private readonly HttpClient httpClient = new HttpClient();
        public AdminRegister()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            checkBox1.Checked = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            string receptorEmail = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(receptorEmail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password and a valid email address.");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[\W_]).+$"))
            {
                MessageBox.Show("Password must contain at least one number and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SqlConnection connect = new SqlConnection(@"Data Source=laptop-amqpha2s;Initial Catalog=WebApp;Integrated Security=True;Encrypt=False"))
            {
                connect.Open();
                string checkEmailQuery = "SELECT COUNT(*) FROM Userss WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(checkEmailQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@Email", receptorEmail);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("This email is already registered. Please proceed to login.", "Email Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Login loginForm = new Login();
                        loginForm.Show();
                        this.Hide(); // Optional: hide the register form
                        return;
                    }
                }

                try
                {
                    var response = await httpClient.PostAsync(
                    $"https://localhost:7154/api/emails?receptor={Uri.EscapeDataString(receptorEmail)}",
                    null
                    );


                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        sentCode = result["code"]; // Store the code
                        MessageBox.Show($"Verification code sent to {receptorEmail}");

                        Verification v = new Verification(sentCode, receptorEmail, password);
                        v.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Failed to send email.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            AdminLogin v = new AdminLogin();
            v.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            RegistrationPage r = new RegistrationPage();
            r.Show();
            this.Close();
        }
    }
    
}

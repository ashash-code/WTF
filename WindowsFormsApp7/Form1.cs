using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp7
{
    public partial class Register : Form
    {
        private string sentCode = "";
        private readonly HttpClient httpClient = new HttpClient();
        public Register()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            checkBox1.Checked = false;


        }
       

        private void Form1_Load(object sender, EventArgs e)
        {

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

            string supabaseUrl = "https://cyugvmkmbjwyjsnutaph.supabase.co/rest/v1/Userss"; // Replace with your actual Supabase URL
            string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN5dWd2bWttYmp3eWpzbnV0YXBoIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODQzNDg0NSwiZXhwIjoyMDc0MDEwODQ1fQ.Fua0y9qZc7vW-Wpa-nD2JE8AYX6Wxab0Vgkuq5kGpXs"; // Replace with your actual anon key

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

            var response = await httpClient.GetAsync($"{supabaseUrl}?Email=eq.{Uri.EscapeDataString(receptorEmail)}&select=Email");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

                if (results.Count > 0)
                {
                    MessageBox.Show("This email is already registered. Please proceed to login.", "Email Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Login loginForm = new Login();
                    loginForm.Show();
                    this.Hide(); // Optional: hide the register form
                    return;
                }
            }
            else
            {
                MessageBox.Show("Failed to connect to Supabase. Please check your API key and URL.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
                {
                    var emailResponse = await httpClient.PostAsync(
                    $"https://localhost:7154/api/emails?receptor={Uri.EscapeDataString(receptorEmail)}",
                    null
                    );


                    if (emailResponse.IsSuccessStatusCode)
                    {
                        var json = await emailResponse.Content.ReadAsStringAsync();
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
            Login v = new Login();
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

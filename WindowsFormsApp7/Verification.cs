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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp7
{
    public partial class Verification: Form
    {
        private string sentCode = "";
        private string receptorEmail;
        private string PasswordHash;
        private readonly HttpClient httpClient = new HttpClient();

        public Verification(string code, string email, string password)
        {
            InitializeComponent();
            sentCode = code;
            this.receptorEmail = email;
            this.PasswordHash = password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register v = new Register();
            v.Show();
            this.Hide();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string userInput = txtVerify.Text.Trim();

            if (userInput == sentCode)
            {
                MessageBox.Show("Verification successful!");
                string CreatedAt = DateTime.Today.ToString("yyyy-MM-dd");
                string supabaseUrl = "https://cyugvmkmbjwyjsnutaph.supabase.co/rest/v1/Userss"; // Replace with your actual table endpoint
                string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN5dWd2bWttYmp3eWpzbnV0YXBoIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODQzNDg0NSwiZXhwIjoyMDc0MDEwODQ1fQ.Fua0y9qZc7vW-Wpa-nD2JE8AYX6Wxab0Vgkuq5kGpXs"; // Replace with your actual anon key

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
                httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var newUser = new
                {
                    Email = receptorEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordHash),
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");
                var insertResponse = await httpClient.PostAsync(supabaseUrl, content);

                var responseBody = await insertResponse.Content.ReadAsStringAsync();
                //MessageBox.Show(responseBody);

                if (insertResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("User registered successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to register user.");
                }
                Login l = new Login();
                l.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.");
            }
        }
    }
    
}

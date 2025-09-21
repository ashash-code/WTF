using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp7
{
    public partial class Login: Form
    {
        public static string LoggedInUser = "";
        SqlConnection connect = new SqlConnection(@"Data Source=laptop-amqpha2s;Initial Catalog=WebApp;Integrated Security=True;Encrypt=False");

        public Login()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            checkBox1.Checked = false;
        }

        private  void button1_Click(object sender, EventArgs e)
        {
            string Email = txtEmail.Text.Trim();
           
            string enteredPassword = txtPassword.Text;

            

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please enter a password and a valid email address.");
                return;
            }


            try
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();

                string selectData = "SELECT Id, PasswordHash FROM Userss WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            MessageBox.Show("This email is not registered. Please sign up first.", "Email Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Register registerForm = new Register();
                            registerForm.Show();
                            this.Hide(); // Optional: hide login form
                            return;
                        }

                        

                        if (dr.Read())
                        {

                            string storedHash = dr["PasswordHash"].ToString();

                            bool isValid = BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);

                            if (isValid)
                            {

                                LoggedInUser = Email;
                                int accountID = Convert.ToInt32(dr["Id"]);
                                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Home home = new Home();
                                home.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Email/Password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Connection error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                    connect.Close();
            }
        }
        

        private void label3_Click(object sender, EventArgs e)
        {
            Register v = new Register();
            v.Show();
            this.Hide();
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

        private void label4_Click(object sender, EventArgs e)
        {
            RegistrationPage r = new RegistrationPage();
            r.Show();
            this.Close();
        }
    }
    
}

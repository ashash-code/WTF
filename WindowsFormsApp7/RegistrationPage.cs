using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class RegistrationPage: Form
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register v = new Register();
            v.Show();
            this.Hide();

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminRegister v = new AdminRegister();
            v.Show();
            this.Hide();
        }
    }
}

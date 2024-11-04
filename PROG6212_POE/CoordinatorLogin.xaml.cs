using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace PROG6212_POE
{
    /// <summary>
    /// Interaction logic for CoordinatorLogin.xaml
    /// </summary>
    public partial class CoordinatorLogin : Window
    {
        public CoordinatorLogin()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the input fields
        string email = txtEmail.Text; // Assuming you have a TextBox for the email
            string password = txtPassword.Password; // Assuming you have a PasswordBox for the password

            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your email and password.");
                return;
            }

            // Hash the entered password (ensure this matches how passwords are hashed in the database)
            string passwordHash = HashPassword(password);

            // Connect to the database to validate credentials
            string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = 'Coordinator'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) // If a record is found
                    {
                        MessageBox.Show("Login successful!");

                        // Proceed to the Coordinator view
                        CoordinatorView coordinatorView = new CoordinatorView();
                        coordinatorView.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password. Please try again.");
                    }
                }
            }
        }

        // Hashing method to convert plain text password to a hashed format
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to a byte array
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a Base64 string
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

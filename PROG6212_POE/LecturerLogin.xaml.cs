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
    /// Interaction logic for LecturerLogin.xaml
    /// </summary>
    public partial class LecturerLogin : Window
    {
        string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";
        public LecturerLogin()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Hash the password before validation
            string passwordHash = HashPassword(password);

            // Validate the login
            if (ValidateLogin(email, passwordHash, "Lecturer"))
            {
                MessageBox.Show("Login successful!");

                // Navigate to Lecturer View after successful login
                LecturerView lecturerView = new LecturerView();
                lecturerView.Show();
                
            }
            else
            {
                //error message
                MessageBox.Show("Invalid email or password.");
            }
        }

        private bool ValidateLogin(string email, string passwordHash, string accountType)
        {

            string query = "SELECT COUNT(1) FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = @AccountType";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@AccountType", accountType);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 1;
                }
            }
        }

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

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
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the form fields
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string email = txtEmail.Text;
            string phoneNumber = txtPhoneNumber.Text;
            string password = txtPassword.Password;

            // Determine if the user is a Lecturer or Coordinator
            string accountType = rbtnLecturer.IsChecked == true ? "Lecturer" : "Coordinator";

            // Hash the password for security (use a proper hashing function in production)
            string passwordHash = HashPassword(password); // Implement your hash function here

            // Validate fields
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber) ||
                string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            // Insert the new account into the database
            string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO AccountUser (FirstName, LastName, Email, PhoneNumber, PasswordHash, AccountType) " +
                                   "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @AccountType)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created successfully!");
                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
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

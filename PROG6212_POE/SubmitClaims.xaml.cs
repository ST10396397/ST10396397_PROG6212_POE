using Microsoft.Win32;
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
using System.Globalization;

namespace PROG6212_POE
{
    /// <summary>
    /// Interaction logic for SubmitClaims.xaml
    /// </summary>
    public partial class SubmitClaims : Window
    {
        // Declare selectedFilePath as a member variable
        private string? selectedFilePath;

        public SubmitClaims()
        {
            InitializeComponent();
        }

        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog for selecting a document
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Document files (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx|All files (*.*)|*.*"; // File type filters
            openFileDialog.Title = "Select Supporting Document";

            if (openFileDialog.ShowDialog() == true)
            {
                // Store the selected file path
                selectedFilePath = openFileDialog.FileName;
                // Update UI to display selected file name
                txtSelectedFile.Text = System.IO.Path.GetFileName(selectedFilePath);
            }
        }

        private void CalculateTotalAmount(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtNoOfSessions.Text, out int noOfSessions) && decimal.TryParse(txtHourlyRate.Text, out decimal hourlyRate))
            {
                decimal totalAmount = noOfSessions * hourlyRate;
                txtTotalAmount.Text = $"R {totalAmount:F2}"; // Display in currency format
            }
            else
            {
                txtTotalAmount.Text = "Invalid input";
            }
        }

        private void SubmitClaim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the claim data from the form
                string classTaught = txtClassTaught.Text;
                int noOfSessions = int.Parse(txtNoOfSessions.Text);
                decimal hourlyRate = decimal.Parse(txtHourlyRate.Text);
                string notes = txtNotes.Text;

                // Ensure all fields are filled
                if (string.IsNullOrEmpty(classTaught) || noOfSessions <= 0 || hourlyRate <= 0 || string.IsNullOrEmpty(selectedFilePath))
                {
                    MessageBox.Show("Please fill in all the required fields and upload a document.");
                    return;
                }

                decimal totalAmount = noOfSessions * hourlyRate;

                // Use @ to avoid needing double backslashes in the connection string
                string connectionString = @"Data Source=labG9AEB3\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True"; 
                // Insert claim into the database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Insert the claim into the Claims table
                    string query = "INSERT INTO Claims (AccountUserID, ClassTaught, NoOfSessions, HourlyRatePerSession, SupportingDocumentPath, ClaimStatus, ClaimTotalAmount) " +
                                   "VALUES (@AccountUserID, @ClassTaught, @NoOfSessions, @HourlyRatePerSession, @SupportingDocumentPath, 'Pending' @ClaimTotalAmount)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountUserID", GetCurrentLecturerID()); // Replace with actual lecturer's ID
                        cmd.Parameters.AddWithValue("@ClassTaught", classTaught);
                        cmd.Parameters.AddWithValue("@NoOfSessions", noOfSessions);
                        cmd.Parameters.AddWithValue("@HourlyRatePerSession", hourlyRate);
                        cmd.Parameters.AddWithValue("@SupportingDocumentPath", selectedFilePath); // Store the file path
                        cmd.Parameters.AddWithValue("@ClaimTotalAmount", totalAmount);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Claim submitted successfully!");

                    // Optionally, reset the form for a new claim submission
                    ResetForm();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ResetForm()
        {
            txtClassTaught.Text = "";
            txtNoOfSessions.Text = "";
            txtHourlyRate.Text = "";
            txtNotes.Text = "";
            txtSelectedFile.Text = "";
            txtTotalAmount.Text = "";
            selectedFilePath = string.Empty;
        }

        
        private int GetCurrentLecturerID()
        {
            return 1; 
        }

       
    }
}

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

namespace PROG6212_POE
{
    /// <summary>
    /// Interaction logic for ViewClaims.xaml
    /// </summary>
    public partial class ViewClaims : Window
    {
        public ViewClaims()
        {
            InitializeComponent();
            LoadClaims();
        }

        // Method to load and display the claims
        private void LoadClaims()
        {
            // Clear previous claims if any
            ClaimsStackPanel.Children.Clear();

            // Connection string to the database
            string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True"; // Replace with actual connection string

            try
            {
                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();  // Ensure connection is open before executing any queries

                    // SQL query to retrieve the claims for the current lecturer
                    string query = "SELECT ClaimsID, ClassTaught, NoOfSessions, HourlyRatePerSession, SupportingDocumentPath, ClaimStatus " +
                                   "FROM Claims WHERE AccountUserID = @AccountUserID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Use the actual lecturer ID (for now it's a placeholder method)
                        cmd.Parameters.AddWithValue("@AccountUserID", GetCurrentLecturerID());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a Border for each claim
                                Border claimBorder = new Border
                                {
                                    BorderBrush = Brushes.Gray,
                                    BorderThickness = new Thickness(1),
                                    Padding = new Thickness(10),
                                    Margin = new Thickness(0, 0, 0, 10)
                                };

                                StackPanel claimPanel = new StackPanel();

                                // Display Claim Details
                                TextBlock classTaughtText = new TextBlock
                                {
                                    Text = $"Class Taught: {reader["ClassTaught"]}",
                                    FontWeight = FontWeights.Bold
                                };

                                TextBlock sessionsText = new TextBlock
                                {
                                    Text = $"Number of Sessions: {reader["NoOfSessions"]}"
                                };

                                TextBlock hourlyRateText = new TextBlock
                                {
                                    Text = $"Hourly Rate: {reader["HourlyRatePerSession"]:C}" // Currency format
                                };

                                TextBlock totalAmountText = new TextBlock
                                {
                                    Text = $"Total Amount: {Convert.ToInt32(reader["NoOfSessions"]) * Convert.ToDecimal(reader["HourlyRatePerSession"]):C}"
                                };

                                TextBlock documentText = new TextBlock
                                {
                                    Text = $"Document: {System.IO.Path.GetFileName(reader["SupportingDocumentPath"].ToString())}"
                                };

                                TextBlock statusText = new TextBlock
                                {
                                    Text = $"Status: {reader["ClaimStatus"]}",
                                    Foreground = (reader["ClaimStatus"].ToString() == "Approved") ? Brushes.Green :
                                                 (reader["ClaimStatus"].ToString() == "Rejected") ? Brushes.Red : Brushes.Orange
                                };

                                // Add all the TextBlocks to the claimPanel
                                claimPanel.Children.Add(classTaughtText);
                                claimPanel.Children.Add(sessionsText);
                                claimPanel.Children.Add(hourlyRateText);
                                claimPanel.Children.Add(totalAmountText);
                                claimPanel.Children.Add(documentText);
                                claimPanel.Children.Add(statusText);

                                // Add the claimPanel to the Border
                                claimBorder.Child = claimPanel;

                                // Finally, add the Border (which contains the claim details) to the StackPanel
                                ClaimsStackPanel.Children.Add(claimBorder);
                            }
                        }
                    }
                }

            }

            catch (SqlException sqlEx)
            {
                MessageBox.Show($"A database error occurred: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        // Placeholder method for getting the current lecturer's ID. Replace with actual logic.
        private int GetCurrentLecturerID()
        {
            return 1; // Static ID for example
        }
    }
}




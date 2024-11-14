using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    /// Interaction logic for CoordinatorView.xaml
    /// </summary>
    public partial class CoordinatorView : Window
    {
        public CoordinatorView()
        {
            InitializeComponent();
            LoadClaimsForApproval();
        }

        // Load pending claims and allow the coordinator to approve/reject them
        private void LoadClaimsForApproval()
        {
            string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True"; 

            List<Claim> claims = new List<Claim>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ClaimsID, ClassTaught, NoOfSessions, HourlyRatePerSession, ClaimStatus FROM Claims WHERE ClaimStatus = 'Pending'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var claim = new Claim
                            {
                                ClaimsID = (int)reader["ClaimsID"],
                                ClassTaught = reader["ClassTaught"]?.ToString() ?? string.Empty, // Safe access with null check
                                NoOfSessions = (int)reader["NoOfSessions"],
                                HourlyRatePerSession = (decimal)reader["HourlyRatePerSession"],
                                ClaimStatus = reader["ClaimStatus"]?.ToString() ?? string.Empty, // Safe access with null check
                                ClaimTotalAmount = (int)reader["NoOfSessions"] * (decimal)reader["HourlyRatePerSession"]
                            };

                            // Apply automated verification criteria
                            if (VerifyClaimCriteria(claim))
                            {
                                // Automatically approve claims that meet the criteria
                                UpdateClaimStatus(claim.ClaimsID, "Approved");
                                claim.ClaimStatus = "Approved";
                            }
                            else
                            {
                                // Automatically reject claims that fail to meet criteria
                                UpdateClaimStatus(claim.ClaimsID, "Rejected");
                                claim.ClaimStatus = "Rejected";
                            }

                            claims.Add(claim);
                        }
                    }
                }
            }

            ClaimsListView.ItemsSource = claims;
        }

        // Method to verify if a claim meets predefined criteria
        private bool VerifyClaimCriteria(Claim claim)
        {
            // Define your criteria; for example:
            int maxSessions = 20;
            decimal maxHourlyRate = 500m;

            // Check if claim meets the criteria
            return claim.NoOfSessions <= maxSessions && claim.HourlyRatePerSession <= maxHourlyRate;
        }

        // Method to update claim status
        private void UpdateClaimStatus(int claimID, string status)
        {
            string connectionString = "Data Source=labG9AEB3\\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True"; // Replace with actual connection string

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Claims SET ClaimStatus = @Status WHERE ClaimsID = @ClaimsID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ClaimsID", claimID);
                    cmd.ExecuteNonQuery();
                }
            }

            // Reload claims after updating
            LoadClaimsForApproval();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimsID, "Approved");
            }
            else
            {
                MessageBox.Show("Please select a claim to approve.");
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimsID, "Rejected");
            }
            else
            {
                MessageBox.Show("Please select a claim to reject.");
            }
        }

        private void PendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimsID, "Pending");
            }
            else
            {
                MessageBox.Show("Please select a claim to set as pending.");
            }
        }
    }

    // Claim class to represent each claim
    public class Claim
    {
        public int ClaimsID { get; set; }
        public string? ClassTaught { get; set; } // Make this property nullable
        public int NoOfSessions { get; set; }
        public decimal HourlyRatePerSession { get; set; }
        public string? ClaimStatus { get; set; } // Make this property nullable
        public decimal ClaimTotalAmount { get; set; }
    }
}


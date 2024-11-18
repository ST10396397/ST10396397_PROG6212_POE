using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace PROG6212_POE
{
    /// <summary>
    /// Interaction logic for HRView.xaml
    /// </summary>
    public partial class HRView : Window
    {
        public HRView()
        {
            InitializeComponent();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = @"Data Source=labG9AEB3\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";
                string query = "SELECT AccountUserID, ClassTaught, NoOfSessions, HourlyRatePerSession, ClaimTotalAmount " +
                               "FROM Claims WHERE ClaimStatus = 'Approved'";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable approvedClaims = new DataTable();
                    adapter.Fill(approvedClaims);

                    // Generate and save the report (you could export to CSV, PDF, etc.)
                    string reportPath = "ApprovedClaimsReport.txt";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(reportPath))
                    {
                        file.WriteLine("Approved Claims Report");
                        file.WriteLine("LecturerID\tClassTaught\tSessions\tHourlyRate\tTotalAmount");

                        foreach (DataRow row in approvedClaims.Rows)
                        {
                            file.WriteLine($"{row["AccountUserID"]}\t{row["ClassTaught"]}\t{row["NoOfSessions"]}\t" +
                                           $"{row["HourlyRatePerSession"]:C}\t{row["ClaimTotalAmount"]:C}");
                        }
                    }

                    MessageBox.Show("Claims report generated successfully!");
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

        private void SearchLecturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtSearchEmail.Text;

                if (string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Please enter an email to search.");
                    return;
                }

                string connectionString = @"Data Source=labG9AEB3\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT FullName, Email, PhoneNumber FROM Lecturer WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFullName.Text = reader["FullName"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Lecturer not found.");
                                ResetLecturerFields();
                            }
                        }
                    }
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

        private void UpdateLecturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text;
                string fullName = txtFullName.Text;
                string phoneNumber = txtPhoneNumber.Text;

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                string connectionString = @"Data Source=labG9AEB3\SQLEXPRESS;Initial Catalog=PROG6212POE;Integrated Security=True";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Lecturer SET FullName = @FullName, PhoneNumber = @PhoneNumber WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Lecturer information updated successfully!");
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

        private void ResetLecturerFields()
        {
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
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
    }
}

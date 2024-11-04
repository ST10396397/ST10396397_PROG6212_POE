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
    /// Interaction logic for LecturerView.xaml
    /// </summary>
    public partial class LecturerView : Window
    {
        public LecturerView()
        {
            InitializeComponent();
        }

        private void ViewClaims_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the View Claims window
            ViewClaims viewClaims = new ViewClaims();
            viewClaims.Show();
            
        }

        private void SubmitClaims_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Submit Claims window
        SubmitClaims submitClaims = new SubmitClaims();
            submitClaims.Show();
            
        }
    }
}

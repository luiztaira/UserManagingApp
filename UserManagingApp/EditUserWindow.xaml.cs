using System.Windows;
using UserManagingApp.Models;

namespace UserManagingApp
{
    public partial class EditUserWindow : Window
    {
        private readonly User _user;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            NameTextBox.Text = user.Name;
            EmailTextBox.Text = user.Email;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _user.Name = NameTextBox.Text;
            _user.Email = EmailTextBox.Text;
            DialogResult = true;
        }
    }
}
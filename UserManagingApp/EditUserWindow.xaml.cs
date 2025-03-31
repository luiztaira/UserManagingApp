using System.Windows;
using System.Windows.Controls;
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

            // Initialize fields
            NameTextBox.Text = user.Name;
            EmailTextBox.Text = user.Email;
            PhoneNumberTextBox.Text = user.PhoneNumber;

            // Set privilege selection
            foreach (ComboBoxItem item in PrivilegesComboBox.Items)
            {
                if (item.Content.ToString() == user.Privileges)
                {
                    PrivilegesComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Name and Email are required");
                return;
            }

            _user.Name = NameTextBox.Text.Trim();
            _user.Email = EmailTextBox.Text.Trim();
            _user.PhoneNumber = PhoneNumberTextBox.Text.Trim();
            _user.Privileges = (PrivilegesComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _user.LastUpdatedAt = DateTime.UtcNow;

            DialogResult = true;
            Close();
        }
    }
}
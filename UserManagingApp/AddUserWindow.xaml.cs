using System.Windows;
using System.Windows.Controls;
using UserManagingApp.Models;

namespace UserManagingApp
{
    public partial class AddUserWindow : Window
    {
        private readonly UserManagerContext _context;
        public User? NewUser { get; private set; }

        public AddUserWindow(UserManagerContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter both name and email");
                return;
            }

            NewUser = new User
            {
                Name = NameTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                PhoneNumber = PhoneNumberTextBox.Text.Trim(),
                Privileges = (PrivilegesComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(NewUser);
                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}");
            }
        }
    }
}
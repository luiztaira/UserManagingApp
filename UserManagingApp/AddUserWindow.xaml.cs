using System.Windows;
using System.Windows.Controls;
using UserManagingApp.Models;
using UserManagingApp.Services;

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
            var name = NameTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();

            if (UserValidator.isNameBlank(name, out var NameError))
            {
                MessageBox.Show(
                    NameError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }

            if (UserValidator.isEmailBlank(email, out var EmailError)) 
            {
                MessageBox.Show(
                    EmailError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }


            if (UserValidator.IsNameDuplicate(_context, name, out var dupNameError))
            {
                MessageBox.Show(
                    dupNameError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (UserValidator.IsEmailDuplicate(_context, email, out var dupEmailError))
            {
                MessageBox.Show(
                    dupEmailError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                MessageBox.Show($"ユーザーを追加できませんでした：{ex.Message}");
            }
        }
    }
}
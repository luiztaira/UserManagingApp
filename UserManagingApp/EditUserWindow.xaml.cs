using System.Windows;
using Microsoft.EntityFrameworkCore;
using UserManagingApp.Models;
using UserManagingApp.Services;

namespace UserManagingApp
{
    public partial class EditUserWindow : Window
    {
        private readonly User _user;
        private readonly UserManagerContext _context;

        public EditUserWindow(User user, UserManagerContext context)
        {
            InitializeComponent();
            _user = user;
            _context = context;

            // Initialize fields
            NameTextBox.Text = user.Name;
            EmailTextBox.Text = user.Email;
            PhoneNumberTextBox.Text = user.PhoneNumber;

            // Initialize privilege selection
            var privilegeOptions = new Dictionary<string, string>
            {
                { "Admin", "管理者" },
                { "User", "ユーザー" },
                { "Guest", "ゲスト" }
            };

            PrivilegesComboBox.ItemsSource = privilegeOptions;
            PrivilegesComboBox.DisplayMemberPath = "Value";
            PrivilegesComboBox.SelectedValuePath = "Key";
            PrivilegesComboBox.SelectedValue = _user.Privileges;
        }

        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var newName = NameTextBox.Text.Trim();
            var newEmail = EmailTextBox.Text.Trim();

            // Validate inputs
            if (UserValidator.isNameBlank(newName, out var nameError))
            {
                MessageBox.Show(
                    nameError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (UserValidator.isEmailBlank(newEmail, out var emailError))
            {
                MessageBox.Show(
                    emailError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (newName != _user.Name && UserValidator.IsNameDuplicate(_context, newName, out var dupNameError))
            {
                MessageBox.Show(
                    dupNameError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (newEmail != _user.Email && UserValidator.IsEmailDuplicate(_context, newEmail, out var dupEmailError))
            {
                MessageBox.Show(
                    dupEmailError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _user.Name = NameTextBox.Text.Trim();
            _user.Email = EmailTextBox.Text.Trim();
            _user.PhoneNumber = PhoneNumberTextBox.Text.Trim();
            _user.Privileges = PrivilegesComboBox.SelectedValue?.ToString();
            _user.LastUpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                DialogResult = true;
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"保存に失敗しました：{ex.InnerException?.Message}");
            }
        }
    }
}
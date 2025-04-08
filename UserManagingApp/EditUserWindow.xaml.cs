using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
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
            foreach (ComboBoxItem item in PrivilegesComboBox.Items)
            {
                if (item.Content.ToString() == user.Privileges)
                {
                    PrivilegesComboBox.SelectedItem = item;
                    break;
                }
            }
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

            if (UserValidator.IsNameDuplicate(_context, newName, out var dupNameError))
            {
                MessageBox.Show(
                    dupNameError,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (UserValidator.IsEmailDuplicate(_context, newEmail, out var dupEmailError))
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
            _user.Privileges = (PrivilegesComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
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
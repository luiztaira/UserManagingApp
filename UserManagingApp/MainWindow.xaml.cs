using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UserManagingApp.Models;

namespace UserManagingApp
{
    public partial class MainWindow : Window
    {
        private readonly UserManagerContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new UserManagerContext();
            InitializeComponents();
            RefreshUserList();
        }

        private void InitializeComponents()
        {
            // Initialize placeholder
            UpdatePlaceholderVisibility();
            SearchTextBox.TextChanged += (s, e) => UpdatePlaceholderVisibility();

            // Setup privileges filter
            PrivilegesCombo.ItemsSource = new Dictionary<string, string>
            {
                { "", "All Privileges" },
                { "admin", "Admin" },
                { "user", "User" },
                { "guest", "Guest" }
            };
            PrivilegesCombo.SelectedIndex = 0;

            // Set default date range
            FromDatePicker.SelectedDate = DateTime.Today.AddDays(-30);
            ToDatePicker.SelectedDate = DateTime.Today;
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = _context.Users.AsNoTracking().AsQueryable();
                string searchTerm = SearchTextBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(u =>
                        EF.Functions.ILike(u.Name, $"%{searchTerm}%") ||
                        EF.Functions.ILike(u.Email, $"%{searchTerm}%") ||
                        (u.PhoneNumber != null && EF.Functions.ILike(u.PhoneNumber, $"%{searchTerm}%")) ||
                        (u.Privileges != null && EF.Functions.ILike(u.Privileges, $"%{searchTerm}%")));
                }

                // Convert date picker values to UTC
                if (FromDatePicker.SelectedDate != null)
                {
                    var fromDateUtc = FromDatePicker.SelectedDate.Value.ToUniversalTime();
                    query = query.Where(u => u.CreatedAt >= fromDateUtc);
                }

                if (ToDatePicker.SelectedDate != null)
                {
                    var toDateUtc = ToDatePicker.SelectedDate.Value.AddDays(1).ToUniversalTime();
                    query = query.Where(u => u.CreatedAt <= toDateUtc);
                }

                // Privileges filter
                if (PrivilegesCombo.SelectedValue is string selectedPrivilege && !string.IsNullOrEmpty(selectedPrivilege))
                {
                    query = query.Where(u => u.Privileges == selectedPrivilege);
                }

                UserListBox.ItemsSource = query
                    .OrderBy(u => u.Name)
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        PhoneNumber = u.PhoneNumber ?? "-",
                        Privileges = u.Privileges ?? "-",
                        CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd")
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshUserList()
        {
            UserListBox.ItemsSource = _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    PhoneNumber = u.PhoneNumber ?? "-",
                    Privileges = u.Privileges ?? "-",
                    CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd")
                })
                .ToList();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            FromDatePicker.SelectedDate = DateTime.Today.AddDays(-30);
            ToDatePicker.SelectedDate = DateTime.Today;
            PrivilegesCombo.SelectedIndex = 0;
            RefreshUserList();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddUserWindow(_context);
            if (addWindow.ShowDialog() == true && addWindow.NewUser != null)
            {
                RefreshUserList();
                MessageBox.Show($"User '{addWindow.NewUser.Name}' added successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToEdit = _context.Users.Find(selectedItem.Id);

            if (userToEdit != null)
            {
                var editWindow = new EditUserWindow(userToEdit);
                if (editWindow.ShowDialog() == true)
                {
                    userToEdit.LastUpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    RefreshUserList();
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToDelete = _context.Users.Find(selectedItem.Id);

            if (userToDelete != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete user '{userToDelete.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Users.Remove(userToDelete);
                        _context.SaveChanges();
                        RefreshUserList();
                        MessageBox.Show("User deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete user: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
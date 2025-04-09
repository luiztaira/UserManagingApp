using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UserManagingApp.Models;
using LinqExpression = System.Linq.Expressions.Expression;

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
                { "", "全ての権限" },
                { "Admin", "管理者" },
                { "User", "ユーザー" },
                { "Guest", "ゲスト" }
            };
            PrivilegesCombo.SelectedIndex = 0;

            // Set default date range
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            SearchButton.IsEnabled = NameFilterCheckBox.IsChecked == true ||
                                     EmailFilterCheckBox.IsChecked == true ||
                                     PhoneNumberFilterCheckBox.IsChecked == true ||
                                     DateFilterCheckBox.IsChecked == true ||
                                     PrivilegesFilterCheckBox.IsChecked == true;
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

                if (searchTerm != null && searchTerm.Length > 0)
                {
                    var conditions = new List<Expression<Func<User, bool>>>();
                    if (NameFilterCheckBox.IsChecked == true)
                    {
                        conditions.Add(u => EF.Functions.ILike(u.Name, $"%{searchTerm}%"));
                    }
                    if (EmailFilterCheckBox.IsChecked == true)
                    {
                        conditions.Add(u => EF.Functions.ILike(u.Email, $"%{searchTerm}%"));
                    }
                    if (PhoneNumberFilterCheckBox.IsChecked == true)
                    {
                        conditions.Add(u => u.PhoneNumber != null && EF.Functions.ILike(u.PhoneNumber, $"%{searchTerm}%"));
                    }

                    if (conditions.Any())
                    {
                        var combinedCondition = conditions.Aggregate((c1, c2) => LinqExpression.Lambda<Func<User, bool>>(
                            LinqExpression.OrElse(c1.Body, c2.Body),
                            c1.Parameters[0]));

                        query = query.Where(combinedCondition);
                    }
                }

                // Privileges ComboBox filter
                if (PrivilegesFilterCheckBox.IsChecked == true && PrivilegesCombo.SelectedValue is string selectedPrivilege && !string.IsNullOrEmpty(selectedPrivilege))
                {
                    query = query.Where(u => u.Privileges != null &&
                           u.Privileges.ToLower() == selectedPrivilege.ToLower());
                }

                // Date filter
                if (DateFilterCheckBox.IsChecked == true && FromDatePicker.SelectedDate != null && ToDatePicker.SelectedDate != null)
                {
                    var fromDateUtc = FromDatePicker.SelectedDate.Value.ToUniversalTime();
                    var toDateUtc = ToDatePicker.SelectedDate.Value.AddDays(1).ToUniversalTime();
                    query = query.Where(u => u.CreatedAt >= fromDateUtc && u.CreatedAt <= toDateUtc);
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
                MessageBox.Show($"検索エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
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
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            PrivilegesCombo.SelectedIndex = 0;

            NameFilterCheckBox.IsChecked = false;
            EmailFilterCheckBox.IsChecked = false;
            PhoneNumberFilterCheckBox.IsChecked = false;
            DateFilterCheckBox.IsChecked = false;
            PrivilegesFilterCheckBox.IsChecked = false;

            RefreshUserList();
        }


        // Window size change handler
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            // Optional: Add any dynamic adjustments here
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddUserWindow(_context);
            if (addWindow.ShowDialog() == true && addWindow.NewUser != null)
            {
                RefreshUserList();
                MessageBox.Show($"ユーザー「'{addWindow.NewUser.Name}'」が追加されました",
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                MessageBox.Show("編集したいユーザーを選択してください",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToEdit = _context.Users.Find(selectedItem.Id);

            if (userToEdit != null)
            {
                var editWindow = new EditUserWindow(userToEdit, _context);
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
                MessageBox.Show("削除したいユーザーを選択してください",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToDelete = _context.Users.Find(selectedItem.Id);

            if (userToDelete != null)
            {
                var result = MessageBox.Show($"ユーザー「'{userToDelete.Name}'」を削除します。よろしいですか。",
                    "",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Users.Remove(userToDelete);
                        _context.SaveChanges();
                        RefreshUserList();
                        MessageBox.Show("ユーザーを削除しました。",
                            "",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ユーザーを削除できませんでした: {ex.Message}",
                            "エラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
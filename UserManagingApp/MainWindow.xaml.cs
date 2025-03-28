using System.Text.RegularExpressions;
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
            _context.Database.EnsureCreated();
           //検索ボックスのplaceholderを起動
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // placeholder有無ハンドラー
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        //検索ボタンハンドラー
        private void SearchButton_Click(object sender, RoutedEventArgs e)
{
    string pattern = SearchTextBox.Text;

    if (string.IsNullOrWhiteSpace(pattern))
    {
        MessageBox.Show("Please enter a valid search.");
        return;
    }

    try
    {
        // Entity Frameworkでユーザーをゲット
        var users = _context.Users
            .AsNoTracking()
            .ToList();

        // メモリー内にregexフィルターをかけ、リストを作成する。
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        var filteredUsers = users
            .Select(u => new {
                User = u,
                NameMatch = regex.Match(u.Name),
                EmailMatch = regex.Match(u.Email)
            })
            .Where(x => x.NameMatch.Success || x.EmailMatch.Success)
            .OrderBy(x => x.NameMatch.Success ? x.NameMatch.Index : int.MaxValue)
            .ThenBy(x => x.EmailMatch.Success ? x.EmailMatch.Index : int.MaxValue)
            .ThenBy(x => x.User.Name)
            .Select(x => new { x.User.Id, x.User.Name, x.User.Email })
            .ToList();

        UserListBox.ItemsSource = filteredUsers;
    }
    catch (ArgumentException)
    {
        MessageBox.Show("Invalid format.");
    }
}

        //エンターキーで検索
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToDelete = _context.Users.Find(selectedItem.Id);

            if (userToDelete != null)
            {
                var result = MessageBox.Show($"Delete user {userToDelete.Name}?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Users.Remove(userToDelete);
                    _context.SaveChanges();
                    SearchButton_Click(sender, e); // GUI表示を更新
                    MessageBox.Show("User deleted successfully.");
                }
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            dynamic selectedItem = UserListBox.SelectedItem;
            var userToEdit = _context.Users.Find(selectedItem.Id);

            if (userToEdit != null)
            {
                var editWindow = new EditUserWindow(userToEdit);
                if (editWindow.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    SearchButton_Click(sender, e); // GUI表示を更新
                }
            }
        }
    }
}
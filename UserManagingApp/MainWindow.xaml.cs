using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace UserManagingApp
{
    public partial class MainWindow : Window
    {
        private readonly UserManagerContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new UserManagerContext();

            // placeholder を起動
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // placeholder有無ハンドラー
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // サーチボタンハンドラー
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
                    .AsNoTracking() // メモリーの使用を減らし、パフォーマンスを上げる。Readの時に使うといい。
                    .ToList();

                // メモリー内にregexフィルターをかける。
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                var filteredUsers = users
                    .Where(u => regex.IsMatch(u.Name) || regex.IsMatch(u.Email))
                    .ToList();

                // マッチしたユーザーをUserListBoxに表示する
                UserListBox.ItemsSource = filteredUsers;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid format.");
            }
        }
    }
}

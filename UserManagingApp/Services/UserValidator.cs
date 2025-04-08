using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserManagingApp.Services
{
    class UserValidator
    {
        public static bool isNameBlank(string name, out string? errorMessage) 
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "名前は必要項目です。";
                return true;
            }
            errorMessage = null;
            return false;
        }
        public static bool isEmailBlank(string email, out string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "メールアドレスは必要項目です。";
                return true;
            }
            errorMessage = null;
            return false;
        }
        public static bool IsNameDuplicate(UserManagerContext context, string name, out string? errorMessage) 
        {
            if (context.Users.Any(u => u.Name == name))
            {
                errorMessage = $"名前「{name}」は既に登録されています。";
                return true;
            }
            errorMessage = null;
            return false;
        }
        public static bool IsEmailDuplicate(UserManagerContext context, string email, out string? errorMessage)
        {
            if (context.Users.Any(u => u.Email == email)) 
            {
                errorMessage = $"メールアドレス「{email}」は既に登録されています。";
                return true;
            }
            errorMessage = null;
            return false;
        }
    }
}

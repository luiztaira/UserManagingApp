using System;
using System.Windows;
using DotNetEnv;
using System.Globalization;

namespace UserManagingApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Env.Load();

            SetJapaneseCulture();
        }

        private void SetJapaneseCulture()
        {
            var cultureInfo = new CultureInfo("ja-JP");
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            cultureInfo.DateTimeFormat.LongDatePattern = "yyyy/MM/dd";
            cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }

}
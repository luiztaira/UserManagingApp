using System;
using System.Windows;
using DotNetEnv;

namespace UserManagingApp
{
    public partial class App : Application
    {
        // Application entry point
        public App()
        {
            // Load environment variables from the .env file
            Env.Load();
        }
    }
}

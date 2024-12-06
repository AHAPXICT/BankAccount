using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BankAccount.ViewModels;
using BankAccount.Views;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace BankAccount
{
    public partial class App : Application
    {
        /// <summary> Все имеющиеся аккаунты</summary>
        internal static List<BankAccount> accounts = new List<BankAccount>();
        /// <summary> MainWindow </summary>
        public static MainWindow mv;
        /// <summary> Домашняя страница аккаунта</summary>
        public static HomePage hp = new HomePage();
        /// <summary> Страница входа в аккаунт/перехода на создание аккаунта </summary>
        public static LoginPage lp = new LoginPage();
        /// <summary> Страница создания нового аккаунта</summary>
        public static CreateAccountPage cp = new CreateAccountPage();
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                mv = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow = mv;

                //Вместо предыдущего кода было это
                //desktop.MainWindow = new MainWindow
                //{
                //    DataContext = new MainViewModel(),
                //};
            }
            base.OnFrameworkInitializationCompleted();
        }

    }
}
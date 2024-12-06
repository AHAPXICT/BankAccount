using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using BankAccount.Views;
using System.Reactive;
using MsBox.Avalonia;
using BankAccount.ViewModels;

namespace BankAccount;
/// <summary>
/// Страница входа в аккаунт/перехода на создания аккаунта
/// </summary>
public partial class LoginPage : UserControl
{
    public LoginPage()
    {
        InitializeComponent();
    }
    private void OnHomePage(object sender, RoutedEventArgs e)
    {
        uint accNumber;
        if (!uint.TryParse(loginBox.Text, out accNumber))
        {
            MessageBoxManager.GetMessageBoxStandard("Enter the number", "Enter the account number").ShowAsync();
            return; 
        }

        foreach (BankAccount acc in App.accounts)
        {
            if (acc.AccountNumber == accNumber)
            {
                App.mv.ContentArea.Content = App.hp;
                return;
            }
        }

        MessageBoxManager.GetMessageBoxStandard("Error", "This account doesn't exist").ShowAsync();
    }

    private void OnCreateAccountPage(object sender, RoutedEventArgs e)
    {
        App.mv.ContentArea.Content = App.cp;
    }

}
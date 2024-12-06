using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BankAccount.ViewModels;
using Avalonia.Interactivity;

namespace BankAccount;

public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();
    }
    public void BackClick(object sender, RoutedEventArgs e) => App.mv.ContentArea.Content = App.lp;
}
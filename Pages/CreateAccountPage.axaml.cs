using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.ComponentModel.DataAnnotations;
using System;
using ReactiveUI;
using System.ComponentModel;
using BankAccount.ViewModels;
using Tmds.DBus.Protocol;


namespace BankAccount;

public partial class CreateAccountPage : UserControl
{
    public CreateAccountPage()
    {
        InitializeComponent();
        BirthSelector.MaxYear = DateTimeOffset.Now.AddYears(-14);
    }
    private void BackClick(object sender, RoutedEventArgs e)
    {
        App.mv.ContentArea.Content = App.lp;
    }
}

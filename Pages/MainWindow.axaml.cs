using Avalonia.Controls;
using BankAccount.ViewModels;
using System.Runtime.CompilerServices;
using Avalonia.Interactivity;

namespace BankAccount.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentArea.Content = App.lp;
        }
    }
}
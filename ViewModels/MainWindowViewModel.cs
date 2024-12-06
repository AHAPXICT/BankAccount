using ReactiveUI;
using System;

namespace BankAccount.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public CreateAccountVM CreateAccountVM { get; }
            = new CreateAccountVM();
        public HomePageVm HomePageVM { get; }
            = new HomePageVm();
    }
}

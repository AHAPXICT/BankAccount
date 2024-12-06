using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Remote.Protocol.Viewport;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia.Data;
using System.Xml.Linq;

namespace BankAccount.ViewModels
{
    public class HomePageVm : ViewModelBase, INotifyDataErrorInfo
    {
        public HomePageVm()
        {

        }
        #region verifications
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public bool HasErrors => errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values.SelectMany(static errors => errors);
            }
            if (this.errors.TryGetValue(propertyName!, out List<ValidationResult>? result))
            {
                return result;
            }

            return Array.Empty<ValidationResult>();
        }

        private Dictionary<string, List<ValidationResult>> errors = new Dictionary<string, List<ValidationResult>>();

        protected void ClearErrors(string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                errors.Clear();
            }
            else
            {
                errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            this.RaisePropertyChanged(nameof(HasErrors));
        }
        protected void AddError(string propertyName, string errorMessage)
        {
            if (!errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new List<ValidationResult>();
                errors.Add(propertyName, propertyErrors);
            }

            propertyErrors.Add(new ValidationResult(errorMessage));

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            this.RaisePropertyChanged(nameof(HasErrors));
        }
        #endregion
        public void LoginInAccount()
        {
            uint accNumber;
            if (!uint.TryParse(App.lp.loginBox.Text, out accNumber))
            {
                MessageBoxManager.GetMessageBoxStandard("Enter the number", "Enter the account number").ShowAsync();
                return;
            }

            foreach (BankAccount acc in App.accounts)
            {
                if (acc.AccountNumber == accNumber)
                {
                    App.mv.ContentArea.Content = App.hp;
                    account = acc;
                    App.hp.AmountInAccountTextBlock.Text = account.SumInDeposit.ToString();
                    DateEndOfAccount = DateOnly.FromDateTime(acc.EndOfDeposit().Date);
                    DaysToEndAccount = (acc.EndOfDeposit() - DateTimeOffset.Now).Days;
                    return;
                }
            }

            MessageBoxManager.GetMessageBoxStandard("Error", "This account doesn't exist").ShowAsync();
        }

        BankAccount account;
        BankAccount recipientAccount;

        private string? withdrawAmount;
        private string? replenishAmount;
        private string? recipientNumber;
        private string? amountToTransfer;

        private decimal withdrawAmountDecimal;
        private decimal replenishAmountDecimal;
        private decimal amountToTransferDecimal;
        private uint recipientNumberUint;

        private DateOnly dateEndOfAccount;
        private int daysToEndAccount;

        #region properties
        public string? WithdrawAmount
        {
            get { return withdrawAmount; }
            set { this.RaiseAndSetIfChanged(ref withdrawAmount, value); }
        }
        public string? RecipientNumber
        {
            get { return recipientNumber; }
            set { this.RaiseAndSetIfChanged(ref recipientNumber, value); }
        }
        public string? ReplenishAmount
        {
            get { return replenishAmount; }
            set { this.RaiseAndSetIfChanged(ref replenishAmount, value); }
        }
        public string? AmountToTransfer
        {
            get { return amountToTransfer; }
            set { this.RaiseAndSetIfChanged(ref amountToTransfer, value); }
        }
        public decimal? AmountInAccount
        {
            get { return account?.SumInDeposit; }
            set { this.RaiseAndSetIfChanged(ref account.SumInDeposit, value); }
        }
        public DateOnly DateEndOfAccount
        {
            get { return dateEndOfAccount; }
            set { this.RaiseAndSetIfChanged(ref dateEndOfAccount, value); }
        }
        public int DaysToEndAccount
        {
            get { return daysToEndAccount; }
            set { this.RaiseAndSetIfChanged(ref daysToEndAccount, value); }
        }
        #endregion
        #region Validates
        private bool ValidateWithdraw(string field, string nm)
        {
            if (!ValidateIsNullOrEmpty(field, nm)) 
                return false;
            if (!ValidateDecimalTryParse(field, nm, out withdrawAmountDecimal))
                return false;
            if (withdrawAmountDecimal > AmountInAccount)
            {
                AddError(nm, "Insufficient funds in account");
                return false;
            }
            if (withdrawAmountDecimal < 1)
            {
                AddError(nm, "Minimal withdrawal amount is 1");
                return false;
            }
            return true;
        }
        private bool ValidateReplenishment(string field, string nm)
        {
            if (!ValidateIsNullOrEmpty(field, nm)) 
                return false;
            if (!ValidateDecimalTryParse(field, nm, out replenishAmountDecimal))
                return false;
            if (replenishAmountDecimal <= 0)
            {
                AddError(nm, "Can't replenish zero or less");
                return false;
            }
            return true;
        }
        private bool ValidateAmountToTransfer(string field, string nm)
        {
            if (!ValidateIsNullOrEmpty(field, nm))
                return false;
            if (!ValidateDecimalTryParse(field, nm, out amountToTransferDecimal))
                return false;
            if (amountToTransferDecimal > AmountInAccount)
            {
                AddError(nm, "Insufficient funds in account");
                return false;
            }
            if (amountToTransferDecimal < 1)
            {
                AddError(nm, "Minimal transfer amount is 1");
                return false;
            }
            return true;
        }
        private bool ValidateRecipientNumber(string field, string nm)
        {
            if (!ValidateIsNullOrEmpty(field, nm))
                return false;
            if (!uint.TryParse(field, out recipientNumberUint))
            {
                AddError(nm, "Enter the number");
                return false;
            }
            foreach (BankAccount acc in App.accounts)
            {
                if (acc.AccountNumber == recipientNumberUint)
                {
                    if (acc.AccountNumber == account.AccountNumber)
                    {
                        AddError(nm, "Your number can't be recipient");
                        return false;
                    }
                    recipientAccount = acc;
                    return true;
                }
            }
            AddError(nm, "No account with this number");
            return false;
        }
        private bool ValidateIsNullOrEmpty(string field, string nm)
        {
            ClearErrors(nm);
            if (string.IsNullOrEmpty(field))
            {
                AddError(nm, "This field is required");
                return false;
            }
            return true;
        }
        private bool ValidateDecimalTryParse(string field, string nm, out decimal outParam)
        {
            if (!decimal.TryParse(field, out outParam))
            {
                AddError(nm, "Enter the number");
                return false;
            }
            return true;
        }
        #endregion
        public void WithdrawClick()
        {
            if(ValidateWithdraw(WithdrawAmount, nameof(WithdrawAmount)))
            {
                AmountInAccount -= withdrawAmountDecimal;
                OkMessage();
            }
        }
        public void ReplenishClick()
        {
            if(ValidateReplenishment(ReplenishAmount, nameof(ReplenishAmount)))
            {
                AmountInAccount += replenishAmountDecimal;
                OkMessage();
            }
        }
        public void ResetClick()
        {
            if (AmountInAccount > 0)
            {
                AmountInAccount = 0; 
                OkMessage();
                account.StatusChange();
            }
        }
        public void TransferClick()
        {
            if(ValidateAmountToTransfer(AmountToTransfer, nameof(AmountToTransfer)) 
                & ValidateRecipientNumber(RecipientNumber, nameof(RecipientNumber)))
            {
                recipientAccount.SumInDeposit += amountToTransferDecimal;
                AmountInAccount -= amountToTransferDecimal; 
                OkMessage();
            }
        }
        private void OkMessage() => MessageBoxManager.GetMessageBoxStandard("Success", "Operation was successful").ShowAsync();
    }
}

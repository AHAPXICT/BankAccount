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

namespace BankAccount.ViewModels
{
    public class CreateAccountVM : ViewModelBase, INotifyDataErrorInfo
    {
        public CreateAccountVM()
        {
            // Listen to changes of "ValidationUsingINotifyDataErrorInfo" and re-evaluate the validation
            this.WhenAnyValue(x => x.Name)
                .Subscribe(_ => ValidateBoxes(Name, nameof(Name)));
            this.WhenAnyValue(x => x.Surname)
                .Subscribe(_ => ValidateBoxes(Surname, nameof(Surname)));
            this.WhenAnyValue(x => x.Patronymic)
                .Subscribe(_ => ValidateBoxes(Patronymic, nameof(Patronymic)));
            this.WhenAnyValue(x => x.DateOfBirth)
                .Subscribe(_ => ValidateDate(DateOfBirth, nameof(DateOfBirth)));
            this.WhenAnyValue(x => x.PassportNumber)
                .Subscribe(_ => ValidatePassport(PassportNumber, nameof(PassportNumber)));

            // run INotifyDataErrorInfo-validation on start-up
            //ValidateBoxes();
        }
        #region Validation
        // Implement members of INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // we have errors present if errors.Count is greater than 0
        public bool HasErrors => errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            // Get entity-level errors when the target property is null or empty
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values.SelectMany(static errors => errors);
            }

            // Property-level errors, if any
            if (this.errors.TryGetValue(propertyName!, out List<ValidationResult>? result))
            {
                return result;
            }

            // In case there are no errors we return an empty array.
            return Array.Empty<ValidationResult>();
        }

        // Store Errors in a Dictionary
        private Dictionary<string, List<ValidationResult>> errors = new Dictionary<string, List<ValidationResult>>();

        /// <summary>
        /// Clears the errors for a given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to clear or all properties if <see langword="null"/></param>
        protected void ClearErrors(string? propertyName = null)
        {
            // Clear entity-level errors when the target property is null or empty
            if (string.IsNullOrEmpty(propertyName))
            {
                errors.Clear();
            }
            else
            {
                errors.Remove(propertyName);
            }

            // Notify that errors have changed
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            this.RaisePropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Adds a given error message for a given property name.
        /// </summary>
        /// <param name="propertyName">the name of the property</param>
        /// <param name="errorMessage">The error message to show</param>
        protected void AddError(string propertyName, string errorMessage)
        {
            // Add the cached errors list for later use.
            if (!errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new List<ValidationResult>();
                errors.Add(propertyName, propertyErrors);
            }

            propertyErrors.Add(new ValidationResult(errorMessage));

            // Notify that errors have changed
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            this.RaisePropertyChanged(nameof(HasErrors));
        }
        #endregion

        private string? name;
        private string? surname;
        private string? patronymic;
        private DateTimeOffset? dateOfBirth;
        private string? passportNumber;

        #region Properties
        public string? Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }
        public string? Surname
        {
            get { return surname; }
            set { this.RaiseAndSetIfChanged(ref surname, value); }
        }
        public string? Patronymic
        {
            get { return patronymic; }
            set { this.RaiseAndSetIfChanged(ref patronymic, value); }
        }
        public DateTimeOffset? DateOfBirth
        {
            get { return dateOfBirth; }
            set { this.RaiseAndSetIfChanged(ref dateOfBirth, value); }
        }
        public string? PassportNumber
        {
            get { return passportNumber; }
            set { this.RaiseAndSetIfChanged(ref passportNumber, value); }
        }
        #endregion
        private void ValidateBoxes(string field, string nm)
        {
            // first of all clear all previous errors
            ClearErrors(nm);
            if (string.IsNullOrEmpty(field))
                AddError(nm, "This field is required");
        }
        private void ValidateDate(DateTimeOffset? field, string nm)
        {
            ClearErrors(nm);
            if (field == null)
                AddError(nm, "This field is required");
        }
        private void ValidatePassport(string field, string nm)
        {
            ClearErrors(nm);
            if (string.IsNullOrEmpty(field))
            {
                AddError(nm, "This field is required");
                return;
            }
            if (field.Contains('_'))
                AddError(nm, "Enter all numbers");
        }
        public void CreateClick()
        {
            ulong passNumber;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(patronymic)
                || dateOfBirth == null || !ulong.TryParse(passportNumber.Remove(4, 1), out passNumber))
                return;
            foreach (var item in App.accounts)
            {
                if (item.Client.passportNumber == passNumber)
                {
                    ClearErrors(nameof(PassportNumber));
                    AddError(nameof(PassportNumber), "This passport is already in use");
                    return;
                }
            }

            uint accNumber = (uint)App.accounts.Count + 1;
            App.accounts.Add(new BankAccount(new Client(name, surname, patronymic, passNumber, dateOfBirth.Value), accNumber, DateTimeOffset.Now));

            MessageBoxManager.GetMessageBoxStandard("Success!", $"You have successfully registered. Your bank account number is {accNumber}").ShowAsync();
            App.mv.ContentArea.Content = App.lp;
        }
    }
}

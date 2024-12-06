using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccount
{
    /// <summary>
    /// Realized the bank account
    /// </summary>
    internal class BankAccount
    {

        public uint AccountNumber;
        public Client Client;

        public DateTimeOffset OpenDepositDate;
        public ushort DepositDays;
        public decimal? SumInDeposit;
        public AccountStatus Status;

        /// <summary>
        /// Filled Bank Account
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="openDepositDate">Date opened deposit</param>
        /// <param name="client">Info about client</param>

        public BankAccount(Client client, uint accountNumber, DateTimeOffset openDepositDate)
        {
            this.Client = client;
            this.AccountNumber = accountNumber;
            SumInDeposit = 0;
            this.OpenDepositDate = openDepositDate;
            DepositDays = 30;
        }

        public DateTimeOffset EndOfDeposit() => OpenDepositDate.AddDays(DepositDays);
        public void StatusChange()
        {
            switch (SumInDeposit)
            {
                case < 0: Status = AccountStatus.Bankrupt; break;
                case 0: Status = AccountStatus.Closed; break;
                case > 0: Status = AccountStatus.Open; break;
            }
        }

        public void Replenishment(decimal amount) {
                SumInDeposit += amount;
        }
        public void WithdrawalMoney(decimal amount)
        {
                SumInDeposit -= amount;
        }

        public void Transfer(BankAccount recipientAccount, decimal amount)
        {
            if (amount > 0 && recipientAccount.AccountNumber != 0 && recipientAccount.Status == AccountStatus.Open && SumInDeposit - amount >= 0)
            {
                recipientAccount.SumInDeposit += amount;
                SumInDeposit -= amount;
            }
        }
    }

    enum AccountStatus
    {
        Open,
        Bankrupt,
        Closed
    }
    
}

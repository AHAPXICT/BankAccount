using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccount
{
    /// <summary>
    /// Information about client
    /// </summary>
    /// <param name="name"></param>
    /// <param name="passportNumber"></param>
    /// <param name="birthDay"></param>
    internal class Client(string name, string surname, string patronymic, ulong passportNumber, DateTimeOffset birthDay)
    {
        public string name = name;
        public string surname = surname;
        public string patronymic = patronymic;
        public ulong passportNumber = passportNumber;
        public DateTimeOffset birthDay = birthDay;
    }
}

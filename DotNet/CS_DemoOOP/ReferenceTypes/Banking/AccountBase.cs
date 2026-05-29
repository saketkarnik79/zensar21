using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoOOP.ReferenceTypes.Banking
{
    internal abstract class AccountBase
    {
        public decimal Balance { get; protected set; }

        public abstract void Deposit(decimal amount);

        public abstract void Withdraw(decimal amount);
    }
}

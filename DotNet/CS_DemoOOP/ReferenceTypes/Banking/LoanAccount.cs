using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoOOP.ReferenceTypes.Banking
{
    internal class LoanAccount : AccountBase
    {
        public override void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }
            else
            {
                if(amount <= Balance)
                {
                    Balance -= amount; // For a loan account, depositing money reduces the balance (the amount owed)
                }
                else
                {
                    throw new InvalidOperationException("Deposit amount cannot exceed the current balance (amount owed).");
                }
            }
        }

        public override void Withdraw(decimal amount)
        {
            if (amount > 0) 
            { 
                Balance += amount; // For a loan account, depositing money reduces the balance (the amount owed)
            }
            else
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }
        }
    }
}

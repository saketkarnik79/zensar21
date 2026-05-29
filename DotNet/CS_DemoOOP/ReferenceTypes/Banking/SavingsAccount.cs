using System;
using System.Collections.Generic;
using System.Text;
using CS_DemoOOP.ReferenceTypes.Insurance;

namespace CS_DemoOOP.ReferenceTypes.Banking
{
    internal class SavingsAccount : AccountBase, ILifeInsurance, IDisposable
    {
        public decimal CalculatePremium()
        {
            // Calculate insurance premium as 0.5% of the current balance and deduct it from the balance
            decimal premium = Balance * 0.005m; // 0.5% of the balance
            Withdraw(premium); // Deduct the premium from the balance
            return premium;
        }

        public override void Deposit(decimal amount)
        {
            if (amount > 0) 
            { 
                Balance += amount; 
            }
            else
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Object disposed...");
        }

        public override void Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }
            else
            {
                if(Balance >= amount)
                {
                    Balance -= amount;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds for withdrawal.");
                }
            }
        }
    }
}
